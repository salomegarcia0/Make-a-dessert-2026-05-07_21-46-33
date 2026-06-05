using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("plate"))
        {
            // Obtenemos el script Plate para saber qué comida lleva
            Plate plateScript = other.GetComponent<Plate>();

            if (plateScript != null)
            {
                // Llamamos a la función pasando el tipo de comida del plato
                EntregarCliente(plateScript.foodOnPlate);

                // Destruimos el objeto del plato físico porque ya fue entregado
                Destroy(other.gameObject);
            }
            else
            {
                Debug.LogError("El objeto tiene el tag 'plate' pero no tiene el script Plate adjunto.");
            }
        }
    }

    private void EntregarCliente(FoodType food)
    {
        Debug.Log("Procesando entrega en el mostrador...");
        
        // Le enviamos la comida al administrador para que se la asigne al NPC correcto
        OrderManager.Instance.DeliverFoodToNPCs(food);
    }
}