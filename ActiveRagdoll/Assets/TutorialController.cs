using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMPro.TextMeshProUGUI dialogueText;

    [Header("Muñeco golpeable")]
    public DummyTarget dummy;

    [Header("Interno")]
    private int step = 0;
    private bool dialogueOpen = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (dialogueOpen && Input.GetKeyDown(KeyCode.F))
        {
            NextDialogue();
        }
    }

    public void StartFirstDialogue()
    {
        if (step != 0) return;

        ShowDialogue("Qué tal prisionero… aquí te convertirás en un guerrero de élite.\nPresiona F.");
        step = 1;
    }


    public void RegisterHit(bool heavy)
    {
        if (step == 3 && !heavy)
        {
            dummy.basicHits++;

            if (dummy.basicHits >= 4)
            {
                ShowDialogue("Excelente. Ahora prueba un ataque poderoso con click derecho.\nPresiona F. para comenzar la practica");
                step = 4;
            }
        }

        if (step == 5 && heavy)
        {
            dummy.heavyHits++;

            if (dummy.heavyHits >= 3)
            {
                ShowDialogue("Perfecto. También puedes usar el dash con CTRL. Prueba cualquier dirección.\nPresiona F. para comenzar la practica");
                step = 6;
            }
        }
    }

    public void RegisterDash()
    {
        if (step == 7)
        {
            dummy.dashDone = true;

            ShowDialogue("Muy bien… ¿estás listo?\nPresiona F para comenzar tu destino.");
            step = 8;
        }
    }


    void NextDialogue()
    {
        if (step == 1)
        {
            ShowDialogue("Primero aprende a atacar.\nPresiona CLICK IZQUIERDO varias veces sobre el muñeco.");
            step = 2;
        }
        else if (step == 2)
        {
            dialoguePanel.SetActive(false);
            dialogueOpen = false;
            step = 3;  
        }
        else if (step == 4)
        {
            dialoguePanel.SetActive(false);
            dialogueOpen = false;
            step = 5;  
        }
        else if (step == 6)
        {
            dialoguePanel.SetActive(false);
            dialogueOpen = false;
            step = 7;
        }
        else if (step == 8)
        {
            dialoguePanel.SetActive(false);
            dialogueOpen = false;

            GameController.Instance.NextLevel();
        }
    }


    void ShowDialogue(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
        dialogueOpen = true;
    }
}
