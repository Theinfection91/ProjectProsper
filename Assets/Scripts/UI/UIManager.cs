using Assets.Scripts.Item;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Current Opened UI")]
    // Reference to the currently opened canvas
    public Canvas currentOpenedCanvas;

    [Header("Dialogue UI")]
    public Canvas dialogueCanvas;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueContentText;

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
    public TMP_Text shopNameText;
    public GameObject shopInvSlot;
    public Image shopInvSlotPanel;
    public TMP_Text todaysEarningsText;
    public TMP_Text yesterdayEarningsText;
    public TMP_Text totalEarningsText;

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

    [Header("Wholesale UI")]
    public Canvas wholesaleCanvas;
    public GameObject itemForSaleSlot;
    public Image itemForSaleSlotPanel;

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
        dialogueCanvas.enabled = false;
        if (bankUICanvas != null)
        {
            bankUICanvas.enabled = false;
            //errorMessageText.gameObject.SetActive(false);
            blackScreenFade.gameObject.SetActive(false);
        }
        shopSignCanvas.enabled = false;
        shopCommandCanvas.enabled = false;
        wholesaleCanvas.enabled = false;
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

    public void OnTimeControlButtonClicked(int buttonNumber)
    {
        switch (buttonNumber)
        {
            case 0:
                TimeManager.Instance.currentSpeed = TimeManager.TimeSpeed.Paused;
                if (GameManager.IsGamePaused == false)
                {
                    GameManager.Instance.PauseGame();
                }
                break;
            case 1:
                TimeManager.Instance.currentSpeed = TimeManager.TimeSpeed.Normal;
                break;
            case 2:
                TimeManager.Instance.currentSpeed = TimeManager.TimeSpeed.Fast;
                break;
            case 3:
                TimeManager.Instance.currentSpeed = TimeManager.TimeSpeed.Faster;
                break;
            case 4:
                TimeManager.Instance.currentSpeed = TimeManager.TimeSpeed.Ultra;
                break;
            default:
                TimeManager.Instance.currentSpeed = TimeManager.TimeSpeed.Normal;
                break;
        }
        if (buttonNumber != 0 && GameManager.IsGamePaused)
        {
            GameManager.Instance.ResumeGame();
        }
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

    public void CloseCurrentUIWindow()
    {
        if (currentOpenedCanvas != null)
        {
            if (currentOpenedCanvas == shopSignCanvas)
            {
                // Deselect and deactivate the input field before hiding
                shopNameInput.DeactivateInputField();
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
            currentOpenedCanvas.enabled = false;
            GameManager.Instance.ResumeGame();
        }
    }

    public void OpenBankWindowUI()
    {
        if (bankUICanvas == null) return;
        bankUICanvas.enabled = true;
        currentOpenedCanvas = bankUICanvas;
        GameManager.Instance.PauseGame();
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
        currentOpenedCanvas = shopSignCanvas;

        // Reset the input field text and deselect it
        shopNameInput.text = "Your new shop's name here...";
        shopNameInput.DeactivateInputField();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        GameManager.Instance.PauseGame();
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
        ShopManager.Instance.CreatePlayerShop(currentShopSign.id, shopNameInput.text, currentShopSign.size, currentShopSign.dailyRent, currentShopSign.footTrafficScore, currentShopSign.currentItemSlots, currentShopSign.maxItemSlots);

        Debug.Log($"{ShopManager.Instance.GetShop(currentShopSign.id).name} created.");

        // Increment player shop count
        PlayerCharacter.Instance.currentShopsCount++;

        currentShopSign.ClaimProperty(8);
        currentShopSign.isClaimed = true;
        currentShopSign = null;

        //CloseShopSignUI();
        CloseCurrentUIWindow();
    }

    public void PopulateStallShopCommand(Shop shop)
    {
        if (shopCommandCanvas == null) return;
        shopNameText.text = shop.name;
        todaysEarningsText.text = $"Today's Earnings: {shop.earningsToday}";
        yesterdayEarningsText.text = $"Yesterday's Earnings: {shop.yesterdayEarnings}";
        totalEarningsText.text = $"Total Earnings: {shop.totalEarnings}";
        // Clear existing slots
        foreach (Transform child in shopInvSlotPanel.transform)
        {
            Destroy(child.gameObject);
        }
        // Populate with current slot amount
        for (int i = 0; i < shop.currentItemSlots; i++)
        {
            GameObject slot = Instantiate(shopInvSlot, shopInvSlotPanel.transform);
        }
    }

    public void OpenStallShopCommand()
    {
        if (shopCommandCanvas == null) return;
        shopCommandCanvas.enabled = true;
        currentOpenedCanvas = shopCommandCanvas;
        GameManager.Instance.PauseGame();
    }

    public void OpenWholesaleUI()
    {
        if (wholesaleCanvas == null) return;
        wholesaleCanvas.enabled = true;
        currentOpenedCanvas = wholesaleCanvas;
        GameManager.Instance.PauseGame();
    }

    public void CloseWholesaleUI()
    {
        if (wholesaleCanvas == null) return;
        wholesaleCanvas.enabled = false;
        GameManager.Instance.ResumeGame();
    }

    public void PopulateWholesaleItems(List<ItemData> itemsForSale)
    {
        // Clear existing slots
        foreach (Transform child in itemForSaleSlotPanel.transform)
        {
            Destroy(child.gameObject);
        }
        // Populate with items for sale
        foreach (var item in itemsForSale)
        {
            GameObject slot = Instantiate(itemForSaleSlot, itemForSaleSlotPanel.transform);
            ItemForSaleSlot slotComponent = slot.GetComponent<ItemForSaleSlot>();
            if (slotComponent != null)
            {
                slotComponent.SetupItemForSale(item);
            }
        }
    }
}
