using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Referencias de Textos (UI)")]
    [SerializeField] private TextMeshProUGUI trashText;
    [SerializeField] private TextMeshProUGUI plantsText;
    [SerializeField] private TextMeshProUGUI animalsText;

    [Header("Paneles")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject introPanel;

    [Header("Mensajes")]
    [SerializeField] private GameObject messageRoot;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject hintRoot;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Barra de herramientas (1 Pinza, 2 Pala, 3 Mano)")]
    [SerializeField] private Image[] toolIcons;
    [SerializeField] private GameObject[] toolFrames;

    private float messageTimer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (introPanel != null) introPanel.SetActive(true);
        if (messageRoot != null) messageRoot.SetActive(false);
        SetHint("");
    }

    void Update()
    {
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f && messageRoot != null) messageRoot.SetActive(false);
        }
    }

    public void UpdateTrashUI(int current, int target)   { if (trashText != null) trashText.text = $"{current} / {target}"; }
    public void UpdatePlantsUI(int current, int target)  { if (plantsText != null) plantsText.text = $"{current} / {target}"; }
    public void UpdateAnimalsUI(int current, int target) { if (animalsText != null) animalsText.text = $"{current} / {target}"; }

    public void UpdateToolUI(int tool)
    {
        if (toolIcons == null) return;
        for (int i = 0; i < toolIcons.Length; i++)
        {
            bool active = (i + 1) == tool;
            if (toolIcons[i] != null)
            {
                toolIcons[i].color = active ? Color.white : new Color(1f, 1f, 1f, 0.55f);
                toolIcons[i].rectTransform.localScale = Vector3.one * (active ? 1.12f : 1f);
            }
            if (toolFrames != null && i < toolFrames.Length && toolFrames[i] != null)
                toolFrames[i].SetActive(active);
        }
    }

    public void ShowMessage(string text, float duration = 2.5f)
    {
        if (messageText == null) return;
        messageText.text = text;
        if (messageRoot != null) messageRoot.SetActive(true);
        messageTimer = duration;
    }

    public void SetHint(string text)
    {
        if (hintText != null) hintText.text = text;
        if (hintRoot != null) hintRoot.SetActive(!string.IsNullOrEmpty(text));
    }

    public void HideIntro()
    {
        if (introPanel != null) introPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        SetHint("");
        if (messageRoot != null) messageRoot.SetActive(false);
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Debug.Log("¡Panel de victoria activado en la pantalla!");
        }
    }
}
