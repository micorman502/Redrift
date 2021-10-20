using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float rareSize;
    float speed;
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 path;
    [SerializeField] Vector3 rarePath;
    [SerializeField] float xzBounds;
    [SerializeField] int penetrationTicks; //how long this can touch an object before being destroyed
    [SerializeField] ParticleSystem particles;
    int currentPenetration;
    bool preloaded;
    // Start is called before the first frame update
    void Start()
    {
        if (!preloaded)
        {
            currentPenetration = penetrationTicks;
            speed = Random.Range(minSpeed, maxSpeed);
            float rnd = Random.Range(minSpeed, maxSpeed);
            if (Random.Range(0f, 10f) > 9.5f)
            {
                transform.localScale = new Vector3(rareSize, rareSize, rareSize);
            }
            else
            {
                transform.localScale = new Vector3(rnd, rnd, rnd);
            }
            if (Random.Range(0f, 10f) > 9.5f)
            {
                path = rarePath;
            }
            else
            {
                path += new Vector3(Random.Range(-xzBounds, xzBounds), 0, Random.Range(-xzBounds, xzBounds));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!rb)
        {
            transform.position += path * speed * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (rb)
        {
            rb.AddForce(path * speed * Time.fixedUnscaledDeltaTime);
        }
        if (transform.position.y <= -300f)
        {
            DestroyMeteorite();
        }
    }

    public void DestroyMeteorite ()
    {
        if (gameObject.transform.parent)
        {
            particles.transform.parent = gameObject.transform.parent;
        } else
        {
            particles.transform.parent = null;
        }
        particles.Emit(15);
        particles.Stop();
        Destroy(particles.gameObject, 10f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        ResourceHandler res = other.GetComponent<ResourceHandler>();
        if (res)
        {
            res.Gather(res.health);
        }
        currentPenetration -= 1;
        if (currentPenetration < 0)
        {
            DestroyMeteorite();
        }
    }

    void OnTriggerStay(Collider other)
    {
        ResourceHandler res = other.GetComponent<ResourceHandler>();
        if (res)
        {
            res.Gather(res.health);
        }
        currentPenetration -= 1;
        if (currentPenetration < 0)
        {
            DestroyMeteorite();
        }
    }

    public Vector3 GetPath ()
    {
        return path;
    }

    public float GetSize ()
    {
        return transform.localScale.x;
    }

    public float GetSpeed ()
    {
        return speed;
    }

    public int GetPticks ()
    {
        return penetrationTicks;
    }

    public void LoadValues (Vector3 thisPath, float thisSize, float thisSpeed, int penetration)
    {
        preloaded = true;
        path = thisPath;
        transform.localScale = new Vector3(thisSize, thisSpeed, thisSize);
        speed = thisSpeed;
        penetrationTicks = penetration;
        currentPenetration = penetration;
    }
}
