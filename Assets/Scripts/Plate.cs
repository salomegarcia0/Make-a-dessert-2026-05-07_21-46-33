using UnityEngine;

// Definimos los tipos de comida disponibles en el juego
public enum FoodType { Ninguna, Hamburguesa, Pizza, Ensalada }

public class Plate : MonoBehaviour
{
    [Header("Contenido del Plato")]
    public FoodType foodOnPlate = FoodType.Ninguna; 
    
    // Podrías crear una función más adelante para cambiar visualmente 
    // el plato cuando el jugador le añade ingredientes.
    public void PutFood(FoodType newFood)
    {
        foodOnPlate = newFood;
        Debug.Log("Se ha preparado: " + newFood + " en el plato.");
    }
}