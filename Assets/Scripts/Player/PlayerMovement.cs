using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    // Animation TODO
    //public Animator playerAnimator;
    public Vector2 moveDirection = new(0, 0);
    public Vector2 lastKnownXDirection = new(0, 0);

    // Input
    public InputAction moveAction;
    public InputAction interactAction;

    // Move
    public float moveSpeed = 5;
    public bool canMove = true;
    public Vector2 _moveVector;
    private bool _isMoving = false;

    // Physics
    public Rigidbody2D rb2d;

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
        // Stop if game is paused.
        if (GameManager.IsGamePaused) return;

        //if (Keyboard.current.eKey.wasPressedThisFrame)
        //{
        //    InteractWithNPC();
        //    InteractWithForRentSign();
        //    InteractWithShopCommand();
        //}

        //if (Keyboard.current.iKey.wasPressedThisFrame)
        //{
        //    UIManager.Instance.OpenCloseInventoryWindow();
        //}

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
        //playerAnimator.SetFloat("Horizontal", moveDirection.x);
        //playerAnimator.SetFloat("Vertical", moveDirection.y);

        _isMoving = _moveVector != Vector2.zero;
        //playerAnimator.SetBool("isMoving", _isMoving);
    }

    private void FixedUpdate()
    {
        // Stop if game is paused.
        if (GameManager.IsGamePaused) return;

        if (canMove)
        {
            Vector2 position = rb2d.position + _moveVector * moveSpeed * Time.deltaTime;
            rb2d.MovePosition(position);
        }
    }
}
