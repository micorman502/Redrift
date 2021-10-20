using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    GameObject tooltip;
    RectTransform tooltipTransform;
    RectTransform tooltipUITransform;
    [SerializeField] Vector3 tooltipUIOffset; //have the X set to 125
    [SerializeField] Text title;
    [SerializeField] Text description;
    void Start()
    {
        tooltip = gameObject;
        tooltipTransform = tooltip.GetComponent<RectTransform>();
        tooltipUITransform = tooltip.GetComponentInChildren<RectTransform>();
    }

    void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            tooltipTransform.position = Input.mousePosition;
            if (tooltip.transform.position.x >= Screen.width / 2)
            {
                SetTextAlignment(TextAnchor.UpperRight);
                tooltipUITransform.position = tooltipTransform.position - tooltipUIOffset;
            }
            else
            {
                SetTextAlignment(TextAnchor.UpperLeft);
                tooltipUITransform.position = tooltipTransform.position + tooltipUIOffset;
            }
        }
    }

    void SetTextAlignment (TextAnchor alignment)
    {
        title.alignment = alignment;
        description.alignment = alignment;
    }
}
