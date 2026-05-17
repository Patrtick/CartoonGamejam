using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI элементы")]
    [SerializeField] private Text countdownText;

    [SerializeField] private GameObject backForText;

    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject winPanel;

    [SerializeField] private Text winText;

    [Header("Музыка")]
    [SerializeField] private RandomMusicPlayer musicPlayer;

    [Header("Интро")]
    [SerializeField] private FightIntroSystem introSystem;

    [Header("Настройки")]
    [SerializeField] private float countdownDuration = 3f;

    [SerializeField] private string startFightMessage = "ЗАРУБА!";

    [SerializeField] private int scoreToWin = 2;

    [SerializeField] private string winMessage = "Победа!";

    [SerializeField] private string loseMessage = "Поражение!";

    [Header("Звук отсчёта")]
    [SerializeField] private AudioSource countdownAudioSource;

    [SerializeField] private AudioClip countdownSound;

    private int player1Score;

    private int player2Score;

    private bool firstRoundPlayed;

    public bool CanFight { get; private set; }

    private SpawnSystem spawnSystem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        spawnSystem = FindAnyObjectByType<SpawnSystem>();
    }

    private void Start()
    {
        Entity.OnEntityDeath += HandleEntityDeath;

        UpdateScoreUI();

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (musicPlayer != null)
        {
            musicPlayer.PlayBattleMusic();
        }

        StartNewRound();
    }

    private void OnDestroy()
    {
        Entity.OnEntityDeath -= HandleEntityDeath;
    }

    public void StartNewRound()
    {
        StopAllCoroutines();

        CanFight = false;

        var entities =
            FindObjectsByType<Entity>(
                FindObjectsInactive.Exclude
            );

        foreach (var entity in entities)
        {
            Destroy(entity.gameObject);
        }

        spawnSystem.SpawnAll();

        StartCoroutine(StartRoundSequence());
    }

    private IEnumerator StartRoundSequence()
    {
        if (!firstRoundPlayed)
        {
            int player1Character =
                GameSettings.Player1Character;

            int player2Character =
                GameSettings.CurrentMode == GameMode.VsPlayer
                ? GameSettings.Player2Character
                : spawnSystem.SelectedEnemyIndex;

            if (introSystem != null)
            {
                yield return StartCoroutine(
                    introSystem.PlayIntro(
                        player1Character,
                        player2Character
                    )
                );
            }

            firstRoundPlayed = true;
        }

        yield return StartCoroutine(
            RoundStartCoroutine()
        );
    }

    private IEnumerator RoundStartCoroutine()
    {
        CanFight = false;

        backForText.SetActive(true);

        if (
            countdownAudioSource != null &&
            countdownSound != null
        )
        {
            countdownAudioSource.PlayOneShot(
                countdownSound
            );
        }

        float timer = countdownDuration;

        while (timer > 0)
        {
            countdownText.text =
                Mathf.Ceil(timer).ToString();

            yield return new WaitForSeconds(1f);

            timer -= 1f;
        }

        countdownText.text = startFightMessage;

        CanFight = true;

        yield return new WaitForSeconds(1f);

        countdownText.text = "";

        backForText.SetActive(false);
    }

    private void HandleEntityDeath(Entity entity)
    {
        if (!CanFight)
            return;

        if (entity.TeamId == 1)
        {
            player2Score++;
        }
        else if (entity.TeamId == 2)
        {
            player1Score++;
        }

        UpdateScoreUI();

        CanFight = false;

        if (GameSettings.CurrentMode == GameMode.VsBot)
        {
            if (player1Score >= scoreToWin)
            {
                ShowEndGameMenu(winMessage);
            }
            else if (player2Score >= scoreToWin)
            {
                ShowEndGameMenu(loseMessage);
            }
            else
            {
                Invoke(nameof(StartNewRound), 2f);
            }
        }
        else
        {
            if (player1Score >= scoreToWin)
            {
                ShowEndGameMenu("Выиграл игрок 1");
            }
            else if (player2Score >= scoreToWin)
            {
                ShowEndGameMenu("Выиграл игрок 2");
            }
            else
            {
                Invoke(nameof(StartNewRound), 2f);
            }
        }
    }

    private void ShowEndGameMenu(string message)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (winText != null)
        {
            winText.text = message;
        }

        if (musicPlayer != null)
        {
            musicPlayer.PlayEndGameMusic();
        }
    }

    public void PlayAgain()
    {
        player1Score = 0;

        player2Score = 0;

        firstRoundPlayed = false;

        UpdateScoreUI();

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (musicPlayer != null)
        {
            musicPlayer.PlayBattleMusic();
        }

        StartNewRound();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void UpdateScoreUI()
    {
        if (scoreText)
        {
            scoreText.text =
                $"{player1Score} : {player2Score}";
        }
    }
}