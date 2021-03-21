using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonLauncher : MonoBehaviour, ITakeInput
{
    public TellParent tellParent;
    public Vector3 velocity;
    [SerializeField] float velocityMult;
    [SerializeField] Transform forwardDir;
    [SerializeField] GameObject automationIndicator;
    [SerializeField] ParabolaRenderer parabola;
    [SerializeField] ParticleSystem particles;
    [SerializeField] Animation launchAnim;
    [SerializeField] float baseForceMult;
    [SerializeField] float forceIncrement;
    private bool automated = false;
    [SerializeField] int internalForceSetting = 1; //from 1 to 10
    void Start()
    {

        velocity = forwardDir.forward * velocityMult;
        automationIndicator.SetActive(false);
        particles.Stop();
        automationIndicator.SetActive(automated);
    }


    void Update()
    {
       
    }

    public void TakeInput (string input)
    {
        if (input == "launch")
        {
            Launch();
        }
        if (input == "automate")
        {
            automated = !automated;
            if (automated == true)
            {
                Launch();
            }
            automationIndicator.SetActive(automated);
        }
        if (input == "autolaunch")
        {
            if (automated)
            {
                Launch();
            }
        }
        if (input == "increaseforce")
        {
            IncreaseInternalForce();
            parabola.SimulateParabola(velocity * baseForceMult);
        }
        if (input == "decreaseforce")
        {
            DecreaseInternalForce();
            parabola.SimulateParabola(velocity * baseForceMult);
        }
    }

    void Launch ()
    {
        launchAnim.Play();
        particles.Play();
        Collider[] launchables = tellParent.Colliders();
        foreach (Collider launchable in launchables)
        {
            GameObject obj = launchable.gameObject;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForce(velocity * baseForceMult, ForceMode.VelocityChange);
            }
        }
    }

    void IncreaseInternalForce ()
    {
        if (internalForceSetting < 10)
        {
            internalForceSetting++;
        } else
        {
            internalForceSetting = 10;
        }
        baseForceMult = forceIncrement * internalForceSetting;
    }

    void DecreaseInternalForce ()
    {
        if (internalForceSetting > 1)
        {
            internalForceSetting--;
        }
        else
        {
            internalForceSetting = 1;
        }
        baseForceMult = forceIncrement * internalForceSetting;
    }

    public int GetInternalForce ()
    {
        return internalForceSetting;
    }

    public void LoadInternalForce (int force)
    {
        internalForceSetting = force;
        if (internalForceSetting > 10)
        {
            internalForceSetting = 10;
        }
        if (internalForceSetting < 1)
        {
            internalForceSetting = 1;
        }
        baseForceMult = forceIncrement * internalForceSetting;
    }

    public bool GetAutomationState ()
    {
        return automated;
    }

    public void LoadAutomationState (bool state)
    {
        automated = state;
        automationIndicator.SetActive(automated);
    }

    Vector3 MultiplyVector3 (Vector3 v1, Vector3 v2)
    {
        Vector3 v3 = new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        return v3;
    }
}
