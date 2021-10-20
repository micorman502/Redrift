using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnacePhysicalManager : MonoBehaviour //solely for reporting collisions to the main furnace.
{
    [SerializeField] private Furnace parentFurnace;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) //Add a rigidbody to the furnaces if items are ever non-physical
    {
        parentFurnace.ReportObject(other);
    }
}
