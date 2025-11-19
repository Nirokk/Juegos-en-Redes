using UnityEngine;

public class CrowdEffect : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * amplitude * 0.5f;
        float y = Mathf.Cos(Time.time * speed * 0.7f) * amplitude;

        transform.position = startPos + new Vector3(x, y, 0);
    }
}