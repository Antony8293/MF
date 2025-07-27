using UnityEngine;
using System.Collections;

public class EyesControl : MonoBehaviour
{
    public GameObject eyes;           // Đối tượng "mắt" sẽ di chuyển theo chuột
    public Camera camera;             // Camera chính dùng để chuyển đổi tọa độ màn hình => thế giới
    public Transform target; // Mục tiêu để mắt hướng tới (có thể là chuột hoặc một đối tượng khác)
    public float intensity = 0.5f; // Khả năng di chuyển tối đa của mắt
    public float speed = 5f; // Tốc độ di chuyển của mắt

    private float originalIntensity; // Lưu giá trị intensity ban đầu
    private float lastTouchTime; // Thời gian touch cuối cùng
    private float lastDragTime; // Thời gian drag cuối cùng
    private Vector3 lastMousePosition; // Vị trí chuột frame trước
    private const float NO_TOUCH_TIMEOUT = 2f; // 2 giây không touch
    private const float NO_DRAG_TIMEOUT = 2f; // 2 giây không drag
    private bool isDelaying; // Cờ để tránh gọi nhiều lần khi đang delay
    private CircleComponent circleCompParent; // Tham chiếu đến CircleComponent để lấy thông tin
    private GameObject fixedPositionTarget; // GameObject tạm thời để giữ vị trí cố định
    void Start()
    {
        camera = Camera.main;         // Gán camera mặc định của scene (nếu chưa gán sẵn trong Inspector)
        originalIntensity = intensity; // Lưu giá trị intensity ban đầu
        lastTouchTime = Time.time; // Khởi tạo thời gian touch cuối cùng
        lastDragTime = Time.time; // Khởi tạo thời gian drag cuối cùng
        lastMousePosition = Input.mousePosition; // Khởi tạo vị trí chuột

        target = GameManager.instance.draggingCircleGO.transform; // Gán mục tiêu là CircleComponent đang kéo   
        isDelaying = false; // target là draggingCircleGO, mặc định không delay

        // Lấy CircleComponent từ GameObject cha
        circleCompParent = GetComponentInParent<CircleComponent>();
    }

    void Update()
    {
        if (camera != null)
        {
            // Kiểm tra touch screen và drag
            if (Input.GetMouseButton(0))
            {
                lastTouchTime = Time.time; // Cập nhật thời gian touch cuối cùng

                // Kiểm tra có drag không - so sánh vị trí chuột giữa các frame
                Vector3 currentMousePosition = Input.mousePosition;
                float mouseDelta = Vector3.Distance(currentMousePosition, lastMousePosition);
                bool isDragging = mouseDelta > 1f; // Threshold là 1 pixel

                // Debug.Log($"Mouse Delta: {mouseDelta:F2} pixels, isDragging={isDragging}");

                if (isDragging)
                {
                    lastDragTime = Time.time; // Cập nhật thời gian drag cuối cùng
                    intensity = originalIntensity; // Khôi phục intensity khi có drag
                    // Debug.Log("Dragging detected, intensity restored to original value.");
                }
                else
                {
                    // Đang touch nhưng không drag - kiểm tra timeout
                    if (Time.time - lastDragTime > NO_DRAG_TIMEOUT)
                    {
                        intensity = 0f; // Đặt intensity = 0 sau 2 giây không drag
                    }
                    else
                    {
                        intensity = originalIntensity; // Vẫn giữ intensity trong thời gian grace
                    }
                }

                lastMousePosition = currentMousePosition; // Cập nhật vị trí chuột cho frame tiếp theo
            }
            else
            {
                // Chỉ set intensity = 0 sau khi thả tay >2 giây
                if (Time.time - lastTouchTime > NO_TOUCH_TIMEOUT)
                {
                    intensity = 0f; // Đặt intensity = 0 sau 2 giây không touch
                }
            }

            // Nếu không đang theo dõi target, tìm target mới
            if (!isDelaying)
            {
                FindTarget(); // Tìm kiếm target mới nếu cần
            }




            EyesAim2();                // Gọi hàm điều khiển mắt mỗi frame
        }
    }

    void FindTarget()
    {
        if (circleCompParent.isMergeAnimationPlaying)
        {
            target = circleCompParent.mergingNeighbor.transform; // Cập nhật target sang đối tượng merge mới
            isDelaying = true; // Đặt cờ để tránh gọi nhiều lần

            StartCoroutine(DelaySetTarget(GameManager.instance.draggingCircleGO.transform));
            return; // Không cần tìm target mới nếu đang trong quá trình merge
        }

        // Kiểm tra nếu target hiện tại có isFirstCollision = true, thì chuyển sang draggingCircleGO mới
        if (target != null && GameManager.instance.draggingCircleGO != null)
        {
            var circleComp = target.GetComponent<CircleComponent>();
            if (circleComp != null && !circleComp.isFirstCollision)
            {
                // Tạo GameObject tạm thời tại vị trí hiện tại của target
                if (fixedPositionTarget == null)
                {
                    fixedPositionTarget = new GameObject("FixedPositionTarget");
                }
                fixedPositionTarget.transform.position = circleComp.transform.position;
                target = fixedPositionTarget.transform;
                
                // Chuyển target sang draggingCircleGO mới sau delay
                isDelaying = true; // Đặt cờ để tránh gọi nhiều lần
                StartCoroutine(DelaySetTarget(GameManager.instance.draggingCircleGO.transform));
            }
        }
    }

    IEnumerator DelaySetTarget(Transform newTarget)
    {
        yield return new WaitForSeconds(2f); // Delay 2 giây trước khi set target mới

        isDelaying = false; // Reset cờ sau khi hoàn thành

        target = newTarget; // Cập nhật target mới
        Debug.Log($"[{name}] Target updated to: {target.name}");
    }

    void EyesAim()
    {
        // Lấy vị trí chuột trên màn hình => thế giới
        var mouseWorldCoord = camera.ScreenPointToRay(Input.mousePosition).origin;

        // Tính vector từ vị trí hiện tại đến chuột
        var originToMouse = mouseWorldCoord - this.transform.position;

        // Giới hạn khoảng cách để mắt không di chuyển quá xa
        originToMouse = Vector3.ClampMagnitude(originToMouse, intensity);

        // Di chuyển đối tượng mắt dần dần (lerp) về phía chuột (giúp chuyển động mượt)
        eyes.transform.position = Vector3.Lerp(
            eyes.transform.position,
            this.transform.position + originToMouse,
            speed * Time.deltaTime
        );
    }


    void EyesAim2()
    {
        // if (target == null || eyes == null || camera == null)
        // {
        //     Debug.LogWarning("Target, eyes or camera is not set!");
        //     return;
        // }
        // // Lấy vị trí chuột trên màn hình => thế giới
        // var mouseWorldCoord = camera.ScreenPointToRay(Input.mousePosition).origin;

        // Tính vector từ vị trí hiện tại đến target
        if (GameManager.instance.isGameOver)
        {
            GetComponent<EyesControl>().enabled = false; // Tắt điều khiển mắt nếu game over hoặc không có target

            return;
        }

        var originToTarget = target.position - this.transform.position;

        // Giới hạn khoảng cách để mắt không di chuyển quá xa
        originToTarget = Vector3.ClampMagnitude(originToTarget, intensity);

        // Di chuyển đối tượng mắt dần dần (lerp) về phía target (giúp chuyển động mượt)
        eyes.transform.position = Vector3.Lerp(
            eyes.transform.position,
            this.transform.position + originToTarget,
            speed * Time.deltaTime
        );
    }
}
