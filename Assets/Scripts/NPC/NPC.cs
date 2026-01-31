using Assets.Scripts.Interactions;
using Assets.Scripts.Quests;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCSO npcData;

    public virtual void Interact()
    {
        Debug.Log($"Interacted with NPC: {npcData.npcName}");
        QuestTracker.OnTalkedToNPC(npcData);
        // Additional interaction logic can be added here
    }

    public virtual bool CanInteract()
    {
        return true; // NPCs are always interactable in this example
    }
}
