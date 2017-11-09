/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2010-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class CloudRecoTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PUBLIC_MEMBERS
    /// <summary>
    /// The scan-line rendered in overlay when Cloud Reco is in scanning mode.
    /// </summary>
    public ScanLine scanLine;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    private TrackableBehaviour mTrackableBehaviour;
    private GameObject prefab;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.UNKNOWN &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            // Ignore this specific combo
            return;
        }
        else
        {
            OnTrackingLost();
        }
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS
    private void OnTrackingFound()
    {


        // Stop finder since we have now a result, finder will be restarted again when we lose track of the result
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (objectTracker != null)
        {
            objectTracker.TargetFinder.Stop();

            // Stop showing the scan-line
            ShowScanLine(false);


            if (GameManager.Instance.VideoType == 0)
            {
                SceneManager.LoadScene("FullScreen");
            }
			else if (GameManager.Instance.VideoType == 1)
            {
                SceneManager.LoadScene("VR");
            }
			else if (GameManager.Instance.VideoType == 2)
            {
                prefab = Instantiate(Resources.Load(GameManager.Instance.TargetName), this.transform) as GameObject;
            }
			else if (GameManager.Instance.VideoType == 3)
			{
				SceneManager.LoadScene("FullScreenYoutube");
			}
			else if (GameManager.Instance.VideoType == 4)
			{
				SceneManager.LoadScene("VRYoutube");
			}
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");

		RingTargetFoundGingle();
    }

    private void OnTrackingLost()
    {
        //Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        //Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        //// Disable rendering:
        //foreach (Renderer component in rendererComponents)
        //{
        //    component.enabled = false;
        //}

        //// Disable colliders:
        //foreach (Collider component in colliderComponents)
        //{
        //    component.enabled = false;
        //}
        if (prefab != null)
            Destroy(prefab);
        // Start finder again if we lost the current trackable
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (objectTracker != null)
		{
            objectTracker.TargetFinder.ClearTrackables(false);
            objectTracker.TargetFinder.StartRecognition();

            // Start showing the scan-line
            ShowScanLine(true);
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    }

    private void ShowScanLine(bool show)
    {
        // Toggle scanline rendering
        if (scanLine != null)
        {
            Renderer scanLineRenderer = scanLine.GetComponent<Renderer>();
            if (show)
            {
                // Enable scan line rendering
                if (!scanLineRenderer.enabled)
                    scanLineRenderer.enabled = true;

                scanLine.ResetAnimation();
            }
            else
            {
                // Disable scanline rendering
                if (scanLineRenderer.enabled)
                    scanLineRenderer.enabled = false;
            }
        }
    }
    #endregion //PRIVATE_METHODS

	[SerializeField]
	AudioSource audioSource;

	/// <summary>
	/// トラッキングオブジェクトを認識した時の効果音を再生
	/// </summary>
	private void RingTargetFoundGingle()
	{
		if( audioSource == null )
		{
			audioSource = GetComponent<AudioSource>();
		}

		audioSource.Play();
	}
}
