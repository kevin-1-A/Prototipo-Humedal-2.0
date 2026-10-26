using UnityEngine;
using UnityEngine.InputSystem;

// Movimiento y animación del personaje. Requiere Rigidbody2D y Collider2D.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Animación (9 cuadros por dirección)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] walkDown;
    [SerializeField] private Sprite[] walkUp;
    [SerializeField] private Sprite[] walkRight;
    [SerializeField] private Sprite[] walkLeft;
    [SerializeField] private float framesPerSecond = 12f;

    [Header("Poses de acción")]
    [SerializeField] private Sprite pinzaPose;
    [SerializeField] private Sprite sembrarPose;
    [SerializeField] private Sprite agacharPose;
    [SerializeField] private Sprite celebrarPose;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Sprite[] currentSet;
    private float animTimer;
    private int frame;
    private float actionTimer;
    private Sprite actionSprite;
    private bool celebrating;

    public bool IsBusy => actionTimer > 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        currentSet = walkDown;
    }

    void Update()
    {
        movement = Vector2.zero;

        if (GameManager.IsPlaying && !IsBusy && Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) movement.y = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) movement.y = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) movement.x = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) movement.x = 1f;
        }

        // Normalizamos para que la diagonal no sea más rápida
        movement = movement.normalized;
        Animate();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void Animate()
    {
        if (spriteRenderer == null) return;

        if (celebrating && celebrarPose != null)
        {
            spriteRenderer.sprite = celebrarPose;
            return;
        }

        if (actionTimer > 0f)
        {
            actionTimer -= Time.deltaTime;
            if (actionSprite != null) spriteRenderer.sprite = actionSprite;
            return;
        }

        if (movement != Vector2.zero)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                currentSet = movement.x > 0 ? walkRight : walkLeft;
            else
                currentSet = movement.y > 0 ? walkUp : walkDown;

            animTimer += Time.deltaTime;
            if (animTimer >= 1f / framesPerSecond)
            {
                animTimer = 0f;
                frame++;
            }
        }
        else
        {
            frame = 0;
            animTimer = 0f;
        }

        if (currentSet != null && currentSet.Length > 0)
            spriteRenderer.sprite = currentSet[frame % currentSet.Length];
    }

    // Muestra la pose de la acción según el tipo de objeto con el que interactúa
    public void PlayInteraction(InteractableType type, float duration)
    {
        switch (type)
        {
            case InteractableType.Basura: actionSprite = pinzaPose; break;
            case InteractableType.Planta: actionSprite = sembrarPose; break;
            default: actionSprite = agacharPose; break;
        }
        actionTimer = duration;
    }

    public void Celebrate()
    {
        celebrating = true;
    }
}
