using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParabolaRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer lineRend;
    [SerializeField] Transform startingPosition;
    [SerializeField] float trajectoryDisplayTime; //how long the trajectory display should be shown
    float lastRenderTime;

    void Update()
    {
         if (lastRenderTime + trajectoryDisplayTime < Time.time)
        {
            if (lineRend.positionCount > 0)
            {
                lineRend.positionCount = 0;
            }
        }
    }

    public void SimulateParabola(Vector3 force)
    {
        lineRend.positionCount = 0;
        Vector3[] tempPos = GetBallisticPath(startingPosition.position, force, 1,0.2f, Time.fixedUnscaledDeltaTime, 100);
        lineRend.positionCount = tempPos.Length;
        lineRend.SetPositions(tempPos);
        lastRenderTime = Time.time;
    }

    public static Vector3[] GetBallisticPath(Vector3 startPos, Vector3 forward, float velocity,float drag, float timeResolution, float maxTime = Mathf.Infinity)
    {
        //https://github.com/ToughNutToCrack/TrajectoryPrediction/blob/master/Assets/Scripts/PredictionManager.cs Thanks for this function!
        maxTime = Mathf.Min(maxTime, 35f);
        Vector3[] positions = new Vector3[Mathf.CeilToInt(maxTime / timeResolution)];
        Vector3 velVector = forward * velocity;
        int index = 0;
        Vector3 curPosition = startPos;
        for (float t = 0.0f; t < maxTime; t += timeResolution)
        {

            if (index >= positions.Length)
                break;//rounding error using certain values for maxTime and timeResolution

            positions[index] = curPosition;
            curPosition += velVector * timeResolution;
            velVector += Physics.gravity * timeResolution;
            velVector = velVector / (1 + drag/(1 / Time.fixedUnscaledDeltaTime)); //drag calculation
            index++;
        }
        return positions;
    }
}
