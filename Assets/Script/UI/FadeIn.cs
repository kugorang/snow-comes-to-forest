using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeIn : MonoBehaviour
{

	public float animTime = 2f;

	private Image fadeImage;

	private float start = 1f;
	private float end = 0f;
	private float time = 0f;

	private bool isPlaying = false;

	void Awake()
	{
		fadeImage = GetComponent<Image>();
	}

	public void StartFadeAnim()
	{
		Debug.Log("버튼 눌림");
			
		if (isPlaying == true)
			return;

		StartCoroutine("PlayFadeIn");
	}

	IEnumerator PlayFadeIn()
	{
		isPlaying = true;

		Color color = fadeImage.color;
		time = 0f;
		color.a = Mathf.Lerp(start, end, time);

		while (color.a > 0f)
		{
			time += Time.deltaTime / animTime;

			color.a = Mathf.Lerp(start, end, time);

			fadeImage.color = color;

			yield return null;
		}

		isPlaying = false;
	}
	
}
