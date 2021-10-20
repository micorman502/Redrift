using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public Item tempItem;
    public ItemInfo item;
    public GameObject itemGameObject;

    void Awake()
    {
        itemGameObject.SetActive(false);
    }

    public virtual void Use()
    {

    }

    public virtual void UseRepeating()
    {

    }

    public virtual void AltUse()
    {

    }


    public virtual void AltUseRepeating()
    {

    }

    public virtual void StopAltUse()
    {

    }

    public virtual void SpecialUse()
    {

    }

    public virtual void SetChildState(bool _state)
    {
        if (itemGameObject)
        {
            itemGameObject.SetActive(_state);
        }
    }
}
