using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;


public class FileHandler : MonoBehaviour
{
	public void Awake()
    {
		DontDestroyOnLoad(this);
    }
	public static void SaveToJSON<T>(List<T> toSave, string filename)
    {
		Debug.Log(GetPath(filename));
		string content = JsonHelper.ToJson<T>(toSave.ToArray());
		WriteFile(GetPath(filename), content);
    }

	public static List<T> ReadFromJSON<T>(string filename)
    {
		string content = ReadFile(GetPath(filename));

		if(string.IsNullOrEmpty(content) || content == "{}")
        {
			return new List<T>();
        }

		List<T> res = JsonHelper.FromJson<T>(content).ToList();
		return res;
    }

	private static string GetPath(string filename)
    {
		return filename;
    }

	private static void WriteFile(string path, string content)
    {
		FileStream fileStream = new FileStream(path, FileMode.Create);

		using (StreamWriter writer = new StreamWriter(fileStream)) { 
			writer.Write(content);
		
		}
    }

	public static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
			using (StreamReader reader = new StreamReader(path))
            {
				string content = reader.ReadToEnd();
				return content;
            }
        }
		return "";
    }


}

public static class JsonHelper
{
	public static T[] FromJson<T> (string json)
    {
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
    }

	public static string ToJson<T> (T[] array)
    {
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper);
    }

	public static string ToJson<T> (T[] array, bool prettyPrint)
    {
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper, prettyPrint);
    }

	[Serializable]
	private class Wrapper<T>
    {
		public T[] Items;
    }
}

/*
public class highscorescript : MonoBehaviour
{

	const string privateCode1 = "XZH-wLEho0yay3IJu1hTwAckncI5EcC0aP8brURLM3sQ";
	const string publicCode1 = "6198febe8f40bb1278790790";

	const string privateCode2 = "grOp9ZlrMkyShMXlizuIcgbeWgLj9mR0ukYDvf0Mf4iw";
	const string publicCode2 = "6198fec48f40bb12787907a2";

	const string privateCode3 = "BtZpRjxN8EyGgIwjXCU8bQyrPVS4xqn0CYH1TRdeqKTQ";
	const string publicCode3 = "6198fec58f40bb12787907a4";

	const string privateCode4 = "kmrkSmJMmkebGIuG6yxBoQAUoGsoJ4T0eF1BamTkCdWw";
	const string publicCode4 = "6198fec78f40bb12787907ab";

	const string privateCode5 = "TBT-uMwONUqKqnHAdTBq-gozTovHKgfEqdqdAWQGetfQ";
	const string publicCode5 = "6198fec68f40bb12787907a7";

	const string privateCode6 = "BeIZN1wNXkiGobIg_XsCLQW3rj54j6vUWAByriDZhYRg";
	const string publicCode6 = "6198fec88f40bb12787907ad";

	const string privateCode7 = "enjbN4BUdUOKKS24juLOPgF3E8R2_R8Ue6KP8-AQ-_iQ";
	const string publicCode7 = "6198fec78f40bb12787907aa";

	const string privateCode8 = "Tlbn8PBh102_JvhU2_fcfw8taZidL8ckKGFJH4w5ZTWQ";
	const string publicCode8 = "6198fec88f40bb12787907af";

	const string privateCode9 = "6NDF40avFkeCXbc29ucO8w47BfTVkuxkihhTku4KOKWA";
	const string publicCode9 = "6198fedd8f40bb12787907f1";

	string privateCode = "";
	string publicCode = "";


	const string webURL = "http://dreamlo.com/lb/";

	public Highscore[] highscoresList;

	public checkShit cs;
	public GameObject hs;
	public GameObject welcomeScreen;
	public playernamescript ps;
	public GameObject sl;
	public selectedLevel sl_script;
	public Text _text;

	public Text levelGlobalRecord;
	public Text levelRecord;

	public string username = "";
	public int entered;
	public bool welcomeChecked = false;
	public int levelIndex;


	void Awake()
	{
		username = PlayerPrefs.GetString("name", "");
		hs = GameObject.FindGameObjectWithTag("highscore");
		DontDestroyOnLoad(hs);
		DownloadHighscores(1);
		
	}

    private void Start()
    {
		//EnteredName();
    }

	public void OpenTouch()
    {
		TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
	}

    public void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
			if (entered != 0)
            {
				welcomeScreen = GameObject.FindGameObjectWithTag("welcome");
				if (welcomeScreen != null)
				welcomeScreen.SetActive(false);

            }

		}
		if (level == 1)
        {			
			sl = GameObject.FindGameObjectWithTag("level");
			sl_script = sl.GetComponent<selectedLevel>();
			levelIndex = sl_script.levelIndex - 1;
			DownloadHighscores(1);

		}
    }

    public void EnteredName()
    {
		if (entered != 0)
        {
			PlayerPrefs.SetString("name", username);
			PlayerPrefs.Save();
			welcomeScreen.SetActive(false);

        }

		else _text.text = "Enter a name!";
	}


    public void AddNewHighscore(string username, int score, int level)
	{
		StartCoroutine(UploadNewHighscore(username, score, level));
	}

	public IEnumerator UploadNewHighscore(string username, int score, int level)
	{
		if (level == 0)
			privateCode = privateCode1;
		else if (level == 1)
			privateCode = privateCode2;
		else if (level == 2)
			privateCode = privateCode3;
		else if (level == 3)
			privateCode = privateCode4;
		else if (level == 4)
			privateCode = privateCode5;
		else if (level == 5)
			privateCode = privateCode6;
		else if (level == 6)
			privateCode = privateCode7;
		else if (level == 7)
			privateCode = privateCode8;
		else 
			privateCode = privateCode9;

		WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
		yield return www;

		if (string.IsNullOrEmpty(www.error))
			print("Upload Successful");
		else
		{
			print("Error uploading: " + www.error);
		}
	}

	public void DownloadHighscores<T>(List<T> toSave, string filename)
	{
		string content = 
		WriteFile();
		levelIndex = _index - 1;
		StartCoroutine("DownloadHighscoresFromDatabase");
	}

	private string GetPath (string filename)
    {
		return Application.persistentDataPath + "/" + filename;
    }

	private void WriteFile(string path, string content)
    {
		FileStream filestream = new FileStream(path, FileMode.Create);
		using (StreamWriter writer = new StreamWriter(filestream))
        {
			writer.Write(content);
        }
    }

	IEnumerator DownloadHighscoresFromDatabase()
	{


		if (levelIndex == 0)
			publicCode = publicCode1;
		else if (levelIndex == 1)
			publicCode = publicCode2;
		else if (levelIndex == 2)
			publicCode = publicCode3;
		else if (levelIndex == 3)
			publicCode = publicCode4;
		else if (levelIndex == 4)
			publicCode = publicCode5;
		else if (levelIndex == 5)
			publicCode = publicCode6;
		else if (levelIndex == 6)
			publicCode = publicCode7;
		else if (levelIndex == 7)
			publicCode = publicCode8;
		else
			publicCode = publicCode9;

		WWW www = new WWW("http://brammelam-everest.nord/proxy.php?url=" + webURL + publicCode + "/pipe/");

		yield return www;

		if (string.IsNullOrEmpty(www.error))
			FormatHighscores(www.text);
		else
		{
			print("Error Downloading: " + www.error);
		}
	}

	void FormatHighscores(string textStream)
	{
		string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		highscoresList = new Highscore[entries.Length];

		for (int i = 0; i < entries.Length; i++)
		{
			string[] entryInfo = entries[i].Split(new char[] { '|' });
			string username = entryInfo[0];
			int score = int.Parse(entryInfo[1]);
			highscoresList[i] = new Highscore(username, score);
		}

		if (SceneManager.GetActiveScene().buildIndex == 1)
		{
			levelGlobalRecord = GameObject.FindGameObjectWithTag("globalrecord").GetComponent<UnityEngine.UI.Text>();
			levelRecord = GameObject.FindGameObjectWithTag("localrecord").GetComponent<UnityEngine.UI.Text>();

			int index = highscoresList.Length - 1;
			float _levelGlobalRecord = highscoresList[index].score / 100f;
			_levelGlobalRecord = Mathf.Round(_levelGlobalRecord * 100) / 100;

			string getRecord = "Record level " + levelIndex;

			float _levelRecord = PlayerPrefs.GetFloat(getRecord, -1f);


			if (_levelRecord == -1f)
			{
				levelRecord.text = "??";
			}
			else

			{
				_levelRecord = Mathf.Round(_levelRecord * 100) / 100;
				levelRecord.text = _levelRecord.ToString();
			}
			levelGlobalRecord.text = _levelGlobalRecord.ToString();
		}
	}

}

public struct Highscore
{
	public string username;
	public int score;

	public Highscore(string _username, int _score)
	{
		username = _username;
		score = _score;
	}

}
*/