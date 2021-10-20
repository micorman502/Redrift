using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialObjectResizer : MonoBehaviour
{
    [SerializeField] private Vector3 resizeBounds; //from a recommended range of 0.01 to 1 for each value.
    void Start()
    {
        gameObject.transform.localScale = new Vector3(RandomRange(0), RandomRange(1), RandomRange(2));
        Destroy(this);
    }

    float RandomRange(int index)
    {
        float outN = 0;
        if (index == 0) {
            outN = Random.Range(1 - resizeBounds.x, 1 + resizeBounds.x);
        } else if (index == 1)
        {
            outN = Random.Range(1 - resizeBounds.y, 1 + resizeBounds.y);
        } else if (index == 2)
        {
            outN = Random.Range(1 - resizeBounds.z, 1 + resizeBounds.z);
        }
        return outN;
    }
}
