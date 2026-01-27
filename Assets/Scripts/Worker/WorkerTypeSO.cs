using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkerType", menuName = "Scriptable Objects/WorkerTypeSO")]
public class WorkerTypeSO : ScriptableObject
{
    [Header("Basic Info")]
    public string workerTypeName;
    public string description;
    public Sprite workerIcon;

    [Header("Category")]
    public WorkerCategory workerCategory;

    [Header("Costs & Wages")]
    public int hiringCost;
    public int dailyWage;

    [Header("Skills & Progression")]
    public bool canLevelUp;
    public bool canMentor;

    [Header("For Gatherers: What They Produce")]
    [Tooltip("Leave empty if not a gatherer")]
    //public ItemTypeSO gatheredResource;
    public int gatherRatePerDay;

    [Header("For Service Workers: What Shops They Work In")]
    [Tooltip("Leave empty if not a service specialist")]
    public ShopTypeSO primaryShop;
}
