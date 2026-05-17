using UnityEngine;
using UnityEngine.UI;

public class SpawnSystem : MonoBehaviour
{
    [Header("Префабы игроков")]
    [SerializeField] private GameObject[] playerPrefabs;

    [Header("Префабы врагов")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Точки спавна")]
    [SerializeField] private Transform playerSpawnPoint;

    [SerializeField] private Transform enemySpawnPoint;

    [Header("ХП шкалы")]
    [SerializeField] private Image player1HpBar;

    [SerializeField] private Image player2HpBar;

    [SerializeField] private LayerMask groundLayer;

    private int selectedEnemyIndex = -1;
    [SerializeField] private GameObject playerArrowPrefab;
    public int SelectedEnemyIndex => selectedEnemyIndex;

    private void Start()
    {
        if (GameSettings.CurrentMode == GameMode.VsBot)
        {
            selectedEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        }
    }

    public void SpawnAll()
    {
        SpawnPlayer1();

        if (GameSettings.CurrentMode == GameMode.VsBot)
        {
            SpawnEnemy();
        }
        else
        {
            SpawnPlayer2();
        }
    }

    private GameObject SpawnPlayer1()
    {
        var prefab = playerPrefabs[GameSettings.Player1Character];

        var player = Spawn(prefab, playerSpawnPoint);

        var direction =
            enemySpawnPoint.position.x -
            playerSpawnPoint.position.x;

        player.transform.localScale =
            new Vector3(direction > 0 ? 1f : -1f, 1f, 1f);

        var controller =
            player.GetComponent<PlayerController>();

        controller.SetControls(
            KeyCode.A,
            KeyCode.D,
            KeyCode.G,
            KeyCode.H,
            KeyCode.Space
        );

        var entity = player.GetComponent<Entity>();

        entity.SetTeam(1);

        entity.SetHealthBar(player1HpBar);

        Instantiate(
            playerArrowPrefab,
            player.transform
        );

        return player;
    }

    private GameObject SpawnPlayer2()
    {
        var prefab = playerPrefabs[GameSettings.Player2Character];

        var player = Spawn(prefab, enemySpawnPoint);

        var direction =
            playerSpawnPoint.position.x -
            enemySpawnPoint.position.x;

        player.transform.localScale =
            new Vector3(direction > 0 ? 1f : -1f, 1f, 1f);

        var controller =
            player.GetComponent<PlayerController>();

        controller.SetControls(
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.K,
            KeyCode.L,
            KeyCode.N
        );

        var entity = player.GetComponent<Entity>();

        entity.SetTeam(2);

        entity.SetHealthBar(player2HpBar);

        return player;
    }

    private GameObject SpawnEnemy()
    {
        var prefab = enemyPrefabs[selectedEnemyIndex];

        var enemy = Spawn(prefab, enemySpawnPoint);

        var direction =
            playerSpawnPoint.position.x -
            enemySpawnPoint.position.x;

        enemy.transform.localScale =
            new Vector3(direction > 0 ? 1f : -1f, 1f, 1f);

        var entity = enemy.GetComponent<Entity>();

        entity.SetTeam(2);

        entity.SetHealthBar(player2HpBar);

        return enemy;
    }

    private GameObject Spawn(GameObject prefab, Transform spawnPoint)
    {
        var rayStart =
            new Vector2(spawnPoint.position.x, 100f);

        var hit = Physics2D.Raycast(
            rayStart,
            Vector2.down,
            200f,
            groundLayer
        );

        if (hit.collider != null)
        {
            var spawnPosition = new Vector3(
                spawnPoint.position.x,
                hit.point.y,
                0f
            );

            return Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity
            );
        }

        return null;
    }
}