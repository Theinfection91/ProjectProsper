using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Banker UI
    public Canvas bankUICanvas;

    // Black Screen Fade
    public Image blackScreenFade;
    public float fadeDuration = .5f;

    // Clock UI
    public TMP_Text clockText;

    // Gold UI
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
}
