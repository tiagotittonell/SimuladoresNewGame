using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Pantallas")]
    public GameObject deathScreen;
    public GameObject victoryPanel; // panel en la misma escena (Canvas)

    [Header("Niveles internos")]
    public List<GameObject> levels = new List<GameObject>();
    private int currentLevel = 0;

    [Header("Configuración de transición")]
    public float victoryDelay = 2f; // Tiempo antes de mostrar pantalla de victoria

    private int totalEnemies;
    private int enemiesKilled;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Desactivar todas las "EscenaX" menos la actual
        for (int i = 0; i < levels.Count; i++)
            levels[i].SetActive(i == currentLevel);

        ContarEnemigosDelNivel();
    }

    private void ContarEnemigosDelNivel()
    {
        EnemyHealth[] enemigos = levels[currentLevel].GetComponentsInChildren<EnemyHealth>();
        totalEnemies = enemigos.Length;
        enemiesKilled = 0;
        Debug.Log($"[GameController] Enemigos en nivel {currentLevel + 1}: {totalEnemies}");
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
            StartCoroutine(MostrarVictoria());
    }

    private IEnumerator MostrarVictoria()
    {
        Debug.Log("[GameController] Todos los enemigos eliminados, mostrando pantalla de victoria...");
        yield return new WaitForSeconds(victoryDelay);

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        victoryPanel.SetActive(false);

        levels[currentLevel].SetActive(false);
        currentLevel++;

        if (currentLevel < levels.Count)
        {
            levels[currentLevel].SetActive(true);
            ContarEnemigosDelNivel();
        }
        else
        {
            Debug.Log("🎉 ¡Todos los niveles completados!");
            SceneManager.LoadScene("VictoryScene"); // vuelve a usar tu escena final si querés
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
