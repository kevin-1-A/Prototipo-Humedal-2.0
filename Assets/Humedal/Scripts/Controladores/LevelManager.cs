using UnityEngine;
using UnityEngine.InputSystem; // Necesario para leer el teclado
using UnityEngine.SceneManagement; // Para cargar escenas

public class LevelManager : MonoBehaviour
{
    void Update()
    {
        // Si el jugador presiona R, se reinicia el nivel
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("¡Tecla R detectada con Input System! Reiniciando el nivel...");
            RestartGame();
        }
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
