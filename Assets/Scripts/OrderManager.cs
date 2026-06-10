using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Lista de Clientes Activos")]
    public List<CustomerNPC> activeCustomers = new List<CustomerNPC>();

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Los NPCs se añaden solos a esta lista al aparecer
    public void RegisterCustomer(CustomerNPC npc)
    {
        activeCustomers.Add(npc);
    }

    // Recibe la comida de la zona de entrega y busca qué NPC la quiere
    public void DeliverFoodToNPCs(FoodType food)
    {
        for (int i = 0; i < activeCustomers.Count; i++)
        {
            // Intentamos darle la comida al NPC de la lista
            if (activeCustomers[i].TryDeliverFood(food))
            {
                // Si el NPC la aceptó, lo quitamos de la lista de espera
                activeCustomers.RemoveAt(i);
                return; 
            }
        }

        Debug.LogWarning("Se entregó " + food + " pero ningún cliente la había ordenado o no se ha tomado su orden.");
    }
}