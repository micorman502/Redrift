using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBlock : MonoBehaviour
{
    [SerializeField] private float chainLength;
    [SerializeField] private Vector3 chainBase;
    private StructurePositionReferences posRef;
    private GridPosition gr;
    private int lastUpdate = -10; //last time this got an update
    private Elevator attachedElevator;
    // Start is called before the first frame update
    void Start()
    {
        gr = gameObject.GetComponent<GridPosition>();
        if (posRef == null)
        {
            posRef = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<StructurePositionReferences>();
            StartUpdate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosRef (StructurePositionReferences thisPosRef) //for the save manager to use
    {
        posRef = thisPosRef;
        StartUpdate();
    }

    public void AttachElevator (Elevator elevator)
    {
        attachedElevator = elevator;
    }



    public void UpdateSides ()
    {
        GameObject above = posRef.GetPositionReference(gr.Position() + Vector3.up);
        if (above != null)
        {
            ElevatorBlock block = above.GetComponent<ElevatorBlock>();
            if (block != null && block.transform.rotation == transform.rotation)
            {
                //block.();
            }
        }

        GameObject below = posRef.GetPositionReference(gr.Position() + Vector3.down);
        if (below != null)
        {
            ElevatorBlock block = below.GetComponent<ElevatorBlock>();
            if (block != null && block.transform.rotation == transform.rotation)
            {
                //block.UpdateAllSelf();
            } else
            {
                chainBase = gr.Position();
                GameObject abovealt = posRef.GetPositionReference(gr.Position() + Vector3.up);
                if (above != null)
                {
                    ElevatorBlock blockabove = above.GetComponent<ElevatorBlock>();
                    if (blockabove != null && blockabove.transform.rotation == transform.rotation)
                    {
                        block.UpdateAbove(gr.Position());
                    }
                }
            }
        }
        UpdateElevator();
    }

    public void UpdateBelow()
    {
        GameObject below = posRef.GetPositionReference(gr.Position() + Vector3.down);
        if (below != null)
        {
            ElevatorBlock block = below.GetComponent<ElevatorBlock>();
            if (block != null && block.transform.rotation == transform.rotation)
            {
                block.UpdateBelow();
            } else
            {
                chainBase = gr.Position();
                UpdateAbove(gr.Position());
            }
        } else
        {
            chainBase = gr.Position();
            UpdateAbove(gr.Position());
        }
        UpdateElevator();
    }

    public void UpdateBelowChainLength (int thisChainLength)
    {
        GameObject below = posRef.GetPositionReference(gr.Position() + Vector3.down);
        if (below != null)
        {
            ElevatorBlock block = below.GetComponent<ElevatorBlock>();
            if (block != null && block.transform.rotation == transform.rotation)
            {
                chainLength = thisChainLength;
                block.UpdateBelowChainLength(thisChainLength);
            } else
            {
                chainLength = thisChainLength;
            }
        }
        else
        {
            chainLength = thisChainLength;
        }
        UpdateElevator();
    }

    public void UpdateAbove (Vector3 thisChainBase)
    {
        chainBase = thisChainBase;
        GameObject above = posRef.GetPositionReference(gr.Position() + Vector3.up);
        if (above != null)
        {
            ElevatorBlock block = above.GetComponent<ElevatorBlock>();
            if (block != null && block.transform.rotation == transform.rotation)
            {
                block.UpdateAbove(thisChainBase);
            } else
            {
                this.UpdateBelowChainLength(Mathf.RoundToInt(gr.Position().y - chainBase.y));
            }
        } else
        {
            this.UpdateBelowChainLength(Mathf.RoundToInt(gr.Position().y - chainBase.y));
        }
        UpdateElevator();
    }

    public void UpdateElevator ()
    {
        if (attachedElevator)
        {
            attachedElevator.UpdatePositions();
        }
    }

    public void GetDestroyed ()
    {
        UpdateAbove(gr.Position() + Vector3.up);
        UpdateBelow();
        UpdateElevator();
    }

    public void StartUpdate()
    {
        if (lastUpdate != Time.frameCount && posRef != null && gr)
        {
            lastUpdate = Time.frameCount;
            UpdateBelow();
        }
    }

    public bool IsBase ()
    {
        if (chainBase == gr.Position())
        {
            return true;
        } else
        {
            return false;
        }
    }

    public Vector3 GetBase ()
    {
        return chainBase;
    }

    public float ChainLength ()
    {
        return chainLength;
    }
}
