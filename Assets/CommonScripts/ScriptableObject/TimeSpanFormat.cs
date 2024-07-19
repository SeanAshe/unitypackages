using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticConfig/TimeSpanFormat")]
public class TimeSpanFormat : ScriptableObject
{
    [SerializeField]
    public string Year;
    [SerializeField]
    public string Month;
    [SerializeField]
    public string Day;
    [SerializeField]
    public string Hour;
    [SerializeField]
    public string Minutes;
    [SerializeField]
    public string Seconds;
    [HideInInspector]
    public string[] list => new string[] { Year, Month, Day, Hour, Minutes, Seconds };
    [HideInInspector]
    public int[] carry = new int[] { 12, 30, 24, 60, 60 };
}