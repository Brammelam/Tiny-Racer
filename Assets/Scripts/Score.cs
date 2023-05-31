using System.Collections;

using UnityEngine;
using UnityEngine.UI;


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
	public bool check = false;
	public int tempCount = 0;
	private bool ready = false;


	void Awake()
	{
		player ??= GameObject.FindGameObjectWithTag("Player")?.GetComponent<newAI2>();
		gh ??= GameObject.FindGameObjectWithTag("GameController")?.GetComponent<checkShit>();
		scoreText ??= GameObject.FindGameObjectWithTag("Slider")?.GetComponent<Slider>();
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

		if (player != null && gh != null && scoreText != null) ready = true;
	}

	void Update()
	{
		while (!ready)
		{
			FindObjects();
			return;
		}

		if (gh.currentLevel == 9)
        {
			if (gh.hasStarted)
			{
				welcomeTutorial.SetActive(false);
			}

			if (gh.setSlow && gh.someCount == 0)
			{
				if (gh.tutorialIndex == 0)
					ShowTutorial1();
				else ShowTutorial2();
			}
			else if (!gh.setSlow)
			{

				HideTutorial();

			}

			if (gh.someCount == 1 && !check && tempCount == 0)
			{
				check = true;
				ShowTutorial3();
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
		tutorial2.SetActive(true);
	}

	public void ShowTutorial3()
    {
		completeTutorial.SetActive(true);
		tempCount = 1;
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
