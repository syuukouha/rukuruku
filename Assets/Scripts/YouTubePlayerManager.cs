using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// YouTubeの動画再生シーンの制御
/// </summary>
public class YouTubePlayerManager : MonoBehaviour {

	[SerializeField]
	private HighQualityPlayback youtubePayer;

	/// <summary>
	/// GameManagerに値が設定されていなければこれを使う
	/// </summary>
	[SerializeField]
	private string videoFileName;

	/// <summary>
	/// 再生停止処理中か？
	/// </summary>
	private bool inQuitProcess = false;

	/// <summary>
	/// HighQualityPlayback の Start() より前に呼ばれる必要がある
	/// </summary>
	void Awake()
	{
		inQuitProcess = false;

		string fileName = null;
		if( !GameManager.Instance || string.IsNullOrEmpty( GameManager.Instance.VideoFileName )) 
		{
			Debug.LogError("GameManagerが存在しない または VideoFileNameが未定義 " + videoFileName + "を使用する");
			fileName = videoFileName;
		}
		else
		{
			fileName = GameManager.Instance.VideoFileName;
		}

		youtubePayer.videoId = fileName;

		// 再生終了時イベントをセット ARシーンにもどる
		youtubePayer.unityVideoPlayer.loopPointReached += PlaybackDone;
	}

	private void PlaybackDone(UnityEngine.Video.VideoPlayer vPlayer){
		ReturnToARScene ();
	}

	/// <summary>
	/// ARシーンに戻る
	/// </summary>
	public void ReturnToARScene()
	{
		if( inQuitProcess ) return;

		inQuitProcess = true;

		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("AR");
	}

}
