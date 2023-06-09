using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Score : MonoBehaviour
{
	public newAI2 player;
	public checkShit gh;
	public Slider scoreText;
	public GameObject restartText;
	public Image image;
	public GameObject tutorial;
	public GameObject tutorial2;
	public GameObject welcomeTutorial;
	public GameObject completeTutorial;
	public int tutorialCounter = 0;
	public float maxValue = 5f;
	public float newSpeed = 0f;
	public float lastSpeed = 0f;
	public float rawValue = 0;
	public float lagValue = 0;
	public float roundedLagValue = 0f;
	public Color minTip = Color.green;
	public Color maxTip = Color.red;
	public bool hasStarted = false;
	public IEnumerator coroutine;
	public bool showText = false;
	public int tempCount = 0;
	private bool ready = false;
	private bool tutorialLevel = false;
	private bool tutorialCompleted = false;


	void Awake()
	{
		player ??= GameObject.FindGameObjectWithTag("Player")?.GetComponent<newAI2>();
		gh ??= GameObject.FindGameObjectWithTag("GameController")?.GetComponent<checkShit>();
		scoreText ??= GameObject.FindGameObjectWithTag("Slider")?.GetComponent<Slider>();
		if (SceneManager.GetActiveScene().buildIndex == 2) tutorialLevel = true;
	}

	void FindObjects()
    {
		if (player == null)
		{
			player = GameObject.FindObjectOfType<newAI2>();
		}
		if (gh == null)
		{
			gh = GameObject.FindObjectOfType<checkShit>();
		}
		if (scoreText == null) scoreText = GameObject.FindObjectOfType<Slider>();

		if (player != null && gh != null && scoreText != null) { 
			ready = true;
		}

	}

	public void Update()
	{
		while (!ready)
		{
			FindObjects();
			return;
		}

		if (tutorialLevel)
        {
			if (gh.hasStarted)
			{
				welcomeTutorial.SetActive(false);
				if (gh.tutorialIndex == 1 && !tutorialCompleted)
					ShowTutorial1();
				if (gh.tutorialIndex == 2 && !tutorialCompleted)
					ShowTutorial2(); 
				if (gh.someCount == 1 && !tutorialCompleted)
				{
					tutorialCompleted = true;
					ShowTutorial3();
				}
			}			
		}

		if (!gh.tipped)
		{
			scoreText.value = Mathf.Lerp(0, maxValue, player.speed / maxValue);
			image.color = Color.Lerp(minTip, maxTip, player.speed / maxValue);

		}
		if (gh.tipped)
        {
			if (gh.lapCompleted) restartText.GetComponent<Text>().text = "Saving lap time, please hold";
			else if (!gh.lapCompleted) restartText.GetComponent<Text>().text = "Tap to restart";

			restartText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
			restartText.GetComponent<Text>().fontSize = 40;

			if (!showText)
            {			
				showText = true;
				coroutine = ShowRestartText(0.5f);
				StartCoroutine(coroutine);
            }
        }
	}

	public void ShowTutorial1()
	{
		tutorial.SetActive(true);
	}

	public void ShowTutorial2()
	{
		tutorial.SetActive(false);
		tutorial2.SetActive(true);
	}

	public void ShowTutorial3()
    {
		tutorial.SetActive(false);
		tutorial2.SetActive(false);
		completeTutorial.SetActive(true);

		Destroy(tutorial, 0.1f);
		Destroy(tutorial2, 0.1f);
		Destroy(completeTutorial, 3f);
    }

	public void HideTutorial()
	{
		tutorial.SetActive(false);
		tutorial2.SetActive(false);
	}

	IEnumerator ShowRestartText(float waitTime)
    {
		yield return new WaitForSeconds(waitTime);
		restartText.SetActive(true);		
	}
}
