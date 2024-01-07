using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    public GameObject characterPortrait; // Assign in Inspector
    public TextMeshProUGUI dialogueText; // Assign in Inspector
    public GameObject dialoguePanel; // Assign in Inspector

    public CanvasGroup characterPortraitGroup; // Assign in Inspector
    public CanvasGroup dialoguePanelGroup; // Assign in Inspector
    public CanvasGroup dialogueTextGroup; // Assign in Inspector
    public float fadeDuration = 1f; // Duration for fade-in effect

    private List<string> dialogues;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private bool isFadingIn = false;
    private float typingSpeed = 0.06f;

    void Start()
    {
        // Initialize dialogues and set up UI
        InitializeDialogues();
        StartCoroutine(ShowDialogueUI());
    }

    void Update()
    {
        HandleInput();
    }

    private void InitializeDialogues()
    {
        dialogues = new List<string>()
        {
            // Add your dialogues here
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."

            // ...
        };
    }

    private void HandleInput()
    {
        if (isFadingIn)
        {
            // If fading in, no further input handling needed here
            return;
        }

        if (Input.anyKeyDown)
        {
            if (isTyping)
            {
                // Show the full string if currently typing
                CompleteCurrentDialogue();
            }
            else
            {
                // Move to the next dialogue line
                DisplayNextDialogue();
            }
        }
    }

    private IEnumerator ShowDialogueUI()
    {
        isFadingIn = true;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            if (Input.anyKeyDown)
            {
                // If any key is pressed, complete the fade instantly
                SetUIVisible();
                StartCoroutine(TypeDialogue(dialogues[currentDialogueIndex]));
                yield break; // Exit the coroutine
            }

            float alpha = elapsedTime / fadeDuration;
            SetCanvasGroupAlpha(characterPortraitGroup, alpha);
            SetCanvasGroupAlpha(dialoguePanelGroup, alpha);
            SetCanvasGroupAlpha(dialogueTextGroup, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetUIVisible();
        StartCoroutine(TypeDialogue(dialogues[currentDialogueIndex]));
    }

    private void SetCanvasGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
        {
            group.alpha = alpha;
        }
    }

    private void SetUIVisible()
    {
        SetCanvasGroupAlpha(characterPortraitGroup, 1);
        SetCanvasGroupAlpha(dialoguePanelGroup, 1);
        SetCanvasGroupAlpha(dialogueTextGroup, 1);
        isFadingIn = false;
    }

    private void CompleteCurrentDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = dialogues[currentDialogueIndex];
        isTyping = false;
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void DisplayNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Count - 1)
        {
            currentDialogueIndex++;
            StartCoroutine(TypeDialogue(dialogues[currentDialogueIndex]));
        }
        else
        {
            StartCoroutine(TransitionToNextScene());
        }
    }

    private IEnumerator TransitionToNextScene()
    {
        yield return new WaitForSeconds(1); // Delay before scene change
        SceneManager.LoadScene("Sandbox");
    }
}
