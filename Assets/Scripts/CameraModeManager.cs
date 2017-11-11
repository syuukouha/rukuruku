using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラモードの切り替え・保存・読み込みを行う
/// 意図：プラグインのソースに直書きで追記されていた処理を切り分ける
/// </summary>
public class CameraModeManager : MonoBehaviour 
{
	/// <summary>
	/// 単独カメラ
	/// </summary>
	[SerializeField]
	private Camera singleCamera;

	/// <summary>
	/// 単独カメラボタン
	/// </summary>
	[SerializeField]
	private GameObject singleCameraButton;

	/// <summary>
	/// 分割カメラ
	/// </summary>
	[SerializeField]
	private Camera[] dualCamera;

	/// <summary>
	/// 分割カメラボタン
	/// </summary>
	[SerializeField]
	private GameObject dualCameraButton;

	/// <summary>
	/// ARシーンはカメラの構造が特殊なため若干処理を帰る必要がある（？）
	/// </summary>
	[SerializeField]
	private bool isArScene = false;

	void Start()
	{
		LoadCameraMode();
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
		// 一旦すべて無効
		{
			if( isArScene )
			{
				singleCamera.enabled = false;        
				foreach (Camera c in dualCamera)
				{
					c.enabled = false;
				}
			}
			else
			{
				singleCamera.gameObject.SetActive( false );        
				foreach (Camera c in dualCamera)
				{
					c.gameObject.SetActive(false);
				}
			}

			singleCameraButton.SetActive(false);
			dualCameraButton.SetActive(false);
		}

		// 指定モードで特定のオブジェクトを有効にする
		{
			if (mode == 1)
			{
				// 単独カメラ関連オブジェクトを有効
				if( isArScene )
				{
					singleCamera.enabled = true;
				}
				else
				{
					singleCamera.gameObject.SetActive( true );
				}
				dualCameraButton.SetActive(true);
			}
			else
			{
				// 単独分割カメラ関連オブジェクトを有効
				if( isArScene )
				{
					foreach (Camera c in dualCamera)
					{
						c.enabled = true;
					}
				}
				else
				{
					foreach (Camera c in dualCamera)
					{
						c.gameObject.SetActive(true);
					}
				}
				singleCameraButton.SetActive(true);
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
}
