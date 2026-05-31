using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    // Esta función de Unity se ejecuta automáticamente cuando un objeto entra en su área de colisión (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que acaba de entrar tiene el tag exacto "Plate"
        if (other.CompareTag("Plate"))
        {
            EntregarCliente();
        }
    }

    // Tu función reservada para la lógica futura
    private void EntregarCliente()
    {
        // Usamos un Debug.Log por ahora para confirmar que la detección funciona correctamente
        Debug.Log("¡Plato colocado en la zona de entrega! Ejecutando EntregarCliente...");
        
        // TODO: Aquí desarrollarás la lógica para evaluar el plato, sumar puntos, etc.
    }
}