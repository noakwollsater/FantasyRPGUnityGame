using Opsive.UltimateCharacterController.Traits;
using Unity.FantasyKingdom;
using UnityEngine;

public class HungerAndThirst : MonoBehaviour
{
    [Header("Hunger and Thirst Settings")]
    [SerializeField] private float hungerDrainPerSecond = 0.1f;
    [SerializeField] private float thirstDrainPerSecond = 0.1f;

    private AttributeManager _attributeManager;
    private Attribute _hungerAttribute;
    private Attribute _thirstAttribute;
    private Attribute _healthAttribute;

    private HotbarStats _hotbarStats;

    private void Awake()
    {
        _attributeManager = GetComponent<AttributeManager>();

        if (_attributeManager != null)
        {
            _hungerAttribute = _attributeManager.GetAttribute("Hunger");
            _thirstAttribute = _attributeManager.GetAttribute("Thirst");
            _healthAttribute = _attributeManager.GetAttribute("Health");


            if (_hungerAttribute == null) Debug.LogError("Hunger attribute not found!");
            if (_thirstAttribute == null) Debug.LogError("Thirst attribute not found!");
            if (_healthAttribute == null) Debug.LogError("Health attribute not found!");
        }
        else
        {
            Debug.LogError("No AttributeManager found on player!");
        }

        GameObject UI = GameObject.Find("UI");
        if (UI == null)
        {
            Debug.LogError("No UI found in the scene!");
            return;
        }
        _hotbarStats = UI.GetComponentInChildren<HotbarStats>();
        if (_hotbarStats == null)
        {
            Debug.LogError("No HotbarStats found on player!");
        }
    }

    private void Update()
    {
        if (_hungerAttribute == null || _thirstAttribute == null) return;

        DrainHunger();
        DrainThirst();
    }

    private void DrainHunger()
    {
        if (_hungerAttribute.Value > 0)
        {
            _hungerAttribute.Value -= hungerDrainPerSecond * Time.deltaTime;
        }
        else
        {
            if (_hotbarStats != null)
            {
                _hotbarStats.DamageHealthOverTime(0.1f);
            }
        }
    }

    private void DrainThirst()
    {
        if (_thirstAttribute.Value > 0)
        {
            _thirstAttribute.Value -= thirstDrainPerSecond * Time.deltaTime;
        }
        else
        {
            if (_hotbarStats != null)
            {
                _hotbarStats.DamageHealthOverTime(0.1f);
            }
        }
    }

    public void RestoreHunger(float amount)
    {
        if (_hungerAttribute != null)
        {
            _hungerAttribute.Value = Mathf.Min(_hungerAttribute.Value + amount, _hungerAttribute.MaxValue);
        }
    }

    public void RestoreThirst(float amount)
    {
        if (_thirstAttribute != null)
        {
            _thirstAttribute.Value = Mathf.Min(_thirstAttribute.Value + amount, _thirstAttribute.MaxValue);
        }
    }
}
