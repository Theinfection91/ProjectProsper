using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "Scriptable Objects/NPC/NPC")]
public class NPCSO : ScriptableObject
{
    public string npcName;
    public string npcRole;
    public Sprite npcPortrait;
}
