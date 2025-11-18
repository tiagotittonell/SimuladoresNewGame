using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    public int goldValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GoldManager.Instance.AddGold(goldValue);
            Destroy(gameObject);  // La moneda desaparece
        }
    }
}
