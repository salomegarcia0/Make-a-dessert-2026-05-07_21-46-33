using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    public KeyCode interactKey = KeyCode.F; 
    public float interactRange = 3f;

    private PlayerPickup playerPickupScript;

    void Start()
    {
        // Buscamos el script de agarrar que está en este mismo jugador
        playerPickupScript = GetComponent<PlayerPickup>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider col in colliders)
        {
            // 1. ¿Es la Nevera?
            FridgeMenu fridge = col.GetComponent<FridgeMenu>();
            if (fridge != null)
            {
                if (!fridge.uiPanel.activeSelf)
                {
                    // Le enviamos a la nevera nuestro script de inventario
                    fridge.OpenMenu(playerPickupScript); 
                }
                return; 
            }

            // 2. ¿Es un NPC?
            CustomerNPC npc = col.GetComponent<CustomerNPC>();
            if (npc != null && !npc.orderTaken)
            {
                npc.TakeOrder();
                return; 
            }
        }
    }
}