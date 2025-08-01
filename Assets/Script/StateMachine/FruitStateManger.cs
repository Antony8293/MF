using UnityEngine;

public class FruitStateManager : MonoBehaviour
{
    FruitBaseState currentState;

    public FruitDraggingState draggingState = new FruitDraggingState();
    public FruitIdleState idleState = new FruitIdleState();
    public FruitMergingState mergingState = new FruitMergingState();
    public FruitDeadState deadState = new FruitDeadState();

    private bool _isSpawnedByMerge = false;

    // Animation Clips
    public bool isAnimated = true;
    public AnimationClip deadClip;
    public AnimationClip idleClip;
    public AnimationClip mergeClip;

    // Components
    public MoveCircle _moveCircle;
    public Rigidbody2D _rigidbody;
    public Animator _animator;
    public EyesControl _eyesControl;
    public AimingComponent _aimingComponent;

    private void Awake()
    {
        // Initialize components
        _rigidbody = GetComponent<Rigidbody2D>();
        _moveCircle = GetComponent<MoveCircle>();
        _animator = GetComponentInChildren<Animator>();
        _eyesControl = GetComponentInChildren<EyesControl>();
        _aimingComponent = GetComponentInChildren<AimingComponent>();
    }

    private void OnEnable()
    {
        // Gán Animation Clips
        if (isAnimated)
        {
            var overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

            overrideController["eyefollow_idle_f1"] = idleClip;
            overrideController["eyefollow_merge_f1"] = mergeClip;
            overrideController["eyefollow_dead_f1"] = deadClip;

            _animator.runtimeAnimatorController = overrideController;
        }
        else
        {
            _animator.enabled = false;
        }

        _aimingComponent.gameObject.SetActive(false); // Ẩn AimingComponent ban đầu

    }

    private void Start()
    {
        currentState = draggingState;

        if (_isSpawnedByMerge) {
            currentState = mergingState; // Nếu được spawn bởi merge, chuyển sang trạng thái merging
        }

        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }

    public void SwitchState(FruitBaseState newState)
    {
        // currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
}