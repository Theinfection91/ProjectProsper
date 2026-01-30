using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Banker UI
    [Header("Banker UI")]
    public Canvas bankUICanvas;

    // ShopSign UI
    [Header("ShopSign UI")]
    public Canvas shopSignCanvas;
    public TMP_Text shopSizeText;
    public TMP_Text itemSlotsText;
    public TMP_Text rentAmountText;
    public TMP_Text maxWorkersAmountText;
    public TMP_Text depositAmountText;
    public TMP_Text footTrafficAmountText;
    public TMP_InputField shopNameInput;
    public ShopSign currentShopSign;

    // ShopCommand (Vendor Stall) UI
    [Header("Shop Command (Vendor Stall) UI")]
    public Canvas shopCommandCanvas;
    public GameObject shopInvSlot;
    public Image shopInvSlotPanel;

    // Black Screen Fade
    [Header("Black Screen Fade")]
    public Image blackScreenFade;
    public float fadeDuration = .5f;

    // Clock UI
    [Header("Clock UI")]
    public TMP_Text clockText;

    // Gold UI
    [Header("Gold UI")]
    public TMP_Text goldAmountText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        if (bankUICanvas != null)
        {
            bankUICanvas.enabled = false;
            //errorMessageText.gameObject.SetActive(false);
            blackScreenFade.gameObject.SetActive(false);
        }
        shopSignCanvas.enabled = false;
        shopCommandCanvas.enabled = false;
    }

    void Update()
    {
        UpdateGoldAmount();
    }

    public void FixedUpdate()
    {
        UpdateClock();
    }

    public void UpdateClock()
    {
        clockText.text = $"Day: {TimeManager.Instance.dayCount} | Time: {TimeManager.Instance.currentTimeOfDay * 24f:00}:{(TimeManager.Instance.currentTimeOfDay * 24f % 1) * 60f:00}";

        clockText.text = TimeManager.Instance.GetFormattedTime();
    }

    public void UpdateGoldAmount()
    {
        goldAmountText.text = $"{PlayerInventory.Instance.gold}";
    }

    public void BlackScreenTransition()
    {
        StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        if (blackScreenFade != null)
        {
            GameManager.Instance.PauseGame();
            blackScreenFade.gameObject.SetActive(true);

            // Hold black screen
            yield return new WaitForSeconds(0.5f);

            // Fade out
            blackScreenFade.CrossFadeAlpha(0f, fadeDuration, true);
            yield return new WaitForSeconds(fadeDuration);

            blackScreenFade.gameObject.SetActive(false);
            blackScreenFade.CrossFadeAlpha(1f, 0f, false);
            GameManager.Instance.ResumeGame();
        }
    }

    public void OpenBankWindowUI()
    {
        if (bankUICanvas == null) return;
        bankUICanvas.enabled = true;
        GameManager.Instance.PauseGame();
    }

    public void CloseBankWindowUI()
    {
        if (bankUICanvas == null) return;
        bankUICanvas.enabled = false;
        GameManager.Instance.ResumeGame();
    }

    public void OnTakeOutLoanButtonClicked()
    {
        if (!BankManager.Instance.CanTakeLoan())
        {
            // TODO : Show error message to player
            Debug.LogWarning("Cannot take out more loans. Maximum limit reached.");
            return;

        }
        else
        {
            BankManager.Instance.CreateNewLoan();
            Debug.Log("New loan taken out successfully.");
        }
    }

    public void OpenShopSignUI()
    {
        if (shopSignCanvas == null) return;
        shopSignCanvas.enabled = true;

        // Reset the input field text and deselect it
        shopNameInput.text = "Your new shop's name here...";
        shopNameInput.DeactivateInputField();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        GameManager.Instance.PauseGame();
    }

    public void CloseShopSignUI()
    {
        if (shopSignCanvas == null) return;

        // Deselect and deactivate the input field before hiding
        shopNameInput.DeactivateInputField();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        shopSignCanvas.enabled = false;
        GameManager.Instance.ResumeGame();
    }

    public void PopulateShopSignData(ShopSign currentShopSign)
    {
        this.currentShopSign = currentShopSign;
        shopSizeText.text = $"Size: {currentShopSign.size}";
        itemSlotsText.text = $"Item Slots (Start/Max): {currentShopSign.currentItemSlots}/{currentShopSign.maxItemSlots}";
        maxWorkersAmountText.text = $"Max Workers: {currentShopSign.maxWorkers}";
        rentAmountText.text = $"Daily Rent Amount: {currentShopSign.dailyRent}";
        depositAmountText.text = $"Deposit (One Time): {currentShopSign.initialDepositAmount}";
        footTrafficAmountText.text = $"Foot Traffic Index: {currentShopSign.footTrafficScore}";
    }

    public void OnOpenShopButtonClicked()
    {
        if (currentShopSign == null) return;

        if (shopNameInput.text == "Your new shop's name here..." || shopNameInput.text.Length < 5) return;

        if (!ShopManager.Instance.CanPlayerOpenNewShop() || !PlayerInventory.Instance.HasGoldAmount(currentShopSign.dailyRent + currentShopSign.initialDepositAmount))
        {
            // Add UI logic showing error message that player has max shops or not enough gold
            return;
        }

        // Deduct costs
        PlayerInventory.Instance.RemoveGold(currentShopSign.dailyRent + currentShopSign.initialDepositAmount);

        // Create shop and add to list
        ShopManager.Instance.CreatePlayerShop(shopNameInput.text, currentShopSign.size, currentShopSign.dailyRent, currentShopSign.footTrafficScore, currentShopSign.currentItemSlots, currentShopSign.maxItemSlots);

        Debug.Log($"{ShopManager.Instance.GetShop(shopNameInput.text).name} created.");

        // Increment player shop count
        PlayerCharacter.Instance.currentShopsCount++;

        currentShopSign.ClaimProperty(8);
        currentShopSign.isClaimed = true;
        currentShopSign.claimedShop = ShopManager.Instance.GetShop(shopNameInput.text);
        currentShopSign = null;

        CloseShopSignUI();
    }
}
