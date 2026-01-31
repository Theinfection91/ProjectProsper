using Assets.Scripts.NPC;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(NPCSO))]
public class NPCSOEditor : Editor
{
    private static string[] _roleOptions;

    private static string[] GetRoleOptions()
    {
        if (_roleOptions == null)
        {
            _roleOptions = typeof(NPCRoles)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => (string)fi.GetRawConstantValue())
                .Concat(new[] { "Custom..." })
                .ToArray();
        }
        return _roleOptions;
    }

    public override void OnInspectorGUI()
    {
        NPCSO npc = (NPCSO)target;

        npc.npcName = EditorGUILayout.TextField("NPC Name", npc.npcName);

        var roleOptions = GetRoleOptions();
        int selected = Array.IndexOf(roleOptions, npc.npcRole);
        bool isCustom = false;

        // If the current value is not in the list or is empty, treat as custom
        if (selected == -1)
        {
            isCustom = !string.IsNullOrEmpty(npc.npcRole);
            selected = roleOptions.Length - 1; // "Custom..."
        }

        int newSelected = EditorGUILayout.Popup("NPC Role", selected, roleOptions);

        if (newSelected < roleOptions.Length - 1) // Not "Custom..."
        {
            npc.npcRole = roleOptions[newSelected];
        }
        else
        {
            // If just switched to custom, clear the field if it wasn't already custom
            if (!isCustom && selected != newSelected)
                npc.npcRole = "";

            npc.npcRole = EditorGUILayout.TextField("Custom Role", npc.npcRole);
        }

        npc.npcPortrait = (Sprite)EditorGUILayout.ObjectField("Portrait", npc.npcPortrait, typeof(Sprite), false);

        if (GUI.changed)
            EditorUtility.SetDirty(npc);
    }
}
