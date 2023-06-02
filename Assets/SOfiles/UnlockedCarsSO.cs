using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class UnlockedCarsSO : ScriptableObject
{
    [SerializeField]
    private List<string> _unlockedCars;

    public List<string> UnlockedCars
    {
        get { return _unlockedCars; }
        set { _unlockedCars = value; }
    }
}
