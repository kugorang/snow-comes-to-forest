#region

using UnityEngine;

#endregion

public class LeafSpawner : MonoBehaviour
{
    public float interval;
    public float maxRange;

    public float minRange;
    public string objTag;
    public GameObject objToSpawn;
    private Transform origin;
    public float posY;


    private void Awake()
    {
        origin = GameObject.FindGameObjectWithTag(objTag).GetComponent<Transform>();
    }

    private void Start()
    {
        InvokeRepeating("Spawn", 0f, interval);
    }

    private void Spawn()
    {
        if (origin == null) return;

        var randomX = Random.Range(minRange, maxRange);
        var SpawnPos = origin.position + Random.onUnitSphere * randomX;
        SpawnPos = new Vector3(SpawnPos.x, posY, 0f);
        Instantiate(objToSpawn, SpawnPos, Quaternion.identity);
    }
}