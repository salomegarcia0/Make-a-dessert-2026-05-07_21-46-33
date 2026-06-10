using UnityEngine;
using UnityEngine.AI;
using TMPro; // Esencial para usar textos modernos en Unity

public class CustomerNPC : MonoBehaviour
{
    [Header("Configuración de Orden")]
    public FoodType desiredFood;
    public bool orderTaken = false;
    public bool isSatisfied = false;

    [Header("Puntos de Movimiento")]
    public Vector3 counterPosition;   // Dónde hace la fila para pedir
    public Vector3 waitingPosition;   // A dónde va a esperar su comida
    public Vector3 exitPosition;      // Por dónde se va del nivel

    [Header("Interfaz (Bocadillo de texto)")]
    public TextMeshPro orderText;     // El texto flotante sobre su cabeza

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Elige comida aleatoria al nacer
        int randomIndex = Random.Range(1, System.Enum.GetValues(typeof(FoodType)).Length);
        desiredFood = (FoodType)randomIndex;

        OrderManager.Instance.RegisterCustomer(this);

        // Mostrar puntos suspensivos porque aún no has tomado su orden
        if (orderText != null) orderText.text = "...";

        // Va al mostrador de pedidos
        WalkTo(counterPosition);
    }

    public void WalkTo(Vector3 destination)
    {
        if (agent != null) agent.SetDestination(destination);
    }

    // Esta función la llamará el jugador cuando interactúe con el NPC
    public void TakeOrder()
    {
        if (!orderTaken)
        {
            orderTaken = true;
            
            // Mostrar la comida solicitada en el texto flotante
            if (orderText != null) orderText.text = desiredFood.ToString();
            
            // Se mueve a la zona de delivery para esperar el plato
            WalkTo(waitingPosition);
        }
    }

    public bool TryDeliverFood(FoodType foodDelivered)
    {
        if (orderTaken && !isSatisfied && foodDelivered == desiredFood)
        {
            isSatisfied = true;
            if (orderText != null) orderText.text = "¡Gracias!";
            LeaveCounter();
            return true;
        }
        return false;
    }

    void LeaveCounter()
    {
        WalkTo(exitPosition);
        Destroy(gameObject, 6f); // Se destruye en 6 segundos para no saturar la memoria
    }
}