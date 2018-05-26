using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage2_modified : MonoBehaviour
{
	public GameObject textBox;

	public bool getCard = false;
	public bool alreadyHave = false;
	public bool isClear = false;
	
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
                other.gameObject.SetActive(false);
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

	public void NextBtnPress()
	{
		textBox.SetActive(false);
		this.GetComponent<PlayerPlatformerController>().enabled = true;
	}
}
