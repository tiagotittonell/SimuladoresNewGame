
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("UI")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject hudCanvas;

    [Header("Niveles")]
    [SerializeField] private List<GameObject> levels = new List<GameObject>();
    private int currentLevel = 0;

    [Header("Final")]
    [SerializeField] private GameObject finalGamePanel;

    private int totalEnemies;
    private int enemiesKilled;
    private bool levelCompleted = false;

    private GameObject playerInstance;
    private Camera mainCamera;
    private Transform defaultCameraParent;
    private Vector3 defaultCameraPos;
    private Quaternion defaultCameraRot;

    [Header("Skyboxes por Nivel")]
    [SerializeField] private Material[] skyboxes;
    // skyboxes[0] = mañana
    // skyboxes[1] = tarde
    // skyboxes[2] = noche


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Crear EventSystem si falta
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }

        for (int i = 0; i < levels.Count; i++)
            levels[i].SetActive(i == currentLevel);

        victoryPanel.SetActive(false);

        playerInstance = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;

        defaultCameraParent = mainCamera.transform.parent;
        defaultCameraPos = mainCamera.transform.position;
        defaultCameraRot = mainCamera.transform.rotation;

        ContarEnemigosDelNivel();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 🟦 Aplicar skybox del primer nivel
        AplicarSkyboxDelNivel(currentLevel);
    }

    private void ContarEnemigosDelNivel()
    {
        EnemyHealth[] enemigos = levels[currentLevel].GetComponentsInChildren<EnemyHealth>(true);
        totalEnemies = enemigos.Length;
        enemiesKilled = 0;
        levelCompleted = false;
    }

    public void PlayerDied()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void EnemyDied()
    {
        enemiesKilled++;

        if (!levelCompleted && enemiesKilled >= totalEnemies && totalEnemies > 0)
        {
            levelCompleted = true;
            StartCoroutine(MostrarPanelVictoria());
        }
    }

    private IEnumerator MostrarPanelVictoria()
    {
        yield return new WaitForSeconds(5f);
        Time.timeScale = 0f;

        if (mainCamera != null)
        {
            mainCamera.transform.parent = null;
            mainCamera.transform.position = defaultCameraPos;
            mainCamera.transform.rotation = defaultCameraRot;
        }

        if (playerInstance != null)
            playerInstance.SetActive(false);

        if (hudCanvas != null)
            hudCanvas.SetActive(false);

        levels[currentLevel].SetActive(false);

        // ✔ SI ES EL ÚLTIMO NIVEL → MOSTRAR FINAL
        if (currentLevel + 1 >= levels.Count)
        {
            Debug.Log("🔥 Mostrando panel FINAL del juego");
            if (finalGamePanel != null)
                finalGamePanel.SetActive(true);
        }
        else
        {
            // ✔ SI NO ES EL ÚLTIMO NIVEL → MOSTRAR VICTORIA NORMAL
            Debug.Log("🏆 Mostrando panel de VICTORIA del nivel");
            victoryPanel.SetActive(true);
        }

        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void NextLevel()
    {
        Debug.Log("Botón NextLevel presionado");
        Time.timeScale = 1f;

        victoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Apagar nivel actual
        levels[currentLevel].SetActive(false);
        currentLevel++;

        // ¿YA TERMINÓ EL JUEGO?
        if (currentLevel >= levels.Count)
        {
            Debug.Log("🔥 ¡Juego completado! No hay más niveles.");

            // Mostrar pantalla final
            if (finalGamePanel != null)
                finalGamePanel.SetActive(true);

            // Desactivar HUD y jugador
            if (hudCanvas) hudCanvas.SetActive(false);
            if (playerInstance) playerInstance.SetActive(false);

            // Mostrar cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Pausar juego
            Time.timeScale = 0f;

            return; 
        }

        levels[currentLevel].SetActive(true);

        playerInstance = GameObject.FindGameObjectWithTag("Player");

        var cam = Camera.main?.GetComponent<ThirdPersonCamera>();
        if (cam != null && playerInstance != null)
            cam.target = playerInstance.transform;

        if (playerInstance != null)
        {
            var playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.InitializeHealthUI();
        }

        ContarEnemigosDelNivel();

        if (hudCanvas != null)
            hudCanvas.SetActive(true);

        AplicarSkyboxDelNivel(currentLevel);

        // Nivel Noche
        if (currentLevel == 2)
        {
            DesactivarLucesDelNivel();
            ActivarLucesPersonalizadasNoche();
        }
    }

    private void DesactivarLucesDelNivel()
    {
        Light[] luces = levels[currentLevel].GetComponentsInChildren<Light>(true);

        foreach (Light l in luces)
        {
            if (l.CompareTag("Torch"))
                continue;

            l.enabled = false;
        }

        Debug.Log("🔦 Todas las luces del nivel (menos las antorchas) fueron desactivadas.");
    }
    private void ActivarLucesPersonalizadasNoche()
    {
       
        GameObject luzLunaObj = new GameObject("LuzLuna");
        Light luzLuna = luzLunaObj.AddComponent<Light>();
        luzLuna.type = LightType.Directional;
        luzLuna.color = new Color(0.2f, 0.3f, 0.6f); 
        luzLuna.intensity = 0.15f;                  

        luzLunaObj.transform.rotation = Quaternion.Euler(45, 170, 0);


        GameObject punto = new GameObject("LuzPunto");
        Light lp = punto.AddComponent<Light>();
        lp.type = LightType.Point;
        lp.range = 10f;
        lp.intensity = 0.5f;
        lp.color = new Color(0.1f, 0.2f, 0.8f);
        punto.transform.position = new Vector3(5, 2, 3);


        Debug.Log("🌙 Luces personalizadas del nivel noche activadas.");
    }
    public void ExitGame()
    {
        Debug.Log("🚪 Saliendo del juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }


    public void RestartGame()
    {
        Debug.Log("🔄 Reiniciando juego...");

        if (finalGamePanel != null)
            finalGamePanel.SetActive(false);

        currentLevel = 0;

        for (int i = 0; i < levels.Count; i++)
            levels[i].SetActive(false);

        levels[0].SetActive(true);

        playerInstance = GameObject.FindGameObjectWithTag("Player");
        if (playerInstance != null)
            playerInstance.SetActive(true);

        if (hudCanvas != null)
            hudCanvas.SetActive(true);

        ContarEnemigosDelNivel();

        AplicarSkyboxDelNivel(0);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("✔ Juego listo desde el primer nivel.");
    }

    private void AplicarSkyboxDelNivel(int index)
    {
        if (skyboxes != null && index < skyboxes.Length)
        {
            RenderSettings.skybox = skyboxes[index];
            DynamicGI.UpdateEnvironment(); 
        }
        else
        {
            Debug.LogWarning("⚠ No hay skybox asignado para el nivel " + index);
        }
    }
}


