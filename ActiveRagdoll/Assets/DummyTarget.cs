using UnityEngine;

public class DummyTarget : MonoBehaviour
{
    public int basicHits = 0;
    public int heavyHits = 0;
    public bool dashDone = false;

    public void GetHit(bool heavy)
    {
        TutorialController.Instance.RegisterHit(heavy);
    }
}
