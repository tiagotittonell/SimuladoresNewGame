using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Pantalla de Muerte")]
    public GameObject deathScreen;

    [Header("Victoria")]
    public string victorySceneName = "VictoryScene"; // pon el nombre de tu escena de victoria
    public float victoryDelay = 5f; // ⏳ tiempo antes de cambiar de escena

    private int totalEnemies;
    private int enemiesKilled;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        totalEnemies = enemies.Length;
        enemiesKilled = 0;

        Debug.Log($"[GameController] Enemigos detectados al inicio: {totalEnemies}");
    }

    public void PlayerDied()
    {
        if (deathScreen != null)
            deathScreen.SetActive(true);
    }

    public void EnemyDied()
    {
        enemiesKilled++;
        Debug.Log($"[GameController] Un enemigo murió. Total: {enemiesKilled}/{totalEnemies}");

        if (enemiesKilled >= totalEnemies && totalEnemies > 0)
        {
            StartCoroutine(LoadVictoryWithDelay());
        }
    }

    private IEnumerator LoadVictoryWithDelay()
    {
        Debug.Log($"[GameController] Todos los enemigos eliminados. Cambiando de escena en {victoryDelay} segundos...");
        yield return new WaitForSeconds(victoryDelay);
        SceneManager.LoadScene(victorySceneName);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
