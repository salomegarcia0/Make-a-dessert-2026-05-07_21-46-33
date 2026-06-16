using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    public Transform holdPosition;
    public KeyCode interactKey = KeyCode.E;
    public float pickupRange = 2f;
    public LayerMask pickupLayer; // Asegúrate de que tus ingredientes tengan la capa "Grab"

    private GameObject heldItem; // El objeto que tenemos en las manos

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (heldItem == null) TryPickup();
            else DropItem();
        }
    }

    void TryPickup()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);
        if (colliders.Length > 0)
        {
            // Agarramos el primer objeto que detecte
            ForcePickup(colliders[0].gameObject);
        }
    }

    // Esta es la función mágica que usará la Nevera
    public void ForcePickup(GameObject item)
    {
        if (heldItem != null) return; // Si ya tenemos algo, no podemos agarrar más

        heldItem = item;
        
        // Lo emparentamos y lo posicionamos en la mano
        heldItem.transform.SetParent(holdPosition);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        // Apagamos físicas para que no se caiga
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        // Volvemos el collider un "Trigger" para que no empuje al jugador, 
        // pero siga existiendo para poder soltarlo/agarrarlo luego
        Collider col = heldItem.GetComponent<Collider>();
        if (col != null) col.isTrigger = true; 
    }

    public void DropItem()
    {
        if (heldItem == null) return;

        // Lo desvinculamos del jugador
        heldItem.transform.SetParent(null);
        
        // Encendemos físicas para que caiga al suelo
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Quitamos el modo Trigger para que choque con el suelo
        Collider col = heldItem.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        heldItem = null; // Vaciamos las manos
    }

    // Función extra para que la nevera sepa si podemos cargar cosas
    public bool IsHoldingItem()
    {
        return heldItem != null;
    }
}