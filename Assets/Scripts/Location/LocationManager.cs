using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }

    private float teleportCooldown = 1.5f;
    private float lastTeleportTime = -Mathf.Infinity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void MovePlayerToLocation(string targetLocationID)
    {
        if (Time.time - lastTeleportTime < teleportCooldown)
        {
            Debug.Log("Teleport on cooldown. Please wait.");
            return;
        }

        lastTeleportTime = Time.time;

        LocationTransition[] transitionPoints = FindObjectsByType<LocationTransition>(FindObjectsSortMode.None);
        Debug.Log($"Transition points found: {transitionPoints.Length}");
        foreach (var transition in transitionPoints)
        {
            Debug.Log($"Checking transition from {transition.thisLocationID} {transition.thisLocationName} to {transition.targetLocationID} {transition.targetLocationName}");
            if (transition.thisLocationID == targetLocationID)
            {
                //GameObject player = GameObject.FindGameObjectWithTag("Player");
                //if (player != null)
                //{
                //    player.transform.position = transition.transform.position;
                //    Debug.Log($"Player moved to location: {locationID}");
                //}
                //else
                //{
                //    Debug.LogWarning("Player object not found!");
                //}
                //return;

                Debug.Log($"Transitioning to {transition.targetLocationID} at position {transition.transform.position}");
                PlayerMovement.Instance.transform.position = transition.transform.position;
                Debug.Log($"Player moved to location: {targetLocationID}");
            }
        }
    }
}
