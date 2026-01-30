using Assets.Scripts.Interactions;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public string npcName;

    public virtual void Interact()
    {
        Debug.Log($"Interacted with NPC: {npcName}");
        // Additional interaction logic can be added here
    }

    public virtual bool CanInteract()
    {
        return true; // NPCs are always interactable in this example
    }
}
