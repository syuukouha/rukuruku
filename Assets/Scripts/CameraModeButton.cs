using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラモード切替ボタン
/// </summary>
public class CameraModeButton : MonoBehaviour
{
    /// <summary>
    /// 単独カメラ使用時に有効なオブジェクト
    /// </summary>
    [SerializeField]
    private GameObject[] singleCameraObjects;

    /// <summary>
    /// 分割カメラ使用時に有効なオブジェクト
    /// </summary>
    [SerializeField]
    private GameObject[] dualCameraObjects;

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
        foreach (GameObject o in singleCameraObjects)
        {
            o.SetActive(false);
        }
        foreach (GameObject o in dualCameraObjects)
        {
            o.SetActive(false);
        }

        if (mode == 1)
        {
            // 単独カメラ関連オブジェクトを有効
            foreach (GameObject o in singleCameraObjects)
            {
                o.SetActive(true);
            }
        }
        else
        {
            // 単独分割カメラ関連オブジェクトを有効
            foreach (GameObject o in dualCameraObjects)
            {
                o.SetActive(true);
            }
        }
    }
}
