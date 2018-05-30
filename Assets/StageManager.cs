using System.Collections;
using BTAI;
using Gamekit2D;
using UnityEngine;

public class StageManager : MonoBehaviour
{
	private bool _active;
	public GameObject Jail;
	public SpriteRenderer FadeImg;
	public Sprite StandImg;
	public GameObject[] LightArr;
	public GameObject[] SoundTile;
	public GameObject[] SpeechBubble;
	public MovingPlatformTest MovingPlatform;
	public BoxCollider2D PressurePad;
	public PlayerPlatformerController Player;
	private bool _stageClear;

	public void Start()
	{
		StartCoroutine("FadeCorutine", true);
		MovingPlatform.movingSpeedx = 0;
		PressurePad.enabled = false;

		foreach (var li in SoundTile)
		{
			li.GetComponent<InteractOnCollision2D>().enabled = false;
		}
	}

	public void StartMoving()
	{
		MovingPlatform.movingSpeedx = 2;
	}

	public void FadeOut()
	{
		StartCoroutine("FadeCorutine", false);
	}

	public void LightOff()
	{
		foreach (var lightTile in LightArr)
		{
			if (lightTile.activeSelf)
				continue;
			
			_active = true;
			break;
		}

		if (!_active)
		{
			Jail.SetActive(false);
			return;
		}
			
		_active = true;
			
		StartCoroutine("SetActiveFalse");
			
		_active = false;
	}

	public void PressurePadOn()
	{
		PressurePad.enabled = true;
		
		foreach (var li in SoundTile)
		{
			li.GetComponent<InteractOnCollision2D>().enabled = true;
		}
	}

	public void TalkStart()
	{
		if (!_stageClear)
			StartCoroutine("Talk");
	}

	public void StageClear()
	{
		_stageClear = true;
	}

	private IEnumerator SetActiveFalse()
	{
		yield return new WaitForSeconds(1.5f);

		foreach (var lightTile in LightArr)
		{
			lightTile.SetActive(false);
		}
	}

	private IEnumerator FadeCorutine(bool isFadeIn)
	{
		var fadeImgColor = FadeImg.color;
		
		while (true)
		{
			if (isFadeIn)
			{
				fadeImgColor.a -= 0.01f;
			}
			else
			{
				fadeImgColor.a += 0.01f;
			}
			
			FadeImg.color = fadeImgColor;

			if (fadeImgColor.a >= 1.0f || fadeImgColor.a <= 0.0f)
				break;

			yield return new WaitForSeconds(0.01f);
		}

		yield return null;
	}

	private IEnumerator Talk()
	{
		Player.GetComponent<Animator>().enabled = false;
		Player.GetComponent<SpriteRenderer>().sprite = StandImg;
		Player.enabled = false;
		
		foreach (var sb in SpeechBubble)
		{
			sb.SetActive(true);
			yield return new WaitForSeconds(2.0f);
			sb.SetActive(false);
		}

		Player.enabled = true;
		Player.GetComponent<Animator>().enabled = true;
		
		yield return null;
	}
}
