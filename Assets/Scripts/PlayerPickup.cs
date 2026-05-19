using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    [Tooltip("El punto vacío (Empty GameObject) donde el objeto será sostenido, por ejemplo, la mano.")]
    public Transform holdPosition; 
    public KeyCode interactKey = KeyCode.E;
    public float pickupRange = 2f;
    
    [Tooltip("Capa (Layer) de los objetos que el jugador puede agarrar.")]
    public LayerMask pickupLayer; 

    private GameObject heldObject;
    private Rigidbody heldObjectRb;

    void Update()
    {
        // Detecta si presionamos la tecla de interacción (E por defecto)
        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null)
            {
                TryPickup(); // Si no tenemos nada, intentamos agarrar
            }
            else
            {
                DropObject(); // Si ya tenemos algo, lo soltamos
            }
        }
    }

    void TryPickup()
    {
        // Busca objetos alrededor del jugador en un radio específico. 
        // Esto es ideal para vistas periféricas porque no depende de a dónde mire la cámara.
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        if (colliders.Length > 0)
        {
            // Agarramos el primer objeto válido que encontremos en el área
            heldObject = colliders[0].gameObject;
            heldObjectRb = heldObject.GetComponent<Rigidbody>();

            if (heldObjectRb != null)
            {
                // Desactivamos la física para que no colisione bruscamente con el jugador mientras lo carga
                heldObjectRb.isKinematic = true; 
                heldObjectRb.useGravity = false;
            }

            // Emparentamos el objeto a la posición de agarre y reseteamos su posición
            heldObject.transform.SetParent(holdPosition);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }

    void DropObject()
    {
        if (heldObjectRb != null)
        {
            // Reactivamos la física para que el objeto caiga al suelo
            heldObjectRb.isKinematic = false; 
            heldObjectRb.useGravity = true;
        }

        // Desenparentamos el objeto
        heldObject.transform.SetParent(null); 
        heldObject = null;
        heldObjectRb = null;
    }

    // Esta función dibuja una esfera amarilla en el editor de Unity para ayudarte a ajustar el rango
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}