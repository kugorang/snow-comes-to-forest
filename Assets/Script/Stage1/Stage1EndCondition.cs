using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage1EndCondition : MonoBehaviour
{

	public string tag;
	public bool isEnd = false;
	
	public string nextStage;

	public GameObject textCanvas;
	public GameObject sceneManager;

	public Button NextBtn;
    
	public float animTime = 2f;

	public GameObject fade;
	private Image fadeImage;

	private float startFO = 0f;
	private float endFO = 1f;
	private float timeFO = 0f;
	
	private bool isPlaying = false;

	public GameObject endingMessage;

	private void Start()
	{
		fadeImage = fade.GetComponent<Image>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(tag))
		{
			isEnd = true;
		}
	}
	
	private void FixedUpdate()
	{
		if (isEnd)
		{
			textCanvas.SetActive(true);
			this.GetComponent<PlayerPlatformerController>().enabled = false;
			sceneManager.SetActive(false);
		}
	}

	public void NextBtnPress()
	{
		Debug.Log("Btn Press");
		StartFadeOutAnim();
		endingMessage.SetActive(true);
		EndStage();

	}
	
	public void StartFadeOutAnim()
	{
		Debug.Log("fade out");
			
		if (isPlaying == true)
			return;

		StartCoroutine("PlayFadeOut");
	}

	IEnumerator PlayFadeOut()
	{
		isPlaying = true;

		Color color = fadeImage.color;
		timeFO = 0f;
		color.a = Mathf.Lerp(startFO, endFO, timeFO);

		while (color.a < 1f)
		{
			timeFO += Time.deltaTime / animTime;

			color.a = Mathf.Lerp(startFO, endFO, timeFO);

			fadeImage.color = color;

			yield return null;
		}

		isPlaying = false;

	}

	private IEnumerator EndStage()
	{
		yield return new WaitForSeconds(5);
		StartFadeOutAnim();
		SceneManager.LoadScene(nextStage);
	}
	

}
