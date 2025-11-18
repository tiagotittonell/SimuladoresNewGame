using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    public EnemyDamage handHitbox;

    public void EnableHitbox()
    {
        if (handHitbox != null) handHitbox.EnableHitbox();
    }

    public void DisableHitbox()
    {
        if (handHitbox != null) handHitbox.DisableHitbox();
    }
}
