using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class lapTime : MonoBehaviour
{
    public checkShit check;
    public Text timetext;
    public Text recordtext;
    public PlayerManager pm;
    public LeaderBoard lb;
    public float recordScore, lapScore;
    public bool ready;

    private CurrentscoreSO currentScoreSO;

    public void Awake()
    {
        
        recordtext = GameObject.Find("recordTime").GetComponent<Text>();
        timetext = GameObject.Find("lapTime").GetComponent<Text>();

        if (check == null) check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();
        if (pm == null) pm = check.pm;
    }

    private void LookForStuff()
    {
        if (pm != null && check != null)
        {
            LoadRecord();
            currentScoreSO = pm.currentScoreSO;
            currentScoreSO.OnScoreChanged += SetRecord;
            ready = true;
        }
        else
        {
            check = check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();
            pm = check.pm;
        }
    }

    public void Start()
    {
        ready = false;
        LookForStuff();
        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        while (check == null || pm == null) LookForStuff();
        if (check == null) check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();
        if (pm == null) LookForStuff();

        timetext.text = check.elapsedTime.ToString("F2") + "s";
    }

    private void LoadRecord()
    {
        string _newRecordText = PlayerPrefs.GetFloat("highScore").ToString();
        recordtext.text = _newRecordText + "s";
    }

    public void SetRecord(float newScore)
    {
        string _newRecordText = PlayerPrefs.GetFloat("highScore").ToString();
        recordtext.text = _newRecordText + "s";
    }

    private void OnDestroy()
    {
        currentScoreSO.OnScoreChanged -= SetRecord;
    }
}