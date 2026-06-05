using UnityEngine;

public class PlayerTalk : MonoBehaviour
{
    [Tooltip("Usa la misma tecla que usas para agarrar platos u otra distinta (ej. F)")]
    public KeyCode interactKey = KeyCode.F; 
    public float talkRange = 3f;
    public LayerMask npcLayer; // Capa exclusiva para los NPCs

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // Busca NPCs cercanos
            Collider[] colliders = Physics.OverlapSphere(transform.position, talkRange, npcLayer);
            
            foreach (Collider col in colliders)
            {
                CustomerNPC npc = col.GetComponent<CustomerNPC>();
                // Si encontramos un NPC y no le hemos tomado la orden, lo hacemos
                if (npc != null && !npc.orderTaken)
                {
                    npc.TakeOrder();
                    return; // Solo hablamos con uno a la vez
                }
            }
        }
    }
}