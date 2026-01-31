using UnityEngine;

public class Banker : NPC
{
    public override void Interact()
    {
        Debug.Log("Welcome to the bank! How can I assist you today?");
        UIManager.Instance.OpenBankWindowUI();
    }
}
