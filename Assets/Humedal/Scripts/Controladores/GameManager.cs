using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progreso Actual en el Humedal")]
    [SerializeField] private int trashCleaned = 0;
    [SerializeField] private int plantsGrown = 0;
    [SerializeField] private int animalsHelped = 0;

    [Header("Metas del Nivel")]
    [SerializeField] private int targetTrash = 6;
    [SerializeField] private int targetPlants = 6;
    [SerializeField] private int targetAnimals = 5;

    private bool isPlaying;
    private bool hasWon;

    public static bool IsPlaying => Instance != null && Instance.isPlaying;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateAllUI();
    }

    void Update()
    {
        // Pantalla de inicio: ENTER, ESPACIO o E para comenzar
        if (!isPlaying && !hasWon && Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideIntro();
            UIManager.Instance.ShowMessage("Elige una herramienta con 1, 2 o 3");
        }
    }

    public void AddTrashCleaned()
    {
        trashCleaned++;
        if (trashCleaned == targetTrash) GoalReached("¡Meta de basura completada!");
        UpdateAllUI();
        CheckWinCondition();
    }

    public void AddPlantGrown()
    {
        plantsGrown++;
        if (plantsGrown == targetPlants) GoalReached("¡Meta de plantas completada!");
        UpdateAllUI();
        CheckWinCondition();
    }

    public void AddAnimalHelped()
    {
        animalsHelped++;
        if (animalsHelped == targetAnimals) GoalReached("¡Ayudaste a todos los animales!");
        UpdateAllUI();
        CheckWinCondition();
    }

    private void GoalReached(string msg)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayGoal();
        if (UIManager.Instance != null) UIManager.Instance.ShowMessage(msg, 3f);
    }

    private void UpdateAllUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTrashUI(trashCleaned, targetTrash);
            UIManager.Instance.UpdatePlantsUI(plantsGrown, targetPlants);
            UIManager.Instance.UpdateAnimalsUI(animalsHelped, targetAnimals);
        }
    }

    // Revisa si se cumplieron todas las metas para dejar el humedal limpio
    private void CheckWinCondition()
    {
        if (hasWon) return;
        if (trashCleaned >= targetTrash && plantsGrown >= targetPlants && animalsHelped >= targetAnimals)
        {
            hasWon = true;
            isPlaying = false;
            Debug.Log("¡Felicidades! Has restaurado completamente el humedal con éxito, gracias por tus esfuerzos.");
            if (SoundManager.Instance != null) SoundManager.Instance.PlayWin();
            var pc = FindFirstObjectByType<PlayerController>();
            if (pc != null) pc.Celebrate();
            if (UIManager.Instance != null) UIManager.Instance.ShowVictory();
        }
    }
}
