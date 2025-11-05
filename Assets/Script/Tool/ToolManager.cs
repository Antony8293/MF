using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ToolType
{
    None,
    Crate,
    Destroy,
    Fruit,
}

[System.Serializable]
public class CrateData
{
    public string name;
    public Vector3 position;

    public CrateData(string name, Vector3 position)
    {
        this.name = name;
        this.position = position;
    }
}

[System.Serializable]
public class LevelData
{
    public List<CrateData> crates = new List<CrateData>();
    public LevelData(List<CrateData> crates)
    {
        this.crates = crates;
    }

    public LevelData() { }
}

public class ToolManager : MonoBehaviour
{

    public static ToolManager instance;

    [SerializeField]
    private GameObject CrateParent;

    [SerializeField]
    private GameObject Crates;

    [SerializeField]
    private GameObject Circles;

    public GameObject Crate;

    [SerializeField]
    private List<GameObject> Fruits;

    private int fruitIndex;

    public static ToolType tooltype = ToolType.None;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }   
    }

    private void OnEnable()
    {
        AddFruit.ChangeToolTypetoFruit += FruitChoosen;
    }

    private void OnDisable()
    {
        AddFruit.ChangeToolTypetoFruit -= FruitChoosen;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ToolManager.tooltype == ToolType.Crate)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(mousePos.y > 1.5f || mousePos.x < -1.7f || mousePos.x > 1.7f || mousePos.y < -1.8f) return;
            mousePos.z = 0f;
            GameObject crate = Instantiate(Crate, mousePos, Quaternion.identity);
            crate.transform.parent = Crates.transform;
        }

        if(Input.GetMouseButtonDown(0) && ToolManager.tooltype == ToolType.Fruit)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.y > 1.5f || mousePos.x < -1.7f || mousePos.x > 1.7f || mousePos.y < -1.8f) return;
            mousePos.z = 0f;
            GameObject fruit = Instantiate(Fruits[fruitIndex-1], mousePos, Quaternion.identity);
            fruit.transform.parent = Circles.transform;
            fruit.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            fruit.AddComponent<DestroyObject>();
        }
    }

    public void SaveFile()
    {
        LevelData levelData = new LevelData();
        foreach (Transform crate in Crates.transform)
        {
            //Debug.Log(crate.name);
            CrateData cratedata = new CrateData("crate", crate.position);
            levelData.crates.Add(cratedata);
        }
        string json = JsonUtility.ToJson(levelData);
        string path = Application.persistentDataPath + "/level.json";
        File.WriteAllText(path, json);
        Debug.Log("?? l?u JSON t?i: " + path);
    }

    public void LoadFile()
    {
        foreach (Transform crate in Crates.transform)
        {
            Destroy(crate.gameObject);
        }

        string path = Application.persistentDataPath + "/level.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            foreach (CrateData crateData in levelData.crates)
            {
                GameObject crate = Instantiate(Crate, crateData.position, Quaternion.identity);
                crate.transform.parent = Crates.transform;
            }
        }
        else
        {
            Debug.LogWarning("Khong thay file");
        }
    }

    public void CretaChoosen() => tooltype = ToolType.Crate;

    public void DestroyChoosen() => tooltype = ToolType.Destroy;

    private void FruitChoosen(int index) => fruitIndex = index;
}
