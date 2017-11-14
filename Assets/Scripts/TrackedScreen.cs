using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// トラッキングされる動画制御
/// </summary>
public class TrackedScreen : MonoBehaviour 
{
	[SerializeField]
	private VideoPlayer videoPlayer;

	// Use this for initialization
	void Awake () {
		videoPlayer.url = GameManager.Instance.VideoFileName;

		// ここで、
		// Keep Alpha = true
		// とか、
		// Transcode = true
		// とか、できたらいいね？

	}
}
