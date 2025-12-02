
using UnityEngine;

[System.Serializable]
public class Crate : MonoBehaviour
{
    public int level;
    public Vector3 position;
    [SerializeField]
    public Sprite[] CrateLevelSprites;
    private SpriteRenderer spriteRenderer;
    // public int activePreviousFruitLevel = -1;
    // public bool activeDownGrade = true;
    void Start()
    {
        // Try to find SpriteRenderer on the same object or in children
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // If crate sprites exist, set the initial sprite to current level
        if (CrateLevelSprites != null && CrateLevelSprites.Length > 0)
        {
            SetCrateSprite(level);
        }

        

        // SetCrateSprite(level);
    }
    // public void ResetFruitLevelActive() //thả lượt mới sẽ reset lại biến này
    // {
    //     activePreviousFruitLevel = -1;
    //     activeDownGrade = true;
    // } 
   
    public void SetCrateSprite(int ilevel)
    {   
        level = ilevel;
        Debug.Log("SetCrateSprite Level: " + ilevel);
        // Validate sprites array
        if (CrateLevelSprites == null)
        {
            Debug.LogWarning("CrateLevelSprites is null. Assign sprites in the inspector.");
            return;
        }

        if (ilevel < 0 || ilevel >= CrateLevelSprites.Length)
        {
            Debug.LogWarningFormat(this, "SetCrateSprite: level {0} is out of range (0..{1})", ilevel, CrateLevelSprites.Length - 1);
            return;
        }

        // Ensure spriteRenderer is available (in case this method was called before Start)
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogWarning("SetCrateSprite: No SpriteRenderer found on GameObject or children.");
            return;
        }

        spriteRenderer.sprite = CrateLevelSprites[ilevel];
        spriteRenderer.enabled = true;
        // activeDownGrade = false;
    }    
}
