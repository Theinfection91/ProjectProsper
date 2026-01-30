using System.Collections.Generic;
using UnityEngine;

public class BankManager : MonoBehaviour
{
    public static BankManager Instance { get; private set; }

    public int currentLoanRate = 2000;
    public int currentDailyDeduction = 10;
    public List<Loan> activeLoans = new();
    public List<Loan> paidOffLoans = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimeManager.Instance.OnEndOfDay += HandleEndOfDay;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanTakeLoan()
    {
        return PlayerCharacter.Instance.maxLoans > activeLoans.Count;
    }

    public void CreateNewLoan()
    {
        int activeAndPaidOffLoansCount = activeLoans.Count + paidOffLoans.Count;
        Loan newLoan = new(activeAndPaidOffLoansCount + 1, currentLoanRate, currentDailyDeduction);
        activeLoans.Add(newLoan);
        PlayerInventory.Instance.AddGold(currentLoanRate);
    }

    public void IncrementLoanDays(int days = 1)
    {
        foreach (Loan loan in activeLoans)
        {
            loan.daysSinceLoan += days;
        }
    }

    public void DeductLoanPayment()
    {
        if (activeLoans.Count == 0)
        {
            Debug.LogWarning("No loans to deduct payment from.");
            return;
        }

        int totalAmountToDeduct = 0;
        foreach (Loan loan in activeLoans)
        {
            totalAmountToDeduct += loan.dailyInterestRate;
        }
        PlayerInventory.Instance.RemoveGold(totalAmountToDeduct);
        Debug.Log($"Total amount deducted from player: {totalAmountToDeduct} gold.");
    }

    private void HandleEndOfDay()
    {
        IncrementLoanDays();
        DeductLoanPayment();
    }
}
