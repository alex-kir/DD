#if DD_PLATFORM_UNITY3D 
using UnityEngine;
using System.Collections;
using System;

public class DDMusicHelper : MonoBehaviour {
	
	static DDMusicHelper instance = null;
	public static DDMusicHelper Instance
	{
		get
		{
			if(instance == null)
				instance = UnityEngine.Camera.main.gameObject.AddComponent<DDMusicHelper>();

			return instance;
		}
	}
	
	//public delegate IEnumerator CoroutineMethod();
    
	IEnumerator RunCoroutine(Func<IEnumerator> coroutineMethod)
	{
	    return coroutineMethod();
	}

	public void StartCoroutineDelegate(Func<IEnumerator> coroutineMethod)
	{
		StartCoroutine(RunCoroutine(coroutineMethod));
	}
}
#endif