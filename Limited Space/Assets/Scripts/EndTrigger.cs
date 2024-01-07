using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndTrigger : MonoBehaviour
{
    public CanvasGroup endPanelCanvasGroup; // Assign the CanvasGroup for the end panel
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
        yield return StartCoroutine(FadeInCanvasGroup(endPanelCanvasGroup, fadeDuration));

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
