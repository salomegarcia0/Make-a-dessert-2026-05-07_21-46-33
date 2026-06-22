using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Configuración de Agarre")]
    public Transform holdPosition;
    public KeyCode interactKey = KeyCode.E;
    public float pickupRange = 2f;
    public LayerMask pickupLayer;

    private GameObject heldItem;

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
            ForcePickup(colliders[0].gameObject);
        }
    }

    public void ForcePickup(GameObject item)
    {
        if (heldItem != null) return;

        heldItem = item;
        heldItem.transform.SetParent(holdPosition);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = heldItem.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    public void DropItem()
    {
        if (heldItem == null) return;

        heldItem.transform.SetParent(null);

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col = heldItem.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        heldItem = null;
    }

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }

    // === NUEVO: necesario para CookingStation ===
    public GameObject GetHeldItem()
    {
        return heldItem;
    }
}