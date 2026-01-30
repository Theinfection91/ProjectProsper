using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Image blackScreenFade;
    public float fadeDuration = .5f;

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
        blackScreenFade.gameObject.SetActive(false);
    }

    public void BlackScreenTransition()
    {
        StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        if (blackScreenFade != null)
        {
            blackScreenFade.gameObject.SetActive(true);

            // Hold black screen
            yield return new WaitForSeconds(0.5f);

            // Fade out
            blackScreenFade.CrossFadeAlpha(0f, fadeDuration, true);
            yield return new WaitForSeconds(fadeDuration);

            blackScreenFade.gameObject.SetActive(false);
            blackScreenFade.CrossFadeAlpha(1f, 0f, false);
        }
    }
}
