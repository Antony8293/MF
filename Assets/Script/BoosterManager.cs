using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public static int BOOSTER_NON = -1; // Không có booster nào được kích hoạt
    public static int BOOSTER_BOOM = 0; // Booster phá hủy nhỏ nhất
    public static int BOOSTER_HAMMER = 1; // Booster búa
    public static int BOOSTER_UPGRADE = 2; // Booster nâng cấp
    public static int BOOSTER_SHAKE = 3; // Booster lắc hộp
    private int boosterChosen = BOOSTER_NON; // Trạng thái của booster, -1 là không có booster nào được kích hoạt
    private bool isBoosterActived = false;
    public static event Action<String, Vector3, Color, int> PracticeEffect;

    public static event Action ShakeBox;

    public static BoosterManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ BoosterManager không bị hủy khi chuyển cảnh}
        }
    }

    public bool GetBoosterActived()
    {
        return isBoosterActived;
    }

    public void SetBoosterActived(bool isActive)
    {
        isBoosterActived = isActive;
    }
    public int GetBoosterState()
    {
        return boosterChosen;
    }

    public void SetBoosterState(int state)
    {
        Debug.Log($"SetBoosterState: {state} - BoosterChosen: {boosterChosen}");
        if (boosterChosen > BOOSTER_NON && state > BOOSTER_NON) return;
        if (state < BOOSTER_NON || state > BOOSTER_SHAKE)
        {
            return;
        }
        boosterChosen = state;
    }

    public void UseBooster(int boosterIndex, GameObject gameObject)
    {
        SetBoosterState(boosterIndex);
        // Gọi hàm sử dụng booster tương ứng
        switch (boosterIndex)
        {
            case 0:
                // Sử dụng booster Smallest
                DestroySmallest(2); // 2 là cấp độ nhỏ nhất cần phá hủy
                break;
            case 1:
                // Sử dụng booster Hammer
                boosterChosen = BOOSTER_HAMMER;
                break;
            case 2:
                // Sử dụng booster Upgrade
                boosterChosen = BOOSTER_UPGRADE;
                break;
            case 3:
                // Sử dụng booster Shake the Box
                // GameManager.instance.isBoosterTriggered = true; // Đánh dấu đã kích hoạt booster
                SetBoosterActived(true); // Đánh dấu đã chọn mục tiêu
                UIManager.instance.UIScaleShakingBoosterEffect(Const.START_EFFECT); // Hiệu ứng scale
                break;
        }

        gameObject.SetActive(false);
    }


    private void DestroySmallest(int level)
    {
        UnityEngine.Object smallest = null;
        Transform parent = GameObject.Find("Circles").transform;
        int index = 0; // Khởi tạo index để tạo delay khác nhau cho mỗi object

        foreach (var circle in parent)
        {
            smallest = (circle as Transform).gameObject;
            if (smallest.GetComponent<CircleComponent>().Level <= level)
            {
                int smallestLevel = smallest.GetComponent<CircleComponent>().Level;
                // Delay tăng dần cho mỗi object: 0.1f, 0.2f, 0.3f, ...
                StartCoroutine(DelayedDestroySingle(smallest.GameObject(), smallestLevel, 0.1f + (index * 0.05f)));
                index++; // Tăng index cho object tiếp theo
            }
        }
        SetBoosterState(BOOSTER_NON); // Reset trạng thái booster sau khi sử dụng
    }

    public void FinishBooster(Boolean isColorEffect = false, GameObject gameObject = null)
    {
        if (isColorEffect)
        {
            PracticeEffect?.Invoke("VFX/Custom_FruitExplosion", gameObject.transform.position, gameObject.GetComponent<CircleComponent>().evolutionTree.levels[gameObject.GetComponent<CircleComponent>().Level - 1].colorEffect, gameObject.GetComponent<CircleComponent>().Level);
        }

        // GameManager.TriggerMouseNotChoosing();
        SetBoosterState(BOOSTER_NON);
        // GameManager.instance.isBoosterTriggered = false; 
        SetBoosterActived(false); // Reset trạng thái booster đã kích hoạt
        Destroy(gameObject);
    }

    private IEnumerator DelayedDestroySingle(GameObject obj, int smallestLevel, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Kiểm tra null trước khi destroy để tránh lỗi
        if (obj != null)
        {
            PracticeEffect("VFX/Custom_FruitExplosion", obj.transform.position, obj.GetComponent<CircleComponent>().evolutionTree.levels[smallestLevel - 1].colorEffect, smallestLevel);
            AudioManager.instance.PlayBoosterSmallestSound();

            Destroy(obj);
        }
    }

    private System.Collections.IEnumerator DelayBoosterEffect(Action onComplete)
    {
        yield return new WaitForSeconds(1f);
        onComplete?.Invoke();
    }

    public static void ShakeBoxClicked() => ShakeBox?.Invoke();
    
}
