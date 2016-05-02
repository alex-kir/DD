#if DD_PLATFORM_ANDROID

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Android.Media;
using Android.Content;

public partial class DDAudio
{
    private static SoundPool _soundPool;
    private static AudioManager _audioManager;

    private readonly static Dictionary<string, int> effectIds = new Dictionary<string, int>();
//    int soundId;
//
//    bool played = false;
//    bool playedLoop = false;

    private static SoundPool GetSoundPool()
    {
        if (_soundPool == null) {
            _soundPool = new SoundPool(4, Android.Media.Stream.Music, 100);
        }
        return _soundPool;
    }

    private static AudioManager GetAudioManager()
    {
        if (_audioManager == null)
            _audioManager = (AudioManager)DDDirector.Instance.Activity.GetSystemService(Context.AudioService);
        return _audioManager;
    }

    private static async Task<int> LoadEffect(string name)
    {
        if (effectIds.ContainsKey(name))
            return effectIds[name];
        DDDebug.Trace("loading:" + name);

        var pool = GetSoundPool();

        var folder = Path.Combine(DDDirector.Instance.Activity.CacheDir.AbsolutePath, "ddaudio");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        var file_to_sound = Path.Combine(folder, name);
        if (!File.Exists(file_to_sound)) {
            var bytes = DDFile.GetBytes(name);
            File.WriteAllBytes(file_to_sound, bytes);
        }

        var completion = new TaskCompletionSource<int>();
        EventHandler<SoundPool.LoadCompleteEventArgs> action = null;
        action = (s, e) => {
            pool.LoadComplete -= action;
            completion.SetResult(e.SampleId);
        };
        pool.LoadComplete += action;

        pool.Load(file_to_sound, 1);

        var effectId = await completion.Task;
        effectIds[name] = effectId;
        DDDebug.Trace("effectId:" + effectId);
        return effectId;
    }

//    public DDAudio(string name)
//    {
//        if (soundPool == null)
//        {
//            soundPool = new SoundPool(4, Android.Media.Stream.Music, 100);
//            audioManager = (AudioManager)DDDirector.Instance.Activity.GetSystemService(Context.AudioService);
//        }
//        string path = DDFile.FindFile(name);
//        if (effectIds.ContainsKey(path))
//        {
//            soundId = effectIds[path];
//        }
//        else
//        {
//            var fd = DDDirector.Instance.Activity.Assets.OpenFd(path);
//            soundId = soundPool.Load(fd, 1);
//            soundPool.LoadComplete += (s, e) =>
//            {
//                if (e.SampleId == soundId)
//                {
//                    if (played)
//                    {
//                        Play();
//                    }
//                    else if (playedLoop)
//                    {
//                        PlayLoop();
//                    }
//                }
//            };
//            effectIds[path] = soundId;
//        }
//    }

//    public void Play()
//    {
//        played = true;
//        float streamVolumeCurrent = audioManager.GetStreamVolume(Android.Media.Stream.Music);
//        float streamVolumeMax = audioManager.GetStreamMaxVolume(Android.Media.Stream.Music);
//        float volume = streamVolumeCurrent / streamVolumeMax;
//        soundPool.Play(soundId, volume, volume, 1, 0, 1f);
//    }
//
//    public void PlayLoop()
//    {
//        playedLoop = true;
//        float streamVolumeCurrent = audioManager.GetStreamVolume(Android.Media.Stream.Music);
//        float streamVolumeMax = audioManager.GetStreamMaxVolume(Android.Media.Stream.Music);
//        float volume = streamVolumeCurrent / streamVolumeMax;
//        soundPool.Play(soundId, volume, volume, 1, 1, 1f);
//    }
//
//    public void Stop()
//    {
//    }

    public static float GetLength(string name)
    {
        throw new NotImplementedException();
    }

    public static async void PlayEffect(string name)
    {
        try
        {
            var effectId = await LoadEffect(name);

            var manager = GetAudioManager();
            float streamVolumeCurrent = manager.GetStreamVolume(Android.Media.Stream.Music);
            float streamVolumeMax = manager.GetStreamMaxVolume(Android.Media.Stream.Music);
            float volume = streamVolumeCurrent / streamVolumeMax;

            var pool = GetSoundPool();
            pool.Play(effectId, volume, volume, 1, 0, 1f);
        }
        catch(Exception ex) {
            ex.LogException();
        }
    }
}

#endif