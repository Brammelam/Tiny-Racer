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
        
        currentScoreSO = pm.currentScoreSO;
        currentScoreSO.OnScoreChanged += SetRecord;
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
        float _newRecordFloat = pm.currentScoreSO.CurrentScore;
        recordtext.text = _newRecordFloat.ToString();
    }

    public void SetRecord(float newScore)
    {
        float _newRecordFloat = newScore / 100;
        string _newRecordText = _newRecordFloat.ToString();
        recordtext.text = _newRecordText + "s";
    }

    private void OnDestroy()
    {
        currentScoreSO.OnScoreChanged -= SetRecord;
    }
}