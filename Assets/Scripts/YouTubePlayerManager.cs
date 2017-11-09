using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouTubePlayerManager : MonoBehaviour {

	[SerializeField]
	private HighQualityPlayback youtubePayer;

	/// <summary>
	/// GameManagerに値あ設定されていなければこれを使う
	/// </summary>
	[SerializeField]
	private string videoFileName;


	/// <summary>
	/// HighQualityPlayback の Start() より前に呼ばれる必要がある
	/// </summary>
	void Awake()
	{
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
	}

}
