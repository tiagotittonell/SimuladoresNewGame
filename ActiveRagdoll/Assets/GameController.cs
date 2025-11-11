using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Pantalla de Muerte")]
    public GameObject deathScreen;

    [Header("Configuración de victoria")]
    public string victorySceneName = "Victoria"; // nombre exacto de tu escena de victoria
    public float victoryDelay = 2f; // tiempo antes de cambiar de escena

    [Header("Gestión de niveles internos (opcional)")]
    public List<GameObject> levels = new List<GameObject>();
    private int currentLevel = 0;

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
        // Si tenés niveles internos, solo activa el primero
        for (int i = 0; i < levels.Count; i++)
            levels[i].SetActive(i == currentLevel);

        ContarEnemigosDelNivel();
    }

    private void ContarEnemigosDelNivel()
    {
        if (levels.Count > 0 && currentLevel < levels.Count)
        {
            EnemyHealth[] enemigos = levels[currentLevel].GetComponentsInChildren<EnemyHealth>();
            totalEnemies = enemigos.Length;
        }
        else
        {
            EnemyHealth[] enemigos = FindObjectsOfType<EnemyHealth>();
            totalEnemies = enemigos.Length;
        }

        enemiesKilled = 0;
        Debug.Log($"[GameController] Enemigos detectados: {totalEnemies}");
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
            StartCoroutine(CargarVictoria());
        }
    }

    private IEnumerator CargarVictoria()
    {
        Debug.Log($"[GameController] Todos los enemigos eliminados. Cambiando a escena '{victorySceneName}' en {victoryDelay}s...");
        yield return new WaitForSeconds(victoryDelay);
        SceneManager.LoadScene(victorySceneName);
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
