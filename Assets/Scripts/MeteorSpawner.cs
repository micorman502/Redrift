using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour, INeedModifier
{
    [SerializeField] GameObject meteorite;
    [SerializeField] Transform target;
    [SerializeField] Vector3 targetOffset;
    [SerializeField] Vector3 bounds;

    [SerializeField] float minSpawnTime;
    [SerializeField] float maxSpawnTime;
    [SerializeField] float nextSpawnTime;
    [SerializeField] List<string> modifiers = new List<string>(); //what modifiers this will be active on

    public void TakeModifier (string modifier)
    {
        gameObject.SetActive(false);
        foreach (string currentMod in modifiers)
        {
            if (currentMod == modifier)
            {
                gameObject.SetActive(true);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnMeteor();
        }
    }

    public void SpawnMeteor ()
    {
        Vector3 tPos = target.position + targetOffset;
        GameObject meteor = Instantiate(meteorite, new Vector3(tPos.x + Random.Range(-bounds.x, bounds.x), tPos.y + Random.Range(-bounds.y, bounds.y), tPos.z + Random.Range(-bounds.z, bounds.z)) + transform.position, Quaternion.identity);
        meteor.transform.parent = gameObject.transform;
        nextSpawnTime = Time.time + Random.Range(minSpawnTime,maxSpawnTime);
    }
}
