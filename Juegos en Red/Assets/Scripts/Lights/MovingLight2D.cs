using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MovingLight2D : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Vector2 targetPos;
    private System.Action onReachedTarget;

    public void Init(Vector2 target, System.Action callback)
    {
        targetPos = target;
        onReachedTarget = callback;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // If we reached target, notify manager and destroy this light
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            onReachedTarget?.Invoke();
            Destroy(gameObject);
        }
    }
}

