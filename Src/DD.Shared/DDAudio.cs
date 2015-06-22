//
//  DDAudio.cs
//
//  DD engine for 2d games and apps: https://code.google.com/p/dd-engine/
//
//  Copyright (c) 2013 - Alexander Kirienko
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if DD_PLATFORM_UNITY3D
using UnityEngine;
#elif DD_PLATFORM_WIN32
using System.Media;
#elif DD_PLATFORM_ANDROID
using Android.Content;
using Android.Media;
#endif

class DDAudio
{
    private static DDAudio _instance = new DDAudio();
    public static DDAudio Instance { get { return _instance; } }
    private DDAudio() { }

#if DD_PLATFORM_UNITY3D

    private Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
	internal Dictionary<string, AudioSource> _sources = new Dictionary<string, AudioSource>();



    private UnityEngine.AudioClip GetClip(string name)
    {
        if (!_clips.ContainsKey(name))
        {
            try
            {
#if UNITY_STANDALONE_WIN

				var exePath = Application.dataPath;
				if (Application.platform == RuntimePlatform.OSXPlayer) {
					exePath += "/../../";
				}
				else if (Application.platform == RuntimePlatform.WindowsPlayer) {
					exePath += "/../";
				}

				exePath += "Resources/";

				if (File.Exists(exePath + name + ".wav"))
				{
					DDMusicHelper.Instance.StartCoroutineDelegate(() => LoadClipCoroutine(exePath, name, ".wav"));
					_clips[name] = new AudioClip();
				}
				else
#endif
				{
	                var clip = Resources.Load(name, typeof(AudioClip)) as AudioClip;
	                if (clip)
	                    _clips[name] = clip;
	                else
	                    throw new Exception("AudioClip " + name + " is empty or null");
				}
            }
            catch (Exception ex)
            {
                throw new Exception("DDAudio " + name + " not found", ex);
            }
        }
        return _clips[name];
    }

#if UNITY_STANDALONE_WIN

	System.Collections.IEnumerator LoadClipCoroutine(string exePath, string name, string ext = ".wav")
	{
		var name2 = name.Select(ch => ((int)ch).ToString()).Aggregate("", (it, x) => it + "_" + x);
		Directory.CreateDirectory(exePath + "tmp/");
		File.Copy(exePath + name + ext, exePath + "tmp/" + name2 + ext, true);

		var url = "file:///" + exePath + "tmp/" + name2 + ext;
		DDDebug.Log(url);
		DDDebug.Log("load clip 1");
		var www = new WWW(url);
		DDDebug.Log("load clip 2");
		yield return www;
		DDDebug.Log("load clip 3");
		_clips[name] = www.GetAudioClip(false);
		DDDebug.Log("load clip 4");
	}

#endif

    public float MusicVolume
    {
        set
        {
            DDDirector.Instance.Music.volume = DDMath.Min(DDMath.Max(0, value), 1);
        }
    }

    public float EffectsVolume
    {
        set
        {
            DDDirector.Instance.Effects.volume = DDMath.Min(DDMath.Max(0, value), 1);
        }
    }

    public void PlayMusic(string name)
    {
        var audio = DDDirector.Instance.Music;
        if (name == null)
        {
            audio.clip = null;
        }
        else
        {
            audio.clip = GetClip(name);
            audio.Play();
        }
    }

    public void PlayEffect(string name)
    {
        DDDirector.Instance.Effects.PlayOneShot(GetClip(name));
    }
	
	public void PlayEffect(float[] samples, int recordFreq = 44100)
    {
        AudioClip audioClip = AudioClip.Create("tmpClip", samples.Length, 1, recordFreq, false, false);
		audioClip.SetData(samples.ToArray(), 0);
		DDDirector.Instance.Effects.PlayOneShot(audioClip);
    }
	
	public void PlayLoop(string sourceName, string name)
    {
		if (!_sources.ContainsKey(sourceName))
		{
			var source = UnityEngine.Camera.main.gameObject.AddComponent<AudioSource>();
			source.loop = true;
			source.playOnAwake = false;
			_sources[sourceName] = source;
		}
		
		{
			var source = _sources[sourceName];
			source.clip = GetClip(name);
			source.Play();
		}
    }
	
	public void StopLoop(string sourceName)
	{
		if (_sources.ContainsKey(sourceName))
		{
			var source = _sources[sourceName];
			source.Stop();
		}
	}

	public float GetLength(string name)
	{
		return GetClip(name).length;
	}
	
/////////////////////////////////////////////	

#if !UNITY_WP8
	
	//public static int recordFreq = 44100;

//	private Dictionary<string, AudioClip> _customClips = new Dictionary<string, AudioClip>();
	
	public void StartRecord(int recordTime, System.Action<float[]> onEnd, int recordFreq = 44100)
	{
		if(Microphone.devices.Length > 0)
		{
			if(Microphone.IsRecording(null))
				return;
			
			var currentRecord = Microphone.Start(null, false, recordTime, recordFreq);
			
			DDMusicHelper.Instance.StartCoroutineDelegate(()=>WaitRecordEnd(currentRecord, onEnd));
		}
		else
		{
			Debug.Log("Mic not found");	
			if(onEnd != null)
				onEnd(null);
		}
	}
	
	public void StopRecord()
	{
		if(!Microphone.IsRecording(null))
		{
			Microphone.End(null);	
		}
	}
	
	private System.Collections.IEnumerator WaitRecordEnd(AudioClip currentRecord, System.Action<float[]> onRecordEnd)
	{
		while(true)
		{
			if(!Microphone.IsRecording(null))
			{	
				var samples = new float[currentRecord.samples];
				currentRecord.GetData(samples, 0);
				
				if(onRecordEnd != null)
					onRecordEnd(samples);
				
				yield break;
			}
			
			yield return new WaitForSeconds(0.1f);
		}
	}
	
#endif

#elif DD_PLATFORM_WIN32

    private static Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();
    private string name;
    SoundPlayer player;

    public float MusicVolume { set { } }
    public float EffectsVolume { set { } }

    public DDAudio(string name)
    {
        this.name = name;
        var bytes = DDFile.GetBytes(name);
        var ms = new MemoryStream(bytes);
        player = new SoundPlayer(ms);
    }

    public void PlayMusic(string name)
    {

    }
    public void PlayEffect(string name)
    {

    }

#elif DD_PLATFORM_ANDROID
    
    private static SoundPool soundPool;
    private static AudioManager audioManager;

    private static Dictionary<string, int> soundIds = new Dictionary<string, int>();
    int soundId;

    bool played = false;
    bool playedLoop = false;

    public DDAudio(string name)
    {
        if (soundPool == null)
        {
            soundPool = new SoundPool(4, Android.Media.Stream.Music, 100);
            audioManager = (AudioManager)DDDirector.Instance.Activity.GetSystemService(Context.AudioService);
        }
        string path = DDFile.FindFile(name);
        if (soundIds.ContainsKey(path))
        {
            soundId = soundIds[path];
        }
        else
        {
            var fd = DDDirector.Instance.Activity.Assets.OpenFd(path);
            soundId = soundPool.Load(fd, 1);
            soundPool.LoadComplete += (s, e) =>
            {
                if (e.SampleId == soundId)
                {
                    if (played)
                    {
                        Play();
                    }
                    else if (playedLoop)
                    {
                        PlayLoop();
                    }
                }
            };
            soundIds[path] = soundId;
        }
    }

    public void Play()
    {
        played = true;
        float streamVolumeCurrent = audioManager.GetStreamVolume(Android.Media.Stream.Music);
        float streamVolumeMax = audioManager.GetStreamMaxVolume(Android.Media.Stream.Music);
        float volume = streamVolumeCurrent / streamVolumeMax;
        soundPool.Play(soundId, volume, volume, 1, 0, 1f);
    }

    public void PlayLoop()
    {
        playedLoop = true;
        float streamVolumeCurrent = audioManager.GetStreamVolume(Android.Media.Stream.Music);
        float streamVolumeMax = audioManager.GetStreamMaxVolume(Android.Media.Stream.Music);
        float volume = streamVolumeCurrent / streamVolumeMax;
        soundPool.Play(soundId, volume, volume, 1, 1, 1f);
    }

    public void Stop()
    {
    }
    
    public float GetLength(string name)
    {
        throw new NotImplementedException();
    }

    public void PlayEffect(string name)
    {
        // TODO: throw new NotImplementedException();
    }
#else
    public float MusicVolume { set { } }
    public float EffectsVolume { set { } }
    public void PlayMusic(string name) { }
    public void PlayEffect(string name) { }
	public float GetLength(string name) { return 1; }
#endif
}