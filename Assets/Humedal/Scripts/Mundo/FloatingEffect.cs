using UnityEngine;

// Efecto visual que sube y se desvanece (destellos, hojas, agua).
public class FloatingEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private float life = 0.9f;
    private float t;
    private Vector3 startScale;

    public static void Spawn(Sprite sprite, Vector3 position, Material material, float scale = 0.8f)
    {
        if (sprite == null) return;
        var go = new GameObject("Efecto");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * scale;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        if (material != null) sr.sharedMaterial = material;
        sr.sortingOrder = 32000;
        go.AddComponent<FloatingEffect>();
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = t / life;
        transform.position += Vector3.up * Time.deltaTime * 0.9f;
        transform.localScale = startScale * (1f + 0.3f * Mathf.Sin(k * Mathf.PI));
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f - k;
            sr.color = c;
        }
        if (t >= life) Destroy(gameObject);
    }
}
