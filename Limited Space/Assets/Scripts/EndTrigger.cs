using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndTrigger : MonoBehaviour
{
    public CanvasGroup firstEndPanelCanvasGroup; // Assign the first CanvasGroup for the end panel
    public CanvasGroup secondEndPanelCanvasGroup; // Assign the second CanvasGroup for the end panel
    public float fadeDuration = 2f; // Duration for fade-in effect
    public float waitTimeBeforeSceneChange = 2f; // Time to wait before changing scene

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(EndGameSequence());
        }
    }

    private IEnumerator EndGameSequence()
    {
        // Fade in the first panel
        yield return StartCoroutine(FadeInCanvasGroup(firstEndPanelCanvasGroup, fadeDuration));

        // Fade in the second panel
        yield return StartCoroutine(FadeInCanvasGroup(secondEndPanelCanvasGroup, fadeDuration));

        // Wait for a set time after the fade in completes
        yield return new WaitForSeconds(waitTimeBeforeSceneChange);

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, currentTime / duration); 
            yield return null;
        }

        canvasGroup.alpha = 1; 
    }
}
