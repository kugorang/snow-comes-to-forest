using System.Collections;
using System.Linq;
using BTAI;
using Gamekit2D;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	private bool _jailAlreadyOpen;

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

	public void FloorLightOff()
	{
		if (LightArr.All(lightTile => lightTile.activeSelf))
			return;
		
		StartCoroutine("SetActiveFalse");
	}

	public void CheckAllTurnOn()
	{
		if (_jailAlreadyOpen || LightArr.Any(lightTile => !lightTile.activeSelf)) 
			return;

		StartCoroutine("OpenJail");
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
		yield return new WaitForSeconds(1.0f);

		foreach (var lightTile in LightArr)
		{
			lightTile.GetComponentInParent<CollisionSoundTile>().IsTurnOn = false;
			lightTile.SetActive(false);
		}
		
		/*Debug.Log("asdf");*/
	}

	private IEnumerator FadeCorutine(bool isFadeIn)
	{
		var fadeImgColor = FadeImg.color;

		if (!isFadeIn)
		{
			yield return new WaitForSeconds(2.0f);
		}
		
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
		
		if (!isFadeIn)
			SceneManager.LoadScene("Main_modified");

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

	private IEnumerator OpenJail()
	{
		yield return new WaitForSeconds(0.5f);

		if (_jailAlreadyOpen) 
			yield break;
		
		_jailAlreadyOpen = true;
		Jail.SetActive(false);
		GetComponent<AudioSource>().Play();

		/*Debug.Log("OpenJail");*/
	}
}
