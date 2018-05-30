using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{

	public float minRange;
	public float maxRange;
	public float interval;
	public float posY;
	public string objTag;
	public GameObject objToSpawn = null;
	private Transform origin = null;
	


	private void Awake()
	{
		origin = GameObject.FindGameObjectWithTag(objTag).GetComponent<Transform>();
	}

	private void Start()
	{
		InvokeRepeating("Spawn",0f,interval);
	}

	void Spawn()
	{
		if (origin == null) return;
		
		float randomX = Random.Range(minRange, maxRange);
		Vector3 SpawnPos = origin.position + Random.onUnitSphere * randomX;
		SpawnPos = new Vector3(SpawnPos.x, posY, 0f);
		Instantiate(objToSpawn, SpawnPos, Quaternion.identity);
	}
}
