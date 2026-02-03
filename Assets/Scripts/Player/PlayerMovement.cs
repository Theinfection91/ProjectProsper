using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    // Animation TODO
    public Animator playerAnimator;
    public Vector2 moveDirection = new(0, 0);
    public Vector2 lastKnownXDirection = new(0, 0);

    // Animator speed scaling
    [Header("Animation Speed")]
    [Tooltip("Base animator playback speed (1 = normal). Multiplied by TimeManager speed multiplier.")]
    public float baseAnimatorSpeed = 1f;
    [Tooltip("When true, animator.playback will be multiplied by TimeManager.GetSpeedMultiplier() (and paused when game is paused).")]
    public bool scaleAnimatorWithTime = true;

    // Input
    public InputAction moveAction;
    public InputAction interactAction;

    // Move
    public float baseMoveSpeed = 5f; // Set this in the inspector or here
    public bool canMove = true;
    public Vector2 _moveVector;
    private bool _isMoving = false;

    // Physics
    public Rigidbody2D rb2d;

    // Work
    public bool isWorking = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction.Enable();
        interactAction.Enable();
    }

    // Update is called once per frame
    private void Update()
    {
        // Update animator playback speed according to time flow and pause state
        UpdateAnimatorPlaybackSpeed();

        // Stop if game is paused.
        if (GameManager.IsGamePaused) return;
        if (isWorking) return;

        _moveVector = moveAction.ReadValue<Vector2>();
        if (moveDirection.x >= 1 || moveDirection.x <= -1)
        {
            lastKnownXDirection.Set(moveDirection.x, 0);
        }

        if (!Mathf.Approximately(_moveVector.x, 0.0f) || !Mathf.Approximately(_moveVector.y, 0.0f))
        {
            moveDirection.Set(_moveVector.x, _moveVector.y);
            moveDirection.Normalize();
        }

        // Update the animator parameters
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Horizontal", moveDirection.x);
            playerAnimator.SetFloat("Vertical", moveDirection.y);
        }

        _isMoving = _moveVector != Vector2.zero;
        if (playerAnimator != null)
            playerAnimator.SetBool("isMoving", _isMoving);
    }

    private void FixedUpdate()
    {
        // Stop if game is paused.
        if (GameManager.IsGamePaused) return;

        if (canMove)
        {
            float effectiveSpeed = baseMoveSpeed;
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                effectiveSpeed = 15f;
            }
            effectiveSpeed *= TimeManager.Instance.GetSpeedMultiplier();

            Vector2 position = rb2d.position + effectiveSpeed * Time.deltaTime * _moveVector;
            rb2d.MovePosition(position);
        }
    }

    /// <summary>
    /// Adjusts the Animator playback speed according to the TimeManager multiplier and pause state.
    /// Setting Animator.speed scales all layer/playback rates (useful for speeding up idle/walk cycles).
    /// </summary>
    private void UpdateAnimatorPlaybackSpeed()
    {
        if (playerAnimator == null || !scaleAnimatorWithTime) return;

        // If the game is paused or TimeSpeed is Paused, stop animation playback.
        if (GameManager.IsGamePaused || TimeManager.Instance.currentSpeed == TimeManager.TimeSpeed.Paused)
        {
            playerAnimator.speed = 0f;
            return;
        }

        // Multiply the base animator speed by the time flow multiplier.
        playerAnimator.speed = baseAnimatorSpeed * TimeManager.Instance.GetSpeedMultiplier();
    }

    public void SetWorkStatus(bool working)
    {
        isWorking = working;
    }
}
