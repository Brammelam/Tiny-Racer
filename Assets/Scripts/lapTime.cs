using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lapTime : MonoBehaviour
{
    public checkShit check;
    public Text timetext;
    public Text recordtext;
    public PlayerManager pm;
    public LeaderBoard lb;
    public float recordScore, lapScore;
    public float f;

    public void Awake()
    {
        recordtext = GameObject.Find("recordTime").GetComponent<Text>();
        timetext = GameObject.Find("lapTime").GetComponent<Text>();
        
        if (check == null) check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();
        if (pm == null) pm = check.pm;
    }

    private void LookForStuff()
    {
        if (pm == null) pm = check.pm;
    }

    public void Start()
    {
        LookForStuff();
        UpdateRecord();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (check == null) check = GameObject.FindGameObjectWithTag("GameController").GetComponent<checkShit>();
        if (pm == null) LookForStuff();

        timetext.text = check.elapsedTime.ToString("F2") + "s";

        if (pm.currentScoreSObool)
        {           
            pm.currentScoreSObool = false;
            UpdateRecord();
        }

    }

    public void UpdateRecord()
    {
      
        recordtext.text = pm.currentScoreSO.CurrentScore + "s";
    }
}
