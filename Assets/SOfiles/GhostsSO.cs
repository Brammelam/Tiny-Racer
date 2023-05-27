using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GhostsSO : ScriptableObject
{
    [SerializeField]
    private List<int> ghostId;
    [SerializeField]
    private List<int> playerId;




    public List<int> GhostIds
    {
        get { return ghostId; }
        set { ghostId = value; }
    }

    public List<int> PlayerIds
    {
        get { return playerId; }
        set { playerId = value; }
    }




}
