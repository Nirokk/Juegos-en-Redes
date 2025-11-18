using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    public float lifetime = 0.5f; // lo que dure tu animación

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
