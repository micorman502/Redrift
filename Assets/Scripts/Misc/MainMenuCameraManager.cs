using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraManager : MonoBehaviour
{
    [SerializeField] Animator cameraAnimator;
    [SerializeField] int minGraphicsQuality;

    void Start()
    {
        UpdateMenuCam();
    }

    public void UpdateMenuCam ()
    {
        if (PlayerPrefs.GetInt("Settings.Graphics", 0) >= minGraphicsQuality) {
            cameraAnimator.SetBool("Background Anim", true);
        } else
        {
            cameraAnimator.SetBool("Background Anim", false);
        }
    }
}
