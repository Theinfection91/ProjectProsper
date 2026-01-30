using UnityEngine;

public class LocationTransition : MonoBehaviour
{
    public string thisLocationID;
    public string thisLocationName;

    public string targetLocationID;
    public string targetLocationName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Reset physics before teleporting
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            UIManager.Instance.BlackScreenTransition();
            LocationManager.Instance.MovePlayerToLocation(targetLocationID);
        }
    }
}
