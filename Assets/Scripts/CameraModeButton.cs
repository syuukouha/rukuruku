using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラモード切替ボタン
/// </summary>
public class CameraModeButton : MonoBehaviour
{
    /// <summary>
    /// 単独カメラ
    /// </summary>
    [SerializeField]
    private Camera singleCamera;

    /// <summary>
    /// 単独カメラ
    /// </summary>
    [SerializeField]
    private GameObject singleCameraButton;

    /// <summary>
    /// 分割カメラ
    /// </summary>
    [SerializeField]
    private Camera[] dualCamera;

    /// <summary>
    /// 分割カメラ
    /// </summary>
    [SerializeField]
    private GameObject dualCameraButton;

    private void Start()
    {
        LoadCameraMode();
    }

    /// <summary>
    /// 保存されたカメラモードを読み込む
    /// </summary>
    private void LoadCameraMode()
    {
        string key = "CameraMode";
        int cameraMode = 1;
        if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 2)
        {
            cameraMode = 2;
        }

        SetCameraModeObjects(cameraMode);
    }

    /// <summary>
    /// カメラモード変更
    /// UIボタンからの呼び出しを想定
    /// </summary>
    /// <param name="newMode"></param>
    public void SwitchCameraMode(int newMode)
    {
        // 有効なオブジェクト切り替え
        SetCameraModeObjects(newMode);

        // 保存
        PlayerPrefs.SetInt("CameraMode", newMode);
    }

    /// <summary>
    /// 有効なカメラ関連オブジェクトを切り替え
    /// </summary>
    private void SetCameraModeObjects(int mode)
    {
        singleCamera.enabled = false;        
        foreach (Camera c in dualCamera)
        {
            c.enabled = false;
        }
        singleCameraButton.SetActive(false);
        dualCameraButton.SetActive(false);


        if (mode == 1)
        {
            // 単独カメラ関連オブジェクトを有効
            singleCamera.enabled = true;
            dualCameraButton.SetActive(true);
        }
        else
        {
            // 単独分割カメラ関連オブジェクトを有効
            foreach (Camera c in dualCamera)
            {
                c.enabled = true;
                singleCameraButton.SetActive(true);
            }
        }
    }
}
