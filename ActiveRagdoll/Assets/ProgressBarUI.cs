using UnityEngine;

public class ProgressBarUI : MonoBehaviour
{
    [Header("Referencias a los niveles (ordenados)")]
    public GameObject[] levelSlots; // Cada uno tiene vacio y lleno

    // Llamado por GameController
    public void UpdateProgress(int reachedLevel)
    {
        for (int i = 0; i < levelSlots.Length; i++)
        {
            Transform emptyCircle = levelSlots[i].transform.Find("CirculoVacio");
            Transform fullCircle = levelSlots[i].transform.Find("CirculoLleno");

            if (emptyCircle == null || fullCircle == null)
            {
                Debug.LogError("Faltan referencias en " + levelSlots[i].name);
                continue;
            }

            bool completed = i < reachedLevel;

            emptyCircle.gameObject.SetActive(!completed);
            fullCircle.gameObject.SetActive(completed);
        }
    }
}
