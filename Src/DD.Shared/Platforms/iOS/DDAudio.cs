using System;
using OpenTK.Audio.OpenAL;
using System.Collections.Generic;
using OpenTK;

#if DD_PLATFORM_IOS
using AudioToolbox;
#endif

#if DD_PLATFORM_IOS
namespace DD
{
    internal class DDSound
    {
        readonly DDSoundBuffer buffer;
        readonly DDSoundSource[] sources;
        int currentIndex = 0;

        public float Duration { get { return buffer.Duration; } }

        internal DDSound(string name, int count = 1)
        {
            sources = new DDSoundSource[count];

            var buffer = DDSoundBuffer.LoadFromResource(name);
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i] = new DDSoundSource(buffer);
            }
        }

        internal void Play()
        {
            currentIndex = ((currentIndex + 1) % sources.Length);
            sources[currentIndex].Play();
        }

        internal void Stop()
        {
            foreach (var source in sources)
                source.Stop();
        }
    }

    internal class DDSoundSource : IDisposable
    {
        bool disposed = false;
        readonly int handle;
        float pitch = 1;
        float gain = 1;
        bool looping = false;

        public bool IsPlaying { get { return AL.GetSourceState(handle) == ALSourceState.Playing; } }

        public float Pitch
        {
            get { return pitch; }
            set
            {
                AL.Source(handle, ALSourcef.Pitch, value);
                pitch = value;
            }
        }

        public float Gain
        {
            get { return gain; }
            set
            {
                AL.Source(handle, ALSourcef.Gain, value);
                gain = value;
            }
        }

        public bool Looping
        {
            get { return looping; }
            set
            {
                AL.Source(handle, ALSourceb.Looping, value);
                looping = value;
            }
        }

        internal DDSoundSource(DDSoundBuffer buffer)
        {
            handle = AL.GenSource();
            AL.Source(handle, ALSourcei.Buffer, buffer.handle);
            DDSoundBuffer.ThrowIfAlError();
        }

        public void Dispose()
        {
            if (disposed)
                return;
            if (IsPlaying)
                Stop();
            disposed = true;
            AL.Source(handle, ALSourcei.Buffer, 0);
            AL.DeleteSource(handle);
        }

        internal void Play()
        {
            if (disposed)
                return;
            
            Stop();
            AL.SourcePlay(handle);
        }

        internal void Stop()
        {
            if (disposed)
                return;
            if (IsPlaying)
                AL.SourceStop(handle);
        }
    }

    internal class DDSoundBuffer : IDisposable
    {
        private bool disposed = false;
        internal readonly int handle;
        internal readonly ALFormat format;
        internal readonly int bytesPerSample;
        internal readonly int sampleRate;
        internal readonly float Duration;

        internal DDSoundBuffer(byte[] data, int sampleRate, ALFormat format)
        {
            this.sampleRate = sampleRate;
            this.format = format;

            if (format == ALFormat.Mono8)
                bytesPerSample = 1;
            else if (format == ALFormat.Mono16)
                bytesPerSample = 2;
            else if (format == ALFormat.Stereo8)
                bytesPerSample = 2;
            else if (format == ALFormat.Stereo16)
                bytesPerSample = 4;
            else
                throw new Exception("DDSoundBuffer: unknown format " + format);

            Duration = (data.Length / bytesPerSample) / (float)sampleRate;

            if (Alc.GetCurrentContext() == OpenTK.ContextHandle.Zero)
                throw new Exception("Alc.GetCurrentContext() == null");

            handle = AL.GenBuffer();
            ThrowIfAlError();

            AL.BufferData(handle, format, data, data.Length, sampleRate);
            ThrowIfAlError();
        }

        internal static void ThrowIfAlError()
        {
            var err = AL.GetError();
            if (err != ALError.NoError)
                throw new Exception("dd sound: " + AL.GetErrorString(err));
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            AL.DeleteBuffer(handle);
        }

        public static DDSoundBuffer LoadFromResource(string name)
        {
            string path = DDFile.GetPath(name);

            using (var audioFile = AudioFile.Open(path, AudioFilePermission.Read, 0))
            {

                var dataFormat = audioFile.DataFormat;
                if (!dataFormat.HasValue || dataFormat.Value.Format != AudioFormatType.LinearPCM)
                    throw new Exception("DDSoundBuffer: audioFile.DataFormat is incorrect, " + audioFile.DataFormat);
                var format = dataFormat.Value;


                var size = (int)audioFile.Length;
                var data = new byte[size];
                audioFile.Read(0, data, 0, size, true);

                ALFormat alFormat;
                if (format.ChannelsPerFrame == 1 && format.BitsPerChannel == 8)
                    alFormat = ALFormat.Mono8;
                else if (format.ChannelsPerFrame == 1 && format.BitsPerChannel == 16)
                    alFormat = ALFormat.Mono16;
                else if (format.ChannelsPerFrame == 2 && format.BitsPerChannel == 8)
                    alFormat = ALFormat.Stereo8;
                else if (format.ChannelsPerFrame == 2 && format.BitsPerChannel == 16)
                    alFormat = ALFormat.Stereo16;
                else
                    throw new Exception("DDSoundBuffer: dataFormat is not supported");
                    
                return new DDSoundBuffer(data, (int)format.SampleRate, alFormat);
            }
        }
    }

    class DDSoundDevice : IDisposable
    {
        public IntPtr handle { get; private set; }

        public DDSoundDevice(string name)
        {
            handle = Alc.OpenDevice(name);
        }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
                Alc.CloseDevice(handle);
            handle = IntPtr.Zero;
        }
    }

    class DDSoundContext : IDisposable
    {
        bool disposed = false;

        public readonly ContextHandle handle;

        public bool IsCurrent { get { return Alc.GetCurrentContext() == handle; } set { Alc.MakeContextCurrent(value ? handle : ContextHandle.Zero); } }

        public DDSoundContext(DDSoundDevice device)
        {
            handle = Alc.CreateContext(device.handle, new int[]{ });
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            Alc.DestroyContext(handle);

        }


    }

}

public static partial class DDAudio
{
    static DD.DDSoundContext soundContext = null;
    static readonly Dictionary<string, DD.DDSound> sounds = new Dictionary<string, DD.DDSound>();


    private static void InitContext()
    {
        if (soundContext != null)
            return;
        soundContext = new DD.DDSoundContext(new DD.DDSoundDevice(null));
        soundContext.IsCurrent = true;
    }

    public static float MusicVolume { set { } }

    public static float EffectsVolume { set { } }

    public static void PlayMusic(string name)
    {
    }

    public static void PlayEffect(string name)
    {
        InitContext();

        if (!sounds.ContainsKey(name))
            sounds[name] = new DD.DDSound(name);
        sounds[name].Play();
    }

    public static float GetLength(string name)
    {
        return 1;
    }
}

#endif