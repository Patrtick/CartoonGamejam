using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogue : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject bubbleObject;
    [SerializeField] private Text dialogueText;

    [Header("Настройки")]
    [SerializeField] private float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        Hide();
    }

    public IEnumerator ShowPhraseRoutine(string phrase, float visibleTime)
    {
        bubbleObject.SetActive(true);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        yield return typingCoroutine = StartCoroutine(TypeText(phrase));

        yield return new WaitForSeconds(visibleTime);

        Hide();
    }

    private IEnumerator TypeText(string phrase)
    {
        dialogueText.text = "";

        foreach (char letter in phrase)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void Hide()
    {
        bubbleObject.SetActive(false);
    }
}