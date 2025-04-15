using System;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string chapterName;
    public string areaName;
    public SaveType saveType;
    public DateTime saveDateTime;
    public string inGameTimeOfDay;

    public string characterFullName;
    public Vector3 characterPosition;
}

public enum SaveType
{
    Manual,
    AutoSave
}
