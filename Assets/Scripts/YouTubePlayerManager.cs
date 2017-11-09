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
	/// １つのカメラで動画を見る時に使用するオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject[] singleCameraObjects;

	/// <summary>
	/// ２つのカメラで動画を見る時に使用するオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject[] dualCameraObjects;

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

		LoadCameraMode();

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

	/// <summary>
	/// カメラモード読み込み
	/// </summary>
	private void LoadCameraMode()
	{
		string key = "CameraMode";
		int cameraMode = 1;
		if (PlayerPrefs.HasKey(key))
		{
			cameraMode = PlayerPrefs.GetInt(key);
		}

		SetCameraModeObjects(cameraMode);
	}

	/// <summary>
	/// カメラモードでオブジェクトの有効/無効を切り替え
	/// </summary>
	private void SetCameraModeObjects(int mode)
	{
		// いったんすべて無効
		foreach (GameObject o in singleCameraObjects)
		{
			o.SetActive(false);
		}
		foreach (GameObject o in dualCameraObjects)
		{
			o.SetActive(false);
		}

		// モードに応じて対応オブジェクトを有効にする
		if (mode == 1)
		{
			foreach (GameObject o in singleCameraObjects)
			{
				o.SetActive(true);
			}
		}
		else
		{
			foreach (GameObject o in dualCameraObjects)
			{
				o.SetActive(true);
			}
		}
	}

	/// <summary>
	/// カメラモードの切り替え
	/// 現在のモードをセーブ
	/// </summary>
	public void SwitchCameraMode(int newMode)
	{
		SetCameraModeObjects(newMode);

		PlayerPrefs.SetInt("CameraMode", newMode);
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
