using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特定条件でARシーンに戻る
/// </summary>
public class BackToArScene : MonoBehaviour {

	/// <summary>
	/// 終了処理中か？
	/// </summary>
	private bool inQuitProcess = false;

	[SerializeField]
	private MediaPlayerCtrl mediaPlayer; 

	void Awake()
	{
		inQuitProcess = false;
	}

	// Update is called once per frame
	void Update () 
	{
		if( !mediaPlayer )
		{
			Debug.LogError( gameObject.name + "メディアプレイヤーが未登録");
			return;
		}

		// ビデオの再生が終わったらARシーンに戻る
		if( mediaPlayer.GetCurrentState().Equals(MediaPlayerCtrl.MEDIAPLAYER_STATE.END) )
		{
			Execute();
		}
	}

	public void Execute()
	{
		if( !inQuitProcess )
		{
			inQuitProcess = true;

			UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("AR");
		}
	}
}
