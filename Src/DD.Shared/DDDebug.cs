//
//  DDDebug.cs
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
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Linq;

public static class DDDebug
{
    private static DateTime nextDump;

	private class MeasureForUsing : IDisposable
	{
		public Stopwatch Stopwatch;
		public long Count = 0;
		public MeasureForUsing()
		{
			this.Stopwatch = new Stopwatch();
		}
		
		public void Dispose()
		{
			Stopwatch.Stop();
		}
	}

	public static bool UseUtf8 = false;

	private static Dictionary<string, MeasureForUsing> _stopwatches = new Dictionary<string, MeasureForUsing>();
	private static Dictionary<string, string> _info = new Dictionary<string, string>();
	private static DDLabel _infoLabel = null;
	private static float _fps = 0;
	private static float _fpsTime = 0;
	private static int _fpsCounter = 0;

	public static void Error(string msg)
    {
//        List<string> lines = new List<string>();
//        foreach (object obj in objs)
//		{
//            if (obj == null)
//			{
//                lines.Add("<NULL>");
//			}
//            else
//			{
//				if (UseUtf8)
//				{
//#if !NETFX_CORE && !UNITY_WP8
//					var bytes = Encoding.Default.GetBytes(obj.ToString());
//#else
//					var bytes = Encoding.UTF8.GetBytes(obj.ToString());
//#endif
//                	lines.Add(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
//				}
//				else
//					lines.Add(obj.ToString());
//			}
//		}
		
//        var msg = string.Join("", lines.ToArray());
#if DD_PLATFORM_UNITY3D
        UnityEngine.Debug.LogError(msg);
#elif DD_PLATFORM_ANDROID
        Android.Util.Log.Debug("DDDebug", msg);
#elif DD_PLATFORM_WIN32
        System.Diagnostics.Debug.WriteLine(msg);
//#elif DD_PLATFORM_IOS
//		MonoTouch.Foundation.NSLog(@"Do real error checking here");
#else
		System.Diagnostics.Debug.WriteLine(msg);
#endif
	}

	public static void LogException(this Exception ex)
	{
		var msg = "##### [" + DateTime.Now.ToString("HH:mm:ss,ff") + "] " + (ex == null ? "<NULL>" : ex.ToString());

		#if DD_PLATFORM_UNITY3D
		UnityEngine.Debug.LogError (msg);
		#elif DD_PLATFORM_ANDROID
		Android.Util.Log.Debug("EXCEPTION", msg);
		#elif DD_PLATFORM_WIN32
		System.Diagnostics.Debug.WriteLine(msg);
		//#elif DD_PLATFORM_IOS
		//		MonoTouch.Foundation.NSLog(@"Do real error checking here");
		#else
		System.Diagnostics.Debug.WriteLine(msg);
		#endif
	}

    public static void Log(params object[] objs)
    {
        List<string> lines = new List<string>();
        foreach (object obj in objs)
		{
            if (obj == null)
			{
                lines.Add("<NULL>");
			}
            else
			{
				if (UseUtf8)
				{
#if !NETFX_CORE && !UNITY_WP8
					var bytes = Encoding.Default.GetBytes(obj.ToString());
#else
					var bytes = Encoding.UTF8.GetBytes(obj.ToString());
#endif
                	lines.Add(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
				}
				else
					lines.Add(obj.ToString());
			}
		}

        var msg = "##### [" + DateTime.Now.ToString("HH:mm:ss,ff") + "] " + string.Join("", lines.ToArray());
#if DD_PLATFORM_UNITY3D
        UnityEngine.Debug.Log(msg);
#if UNITY_WEBPLAYER
        if (UnityEngine.Application.isWebPlayer)
            UnityEngine.Application.ExternalCall("console.log", msg);
#endif
#elif DD_PLATFORM_ANDROID
        Android.Util.Log.Debug("DDDebug", msg);
#elif DD_PLATFORM_WIN32
        System.Diagnostics.Debug.WriteLine(msg);
//#elif DD_PLATFORM_IOS
//		MonoTouch.Foundation.NSLog(@"Do real error checking here");
#else
		System.Diagnostics.Debug.WriteLine(msg);
#endif
	}

	public static void Assert(bool flag, string message)
	{
		if (!flag)
			Error(message);
	}

    public static void Trace(params object[] objs)
    {
#if !NETFX_CORE && !UNITY_WP8
        StackTrace stackTrace = new StackTrace();
        var frame = stackTrace.GetFrame(1);
        System.Reflection.MethodBase methodBase = frame.GetMethod();
        List<object> objects = new List<object>();
        objects.Add("[" + methodBase.ReflectedType.Name + "." + methodBase.Name + "] ");
        objects.AddRange(objs.Select(it => it + " ").ToArray());
        Log(objects.ToArray());
#else
		Log (objs);	
#endif
    }

    internal static void Dump(object obj, int level, string indent = "")
    {
        if (level < 0)
            return;

        var h = obj as Dictionary<object, object>;
        var l = obj as List<object>;
        var s = obj as string;
        if (h != null)
        {
            foreach (var key in h.Keys)
            {
                DDDebug.Dump(h[key], level - 1, indent + "/" + key);
            }
        }
        else if (l != null)
        {
            for (int i = 0; i < l.Count; i++)
            {
                DDDebug.Dump(l[i], level - 1, indent + "/" + i.ToString());
            }
        }
        else if (s != null)
            DDDebug.Log(indent, "=", s);
        else
            DDDebug.Log(indent, "=<UNKNOWN>");
    }

	public static IDisposable Measure(string name)
	{
		MeasureForUsing ret;
		if (!_stopwatches.TryGetValue(name, out ret))
		{
			ret = new MeasureForUsing();
			_stopwatches[name] = ret;
		}
		ret.Count++;
		ret.Stopwatch.Start();
		return ret;
	}

	public static IDisposable MeasureOne(string name = null)
	{
		var stop = new Stopwatch ();
		stop.Start ();
		return new DDDisposable (() => {
			stop.Stop ();
			DDDebug.Log (stop.Elapsed.TotalMilliseconds.ToString ("0.##") + "ms " + name);
		});
//		MeasureForUsing ret;
//		if (!_stopwatches.TryGetValue(name, out ret))
//		{
//			ret = new MeasureForUsing();
//			_stopwatches[name] = ret;
//		}
//		ret.Count++;
//		ret.Stopwatch.Start();
//		return ret;
	}


	public static void OnTick(float t)
	{
        if (nextDump < DateTime.Now) {
            nextDump = DateTime.Now.AddSeconds(5);
            DDDebug.Log("----------------\n" + GetDebugText());
        }


		foreach (var sw in _stopwatches.Values)
		{
			sw.Stopwatch.Reset();
			sw.Count = 0;
		}

		_fpsTime += t;
		if (_fpsTime > 1)
		{
			_fps = _fpsCounter / _fpsTime;
			_fpsCounter = 0;
			_fpsTime = 0;
		}
		else
		{
			_fpsCounter++;
		}
	}

	public static void CreateInfoLabel(DDFont font = null, DDColor color = default(DDColor), int size = 16)
	{
		_infoLabel = new DDLabel(font ?? DDFont.Default, "");
		_infoLabel.AnchorPoint = DDVector.LeftBottom;
		_infoLabel.Scale = size / _infoLabel.Size.Height;
		if (color != default(DDColor))
			_infoLabel.Color = color;
	}

	public static void Watch(string name, string value)
	{
		_info[name] = value;
	}

	public static DDLabel GetInfoLabel()
	{
		if (_infoLabel == null)
			return null;
        
        _infoLabel.SetText(GetDebugText());

		return _infoLabel;
	}

    private static string GetDebugText()
    {
        _info[" FPS"] = _fps.ToString("0.0");

        foreach (var kv in _stopwatches) {
            _info[kv.Key] = kv.Value.Stopwatch.Elapsed.TotalMilliseconds.ToString("0.0ms") + "/" + kv.Value.Count;
        }

        string debugText = string.Join("\n", _info.Select(it => it.Key + ":" + it.Value).OrderBy(it => it).ToArray());
        return debugText;
    }
}