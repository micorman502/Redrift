using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITooltip
{
    string GetTooltip();
    void SetTooltip(string tooltip);

    /*
    [SerializeField] string tooltip;
    public string GetTooltip () {
        return tooltip;
    }

    public void SetTooltip (string _tooltip) {
        tooltip = _tooltip;
    } */
}
