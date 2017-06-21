using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    private Image _image;
	// Use this for initialization
	void Start ()
	{
	    _image = GetComponent<Image>();
	    _image.fillAmount = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _image.fillAmount += Time.deltaTime;
	    if (_image.fillAmount >= 1.0f)
	    {
            if (PlayerPrefs.HasKey("Guide"))
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
	}
}
