using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PixelGridColorizer : MonoBehaviour
{
    public Transform gridParent;
    private Dictionary<int, Queue<Transform>> _colorListInGrid = new Dictionary<int, Queue<Transform>>();

    public Dictionary<int, Queue<Transform>> ColorListInGrid { get => _colorListInGrid; set => _colorListInGrid = value; }

    private void Start()
    {
    }

    public void ApplyColorEnumToGrid()
    {
        string gridData =
            @"  0 0 1 1 1 0 0 0 0 0 0 0 0 0 0 0
                0 0 1 1 1 1 1 0 0 0 0 2 2 2 1 0
                0 0 0 0 0 0 1 1 1 1 2 2 2 2 1 0
                0 0 0 0 1 1 0 1 1 0 2 2 2 1 0 0
                0 0 0 1 0 0 0 1 1 1 2 1 1 1 0 0
                0 0 1 0 2 2 1 1 1 1 1 1 1 1 1 0
                0 1 0 0 2 2 0 1 1 1 0 0 0 1 1 1
                0 1 1 0 0 0 0 0 0 0 0 0 0 0 1 1
                0 1 1 0 0 0 0 0 0 0 0 0 0 0 1 1
                0 1 2 0 0 0 0 0 0 0 0 0 0 2 1 0
                0 1 2 0 0 0 0 0 0 0 0 0 0 2 1 0
                0 0 1 2 0 0 0 0 0 0 0 0 2 0 1 0
                0 0 1 2 2 2 0 0 0 0 0 2 2 1 0 0
                0 0 1 1 0 2 2 2 2 2 2 2 0 1 0 0
                0 0 0 1 1 1 2 2 1 2 2 1 1 0 0 0
                0 0 0 0 1 1 1 1 1 1 1 1 0 0 0 0";


        // Clean and flatten to list
        string result = new string(gridData.Where(c => !char.IsWhiteSpace(c)).ToArray());
        // Debug.Log(result.Length);
        List<int> colors = new List<int>();
        foreach (char c in result)
        {
            colors.Add(c - '0');
        }
        // Debug.Log(gridParent.childCount + " " + colors.Count);
        if (colors.Count != 256 || gridParent.childCount != 256)
        {
            Debug.LogError("Mismatch: Need exactly 256 units and 256 color entries.");
            return;
        }

        for (int i = 0; i < 256; i++)
        {
            Transform child = gridParent.GetChild(i);

            if (!_colorListInGrid.ContainsKey(colors[i]))
            {
                _colorListInGrid[colors[i]] = new Queue<Transform>();
            }
            _colorListInGrid[colors[i]].Enqueue(child);
        }
        // Debug.Log(_colorListInGrid);
    }
}
