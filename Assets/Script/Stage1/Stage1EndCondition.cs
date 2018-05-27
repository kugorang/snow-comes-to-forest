using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage1EndCondition : MonoBehaviour
{

	public string tag;
	public bool isEnd = false;
	public bool isNextStage;
	
	public string nextStage;

	public GameObject textCanvas;
	public GameObject sceneManager;

	public Button NextBtn;
    
	public float animTime = 2f;

	public GameObject fade;
	private Image fadeImage;
	public GameObject endingfade;
	private Image endingfadeImage;

	private float start = 0f;
	private float end = 1f;
	private float time = 0f;
	
	private bool isPlaying = false;
	private bool sceneload = false;

	public GameObject endingMessage;
	public GameObject endingMessageImage;

	private void Awake()
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
			this.GetComponent<Animator>().enabled = false;
			sceneManager.SetActive(false);
		}		
	}

	private void Update()
	{
		if(isNextStage && Input.GetMouseButtonDown(0))
		{
			SceneManager.LoadScene(nextStage);
			/*sceneload = true;
			StartFadeOutAnim();
			fade.transform.SetAsLastSibling();*/
		}

	}

	public void NextBtnPress()
	{
		StartFadeOutAnim();
	}
	
	public void StartFadeOutAnim()
	{
		if (isPlaying == true)
			return;

		StartCoroutine("PlayFadeOut");
	}

	IEnumerator PlayFadeOut()
	{
		isPlaying = true;

		Color color = fadeImage.color;
		time = 0f;
		color.a = Mathf.Lerp(start, end, time);

		while (color.a < 1f)
		{
			time += Time.deltaTime / animTime;

			color.a = Mathf.Lerp(start, end, time);

			fadeImage.color = color;

			yield return null;
		}

		isPlaying = false;
		SceneManager.LoadScene(nextStage);
		/*if(sceneload)
		{
			SceneManager.LoadScene(nextStage);
		}*/
	}

	private IEnumerator EndStage()
	{
		yield return StartCoroutine(PlayFadeOut());
		endingMessage.SetActive(true);
		yield return new WaitForSeconds(1);
		endingMessageImage.SetActive(true);
		isNextStage = true;
	}
}
