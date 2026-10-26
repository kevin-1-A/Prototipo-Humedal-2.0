using UnityEngine;

// Ordena el sprite según su altura en pantalla: lo que está más abajo se dibuja delante.
public class YSort : MonoBehaviour
{
    [SerializeField] private int offset = 0;
    [SerializeField] private bool isStatic = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Apply();
    }

    void LateUpdate()
    {
        if (!isStatic) Apply();
    }

    public void Apply()
    {
        if (sr != null)
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100f) + offset;
    }
}
