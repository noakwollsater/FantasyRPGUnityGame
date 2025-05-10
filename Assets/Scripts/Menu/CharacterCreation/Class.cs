using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Class
{
    public string ClassName;
    public string Description;
    public string PrimaryAbility;
    public string Role;
    public string[] UniqueAbilities;
    public AttributeSet ClassBonuses;

    public Class(string className, string description, string primaryAbility, string role, string[] uniqueAbilities, AttributeSet classBonuses)
    {
        ClassName = className;
        Description = description;
        PrimaryAbility = primaryAbility;
        Role = role;
        UniqueAbilities = uniqueAbilities;
        ClassBonuses = classBonuses;
    }
}
