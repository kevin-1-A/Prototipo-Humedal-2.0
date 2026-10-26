using UnityEngine;
using UnityEngine.InputSystem;

// Gestor de herramientas. Va en el Player, junto a PlayerController.
// 0: ninguna, 1: Pinza (basura), 2: Pala (sembrar), 3: Mano (ayudar animales)
public class ToolManager : MonoBehaviour
{
    public const int Ninguna = 0;
    public const int Pinza = 1;
    public const int Pala = 2;
    public const int Mano = 3;

    [Header("Configuración de Herramientas")]
    [SerializeField] private int currentTool = 0;

    void Start()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateToolUI(currentTool);
    }

    void Update()
    {
        if (!GameManager.IsPlaying || Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
            SetTool(Pinza);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
            SetTool(Pala);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
            SetTool(Mano);
    }

    public void SetTool(int tool)
    {
        currentTool = tool;
        Debug.Log("Herramienta activa: " + tool + " - " + GetToolName(tool));
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateToolUI(currentTool);
    }

    // Método público para saber qué herramienta está seleccionada
    public int GetActiveTool()
    {
        return currentTool;
    }

    public static string GetToolName(int tool)
    {
        switch (tool)
        {
            case Pinza: return "Pinza";
            case Pala: return "Pala";
            case Mano: return "Mano";
            default: return "Ninguna";
        }
    }
}
