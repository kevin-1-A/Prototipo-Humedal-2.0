using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InteractableType { Basura, Planta, Animal }

// Va en cada objeto interactuable del mapa (basura, punto de siembra, animal).
// Usa un Collider2D "Is Trigger": cuando el jugador está cerca y presiona E,
// revisa la herramienta activa del ToolManager.
[RequireComponent(typeof(Collider2D))]
public class InteractableItem : MonoBehaviour
{
    [Header("Tipo de objeto")]
    [SerializeField] private InteractableType type = InteractableType.Basura;
    [SerializeField] private string displayName = "la basura";

    [Header("Sonido propio del objeto")]
    [SerializeField] private AudioClip itemSound;
    [SerializeField] private float soundStart = 0f;
    [SerializeField] private float soundMaxDuration = 2.5f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite doneSprite;
    [SerializeField] private float idleFps = 2f;
    [SerializeField] private float bobAmount = 0f;
    [SerializeField] private Sprite effectSprite;
    [SerializeField] private GameObject alertIcon;
    [SerializeField] private GameObject promptIcon;

    private static readonly List<InteractableItem> inRange = new List<InteractableItem>();
    private static InteractableItem nearest;
    private static int nearestFrame = -1;

    private ToolManager player;
    private bool done;
    private float animT;
    private Vector3 basePos;
    private Color pendingColor = new Color(0.62f, 0.62f, 0.62f, 1f);

    public int RequiredTool
    {
        get
        {
            switch (type)
            {
                case InteractableType.Basura: return ToolManager.Pinza;
                case InteractableType.Planta: return ToolManager.Pala;
                default: return ToolManager.Mano;
            }
        }
    }

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        basePos = spriteRenderer != null ? spriteRenderer.transform.localPosition : Vector3.zero;
        if (type == InteractableType.Animal && spriteRenderer != null) spriteRenderer.color = pendingColor;
        if (promptIcon != null) promptIcon.SetActive(false);
    }

    void OnDisable()
    {
        inRange.Remove(this);
        if (nearest == this) nearest = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var tm = other.GetComponentInParent<ToolManager>();
        if (tm == null || done) return;
        player = tm;
        if (!inRange.Contains(this)) inRange.Add(this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<ToolManager>() == null) return;
        inRange.Remove(this);
    }

    private static InteractableItem GetNearest()
    {
        if (nearestFrame == Time.frameCount) return nearest;
        nearestFrame = Time.frameCount;
        InteractableItem previous = nearest;
        nearest = null;
        float best = float.MaxValue;
        foreach (var it in inRange)
        {
            if (it == null || it.done || it.player == null) continue;
            float d = ((Vector2)(it.transform.position - it.player.transform.position)).sqrMagnitude;
            if (d < best) { best = d; nearest = it; }
        }
        if (nearest != previous && UIManager.Instance != null)
            UIManager.Instance.SetHint(nearest != null ? nearest.GetHint() : "");
        return nearest;
    }

    void Update()
    {
        AnimateIdle();

        bool isNearest = GetNearest() == this && GameManager.IsPlaying;
        if (promptIcon != null && promptIcon.activeSelf != isNearest) promptIcon.SetActive(isNearest);

        if (isNearest && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            TryInteract();
    }

    private void AnimateIdle()
    {
        if (spriteRenderer == null) return;
        animT += Time.deltaTime;
        if (idleFrames != null && idleFrames.Length > 1 && type == InteractableType.Animal && !(done && doneSprite != null))
            spriteRenderer.sprite = idleFrames[(int)(animT * idleFps) % idleFrames.Length];
        if (bobAmount > 0f)
            spriteRenderer.transform.localPosition = basePos + Vector3.up * Mathf.Sin(animT * 2.5f) * bobAmount;
        if (alertIcon != null && alertIcon.activeSelf)
            alertIcon.transform.localPosition = new Vector3(0f, alertIcon.transform.localPosition.y + Mathf.Sin(animT * 4f) * 0.002f, 0f);
    }

    public string GetHint()
    {
        switch (type)
        {
            case InteractableType.Basura: return "E: recoger " + displayName + " con la Pinza (tecla 1)";
            case InteractableType.Planta: return "E: sembrar una planta con la Pala (tecla 2)";
            default: return "E: ayudar a " + displayName + " con la Mano (tecla 3)";
        }
    }

    public void TryInteract()
    {
        if (done || player == null) return;
        var pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.IsBusy) return;

        int tool = player.GetActiveTool();
        if (tool != RequiredTool)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowMessage("Necesitas la " + ToolManager.GetToolName(RequiredTool) + " (tecla " + RequiredTool + ")");
            return;
        }

        done = true;
        inRange.Remove(this);
        if (nearest == this) nearest = null;
        nearestFrame = -1;
        if (promptIcon != null) promptIcon.SetActive(false);
        if (UIManager.Instance != null) UIManager.Instance.SetHint("");
        GetComponent<Collider2D>().enabled = false;

        if (pc != null) pc.PlayInteraction(type, type == InteractableType.Planta ? 0.8f : 0.5f);
        StartCoroutine(DoInteraction());
    }

    private IEnumerator DoInteraction()
    {
        var sm = SoundManager.Instance;
        Material mat = spriteRenderer != null ? spriteRenderer.sharedMaterial : null;
        Vector3 fx = transform.position + Vector3.up * 0.6f;

        switch (type)
        {
            case InteractableType.Basura:
                if (sm != null) { sm.PlayPinza(); sm.PlayClip(itemSound, soundStart, soundMaxDuration); }
                FloatingEffect.Spawn(effectSprite, fx, mat);
                GameManager.Instance?.AddTrashCleaned();
                UIManager.Instance?.ShowMessage("¡Recogiste " + displayName + "!");
                Vector3 s0 = transform.localScale; Vector3 p0 = transform.position;
                for (float t = 0; t < 0.35f; t += Time.deltaTime)
                {
                    float k = t / 0.35f;
                    transform.localScale = s0 * (1f - k);
                    transform.position = p0 + Vector3.up * 0.4f * k;
                    yield return null;
                }
                Destroy(gameObject);
                break;

            case InteractableType.Planta:
                if (sm != null) sm.PlayPala();
                yield return new WaitForSeconds(0.45f);
                if (sm != null) sm.PlayPlant();
                if (doneSprite != null && spriteRenderer != null) spriteRenderer.sprite = doneSprite;
                FloatingEffect.Spawn(effectSprite, fx, mat);
                GameManager.Instance?.AddPlantGrown();
                UIManager.Instance?.ShowMessage("¡Sembraste una planta nativa!");
                yield return Grow();
                break;

            case InteractableType.Animal:
                if (sm != null) { sm.PlayAnimal(); sm.PlayClip(itemSound, soundStart, soundMaxDuration); }
                if (alertIcon != null) alertIcon.SetActive(false);
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                    if (doneSprite != null) spriteRenderer.sprite = doneSprite;
                }
                FloatingEffect.Spawn(effectSprite, fx, mat);
                GameManager.Instance?.AddAnimalHelped();
                UIManager.Instance?.ShowMessage("¡Ayudaste a " + displayName + "!");
                yield return Hop();
                break;
        }
    }

    private IEnumerator Grow()
    {
        Transform t = spriteRenderer.transform;
        Vector3 target = t.localScale;
        for (float x = 0; x < 0.5f; x += Time.deltaTime)
        {
            float k = x / 0.5f;
            float s = Mathf.Lerp(0.2f, 1f, k) + Mathf.Sin(k * Mathf.PI) * 0.15f;
            t.localScale = new Vector3(target.x * s, target.y * s, target.z);
            yield return null;
        }
        t.localScale = target;
    }

    private IEnumerator Hop()
    {
        Transform t = spriteRenderer.transform;
        for (float x = 0; x < 0.6f; x += Time.deltaTime)
        {
            t.localPosition = basePos + Vector3.up * Mathf.Sin(x / 0.6f * Mathf.PI) * 0.35f;
            yield return null;
        }
        t.localPosition = basePos;
        // Después de un momento feliz vuelve a su animación normal
        yield return new WaitForSeconds(0.8f);
        if (idleFrames != null && idleFrames.Length > 0) doneSprite = null;
    }
}
