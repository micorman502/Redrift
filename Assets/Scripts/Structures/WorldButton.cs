using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldButton : MonoBehaviour, IWorldInteractable
{
    [SerializeField] private Component receiver;
    [SerializeField] private string input;
    [SerializeField] private string displayText;

    public void Interact ()
    {
        if (receiver.GetComponent<ITakeInput>() != null)
        {
            receiver.GetComponent<ITakeInput>().TakeInput(input);
        }
    }

    public string DisplayText ()
    {
        return displayText;
    }
}
