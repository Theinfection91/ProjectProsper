using Unity.Cinemachine;
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
            // TODO Black screen transition effect

            LocationManager.Instance.MovePlayerToLocation(targetLocationID);
        }
    }
}
