using UnityEngine;
using TMPro;

// Los tipos de comida disponibles
public enum FoodType { Ninguna, Hamburguesa, Pizza, Ensalada }

public class Plate : MonoBehaviour
{
    [Header("Contenido del Plato")]
    public FoodType foodOnPlate = FoodType.Ninguna;

    [Header("Visual")]
    // (Opcional) Texto flotante sobre el plato que muestra qué lleva
    public TextMeshPro plateLabel;

    // Objetos 3D que aparecen encima del plato según la comida
    // Asigna en Inspector: índice 0 = Hamburguesa, 1 = Pizza, 2 = Ensalada
    public GameObject[] foodVisuals;

    void Start()
    {
        RefreshVisual();
    }

    // ── Emplatar comida cocinada ───────────────────────────────────────
    // Llama esto cuando el jugador con una comida cocinada interactúa con el plato
    public bool TryPlateFood(GameObject cookedFoodObject)
    {
        if (foodOnPlate != FoodType.Ninguna)
        {
            Debug.Log("El plato ya tiene comida: " + foodOnPlate);
            return false;
        }

        Plate cooked = cookedFoodObject.GetComponent<Plate>();
        if (cooked == null)
        {
            Debug.Log("Ese objeto no es comida cocinada.");
            return false;
        }

        PutFood(cooked.foodOnPlate);

        // Destruir el objeto comida (ya está en el plato)
        Destroy(cookedFoodObject);
        return true;
    }

    // ── Asignar comida directamente (usado por CookingStation) ─────────
    public void PutFood(FoodType newFood)
    {
        foodOnPlate = newFood;
        RefreshVisual();
        Debug.Log("Plato preparado con: " + newFood);
    }

    // ── Vaciar el plato ────────────────────────────────────────────────
    public void ClearPlate()
    {
        foodOnPlate = FoodType.Ninguna;
        RefreshVisual();
    }

    // ── Actualizar texto y modelos 3D según la comida ─────────────────
    private void RefreshVisual()
    {
        // Texto flotante
        if (plateLabel != null)
            plateLabel.text = foodOnPlate == FoodType.Ninguna ? "" : foodOnPlate.ToString();

        // Activar solo el visual que corresponde
        if (foodVisuals != null)
        {
            for (int i = 0; i < foodVisuals.Length; i++)
            {
                if (foodVisuals[i] != null)
                    // índice 0 = Hamburguesa(1), 1 = Pizza(2), 2 = Ensalada(3)
                    foodVisuals[i].SetActive(i == (int)foodOnPlate - 1);
            }
        }
    }
}