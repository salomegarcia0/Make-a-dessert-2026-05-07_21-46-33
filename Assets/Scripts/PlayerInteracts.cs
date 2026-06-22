using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.F;
    public float interactRange = 3f;

    private PlayerPickup playerPickupScript;

    void Start()
    {
        playerPickupScript = GetComponent<PlayerPickup>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }

    void TryInteract()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider col in colliders)
        {
            // 1. Nevera
            FridgeMenu fridge = col.GetComponent<FridgeMenu>();
            if (fridge != null)
            {
                if (!fridge.uiPanel.activeSelf)
                    fridge.OpenMenu(playerPickupScript);
                return;
            }

            // 2. Cocina
            CookingStation kitchen = col.GetComponent<CookingStation>();
            if (kitchen != null)
            {
                kitchen.TryDepositIngredient(playerPickupScript);
                return;
            }

            // 3. Plato vacío: emplatar la comida que lleva el jugador
            Plate plate = col.GetComponent<Plate>();
            if (plate != null && plate.foodOnPlate == FoodType.Ninguna)
            {
                if (playerPickupScript.IsHoldingItem())
                {
                    GameObject held = playerPickupScript.GetHeldItem();
                    Plate heldPlate = held.GetComponent<Plate>();

                    if (heldPlate != null && heldPlate.foodOnPlate != FoodType.Ninguna)
                    {
                        // Transferir la comida al plato
                        plate.PutFood(heldPlate.foodOnPlate);
                        // Destruir el objeto cocinado de las manos
                        playerPickupScript.DropItem();
                        Destroy(held);
                        Debug.Log("¡Comida emplatada!");
                        return;
                    }
                }
            }

            // 4. NPC
            CustomerNPC npc = col.GetComponent<CustomerNPC>();
            if (npc != null && !npc.orderTaken)
            {
                npc.TakeOrder();
                return;
            }
        }
    }
}