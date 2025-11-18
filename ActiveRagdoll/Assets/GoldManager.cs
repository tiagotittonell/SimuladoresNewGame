using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    public int gold = 0;  // Oro acumulado

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("💰 Oro actual: " + gold);
    }

    public void ResetGold()
    {
        gold = 0;
    }
}
