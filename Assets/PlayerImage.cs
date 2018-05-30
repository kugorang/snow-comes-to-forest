using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImage : MonoBehaviour
{
	private SpriteRenderer _characterSprite;
	private SpriteRenderer _itemSprite;
	
	// Use this for initialization
	void Start ()
	{
		 _characterSprite = transform.parent.GetComponent<SpriteRenderer>();
		_itemSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_characterSprite.flipX)
		{
			_itemSprite.flipY = true;
		}
		else
		{
			_itemSprite.flipY = false;
		}
	}
}