using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSound : MonoBehaviour 
{
	public static TrackingSound instance;
	private AudioSource audioSource;

	// Use this for initialization
	void Start () 
	{
		instance = this;
	}

	/// <summary>
	/// トラッキングオブジェクトを認識した時の効果音を再生
	/// </summary>
	public void RingTargetFoundGingle()
	{
		if( audioSource == null )
		{
			audioSource = GetComponent<AudioSource>();
		}

		audioSource.Play();
	}
}
