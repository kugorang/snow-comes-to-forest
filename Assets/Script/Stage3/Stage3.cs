using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage3 : MonoBehaviour
{

	public GameObject firstCard;
	public GameObject secondCard;
	public GameObject thirdCard;
	public GameObject monster;

	public GameObject monsterHealth3;
	public GameObject monsterHealth2;
	public GameObject monsterHealth1;

	public GameObject textBox;

	public bool getCard = false;
	public bool alreadyHave = false;
	public bool isClear = false;

	private int touchCount = 0;
	
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


		if (other.CompareTag("monster"))
		{
			if (getCard)
			{
				touchCount++;
				getCard = false;
				alreadyHave = false;
			}
			else
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

	private void Update()
	{
		switch (touchCount)
		{
		case 1 :
			monsterHealth3.SetActive(false);
			monsterHealth2.SetActive(true);
			break;
		case 2 :
			monsterHealth2.SetActive(false);
			monsterHealth1.SetActive(true);
			break;
		case 3 :
			monster.SetActive(false);
			isClear = true;
			break;
		default:
			break;
		}
	}

	public void NextBtnPress()
	{
		textBox.SetActive(false);
		this.GetComponent<PlayerPlatformerController>().enabled = true;
	}
}
