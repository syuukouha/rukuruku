using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Guide : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        PlayerPrefs.SetInt("Guide", 1);
	}
    public void OnClick()
    {
        SceneManager.LoadScene(2);
    }
}
