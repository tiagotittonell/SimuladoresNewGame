using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TutorialController.Instance != null)
                TutorialController.Instance.StartFirstDialogue();
        }
    }
}
