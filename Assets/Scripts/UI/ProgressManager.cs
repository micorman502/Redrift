using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    [SerializeField] private Text progressText;
    [SerializeField] private Image progressImage;
    private float progress;
    private float time;
    public void UpdateProgress (float amount, float thisTime)
    {
        progress = amount;
        time = thisTime;
        progressText.text = (Mathf.Round(time*10) / 10).ToString();
        progressImage.fillAmount = amount;
    }

    public void EnableUI ()
    {
        gameObject.SetActive(true);
    }

    public void DisableUI ()
    {
        gameObject.SetActive(false);
        progressText.text = "0";
        progressImage.fillAmount = 0;
    }

    public bool UIEnabled ()
    {
        return gameObject.activeSelf;
    }
}
