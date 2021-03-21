using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    [SerializeField] bool attachedToTree;
    [SerializeField] Vector3 treeBase;
    [SerializeField] GameObject treeObj;
    // Start is called before the first frame update
    void Start()
    {
        treeObj = GetTreeAt(FindObjectsOfType<TreeResource>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attach (Vector3 tree)
    {
        attachedToTree = true;
        treeBase = tree;
    }

    public bool Attached ()
    {
        return attachedToTree;
    }

    public Vector3 GetTreeBase ()
    {
        return treeBase;
    }

    GameObject GetTreeAt(TreeResource[] trees)
    {
        Vector3 currentPosition = transform.position;
        GameObject target = new GameObject();
        foreach (TreeResource potentialTarget in trees)
        {
            if (potentialTarget.transform.position == treeBase)
            {
                target = potentialTarget.gameObject;
                break;
            }
        }
        return target;
    }
}
