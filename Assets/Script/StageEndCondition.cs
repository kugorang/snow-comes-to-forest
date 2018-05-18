using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageEndCondition : MonoBehaviour
{

	public string tag;
	public bool isEnd = false;
	
	public string nextStage;

	public GameObject textCanvas;
	public GameObject sceneManager;

	public Button NextBtn;
    
	public UnityEngine.UI.Image fade;
	private float fades = 0.0f;
	private float time = 0;
	private bool isFade = true;
	
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
		SceneManager.LoadScene(nextStage);
	}
	
}
