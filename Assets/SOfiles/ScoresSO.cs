using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScoresSO : ScriptableObject
{
    [SerializeField]
    private List<string> _names;
    [SerializeField]
    public List<string> _scores;




    public List<string> Names
    {
        get { return _names; }
        set { _names = value; }
    }

    public List<string> Values
    {
        get { return _scores; }
        set { _scores = value; }
    }


}
