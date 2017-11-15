using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// トラッキングされる動画制御
/// </summary>
public class TrackedScreen : MonoBehaviour 
{
	/// <summary>
	/// true = 常にVideoPlayerコンポーネントを使用する
	/// false = VideoPlayerコンポーネントを使用しなければいけない時のみ使用し、それ以外の時はEasyMovieTextureコンポーネントを使用する
	/// </summary>
	[SerializeField]
	private bool alwaysUseVideoPlayer;

	[SerializeField]
	private MediaPlayerCtrl mediaPlayerCtrl;

	[SerializeField]
	private SimplePlayback simplePlayback;

	// Use this for initialization
	void Awake () 
	{
		if( !GameManager.Instance || string.IsNullOrEmpty( GameManager.Instance.VideoFileName ) )
		{
			Debug.LogError( "VideoFileName has not been defined" );
			return;
		}

		var fileName = GameManager.Instance.VideoFileName;

		// YouTubeの動画かどうか判別
		var isYouTubeVideo = IsYouTubeVideo( fileName );

		// VideoPlayerコンポーネントを使うか？
		var useUnityVideoPlayerFlag = isYouTubeVideo || alwaysUseVideoPlayer;

		simplePlayback.gameObject.SetActive( useUnityVideoPlayerFlag );
		mediaPlayerCtrl.gameObject.SetActive( !useUnityVideoPlayerFlag );

		if( useUnityVideoPlayerFlag )
		{
			if( isYouTubeVideo ){
				simplePlayback.Start();
				simplePlayback.PlayYoutubeVideo( GameManager.Instance.VideoFileName );
			}
			else
			{
				var videoPlayer = simplePlayback.unityVideoPlayer;
				videoPlayer.url = GameManager.Instance.VideoFileName;

				simplePlayback.enabled = false;
			}
		}
		// ここで、
		// Keep Alpha = true
		// とか、
		// Transcode = true
		// とか、できたらいいね？
	}

	/// <summary>
	/// ファイル名でYouTubeの動画かどうか判別
	/// </summary>
	/// <returns><c>true</c> if this instance is you tube video the specified fileName; otherwise, <c>false</c>.</returns>
	/// <param name="fileName">File name.</param>
	private bool IsYouTubeVideo( string fileName )
	{
		if( !fileName.Contains("/") && !fileName.Contains(".") )
		{
			// "/"なしかつ"."なしはYouTubeの動画IDのみの指定と解釈
			return true;
		}
		else if( fileName.Contains("youtube") )
		{
			return true;
		}

		return false;
	}
}
