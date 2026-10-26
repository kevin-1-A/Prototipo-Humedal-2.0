using UnityEngine;

// Cámara que sigue al jugador sin salirse del mapa.
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer mapBounds;
    [SerializeField] private float smoothTime = 0.15f;

    private Camera cam;
    private Vector3 velocity;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target != null)
            transform.position = Clamp(new Vector3(target.position.x, target.position.y, transform.position.z));
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 goal = new Vector3(target.position.x, target.position.y + 0.6f, transform.position.z);
        transform.position = Clamp(Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime));
    }

    private Vector3 Clamp(Vector3 p)
    {
        if (mapBounds == null || cam == null) return p;
        Bounds b = mapBounds.bounds;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        p.x = (b.size.x > halfW * 2f) ? Mathf.Clamp(p.x, b.min.x + halfW, b.max.x - halfW) : b.center.x;
        p.y = (b.size.y > halfH * 2f) ? Mathf.Clamp(p.y, b.min.y + halfH, b.max.y - halfH) : b.center.y;
        return p;
    }
}
