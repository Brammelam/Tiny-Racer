using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;


public class InputHandler : MonoBehaviour
{
	[SerializeField] InputField nameInput;
	[SerializeField] InputField outputArea;
	[SerializeField] string filename;
	List<InputEntry> entries = new List<InputEntry>();

	public void Awake()
	{
		DontDestroyOnLoad(this);

	}

	public void Get()
    {
		//entries = FileHandler.ReadFromJSON<InputEntry>(filename);
		StartCoroutine(GetRequest());
    }

	public IEnumerator GetRequest()
    {
		string uri = "http://localhost/tinyracer/randomScoresbby.json";
		using(UnityWebRequest request = UnityWebRequest.Get(uri))
        {
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
				outputArea.text = request.error;
            else
            {
				
				outputArea.text = request.downloadHandler.text;
			}
					
			
        }
    }
	
	public void PostData() => StartCoroutine(PostData_Coroutine());

	IEnumerator PostData_Coroutine()
	{
		string uri = "http://localhost/tinyracer/randomScoresbby.json";
		string alla = outputArea.text;
		byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(alla);



		using (UnityWebRequest www = UnityWebRequest.Put(uri, jsonToSend))
		{
			www.uploadHandler = new UploadHandlerRaw(jsonToSend);
			www.SetRequestHeader("Content-Type", "application/json");

			//Send the request then wait here until it returns
			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error While Sending: " + www.error);
			}
			else
			{
				outputArea.text = www.downloadHandler.text;
			}
		}
	}
	

	public void AddNameToList()
	{
		entries.Add(new InputEntry(nameInput.text, UnityEngine.Random.Range(0, 100)));
		nameInput.text = "";

		FileHandler.SaveToJSON<InputEntry>(entries, filename);
	}

	static string url()
    {
		string url = "http://localhost/tinyracer/randomScoresbby.json";
		return url;
    }
}