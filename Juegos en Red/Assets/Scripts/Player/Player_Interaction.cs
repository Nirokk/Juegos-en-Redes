using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    Player_Model _player;

    [SerializeField] private float interactionRadius = 1f;

    private void Start()
    {
        _player = GetComponent<Player_Model>();
    }

    private void Update()
    {
        if (!_player._photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        foreach (Collider2D hit in hits)
        {
            // Busca algo que implemente IInteractable
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact(_player);
                break; // Interactúa con lo primero que encuentre
            }
        }
    }

    // Visualizar el rango en editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
