using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayTrigger : MonoBehaviour 
{
	[SerializeField]
	private string fileName;

	private void OnEnable()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod());

		if( !GameManager.Instance )
		{
			Debug.LogError("GameManager is null");
			return;
		}

		GameManager.Instance.VideoFileName = fileName;
		SceneManager.LoadSceneAsync("VR");
	}

}
