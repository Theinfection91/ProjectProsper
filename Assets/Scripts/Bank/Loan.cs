using UnityEngine;

public class Loan
{
    public int id;
    public int loanAmount;
    public int dailyInterestRate;
    public int balanceToPay;
    public int daysSinceLoan;

    public Loan(int id, int amount, int interestRate)
    {
        this.id = id;
        loanAmount = amount;
        dailyInterestRate = interestRate;
        balanceToPay = amount;
        daysSinceLoan = 0;
    }
}
