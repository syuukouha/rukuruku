using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get { return _instance; }
    }
    public int VideoType;
    public string VideoFileName;

    void Awake()
    {
        _instance = this;
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
}
