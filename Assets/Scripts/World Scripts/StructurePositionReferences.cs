using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePositionReferences : MonoBehaviour
{
    [SerializeField] private Dictionary<string, GameObject> positions = new Dictionary<string, GameObject>();
    public void AddPositionReference(GameObject thisObject, Vector3 position)
    {
        if (!positions.ContainsKey(position.ToString()))
        {
            positions.Add(position.ToString(), thisObject);
        }
    }

    public void RemovePositionReference(Vector3 position)
    {
        positions.Remove(position.ToString());
    }

    public GameObject GetPositionReference (Vector3 position)
    {
        if (positions.ContainsKey(position.ToString()))
        {
            return positions[position.ToString()];
        } else
        {
            return null;
        }
    }
}
