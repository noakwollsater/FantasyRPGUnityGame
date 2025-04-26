using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

public class HungerAndThirst : MonoBehaviour
{
    [Header("Hunger and Thirst Settings")]
    [SerializeField] private float hungerDrainPerSecond = 0.1f;
    [SerializeField] private float thirstDrainPerSecond = 0.1f;

    private AttributeManager _attributeManager;
    private Attribute _hungerAttribute;
    private Attribute _thirstAttribute;

    private void Awake()
    {
        _attributeManager = GetComponent<AttributeManager>();

        if (_attributeManager != null)
        {
            _hungerAttribute = _attributeManager.GetAttribute("Hunger");
            _thirstAttribute = _attributeManager.GetAttribute("Thirst");

            if (_hungerAttribute == null) Debug.LogError("Hunger attribute not found!");
            if (_thirstAttribute == null) Debug.LogError("Thirst attribute not found!");
        }
        else
        {
            Debug.LogError("No AttributeManager found on player!");
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
    }

    private void DrainThirst()
    {
        if (_thirstAttribute.Value > 0)
        {
            _thirstAttribute.Value -= thirstDrainPerSecond * Time.deltaTime;
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
