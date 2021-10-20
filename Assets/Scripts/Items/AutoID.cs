using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoID : MonoBehaviour
{
    [SerializeField] ItemInfo[] items;

    void Awake ()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].id = i;
        }
    }
}
