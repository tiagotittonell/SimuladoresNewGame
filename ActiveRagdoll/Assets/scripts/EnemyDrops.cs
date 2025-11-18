using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
    public GameObject coinPrefab;
    public int dropAmount = 1;

    public void DropCoins()
    {
        for (int i = 0; i < dropAmount; i++)
        {
            Instantiate(
                coinPrefab,
                transform.position + Vector3.up * 1f,
                Quaternion.identity
            );
        }
    }
}
