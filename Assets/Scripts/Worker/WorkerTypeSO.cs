using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkerType", menuName = "Scriptable Objects/Worker/WorkerTypeSO")]
public class WorkerTypeSO : ScriptableObject
{
    [Header("Basic Info")]
    public string workerName;
    [TextArea]
    public string description;
    public Sprite workerIcon;

    [Header("Category")]
    public WorkerCategory category;

    [Header("Costs & Wages")]
    public int baseHiringCost;
    public int baseDailyWage;

    [Header("Skills & Progression")]
    public bool canLevelUp;
    public bool canMentor;

    [Header("Workable Shops")]
    public List<ShopTypeSO> employableShops = new();

    [Header("For Gatherers: What They Produce")]
    [Tooltip("Leave empty if not a gatherer")]
    public List<ItemSO> gatherableResources = new();
    public int baseGatherRate;

    //[Header("For Service Workers: What Shops They Work In")]
    //[Tooltip("Leave empty if not a service specialist")]
    
}
