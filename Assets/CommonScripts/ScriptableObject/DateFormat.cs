using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticConfig/DateFormat")]
public class DateFormat : ScriptableObject
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
    [SerializeField]
    public string Morning;
    [SerializeField]
    public string Afternoon;
}
