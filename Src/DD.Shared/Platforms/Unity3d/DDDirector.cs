//
//  DDDirector_Unity3d.cs
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
#if DD_PLATFORM_UNITY3D

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public partial class DDDirector
{
    public UnityEngine.AudioSource Effects = null;
    public UnityEngine.AudioSource Music = null;
    private UnityEngine.Camera camera = null;

    DDRenderer renderer;
#if UNITY_ANDROID && UNITY_3_5
//	bool _isCrazyKindle = new [] { "Amazon KFAPWA", "Amazon KFAPWI", "Amazon KFTHWA", "Amazon KFTHWI", "Amazon KFSOWI" }.Contains(SystemInfo.deviceModel);
	bool _isCrazyKindle = false;
#endif
	
	public void OnStart(DDRenderer rndr, Func<DDScene> scene)
	{
#if UNITY_ANDROID && UNITY_3_5
		_isCrazyKindle = new [] { "KFAPWA", "KFAPWI", "KFTHWA", "KFTHWI", "KFSOWI" }.FirstOrDefault(it => SystemInfo.deviceModel.ToUpper().Contains(it)) != null;
#endif

        WinSize = new DDVector(Screen.width, Screen.height);
        renderer = rndr;

        camera = UnityEngine.Camera.main;
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.backgroundColor = Color.black;
		camera.orthographic = true;
        camera.orthographicSize = Screen.width / 2;
        camera.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, -100);
        camera.transform.rotation = Quaternion.identity;
        //Application.targetFrameRate = 35;

        Effects = camera.gameObject.AddComponent<AudioSource>();
        Effects.playOnAwake = false;

        Music = camera.gameObject.AddComponent<AudioSource>();
        Music.playOnAwake = false;
        Music.loop = true;
        

        SetScene(scene());
	}

	public void OnUpdate()
	{
		DPI = DDMath.Abs(Screen.dpi) < 1 ? 100 : DDMath.Abs(Screen.dpi);
        WinSize = new DDVector(Screen.width, Screen.height);
        camera.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, -100);
        camera.orthographicSize = Screen.height / 2;

        OnTick(Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Escape))
            DDDirector.Instance.OnEscapePressed();

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		foreach (var t in Input.touches)
		{
			if (t.phase == TouchPhase.Began)
			{
				DDTouchDispatcher.Instance.OnTouch(t.fingerId, t.position.x, t.position.y, DDTouchPhase.Began);
			}
			else if(t.phase == TouchPhase.Moved)
			{
				DDTouchDispatcher.Instance.OnTouch(t.fingerId, t.position.x, t.position.y, DDTouchPhase.Moved);
			}
			else if (t.phase == TouchPhase.Ended) // checking might be removed
			{
				DDTouchDispatcher.Instance.OnTouch(t.fingerId, t.position.x, t.position.y, DDTouchPhase.Ended);
			}
		}
#else
		if (Input.GetMouseButtonDown(0))
		{
			DDTouchDispatcher.Instance.OnTouch(3030, Input.mousePosition.x, Input.mousePosition.y, DDTouchPhase.Began);
		}
		else if(Input.GetMouseButton(0))
		{
			DDTouchDispatcher.Instance.OnTouch(3030, Input.mousePosition.x, Input.mousePosition.y, DDTouchPhase.Moved);
		}
		else if (Input.GetMouseButtonUp(0)) // checking might be removed
		{
			DDTouchDispatcher.Instance.OnTouch(3030, Input.mousePosition.x, Input.mousePosition.y, DDTouchPhase.Ended);
		}
#endif

		if (Input.touches.Length > 0)
		{
			var touches = Input.touches.DDSelect(t =>
			{
				DDTouch touch = new DDTouch();
				touch.Position = new DDVector(t.position.x, t.position.y);
				touch.Finger = t.fingerId;

				if (t.phase == TouchPhase.Began)
					touch.Phase = DDTouchPhase.Began;
				else if(t.phase == TouchPhase.Moved)
					touch.Phase = DDTouchPhase.Moved;
				else if (t.phase == TouchPhase.Ended)
					touch.Phase = DDTouchPhase.Ended;
				return touch;
			}).ToArray();
			OnTouches(touches);
		}
		else
		{
			DDTouch touch = null;
			if (Input.GetMouseButtonDown(0))
				touch = new DDTouch(){Phase = DDTouchPhase.Began};
			else if(Input.GetMouseButton(0))
				touch = new DDTouch(){Phase = DDTouchPhase.Moved};
			else if (Input.GetMouseButtonUp(0)) // checking might be removed
				touch = new DDTouch(){Phase = DDTouchPhase.Ended};

			if (touch != null)
			{
				touch.Position = new DDVector(Input.mousePosition.x, Input.mousePosition.y);
				touch.Finger = 12345;
				OnTouches(touch);
			}
		}


#if UNITY_ANDROID && UNITY_3_5
		if (_isCrazyKindle)
		{
			if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft )
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
			else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight )
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
		}
		else
		{
			if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft )
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
			else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight )
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
		}
#endif
	}

	public void OnPostRender()
	{
        OnDraw(renderer);
	}

    private void StopAllAudioEffects()
    {
        UnityEngine.Object.Destroy(Effects);
        foreach (var source in DDAudio.Instance._sources.Values)
            UnityEngine.Object.Destroy(source);

        Effects = camera.gameObject.AddComponent<AudioSource>();
        Effects.playOnAwake = false;
    }

#region Utils
	
	public void SetPrefs(string key, string value)
	{
		UnityEngine.PlayerPrefs.SetString(key, value);
	}
	
	public string GetPrefs(string key, string defaultValue)
	{
		return UnityEngine.PlayerPrefs.GetString(key, defaultValue);
	}

	public void SetPrefs(string key, int value)
	{
		UnityEngine.PlayerPrefs.SetInt(key, value);
	}
	
	public int GetPrefs(string key, int defaultValue)
	{
		return UnityEngine.PlayerPrefs.GetInt(key, defaultValue);
	}
	
	public void SetPrefs(string key, float value)
	{
		UnityEngine.PlayerPrefs.SetFloat(key, value);
	}
	
	public float GetPrefs(string key, float defaultValue)
	{
		return UnityEngine.PlayerPrefs.GetFloat(key, defaultValue);
	}	
	
//    public void SetPrefs<T>(string key, T value) where T : class
//	{
//		UnityEngine.PlayerPrefs.SetString(key, DDJson.Encode(value));
//	}
//	
//	public T GetPrefs<T>(string key, T defaultValue) where T : class 
//	{
//		return DDJson.DecodeOrNull<T>(UnityEngine.PlayerPrefs.GetString(key, null)) ?? defaultValue;
//	}

	public void OpenUrl(string url)
	{
		UnityEngine.Application.OpenURL(url);
	}

    public void Quit()
    {
        UnityEngine.Application.Quit();
    }

#endregion
}

#endif