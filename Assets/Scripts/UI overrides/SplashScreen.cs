using System.Collections;
using UnityEngine;

/// <summary>
/// Handles intro fading in and out when opening main menu
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class SplashScreen : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;
    private CanvasGroup canvasGroup;
    private MainMenu menu;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
    private void Start()
    {
        menu = MainMenu.Instance;
        StartCoroutine(PlaySplash());
    }
    private IEnumerator PlaySplash()
    {
        menu.isInSplashScreen = true;
        yield return Fade(0f, 1f, fadeInDuration);
        yield return new WaitForSeconds(visibleDuration);
        yield return Fade(1f, 0f, fadeOutDuration);
        menu.EndSplashScreen();
    }
    private IEnumerator Fade(float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}