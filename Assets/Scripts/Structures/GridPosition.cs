using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private Vector3 position;
    [SerializeField] private bool staticStructure;
    private StructurePositionReferences posRef;

    void Start()
    {
        Setup();
    }

    public void Setup ()
    {
        posRef = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<StructurePositionReferences>();
        position = transform.position;
        if (staticStructure)
        {
            posRef.AddPositionReference(gameObject, transform.position);
        }
    }
    public Vector3 Position ()
    {
        return position;
    }

    public void GetDestroyed()
    {
        if (staticStructure)
        {
            posRef.RemovePositionReference(position);
        }
    }
}
