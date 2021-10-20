using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBar : MonoBehaviour
{
    [SerializeField] Transform barBackground = null;
    [SerializeField] Vector3 barOffset;
    [SerializeField] GameObject barFill = null;
    Vector3 initialScale;
    float percent;
    bool initialised = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateBar ()
    {
        Init();
        barFill.transform.localScale = barBackground.localScale - new Vector3(0, barBackground.localScale.y * (1 - percent), 0);
        barFill.transform.position = (barBackground.position + barOffset) - new Vector3(0, initialScale.y - barFill.transform.localScale.y, 0);
    }

    public void SetPercentNormalized (float thisPercent) //takes a value from 0 - 1
    {
        percent = Mathf.Clamp(thisPercent + 0.01f, 0, 1);
        UpdateBar();
    }

    public void SetPercent (float thisPercent) //takes a value from 0 - 100
    {
        percent = Mathf.Clamp(thisPercent / 100 + 0.01f, 0, 1);
        UpdateBar();
    }

    void Init()
    {
        if (!initialised)
        {
            initialScale = barBackground.localScale;
            initialised = true;
        }
    }
}
