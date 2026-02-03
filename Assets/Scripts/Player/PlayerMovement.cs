using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    // Animation TODO
    public Animator playerAnimator;
    public Vector2 moveDirection = new(0, 0);
    public Vector2 lastKnownXDirection = new(0, 0);

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
        if (GameManager.IsGamePaused) return;
        if (isWorking) return;

        // Update animator playback speed according to time flow and pause state
        UpdateAnimatorPlaybackSpeed();

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

    private void UpdateAnimatorPlaybackSpeed()
    {
        if (playerAnimator == null) return;

        // If the game is paused or TimeSpeed is Paused, stop animation playback.
        if (GameManager.IsGamePaused || TimeManager.Instance.currentSpeed == TimeManager.TimeSpeed.Paused)
        {
            playerAnimator.speed = 0f;
            return;
        }

        // Multiply the base animator speed by the time flow multiplier.
        playerAnimator.speed = TimeManager.Instance.GetSpeedMultiplier();
    }

    public void SetWorkStatus(bool working)
    {
        isWorking = working;
    }
}
