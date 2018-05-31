using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class Stage1 : MonoBehaviour {

    public GameObject girlfriend;
    public GameObject girlfriend_1;
    public GameObject bus;
    public GameObject controlPanel1;
    public GameObject controlPanel2;
    public GameObject player;
    public GameObject exclamation;
	public GameObject manChat;
	public GameObject womanChat;

	public GameObject spawner_up;
	public GameObject spawner_down;
	public GameObject leaf;

	public string nextStage;

	public GameObject playerCamera;

	public GameObject bgm;
	private AudioSource bgmSource;
	public GameObject busSound;
	private AudioSource busSoundSource;
	public GameObject sigh;
	private AudioSource sighSource;

	public Sprite manStand;

    public float girlSpeed;
    public float busSpeed;

    private bool panel2On = false;
	private bool panel2Off = false;
    private bool isSurprised = true;
	private bool bgmOn = false;
    
    public float animTime = 3f;

    public GameObject fade;
    private Image fadeImage;

    private float start = 1f;
    private float end = 0f;
    private float time = 0f;
    
    private bool isPlaying = false;
	private bool playOneTime = true;
	private bool isEnd = false;
	private bool isFail = false;

    private void Awake()
    {
        fadeImage = fade.GetComponent<Image>();
		bgmSource = bgm.GetComponent<AudioSource>();
		busSoundSource = busSound.GetComponent<AudioSource>();
	    sighSource = sigh.GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartFadeInAnim();
    }

    private void FixedUpdate () {
       
        if(player.transform.position.x < 6)
        {
            controlPanel1.SetActive(false);
        }

        if (player.transform.position.x <= -6)
        {
            if (!panel2On)
            {
	            player.GetComponent<SpriteRenderer>().sprite = manStand;
                player.GetComponent<PlayerPlatformerController>().enabled = false;
                player.GetComponent<Animator>().enabled = false;
                if (isSurprised)
                {
                    exclamation.SetActive(true);
                }
                StartCoroutine(MeetGirl());
            }
            else
            {
                player.GetComponent<PlayerPlatformerController>().enabled = true;
                player.GetComponent<Animator>().enabled = true;
                girlfriend.SetActive(false);
                girlfriend_1.SetActive(true);
	            if (!panel2Off)
	            {
		            controlPanel2.SetActive(true);
	            }
            }
        }

        if (panel2On)
        {
	        StartCoroutine("BusFirstMove");

            if (player.transform.position.x > -2)
            {
                controlPanel2.SetActive(false);
	            panel2Off = true;
            }
        }

		if(bgmOn)
		{
			if(bgmSource.isPlaying==false)
			{
				bgmSource.Play();
			}
		}

		if(isEnd&&player.transform.position.x>=2)
		{
			StopCoroutine("BusFirstMove");
			player.GetComponent<SpriteRenderer>().sprite = manStand;
			player.GetComponent<PlayerPlatformerController>().enabled = false;
			player.GetComponent<Animator>().enabled = false;
			//spawner_up.GetComponent<LeafSpawner>().enabled = true;
			//spawner_down.GetComponent<LeafSpawner>().enabled = true;
			StartCoroutine("Chat");
			
			StartCoroutine("SuccessStage1");

		}
    }

     private IEnumerator BusFirstMove()
    {
	    yield return new WaitForSeconds(10);
	    bus.SetActive(true);
	    bgmOn = false;
	    StartCoroutine(BGMFadeOut(bgmSource, 10));
		    
	    if(busSoundSource.isPlaying==false)
	    {   
		    busSoundSource.Play();
	    }

	    if (bus.transform.position.x >= 3)
	    {
		    bus.transform.Translate(Vector2.left * busSpeed * Time.deltaTime, Space.World);
	    }
	    else StartCoroutine("BusSecondMove");

    }

    private IEnumerator BusSecondMove()
    {
        yield return new WaitForSeconds(5);
		girlfriend_1.SetActive(false);
	    isFail = true;
	    isEnd = false;
		yield return new WaitForSeconds(5);
	    player.GetComponent<SpriteRenderer>().sprite = manStand;
	    player.GetComponent<PlayerPlatformerController>().enabled = false;
	    player.GetComponent<Animator>().enabled = false;
	    leaf.SetActive(false);
	    
	    if (isLeft(player,bus))
	    {
		    player.GetComponent<SpriteRenderer>().flipX = false;
	    }
	    else
	    {
		    player.GetComponent<SpriteRenderer>().flipX = true;
	    } 
		bus.transform.Translate(Vector2.left * busSpeed * Time.deltaTime, Space.World);

        if(bus.transform.position.x<=-11)
        {
	        StartCoroutine("FailGame");
        }
    }

    private bool isLeft(GameObject baseobj, GameObject otherobj)
    {
        if (baseobj.transform.position.x - otherobj.transform.position.x >= 0)
            return true;
        else return false;
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
        player.GetComponent<PlayerPlatformerController>().enabled = true;
        controlPanel1.SetActive(true);
    }
	
	public void StartFadeOutAnim()
	{
		Debug.Log("fade out");
			
		if (isPlaying == true)
			return;

		StartCoroutine("PlayFadeOut");
		
		playOneTime = false;
		
	}

	IEnumerator PlayFadeOut()
	{
		isPlaying = true;

		Color color = fadeImage.color;
		time = 0f;
		color.a = Mathf.Lerp(end, start, time);

		while (color.a < 1f)
		{
			time += Time.deltaTime / animTime;

			color.a = Mathf.Lerp(end, start, time);

			fadeImage.color = color;

			yield return null;
		}

		isPlaying = false;
		
		if (isEnd)
		{
			SceneManager.LoadScene(nextStage);
		}
		else if (isFail)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

	}

    IEnumerator MeetGirl()
    {
        yield return new WaitForSeconds(1);
        exclamation.SetActive(false);
        isSurprised = false;
		bgmOn = true;

        if (girlfriend.transform.position.x<=3)
        {

			girlfriend.transform.Translate(Vector2.right * girlSpeed * Time.deltaTime, Space.World);
            if (isLeft(player,girlfriend))
            {
                player.GetComponent<SpriteRenderer>().flipX = false;
			}
            else
            {
	            player.GetComponent<SpriteRenderer>().flipX = true;
	            playerCamera.GetComponent<CinemachineVirtualCamera>().Follow = girlfriend.transform;
	            
            }                    
        }
        else
        {
			playerCamera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
			panel2On = true;
			isEnd = true;
        }
    }

	public static IEnumerator BGMFadeOut (AudioSource audioSource, float FadeTime)
	{
		float startVolume = audioSource.volume;

		while(audioSource.volume>0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = startVolume;
	}

	private IEnumerator Chat()
	{
		yield return new WaitForSeconds(0.5f);
		StartCoroutine("ManChat");
		yield return new WaitForSeconds(0.5f);
		StartCoroutine("WomanChat");

		StopCoroutine("Chat");
	}

	private IEnumerator ManChat()
	{
		manChat.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		manChat.SetActive(false);
		yield return new WaitForSeconds(0.5f);

		StopCoroutine("ManChat");
	}
	private IEnumerator WomanChat()
	{
		womanChat.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		womanChat.SetActive(false);
		yield return new WaitForSeconds(0.5f);

		StopCoroutine("WomanChat");
	}

	private IEnumerator FailGame()
	{		
		BGMFadeOut(busSoundSource, 5);
		yield return new WaitUntil(BusSoundPlay);
		sighSource.Play();

		if (playOneTime)
		{
			StartFadeOutAnim();
		}
	}

	private bool BusSoundPlay()
	{
		if (busSoundSource.isPlaying)
		{
			return true;
		}
		else return false;
	}

	private IEnumerator SuccessStage1()
	{
		yield return new WaitForSeconds(3);
		if (playOneTime)
		{
			StartFadeOutAnim();
		}
	}

}
