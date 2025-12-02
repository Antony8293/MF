using System;
using TMPro;
// Removed editor utilities import — not needed in runtime scripts
using UnityEngine;
using UnityEngine.UI;

public class AddFruit : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;

    public static Action<int> ChangeToolTypetoFruit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (dropdown == null)
        {
            Debug.LogWarning("AddFruit: dropdown reference is not assigned in inspector.");
            return;
        }

        // Subscribe to the dropdown value change with the int parameter
        dropdown.onValueChanged.AddListener(valueDropdownChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void valueDropdownChanged(int value)
    {
        ToolManager.tooltype = ToolType.Fruit;
        ChangeToolTypetoFruit?.Invoke(value);
    }

    private void OnDestroy()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(valueDropdownChanged);
    }
}
