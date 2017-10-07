using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSound : MonoBehaviour 
{
	public static TrackingSound instance;
	private AudioSource audioSource;
	public float timer{get; private set;}
	const float Wait = 5;

	// Use this for initialization
	void Start () 
	{
		instance = this;
	}


	void Update()
	{
		if( timer > 0 )
		{
			timer -= Time.deltaTime;
		}
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

		timer = Wait;
	}
}
