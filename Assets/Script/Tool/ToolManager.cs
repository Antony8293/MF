
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int levelNumber;

    public List<Crate> crates = new List<Crate>();
    public List<Fruit> fruits = new List<Fruit>();

    public Level(int number, List<Crate> crateList, List<Fruit> fruitList)
    {
        levelNumber = number;
        crates = crateList;
        fruits = fruitList;
    }

    public Level() { }
}

[System.Serializable]
public class Crate
{
    public Vector3 position;

}

[System.Serializable]
public class Fruit
{
    public int fruitName;
    public Vector3 postion;
}

public class ToolManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Shields;

    [SerializeField]
    private GameObject Circles;

    public GameObject Crate;

    public List<GameObject> fruitObjects = new List<GameObject>();

    void Start()
    {
        LoadFile();
    }

    public void SaveFile()
    {
        Level level = new Level();

        foreach (Transform child in Shields.transform)
        {
            Crate crate = new Crate();
            crate.position = child.position;
            level.crates.Add(crate);
        }

        foreach (Transform child in Circles.transform)
        {
            Fruit fruit = new Fruit();
            fruit.postion = child.position;
            child.name = child.name.Replace("(Clone)", "").Replace("Fruit0", "");
            fruit.fruitName = int.Parse(child.name);
            level.fruits.Add(fruit);
        }

        level.levelNumber = 1;

        string json = JsonUtility.ToJson(level);
        string path = Application.persistentDataPath + "/level.json";
        File.WriteAllText(path, json);
        Debug.Log("Level saved to " + path);
    }

    public void LoadFile()
    {

        foreach (Transform child in Shields.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in Circles.transform)
        {
            Destroy(child.gameObject);
        }

        string path = Application.persistentDataPath + "/level.json";

        var jsonfile = Resources.Load<TextAsset>("level_1");


        if (jsonfile != null)
        {
            Level level = JsonUtility.FromJson<Level>(jsonfile.text);

            foreach (Crate crate in level.crates)
            {
                GameObject crateObject = Instantiate(Crate, crate.position, Quaternion.identity);
                crateObject.transform.SetParent(Shields.transform, true);

            }

            foreach (Fruit fruit in level.fruits)
            {
                switch (fruit.fruitName)
                {
                    case 1:
                        {
                            LoadFruit(1, fruit.postion);
                            break;
                        }
                    case 2:
                        {
                            LoadFruit(2, fruit.postion);
                            break;
                        }
                    case 3:
                        {
                            LoadFruit(3, fruit.postion);
                            break;
                        }
                    case 4:
                        {
                            LoadFruit(4, fruit.postion);
                            break;
                        }
                    case 5:
                        {
                            LoadFruit(5, fruit.postion);
                            break;
                        }
                    case 6:
                        {
                            LoadFruit(6, fruit.postion);
                            break;
                        }
                    case 7:
                        {
                            LoadFruit(7, fruit.postion);
                            break;
                        }
                    case 8:
                        {
                            LoadFruit(8, fruit.postion);
                            break;
                        }
                    case 9:
                        {
                            LoadFruit(9, fruit.postion);
                            break;
                        }
                    case 10:
                        {
                            LoadFruit(10, fruit.postion);
                            break;
                        }
                }
            }
        }
    }
    
    private void LoadFruit(int fruitName, Vector3 position)
    {
        GameObject fruitObject = Instantiate(fruitObjects[fruitName - 1], position, Quaternion.identity);
        fruitObject.transform.SetParent(Circles.transform, true);
    }
}
