using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayTrigger : MonoBehaviour 
{
	[SerializeField]
	private VideoClip clip;

	private void OnEnable()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod());

		if( !GameManager.Instance )
		{
			Debug.LogError("GameManager is null");
			return;
		}

		GameManager.Instance.VideoFileName = clip.name;
		SceneManager.LoadSceneAsync("VR");
	}

}
