using UnityEngine;

public class EyesControl : MonoBehaviour
{
    public GameObject eyes;           // Đối tượng "mắt" sẽ di chuyển theo chuột
    public Camera camera;             // Camera chính dùng để chuyển đổi tọa độ màn hình => thế giới
    public Transform target; // Mục tiêu để mắt hướng tới (có thể là chuột hoặc một đối tượng khác)
    public float intensity = 0.5f; // Khả năng di chuyển tối đa của mắt
    public float speed = 5f; // Tốc độ di chuyển của mắt
    
    private float originalIntensity; // Lưu giá trị intensity ban đầu
    private float lastTouchTime; // Thời gian touch cuối cùng
    private const float NO_TOUCH_TIMEOUT = 2f; // 2 giây không touch
    void Start()
    {
        camera = Camera.main;         // Gán camera mặc định của scene (nếu chưa gán sẵn trong Inspector)
        originalIntensity = intensity; // Lưu giá trị intensity ban đầu
        lastTouchTime = Time.time; // Khởi tạo thời gian touch cuối cùng

        target = GameManager.instance.draggingCircleGO.transform; // Gán mục tiêu là CircleComponent đang kéo   
    }

    void Update()
    {
        if (camera != null)
        {
            // Kiểm tra touch screen - nếu có touch thì khôi phục intensity, thả tay >2s mới set về 0
            if (Input.GetMouseButton(0))
            {
                lastTouchTime = Time.time; // Cập nhật thời gian touch cuối cùng
                intensity = originalIntensity; // Khôi phục intensity khi có touch
            }
            else
            {
                // Chỉ set intensity = 0 sau khi thả tay >2 giây
                if (Time.time - lastTouchTime > NO_TOUCH_TIMEOUT)
                {
                    intensity = 0f; // Đặt intensity = 0 sau 2 giây không touch
                }
            }
            
            // Kiểm tra nếu target hiện tại có isFirstCollision = true, thì chuyển sang draggingCircleGO mới
            if (target != null && GameManager.instance.draggingCircleGO != null)
            {
                var circleComp = target.GetComponent<CircleComponent>();
                if (circleComp != null && !circleComp.isFirstCollision)
                {
                    // Chuyển target sang draggingCircleGO mới
                    target = GameManager.instance.draggingCircleGO.transform;
                }
            }
            
            EyesAim2();                // Gọi hàm điều khiển mắt mỗi frame
        }
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
        // // Lấy vị trí chuột trên màn hình => thế giới
        // var mouseWorldCoord = camera.ScreenPointToRay(Input.mousePosition).origin;

        // Tính vector từ vị trí hiện tại đến target
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
