using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnOff : MonoBehaviour
{
	private GameObject[] _streetLightOn;
	private GameObject[] _streetLightOff;

	private void Awake()
	{
		_streetLightOn = GameObject.FindGameObjectsWithTag("lightOn");
		_streetLightOff = GameObject.FindGameObjectsWithTag("lightOff");
	}

	// Use this for initialization
	private void Start () 
	{
		foreach (var on in _streetLightOn)
		{
			on.SetActive(false);
		}
	}

	public void On()
	{
		foreach (var on in _streetLightOn)
		{
			on.SetActive(true);
		}

		foreach (var off in _streetLightOff)
		{
			off.SetActive(false);
		}
	}

	public void Off()
	{
		foreach (var on in _streetLightOn)
		{
			on.SetActive(false);
		}

		foreach (var off in _streetLightOff)
		{
			off.SetActive(true);
		}
	}
}