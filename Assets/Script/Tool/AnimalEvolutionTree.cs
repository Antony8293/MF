
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
[CreateAssetMenu(fileName = "AnimalEvolutionTree", menuName = "ScriptableObjects/Animal Evolution Tree")]
public class AnimalEvolutionTree : ScriptableObject
{
    public List<AnimalData> levels;
    public int comboSpawn;
    public void SetComboSpawn(int n) => comboSpawn = n;

    private SpawnMachine spawnMachine;

    // Gọi sau khi GameManager sẵn sàng
    public void InitSpawnMachine(GameMode mode)
    {
        spawnMachine = new SpawnMachine(mode);
    }

    public AnimalData GetLevelData(int level = 0)
    {
        return (level >= 0 && level <= levels.Count) ? levels[level - 1] : null;
    }

    public int GetMaxLevel() => levels.Count;
}

[System.Serializable]
public class AnimalData
{
    public string name;
    public GameObject prefab;
    public float scaleRatio = 1f;
    public Color colorEffect = Color.white;
}

// public class SpawnMachine
// {
//     private List<List<int>> difficultyPatterns = new List<List<int>>
//     {
//         new List<int> { 4, 4 },
//         new List<int> { 5 },
//         new List<int> { 4, 3, 2 },
//         new List<int> { 4, 3, 2, 1, 1 }
//     };

//     private List<int> currentPattern;
//     private List<int> remainingValues;

//     public SpawnMachine()
//     {
//         PickNewPattern();
//     }

//     public int GetNext()
//     {
//         if (remainingValues.Count == 0)
//         {
//             PickNewPattern();
//         }

//         int index = Random.Range(0, remainingValues.Count);
//         int value = remainingValues[index];
//         remainingValues.RemoveAt(index);
//         return value;
//     }

//     private void PickNewPattern()
//     {
//         int patternIndex = Random.Range(0, difficultyPatterns.Count);
//         currentPattern = new List<int>(difficultyPatterns[patternIndex]);
//         remainingValues = new List<int>(currentPattern);
//     }
// }[System.Serializable]


public enum GameMode { Easy, Medium, Hard }

public class SpawnMachine
{
    private readonly System.Random rng;
    private readonly Dictionary<GameMode, int> combosPerMode = new()
    {
        { GameMode.Easy,   2 },
        { GameMode.Medium, 3 },
        { GameMode.Hard,   5 },
    };

    private readonly List<List<int>> patternPool = new()
    {
        new() { 5 },
        new() { 4, 3, 2, 1, 1 },
        new() { 2, 2, 3, 3, 2, 1, 1 },
        new() { 3 },
        new() { 2 },
        new() { 1 },
        new() { 1 },
    };

    private readonly GameMode mode;
    private readonly Dictionary<int, int> bag = new();
    private List<int> poolIndicesLeft = new();

    // nhớ lại quả cuối cùng
    private int? lastPick = null;

    // hệ số anti-streak (0.5 nghĩa là giảm 50% trọng số)
    private readonly float antiStreakFactor = 0.6f;

    public SpawnMachine(GameMode mode, int? seed = null)
    {
        this.mode = mode;
        rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        ResetPoolCycle();
        RefillBag();
    }

    public int Next()
    {
        if (bag.Count == 0) RefillBag();

        // Chuyển bag -> danh sách (level, weight)
        var entries = new List<(int level, float weight)>();
        foreach (var kv in bag)
        {
            float w = kv.Value; // base weight = frequency
            if (lastPick.HasValue && kv.Key == lastPick.Value)
            {
                w *= antiStreakFactor; // giảm weight nếu trùng level trước
            }
            entries.Add((kv.Key, w));
        }

        // rút thăm theo trọng số
        float sum = entries.Sum(e => e.weight);
        float r = (float)(rng.NextDouble() * sum);
        int pick = entries[0].level;
        foreach (var e in entries)
        {
            r -= e.weight;
            if (r <= 0)
            {
                pick = e.level;
                break;
            }
        }

        // cập nhật bag và lastPick
        if (--bag[pick] <= 0) bag.Remove(pick);
        lastPick = pick;
        return pick;
    }

    private void RefillBag()
    {
        int nCombos = combosPerMode[mode];
        for (int i = 0; i < nCombos; i++)
        {
            if (poolIndicesLeft.Count == 0) ResetPoolCycle();
            int idxInLeft = rng.Next(poolIndicesLeft.Count);
            int poolIdx = poolIndicesLeft[idxInLeft];
            poolIndicesLeft.RemoveAt(idxInLeft);
            foreach (int val in patternPool[poolIdx])
            {
                bag.TryGetValue(val, out int c);
                bag[val] = c + 1;
            }
        }
    }

    private void ResetPoolCycle()
    {
        poolIndicesLeft = Enumerable.Range(0, patternPool.Count).ToList();
    }
}
