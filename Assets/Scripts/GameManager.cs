using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance{ get; private set;}
    public int VideoType;
    public string VideoFileName;
    public string TargetName;


    void Awake()
    {
		if( Instance )
		{
			Debug.Log("GameManager is already exist. desroy second one");
			DestroyImmediate(this.gameObject);
			return;
		}
        Instance = this;
    }
	// Use this for initialization
	void Start ()
	{
	    DontDestroyOnLoad(this.gameObject);
	}
}
[Serializable]
public class MetaData
{
    public int Type;
    public string URL;
    public string TargetName;
}
