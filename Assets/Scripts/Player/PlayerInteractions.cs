using Assets.Scripts.Interactions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public static PlayerInteractions Instance { get; private set; }

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

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (GameManager.IsGamePaused) return;
            Interact();
        }
    }

    private RaycastHit2D ShootRaycast(string layerName)
    {
        Vector2 direction = PlayerMovement.Instance.moveDirection.sqrMagnitude > 0 ? PlayerMovement.Instance.moveDirection : PlayerMovement.Instance.lastKnownXDirection;
        Debug.DrawRay(transform.position, direction * 0.6f, Color.red, 1f);
        return Physics2D.Raycast(transform.position, direction, 0.6f, LayerMask.GetMask(layerName));
    }

    private void Interact()
    {
        RaycastHit2D hit;
        foreach (var layer in InteractableLayers.layers)
        {
            hit = ShootRaycast(layer);
            if (hit.collider != null)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract())
                {
                    interactable.Interact();
                    break;
                }
            }
        }
    }
}
