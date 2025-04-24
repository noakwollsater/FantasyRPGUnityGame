using System;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string chapterName;
    public string areaName;
    public SaveType saveType;
    public DateTime saveDateTime;

    //Spara ingame tid här
    public int inGameYear;
    public int inGameMonth;
    public int inGameDay;
    public float inGameTimeMinutes;

    public string characterFullName;
    public Vector3 characterPosition;
}

public enum SaveType
{
    Manual,
    AutoSave
}
