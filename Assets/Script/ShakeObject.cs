using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    [SerializeField]
    private float durationShaking = 2f;

    [SerializeField]
    private float delayShaking = 1f;

    [SerializeField]
    private float magnitude = 0.5f;
    [SerializeField]
    private float rotationMagnitude = 6f;

    [SerializeField]
    private float amplitudeX = 0.2f;

    [SerializeField]
    private float frequencyX = 3f; // Tần số dao động theo trục X



    private Vector3 originalPos;
    private Quaternion originalRot;
    private float elapsed = 0f;
    private bool isShaking = false;

    private void OnEnable()
    {
        Booster.booster4 += StartShaking;
    }

    private void OnDisable()
    {
        Booster.booster4 -= StartShaking;
    }

    private void StartShaking()
    {
        Invoke("DelayShaking", delayShaking);

    }

    private void DelayShaking()
    {
        if (!isShaking)
        {
            originalPos = transform.localPosition;
            originalRot = transform.localRotation;
            elapsed = 0f;
            isShaking = true;
        }
    }

    // void Update()
    // {
    //     if (isShaking)
    //     {
    //         if (elapsed < durationShaking) //rung trong thời gian cài
    //         {
    //             //Tạo giá trị ngẫu nhiên cho vị trí X và Y, giúp đối tượng di chuyển nhẹ quanh vị trí gốc, tạo cảm giác rung.
    //             float x = Random.Range(-1f, 1f) * magnitude;
    //             float y = Random.Range(-1f, 1f) * magnitude;
    //             //Tạo giá trị ngẫu nhiên cho góc xoay Z, giúp đối tượng xoay nhẹ quanh trục Z, tạo cảm giác rung.
    //             float zRot = Mathf.Sin(Time.time * 50f) * rotationMagnitude;
    //             //Cập nhật vị trí và góc xoay của đối tượng dựa trên các giá trị ngẫu nhiên đã tạo.
    //             transform.localPosition = originalPos + new Vector3(x, y, 0f);
    //             transform.localRotation = Quaternion.Euler(0, 0, zRot);
    //             //Tăng thời gian đã trôi qua.
    //             elapsed += Time.deltaTime;
    //         }
    //         else
    //         {
    //             isShaking = false;
    //             transform.localPosition = originalPos;
    //             transform.localRotation = originalRot;
    //         }
    //     }
    // }
    
     void Update()
    {
        if (isShaking)
        {
            if (elapsed < durationShaking)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                float zRot = Mathf.Sin(Time.time * 50f) * rotationMagnitude;

                // Đung đưa theo trục X (dao động sin)
                float swingX = Mathf.Sin(Time.time * frequencyX) * amplitudeX; // Biên độ 0.2, tần số 3

                // Cộng thêm swingX vào vị trí X
                transform.localPosition = originalPos + new Vector3(x + swingX, y, 0f);
                transform.localRotation = Quaternion.Euler(0, 0, zRot);
                elapsed += Time.deltaTime;
            }
            else
            {
                isShaking = false;
                transform.localPosition = originalPos;
                transform.localRotation = originalRot;
                if (gameObject.name == "DynamicBoxCollider")
                {
                    UIManager.instance.UIScaleShakingBoosterEffect(Const.END_EFFECT);
                }
            }
        }
    }
}
