using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogSO", menuName = "Scriptable Objects/WaveTimeline")]
public class WaveTimeline : ScriptableObject
{
    public string waveName = "Wave";
    public List<WaveData> waves;
}