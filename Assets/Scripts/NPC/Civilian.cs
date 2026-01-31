using UnityEngine;

public class Civilian : NPC
{
    public override void Interact()
    {
        // TODO: Implement civilian-specific interaction logic
        // Eventually this will pull from a dialogue system by passing npcData and triggering correct responses
        Debug.Log($"Hello! I'm {npcData.npcName}, just a regular civilian.");
    }
}
