using UnityEngine;

public class FridgeMenu : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject uiPanel; 

    // Guardaremos quién abrió la nevera
    private PlayerPickup currentPlayerPickup; 

    void Start()
    {
        uiPanel.SetActive(false); 
    }

    public void OpenMenu(PlayerPickup pickupScript)
    {
        // Verificamos si el jugador ya tiene las manos llenas antes de abrir
        if (pickupScript.IsHoldingItem())
        {
            Debug.Log("Tienes las manos llenas. Suelta lo que tienes primero.");
            return; // No abre el menú
        }

        currentPlayerPickup = pickupScript;
        uiPanel.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SelectIngredient(GameObject ingredientPrefab)
    {
        if (ingredientPrefab != null && currentPlayerPickup != null)
        {
            // Creamos el ingrediente
            GameObject newItem = Instantiate(ingredientPrefab, currentPlayerPickup.holdPosition.position, Quaternion.identity);
            
            // ¡Magia! Hacemos que el jugador lo agarre usando su propio sistema
            currentPlayerPickup.ForcePickup(newItem);
        }
        
        CloseMenu();
    }

    public void CloseMenu()
    {
        uiPanel.SetActive(false);
        
        // Ocultar el cursor de nuevo al salir
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }
}