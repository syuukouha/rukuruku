using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayTrigger : MonoBehaviour 
{
	[SerializeField]
	private string fileName;
	bool inExecProcess = false;

	void Start()
	{
		inExecProcess = false;
	}

	private void OnEnable()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod());

		if( !GameManager.Instance )
		{
			Debug.LogError("GameManager is null");
			return;
		}

		StartCoroutine( WaitAndExec() );
	}
		
	private IEnumerator WaitAndExec()
	{
		if( inExecProcess )
		{
			Debug.Log("すでに処理中なので中断");
			yield break;
		}

		inExecProcess = true;

		// 効果音が鳴り終わってから処理
		while( TrackingSound.instance && TrackingSound.instance.timer > 0 )
		{
			yield return null;
		}

		GameManager.Instance.VideoFileName = fileName;
		SceneManager.LoadSceneAsync("VR");
	}

}
