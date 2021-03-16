using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, ITakeInput
{
    private string currentInput;
    private StructurePositionReferences posRef;
    [SerializeField] private NegateFallDamage nfd;
    [SerializeField] private float speed;
    [SerializeField] private float direction;
    [SerializeField] private Vector3 elevatorBase;
    [SerializeField] private Vector3 elevatorTop;
    [SerializeField] private Transform hookPoint;
    [SerializeField] private float directionBounds;
    private float smoothedDirection;
    // Start is called before the first frame update
    void Start()
    {
        posRef = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<StructurePositionReferences>();
        UpdatePositions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate ()
    {
        if (direction < 0 && smoothedDirection > -directionBounds)
        {
            smoothedDirection += direction * 0.02f;
        } else if (direction > 0 && smoothedDirection < directionBounds)
        {
            smoothedDirection += direction * 0.02f;
        }

        if (smoothedDirection > 0 && transform.position.y + speed * 0.02 <= elevatorTop.y)
        {
            transform.position += new Vector3(0, smoothedDirection * speed * 0.02f, 0);
        }
        if (smoothedDirection < 0 && transform.position.y - speed * 0.02 >= elevatorBase.y)
        {
            transform.position += new Vector3(0, smoothedDirection * speed * 0.02f, 0);
        }
    }

    public void TakeInput (string input)
    {
        currentInput = input;
        if (input == "up")
        {
            nfd.SetDamageNegate(false);
            direction = 1;
        } else if (input == "down")
        {
            direction = -1;
            nfd.SetDamageNegate(true);
        } else
        {
            direction = 0;
            nfd.SetDamageNegate(false);
        }
        smoothedDirection = 0;
        UpdatePositions();
    }

    public void UpdatePositions ()
    {
        if (posRef.GetPositionReference(Round(hookPoint.position)) != null)
        {
            ElevatorBlock eb = posRef.GetPositionReference(Round(hookPoint.position)).GetComponent<ElevatorBlock>();
            if (eb)
            {
                elevatorBase = eb.GetBase();
                elevatorTop = eb.GetBase() + new Vector3(0, eb.ChainLength(), 0);
            } else
            {

            }
        }
    }

    Vector3 Round (Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    }
}
