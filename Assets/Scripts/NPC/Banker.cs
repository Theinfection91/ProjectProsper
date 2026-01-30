using UnityEngine;

public class Banker : NPC
{
    public override void Interact()
    {
        Debug.Log("Welcome to the bank! How can I assist you today?");
        // Additional banking interaction logic can be added here
    }

    public override bool CanInteract()
    {
        // Add any specific conditions for interacting with the banker
        return true;
    }
}
