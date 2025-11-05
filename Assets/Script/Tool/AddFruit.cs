using System;
using TMPro;
using TMPro.EditorUtilities;
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
        dropdown.onValueChanged.AddListener(delegate { valueDropdownChanged(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void valueDropdownChanged()
    {
        ToolManager.tooltype = ToolType.Fruit;
        ChangeToolTypetoFruit?.Invoke(dropdown.value);
    }
}
