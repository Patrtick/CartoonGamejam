using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightIntroSystem : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public bool player1;
        [TextArea] public string text;
    }

    [System.Serializable]
    public class DialogueVariant
    {
        public List<DialogueLine> lines;
    }

    [System.Serializable]
    public class CharacterIntro
    {
        public int characterA;
        public int characterB;

        public List<DialogueVariant> variants;
    }

    [Header("Диалоги")]
    [SerializeField] private List<CharacterIntro> intros;

    [Header("UI")]
    [SerializeField] private CharacterDialogue player1Dialogue;
    [SerializeField] private CharacterDialogue player2Dialogue;

    [Header("Настройки")]
    [SerializeField] private float phraseDuration = 1.5f;

    public IEnumerator PlayIntro(int p1, int p2)
    {
        var intro = FindIntro(p1, p2);

        if (intro == null)
            yield break;

        var variant =
            intro.variants[Random.Range(0, intro.variants.Count)];

        foreach (DialogueLine line in variant.lines)
        {
            if (line.player1)
            {
                yield return player1Dialogue.ShowPhraseRoutine(
                    line.text,
                    phraseDuration
                );
            }
            else
            {
                yield return player2Dialogue.ShowPhraseRoutine(
                    line.text,
                    phraseDuration
                );
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private CharacterIntro FindIntro(int p1, int p2)
    {
        foreach (var intro in intros)
        {
            var direct =
                intro.characterA == p1 &&
                intro.characterB == p2;

            var reverse =
                intro.characterA == p2 &&
                intro.characterB == p1;

            if (direct || reverse)
                return intro;
        }

        return null;
    }
}