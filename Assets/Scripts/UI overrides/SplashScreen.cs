using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SplashScreen : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
    private void Start()
    {
        StartCoroutine(PlaySplash());
    }
    private IEnumerator PlaySplash()
    {
        yield return Fade(0f, 1f, fadeInDuration);
        yield return new WaitForSeconds(visibleDuration);
        yield return Fade(1f, 0f, fadeOutDuration);

        MainMenu.Instance.OpenMainMenu();
        gameObject.SetActive(false);
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