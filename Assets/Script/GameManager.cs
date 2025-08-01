using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.IO;
using Sych.ShareAssets.Runtime;
using Sych.ShareAssets.Example.Tools;
using UnityEngine.SocialPlatforms.Impl;

public enum mouseState
{
    notChoosing,
    DestroyChoosing,
    UpgradeChoosing,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static event Action MouseNotChoosing;

    public static event Action SetDragging;

    public static mouseState MouseState { get; private set; } = mouseState.notChoosing;

    [SerializeField]
    private UnityEngine.Object[] Circles;

    private AnimalData nextCircleAData;
    private AnimalData draggingCircleAData;

    private GameObject nextCircleGO;
    public GameObject draggingCircleGO;

    [SerializeField]
    private RectTransform waitingUIPos;
    [SerializeField]
    private RectTransform nextUIPos;
    [SerializeField]
    public RectTransform draggingUIPos;
    private bool hasrun = false;
    [SerializeField] public SettingPanelUI pausePanel;
    public GameObject darkUIBackground;
    public bool isPaused = false;

    private List<CircleComponent> waitingColliders = new List<CircleComponent>();

    private Vector3 OriginScale;
    [SerializeField] public AnimalEvolutionTree evolutionTree;
    [SerializeField] private float baseOutlineWidth = 0.03f;
    public float BaseOutlineWidth => baseOutlineWidth;

    public int Scores = 0;

    private int HighScore;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private TextMeshProUGUI HighScoreText;

    [SerializeField]
    private TextMeshProUGUI YourScoreText;

    [SerializeField]
    private GameObject GameOverUICanvas;

    [SerializeField]

    public Vector3 droppingCirclePos;
    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject pipeGO;

    private PipeSquashEffect pipeSquash;

    private int circleSpawningSupportCount = 0; // Biến đếm số lượng circle đã spawn
    private int circleSpawningUnSupportCount = 0; 

    // Mảng lưu trữ các CircleComponent cùng loại
    public List<CircleComponent> warningCircles = new List<CircleComponent>();

    private float lineGameOverY;
    public bool isGameOver = false;

    public bool isPlayingTutorial = false;
    private int maxLevelSpawn = 5;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);

        Application.targetFrameRate = 120;
    }


    private void OnDisable()
    {
        MoveCircle.Setup -= DelaySpawnCircles;
        Booster.boosTer1 -= Destroy_Smallest;
        Booster.booster2 -= ChangeDestroyMouseState;
        Booster.booster3 -= ChangeUpgradeMouseState;
        MouseNotChoosing -= ChangeNotChoosingMouseState;
        // CircleComponent.AddCircleQueueToDestroy -= ReportCollision;
        CircleComponent.OnCircleMerged -= MergeCircles;
        // GameOverLine.GameOVer -= GameOver;
        MoveCircle.PracticeEffect -= PracticeEffect;
        CircleComponent.PracticeEffect -= PracticeEffect;

    }
    private void OnEnable()
    {
        MoveCircle.Setup += DelaySpawnCircles;
        Booster.boosTer1 += Destroy_Smallest;
        Booster.booster2 += ChangeDestroyMouseState;
        Booster.booster3 += ChangeUpgradeMouseState;
        MouseNotChoosing += ChangeNotChoosingMouseState;
        // CircleComponent.AddCircleQueueToDestroy += ReportCollision;
        CircleComponent.OnCircleMerged += MergeCircles;
        // GameOverLine.GameOVer += GameOver;
        MoveCircle.PracticeEffect += PracticeEffect;
        CircleComponent.PracticeEffect += PracticeEffect;

        if (evolutionTree == null)
        {
            evolutionTree = Resources.Load<AnimalEvolutionTree>("AnimalEvolutionTreeData");
            if (evolutionTree == null)
            {
                Debug.LogError("AnimalEvolutionTree not found!");
            }
        }

        pipeSquash = pipeGO.GetComponent<PipeSquashEffect>();
    }

    public int HandleCircleSpawningSupport()
    {
        bool isLowScore = Scores <= 1000;
        bool canSpawnSupport = (isLowScore && circleSpawningSupportCount <= 10) || (!isLowScore && circleSpawningSupportCount <= 5);

        if (canSpawnSupport)
        {
            circleSpawningSupportCount++;
            return GameManager.instance.GetHighestFruitByY();       // Lấy loại quả theo trục Y cao nhất
            //level = GameManager.instance.FruitCount();            // trả về loại quả có nhiều nhất
        }
        else
        {
            circleSpawningUnSupportCount++;

            bool shouldResetCount =
                (isLowScore && circleSpawningUnSupportCount >= 5) ||
                (!isLowScore && circleSpawningUnSupportCount >= 10);

            if (shouldResetCount)
            {
                circleSpawningSupportCount = 0;
                circleSpawningUnSupportCount = 0;
            }
            return 0; // Không spawn circle hỗ trợ, trả về 0
        }
    }
    private int GetHighestFruitByY()
    {
        CircleComponent highestCircleY = null;
        foreach (var circle in warningCircles)
        {

            if (circle != null)
            {
                if (circle.Level > maxLevelSpawn) continue; // Bỏ qua loại quả không trong phạm vi spawn
                if (highestCircleY == null || circle.transform.position.y > highestCircleY.transform.position.y)
                {
                    highestCircleY = circle.GetComponent<CircleComponent>();
                }
            }
        }
        if (highestCircleY == null)
        {
            Debug.LogWarning("Không tìm thấy CircleComponent nào trong warningCircles!");
            return 0; // Hoặc giá trị mặc định khác
        }

        // Debug.Log("Loại quả cao nhất:" + highestCircleY.Level);
        return highestCircleY.Level - 1; // Giảm 1 vì mảng bắt đầu từ 0
    }

    public void ListWarningCircles()
    {
        Debug.Log("Danh sách warningCircles:");
        foreach (var circle in warningCircles)
        {
            if (circle != null)
            {
                Debug.Log($"Circle: {circle.name}, Level: {circle.Level}, Position: {circle.transform.position}");
            }
            else
            {
                Debug.LogWarning("CircleComponent is null in warningCircles!");
            }
        }
    }

    private int FruitCount()
    {
        // Thống kê quả có số lượng nhiều nhất trong circleComponents
        Dictionary<int, int> fruitCount = new Dictionary<int, int>();

        foreach (var circle in warningCircles)
        {
            if (circle != null)
            {
                int fruitLevel = circle.Level; // Assuming fruitValue represents fruit type
                if (fruitCount.ContainsKey(fruitLevel))
                {
                    fruitCount[fruitLevel]++;
                }
                else
                {
                    fruitCount[fruitLevel] = 1;
                }
            }
        }

        // Tìm loại quả có số lượng nhiều nhất
        int mostFruitLevel = 0;
        int maxCount = 0;
        foreach (var kvp in fruitCount)
        {
            //Debug.Log($"Loại quả: {kvp.Key} với {kvp.Value} quả");
            if (kvp.Key > maxLevelSpawn) continue; // Bỏ qua loại quả không trong phạm vi spawn
            if (kvp.Value >= maxCount)
            {
                if (kvp.Key > mostFruitLevel)
                {
                    // Chỉ cập nhật nếu loại quả mới có số lượng lớn hơn hoặc bằng
                    // và loại quả mới có giá trị lớn hơn loại quả hiện tại
                    maxCount = kvp.Value;
                    mostFruitLevel = kvp.Key;
                }
            }
        }

        //Debug.Log($"Loại quả nhiều nhất: {mostFruitLevel} với {maxCount} quả");

        return mostFruitLevel - 1; // Giảm 1 vì mảng bắt đầu từ 0
    }



    void Start()
    {
        MouseState = mouseState.notChoosing;
        GameInit();
        Application.targetFrameRate = 60;

        HighScore = PlayerPrefs.GetInt("highscore", 0);
        HighScoreText.SetText(HighScore.ToString());

        GameObject lineGameOver = GameObject.Find("LineGameOver");
        if (lineGameOver != null)
        {
            lineGameOverY = lineGameOver.transform.position.y;
        }
    }

    private Vector3 UIToWorldPosition(RectTransform uiRect)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiRect.position);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }

    private void GameInit() // Tạo 2 circle khi bắt đầu game
    {
        droppingCirclePos = UIToWorldPosition(draggingUIPos);

        AnimalData data = evolutionTree.GetLevelData();
        if (data != null && data.prefab != null)
        {
            if (isPlayingTutorial)
            {
                InstantiateMergedCircle(5, new Vector3(0, -1.928036f, 0));
            }

            SpawnNextCircle(isPlayingTutorial);
            HandleSpawnCircles();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over triggered");
        isGameOver = true;

        if (HighScore < Scores) PlayerPrefs.SetInt("highscore", Scores);

        GameOverUICanvas.SetActive(true);
        YourScoreText.SetText("Score: " + Scores);
        Destroy(nextCircleGO);
    }

    public void GameRestart()
    {
        PlayerPrefs.Save();

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void HandleSpawnCircles()
    {
        pipeSquash.TriggerPipeSquash();

        draggingCircleGO = nextCircleGO;

        draggingCircleGO.AddComponent<PipeSquashEffect>();

        // Play swoosh sound effect
        AudioSource audioSource = draggingCircleGO.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = draggingCircleGO.AddComponent<AudioSource>();
        }

        // Set volume to 50%
        audioSource.volume = 0.5f;

        AudioClip swooshClip = Resources.Load<AudioClip>("SFX/funny-swish");
        if (swooshClip != null)
        {
            audioSource.clip = swooshClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("SFX/funny-swish.mp3 not found in Resources folder!");
        }

        draggingCircleGO.transform.DOMove(droppingCirclePos, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            draggingCircleGO.GetComponent<PipeSquashEffect>().TriggerDraggingSquash();

            var rb = draggingCircleGO.GetComponent<Rigidbody2D>();
            if (rb != null) rb.gravityScale = 0;

            var move = draggingCircleGO.GetComponent<MoveCircle>();
            if (move != null) move.isReady = true;

            var line = draggingCircleGO.GetComponent<LineRenderer>();
            if (line != null) line.enabled = true;
        });

        nextCircleGO = null;

        SpawnNextCircle();

        // SetDragging?.Invoke();
    }

    private void SpawnNextCircle(bool isPlayingTutorial = false)
    {
        if (isPlayingTutorial)
        {
            nextCircleAData = evolutionTree.GetLevelData(maxLevelSpawn - 1);
        }
        else
        {
            nextCircleAData = evolutionTree.GetLevelData();
        }
        if (nextCircleAData != null && nextCircleAData.prefab != null)
        {
            Vector3 spawnPos = UIToWorldPosition(waitingUIPos);
            nextCircleGO = Instantiate(nextCircleAData.prefab, spawnPos, nextCircleAData.prefab.transform.rotation);
            nextCircleGO.GetComponent<CircleComponent>().SetTargetScale(nextCircleAData.prefab.transform.localScale * nextCircleAData.scaleRatio);
            nextCircleGO.GetComponent<Rigidbody2D>().gravityScale = 0;
            nextCircleGO.GetComponent<MoveCircle>().isReady = false;
            nextCircleGO.GetComponent<LineRenderer>().enabled = false;
            nextCircleGO.transform.DOMove(UIToWorldPosition(nextUIPos), 0.5f).SetEase(Ease.OutBack);
        }
    }

    public GameObject SpawnAnimalAtLevel(int level, Vector3 position, bool isMerged = false)
    {
        AnimalData data = evolutionTree.GetLevelData(level - 1);
        if (data == null) return null;

        GameObject obj = Instantiate(data.prefab, position, data.prefab.transform.rotation);

        if (isMerged)
        {
            // Tắt auto scale ngay sau khi instantiate (trước khi Start() chạy)
            CircleComponent circleComp = obj.GetComponent<CircleComponent>();
            if (circleComp != null)
            {
                circleComp.isAutoScale = false; // Tắt auto scale cho merged objects
                // Debug.Log($"[{obj.name}] Đã tắt auto scale sau khi instantiate.");
            }
        }

        // obj.transform.localScale = data.prefab.transform.localScale * data.scaleRatio;
        obj.GetComponent<CircleComponent>().SetTargetScale(data.prefab.transform.localScale * data.scaleRatio);

        return obj;
    }
    private void MergeCircles(CircleComponent c1, CircleComponent c2, Vector3 spawnPos)
    {
        // Tắt collider trước khi move
        Collider2D col1 = c1.GetComponent<Collider2D>();
        if (col1 != null) col1.enabled = false;

        Collider2D col2 = c2.GetComponent<Collider2D>();
        if (col2 != null) col2.enabled = false;

        // Di chuyển về trung tâm trước (hút vào giữa)
        Rigidbody2D rb1 = c1.GetComponent<Rigidbody2D>();
        if (rb1 != null) rb1.freezeRotation = true;

        Rigidbody2D rb2 = c2.GetComponent<Rigidbody2D>();
        if (rb2 != null) rb2.freezeRotation = true;

        DG.Tweening.Sequence moveSeq = DOTween.Sequence();
        moveSeq.Append(c1.transform.DOMove(spawnPos, 0.1f).SetEase(Ease.OutBack));
        moveSeq.Join(c2.transform.DOMove(spawnPos, 0.1f).SetEase(Ease.OutBack));

        // Đợi di chuyển xong mới thực hiện các hiệu ứng và spawn mới
        moveSeq.OnComplete(() =>
        {
            HandleMergeEffects(c1, c2, spawnPos);
        });

    }
    private void HandleMergeEffects(CircleComponent c1, CircleComponent c2, Vector3 spawnPos)
    {
        // Spawn con vật cấp tiếp theo
        int nextLevel = c1.Level + 1;

        PracticeEffect("VFX/Custom_FruitExplosion", spawnPos, evolutionTree.levels[nextLevel - 1].colorEffect, nextLevel);

        bool isOverLineTriggeredChild = c1.isOverLineTriggered && c2.isOverLineTriggered;
        InstantiateMergedCircle(nextLevel, spawnPos, isOverLineTriggeredChild);

        //set scores
        SetScore(nextLevel);

        // Add force nổ
        Vector2 explosionPos = spawnPos; // ví dụ: transform.position
        float radius = 1f;
        float force = 1f;
        Explode(explosionPos, radius, force);

        c1.transform.DOKill();
        c2.transform.DOKill();
        c1.gameObject.SetActive(false); // Ẩn c1
        c2.gameObject.SetActive(false); // Ẩn c2

        // Huỷ 2 object cũ sau delay 3 giây
        StartCoroutine(DelayedDestroy(c1.gameObject, c2.gameObject, 3f));

    }

    private void PracticeEffect(String effectName, Vector3 position, Color colorEffect = default, int level = 2)
    {
        GameObject gameEffect = Resources.Load<GameObject>(effectName);
        if (gameEffect != null)
        {
            GameObject vfx2 = Instantiate(gameEffect, position, gameEffect.transform.rotation);
            SetColorEffect(vfx2, colorEffect); // Đổi màu hiệu ứng

            if (effectName == "VFX/Custom_FruitExplosion")
            {
                ScaleParticleEffectByLevel(vfx2, level, evolutionTree.GetMaxLevel(), 1.0f, 2.0f);
            }

            Destroy(vfx2, 2f);
        }
    }

    public static void ScaleParticleEffectByLevel(GameObject vfxRoot, int level, int maxLevel = 12, float minScale = 1.0f, float maxScale = 2.0f)
    {
        float levelFactor = Mathf.Clamp01((float)level / maxLevel);
        float finalScale = Mathf.Lerp(minScale, maxScale, levelFactor);

        var particleSystems = vfxRoot.GetComponentsInChildren<ParticleSystem>(includeInactive: true);

        foreach (var ps in particleSystems)
        {
            var main = ps.main;


            // Emission burst count
            var emission = ps.emission;
            if (emission.burstCount > 0)
            {
                var bursts = new ParticleSystem.Burst[emission.burstCount];
                emission.GetBursts(bursts);

                for (int i = 0; i < bursts.Length; i++)
                {
                    float baseCount = bursts[i].count.constant;
                    bursts[i].count = Mathf.Max(1, Mathf.RoundToInt(baseCount * level)); // Sửa từ levelFactor thành finalScale
                }

                emission.SetBursts(bursts);
            }

            // Shape scale (nếu là Cone)
            var shape = ps.shape;
            if (shape.shapeType == ParticleSystemShapeType.Cone)
            {
                Vector3 currentScale = shape.scale;
                shape.scale = new Vector3(
                    currentScale.x * finalScale * 3.5f,
                    currentScale.y * finalScale,
                    currentScale.z * finalScale * 3f
                );
            }
        }

        // Debug.Log($"[ScaleEffect] Scaled all particles with level {level} → scale {finalScale:F2}");
    }


    private void SetColorEffect(GameObject vfx, Color colorEffect)
    {
        // Đổi màu cho hiệu ứng (ví dụ: màu vàng)
        colorEffect.a = 1f; // Opacity 100%

        // Nếu là ParticleSystem - lấy tất cả ParticleSystem con
        var particleSystems = vfx.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            // Set startColor mode thành Random Between Two Colors
            Color darkColor = new Color(colorEffect.r * 0.9f, colorEffect.g * 0.9f, colorEffect.b * 0.9f, colorEffect.a);
            main.startColor = new ParticleSystem.MinMaxGradient(colorEffect, darkColor);
        }

    }

    public void InstantiateMergedCircle(int level, Vector3 spawnPos, bool isOverLineTriggeredChild = false)
    {
        if (level <= evolutionTree.GetMaxLevel() + 1)  //vẫn trong mảng circle có thể next được
        {
            GameObject newObj = SpawnAnimalAtLevel(level, spawnPos, true);
            if (newObj != null)
            {
                // Kiểm tra và set scale ngay sau khi spawn
                CircleComponent circleComp = newObj.GetComponent<CircleComponent>();
                if (circleComp != null && circleComp.targetScale != Vector3.zero)
                {
                    // Kill tween cũ để tránh bị override
                    newObj.transform.DOKill();

                    newObj.transform.localScale = circleComp.targetScale * 0.7f; // Giảm kích thước xuống 50%
                    // Debug.Log($"[{newObj.name}] Scale set to: {newObj.transform.localScale} (target: {circleComp.targetScale})");

                    // Tắt SquashStretch để tránh conflict
                    var squashStretch = newObj.GetComponent<SquashStretch>();
                    if (squashStretch != null)
                    {
                        squashStretch.enabled = false;
                    }

                    // Lấy SpriteRenderer để đổi màu
                    var spriteRenderer = newObj.GetComponentInChildren<SpriteRenderer>();
                    Color originalColor = Color.white;
                    if (spriteRenderer != null)
                    {
                        originalColor = spriteRenderer.color;
                        // Đổi màu tối khi sinh ra
                        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Màu tối
                        // Debug.Log($"[{newObj.name}] Sprite color set to dark");
                    }

                    // DOTween Sequence: Kết hợp Grow + SquashStretch
                    DG.Tweening.Sequence scaleSequence = DOTween.Sequence();

                    // Giai đoạn 1: Stretch (giãn X, co Y)
                    Vector3 stretchScale = new Vector3(
                        circleComp.targetScale.x * 1.1f, // Giãn X
                        circleComp.targetScale.y * 0.9f, // Co Y
                        circleComp.targetScale.z
                    );
                    scaleSequence.Append(newObj.transform.DOScale(stretchScale, 0.15f).SetEase(Ease.OutQuad));


                    // Giai đoạn 2: Squash (co X, giãn Y)
                    Vector3 squashScale = new Vector3(
                        circleComp.targetScale.x * 0.9f, // Co X
                        circleComp.targetScale.y * 1.1f, // Giãn Y
                        circleComp.targetScale.z
                    );
                    scaleSequence.Append(newObj.transform.DOScale(squashScale, 0.15f).SetEase(Ease.OutBack));

                    // Đồng thời đổi màu từ tối về bình thường trong giai đoạn settle
                    if (spriteRenderer != null)
                    {
                        scaleSequence.Join(spriteRenderer.DOColor(originalColor, 0.15f).SetEase(Ease.OutQuad));
                        // Debug.Log($"[{newObj.name}] Started color transition back to normal");
                    }

                    // Giai đoạn 3: Settle về targetScale (ổn định) + Đổi màu trở lại bình thường
                    scaleSequence.Append(newObj.transform.DOScale(circleComp.targetScale, 0.2f).SetEase(Ease.OutBounce));


                    // Giai đoạn 4: Bật lại SquashStretch sau khi animation xong
                    scaleSequence.OnComplete(() =>
                    {
                        if (squashStretch != null)
                        {
                            squashStretch.enabled = true;
                            // Debug.Log($"[{newObj.name}] SquashStretch re-enabled after combined grow+squash animation");
                        }

                        // Đảm bảo màu về đúng trạng thái cuối
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.color = originalColor;
                            // Debug.Log($"[{newObj.name}] Sprite color restored to normal");
                        }
                    });

                    // Debug.Log($"[{newObj.name}] Started combined grow+squash sequence: {newObj.transform.localScale} → squash → stretch → {circleComp.targetScale}");
                }
                else
                {
                    Debug.LogWarning($"[{newObj.name}] CircleComponent hoặc targetScale không hợp lệ!");
                }
                // Gán parent nếu cần
                newObj.transform.SetParent(GameObject.Find("Circles").transform);

                // Vô hiệu hóa LineRenderer nếu có
                LineRenderer lr = newObj.GetComponent<LineRenderer>();
                if (lr != null) lr.enabled = false;

                // Tùy chọn: Tạm tắt điều khiển (nếu cần delay)
                MoveCircle mv = newObj.GetComponent<MoveCircle>();
                if (mv != null) mv.enabled = false;
                mv.isDrop = true;

                // Bật Collider của con sau khi spawn
                Collider2D childCollider = newObj.GetComponentInChildren<CircleCollider2D>();
                if (childCollider != null)
                {
                    childCollider.enabled = true;
                }
                else
                {
                    Debug.LogWarning($"[{newObj.name}] Không tìm thấy CircleCollider2D trong object con.");
                }

                // // kiểm tra của mới sinh nằm dưới hay trên line game over
                // GameObject lineGameOver = GameObject.Find("LineGameOver");
                // if (lineGameOver != null)
                // {
                //     float fruitY = newObj.transform.position.y;
                //     float lineY = lineGameOver.transform.position.y;

                //     if (fruitY < lineY)
                //     {
                //         newObj.GetComponent<CircleComponent>().isOverLineTriggered = true;
                //         // Debug.Log("Quả mới sinh nằm dưới line game over");
                //         // Xử lý khi quả nằm dưới line
                //     }
                //     else
                //     {
                //         CircleComponent cc = newObj.GetComponent<CircleComponent>();
                //         if (cc != null)
                //         {
                //             // cc.StartCoroutine(cc.DelayCheckGameOver());
                //             newObj.GetComponent<CircleComponent>().isOverLineTriggered = false;

                //         }
                //         // Debug.Log("Quả mới sinh nằm trên hoặc bằng line game over");
                //         // Xử lý khi quả nằm trên hoặc bằng line
                //     }
                // }
                // else
                // {
                //     Debug.LogWarning("Không tìm thấy LineGameOver trong scene.");
                // }

                if (isPlayingTutorial)
                    newObj.GetComponent<CircleComponent>().isOverLineTriggered = true;
                else
                {
                    newObj.GetComponent<CircleComponent>().isOverLineTriggered = isOverLineTriggeredChild;
                }
            }
        }
    }
    private void SetScore(int score)
    {
        Scores += score;
        ScoreText.SetText(Scores.ToString());
    }


    // private UnityEngine.Object Find_Smallest_Fruit()
    // {
    //     Transform parent = GameObject.Find("Circles").transform;
    //     int min_index = 100;
    //     UnityEngine.Object smallest = null;

    //     foreach (var circle in parent)
    //     {
    //         string name = (circle as Transform).gameObject.name.Replace("(Clone)", "");

    //         for (int i = 0; i < Circles.Length; i++)
    //         {
    //             if (Circles[i].name == name)
    //             {
    //                 if (i <= min_index)
    //                 {
    //                     min_index = i;
    //                     smallest = (circle as Transform).gameObject;
    //                 }
    //             }
    //         }
    //     }
    //     return smallest;
    // }


    private void Destroy_Smallest()
    {
        UnityEngine.Object smallest = null;
        Transform parent = GameObject.Find("Circles").transform;
        foreach (var circle in parent)
        {
            smallest = (circle as Transform).gameObject;
            if (smallest.GetComponent<CircleComponent>().Level == 1 || smallest.GetComponent<CircleComponent>().Level == 2)
            {
                int smallestLevel = smallest.GetComponent<CircleComponent>().Level;
                PracticeEffect("VFX/Custom_FruitExplosion", smallest.GameObject().transform.position, evolutionTree.levels[smallestLevel - 1].colorEffect, smallestLevel);
                Destroy(smallest.GameObject());
            }
        }
    }

    private void ReportCollision(CircleComponent circle)
    {
        if (!waitingColliders.Contains(circle))
        {
            waitingColliders.Add(circle);
        }

        if (waitingColliders.Count >= 3)
        {
            HandleThreeCollisions();
            return;
        }

        Invoke("ClearwaitingColliders", 0.00001f);
    }

    private void HandleThreeCollisions()
    {
        // Lấy 2 con đầu để "biến mất"
        for (int i = 0; i < 2; i++)
        {
            Destroy(waitingColliders[i].gameObject);
        }

        Debug.Log("2 đối tượng đã bị huỷ, giữ lại: " + waitingColliders[2].name);

        // Reset danh sách
        waitingColliders.Clear();
    }

    private void ClearwaitingColliders()
    {
        for (int i = 0; i < waitingColliders.Count; i++)
        {
            Destroy(waitingColliders[i].gameObject);
        }
        waitingColliders.Clear();
    }

    public void Explode(UnityEngine.Vector2 center, float radius, float explosionForce)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.gameObject != this.gameObject)
            {
                Rigidbody2D rb = hit.attachedRigidbody;

                // Tính hướng từ tâm nổ ra đối tượng
                Vector2 direction = (hit.transform.position - (Vector3)center).normalized;

                // Áp lực tỷ lệ ngược với khoảng cách (tùy chỉnh nếu muốn)
                float distance = Vector2.Distance(center, hit.transform.position);
                float distanceFactor = Mathf.Clamp01(1 - distance / radius);

                // Thêm force
                rb.AddForce(direction * explosionForce * distanceFactor, ForceMode2D.Impulse);
            }
        }
    }

    void ResetFlag() => hasrun = false;

    public void TogglePause()
    {
        Debug.Log("TogglePause clicked");
        if (pausePanel == null)
        {
            Debug.LogWarning("pausePanel is null!");
            return;
        }

        isPaused = !isPaused;
        darkUIBackground.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.Show(); // DOTween show
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.Hide(); // DOTween hide
        }

        if (draggingCircleGO != null)
            draggingCircleGO.GetComponent<MoveCircle>().isBlockByUI = isPaused;
    }


    public void ResumeGame()
    {
        if (!isPaused) return;
        TogglePause(); // dùng lại toggle để đảm bảo đồng bộ
    }


    private void DelayNotChoosingMouseState()
    {
        MouseState = mouseState.notChoosing;
    }

    public static void TriggerMouseNotChoosing() => MouseNotChoosing?.Invoke();
    private void ChangeNotChoosingMouseState() => Invoke("DelayNotChoosingMouseState", 0.5f);
    private void ChangeDestroyMouseState() => MouseState = mouseState.DestroyChoosing;

    private void DelaySpawnCircles() => Invoke("HandleSpawnCircles", 0.2f);

    private void ChangeUpgradeMouseState() => MouseState = mouseState.UpgradeChoosing;

    private IEnumerator DelayedSquashTrigger(SquashStretch squashStretch, Vector2 normal, float velocity, Vector2 contactPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (squashStretch != null && squashStretch.enabled)
        {
            squashStretch.TriggerSquash(normal, velocity, contactPoint, false);
            Debug.Log($"[{squashStretch.name}] Triggered squash effect: normal={normal}, velocity={velocity}");
        }
    }

    /// Chụp ảnh màn hình và lưu ra file PNG, trả về đường dẫn file.
    /// </summary>
    public string CaptureScreenshot(string fileName = "merge.png")
    {
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(filePath, screenImage.EncodeToPNG());
        Destroy(screenImage);
        //Debug.Log($"Screenshot saved: {filePath}");
        return filePath;
    }

    public void ShareClicked()
    {
        if (!Share.IsPlatformSupported)  return;
       
        var items = new List<string>();
        items.Add(CaptureScreenshot("merge.png"));

        Share.Items(items, success =>     {
            //logView.LogMessage($"Share: {(success ? "success" : "failed")}");
        });

    }

    private IEnumerator DelayedDestroy(GameObject obj1, GameObject obj2, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Kiểm tra null trước khi destroy để tránh lỗi
        if (obj1 != null)
        {
            Destroy(obj1);
        }
        if (obj2 != null)
        {
            Destroy(obj2);
        }
    }
}
