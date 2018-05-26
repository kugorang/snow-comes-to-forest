using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage3_modified : MonoBehaviour
{
	public GameObject textBox;

	private bool getCard = false;
	private bool alreadyHave = false;
	private bool isClear = false;
	
	public float animTime = 2f;

	public GameObject fade;
	private Image fadeImage;

	private float start = 1f;
	private float end = 0f;
	private float time = 0f;
    
	private bool isPlaying = false;

	private void Awake()
	{
		fadeImage = fade.GetComponent<Image>();
	}
	
	private void Start()
	{
		StartFadeInAnim();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!alreadyHave)
		{
			if (other.CompareTag("card"))
			{
				getCard = true;
				alreadyHave = true;
				other.gameObject.SetActive(false);
			}
		}


		if (other.CompareTag("teddy"))
		{
			if (isClear)
			{
				//가동교 고쳐지고 앞으로 갈 수 있는 코드
			}
			else
			{
				textBox.SetActive(true);
				this.GetComponent<PlayerPlatformerController>().enabled = false;
			}
		}
		
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		
		if (other.gameObject.tag=="monster")
		{
			if (getCard && other.contacts[0].normal.y>0.9f)
			{
				other.gameObject.SetActive(false);
				getCard = false;
				alreadyHave = false;
			}
			else
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}

	}

	public void NextBtnPress()
	{
		textBox.SetActive(false);
		this.GetComponent<PlayerPlatformerController>().enabled = true;
	}
	
	public void StartFadeInAnim()
	{
		Debug.Log("fade in");
			
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
		this.gameObject.GetComponent<PlayerPlatformerController>().enabled=true;
	}
}
