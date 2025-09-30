using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject deathScreen; // asignar el Canvas con la imagen en el inspector

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlayerDied()
    {
        if (deathScreen != null)
            deathScreen.SetActive(true); // mostrar pantalla de muerte
    }

    // Reinicia la escena actual
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Vuelve al menú principal
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
