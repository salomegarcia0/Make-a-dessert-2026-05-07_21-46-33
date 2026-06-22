using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CookingStation : MonoBehaviour
{
    [Header("Configuración de Receta")]
    public int        requiredIngredients = 3;
    public FoodType   resultFood          = FoodType.Hamburguesa;
    public GameObject cookedFoodPrefab;

    [Header("Punto donde aparece la comida cocinada")]
    public Transform outputPosition;

    [Header("Referencia al Minijuego")]
    public SkillCheckMinigame skillCheckUI;

    [Header("Visualización de ingredientes")]
    public Transform[] ingredientSlots;
    public TextMeshPro stationLabel;

    [SerializeField] private int  currentIngredients = 0;
    [SerializeField] private bool isCooking          = false;

    private List<GameObject> depositedItems = new List<GameObject>();
    private List<GameObject> visualItems    = new List<GameObject>();
    private PlayerPickup     _pendingPickup;

    void Start() => UpdateLabel();

    public bool TryDepositIngredient(PlayerPickup playerPickup)
    {
        if (isCooking) { Debug.Log("[Cocina] En uso."); return false; }

        if (!playerPickup.IsHoldingItem())
        {
            if (currentIngredients >= requiredIngredients)
            {
                StartCooking(playerPickup);
                return true;
            }
            Debug.Log("[Cocina] Deposita " + requiredIngredients + " ingredientes primero.");
            return false;
        }

        if (currentIngredients >= requiredIngredients)
        {
            Debug.Log("[Cocina] Ya llena. Presiona F con manos vacías.");
            return false;
        }

        GameObject item = playerPickup.GetHeldItem();
        playerPickup.DropItem();
        item.SetActive(false);
        depositedItems.Add(item);
        ShowIngredientOnStation(item, currentIngredients);
        currentIngredients++;
        UpdateLabel();
        Debug.Log("[Cocina] Ingrediente " + currentIngredients + "/" + requiredIngredients);
        return true;
    }

    void ShowIngredientOnStation(GameObject original, int index)
    {
        Vector3 pos = (ingredientSlots != null && index < ingredientSlots.Length && ingredientSlots[index] != null)
            ? ingredientSlots[index].position
            : transform.position + Vector3.up * (0.3f + index * 0.25f);

        GameObject visual = Instantiate(original, pos, Quaternion.identity);
        visual.SetActive(true);
        Rigidbody rb = visual.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }
        Collider col = visual.GetComponent<Collider>();
        if (col != null) col.enabled = false;
        visual.transform.SetParent(transform);
        visualItems.Add(visual);
    }

    void UpdateLabel()
    {
        if (stationLabel != null)
            stationLabel.text = currentIngredients + "/" + requiredIngredients;
    }

    void StartCooking(PlayerPickup playerPickup)
    {
        isCooking      = true;
        _pendingPickup = playerPickup;
        foreach (var v in visualItems) if (v != null) v.SetActive(false);
        if (skillCheckUI == null) { Debug.LogError("[Cocina] skillCheckUI no asignado!"); return; }
        skillCheckUI.StartMinigame(this, playerPickup);
    }

    public void OnCookingSuccess(PlayerPickup playerPickup)
    {
        PlayerPickup pickup = (playerPickup != null) ? playerPickup : _pendingPickup;

        Debug.Log("[Cocina] Éxito. pickup=" + (pickup != null ? pickup.name : "NULL") +
                  " prefab=" + (cookedFoodPrefab != null ? cookedFoodPrefab.name : "NULL"));

        // 1. Crear la comida ANTES de limpiar (el fallback necesita depositedItems)
        GameObject cooked = BuildCookedObject();

        // 2. Ahora sí limpiamos la estación
        ClearStation();

        if (cooked == null)
        {
            Debug.LogError("[Cocina] No se creó el objeto cocinado. " +
                           "Asigna 'Cooked Food Prefab' en el Inspector.");
            return;
        }

        // 3. Asignar tipo de comida
        Plate plate = cooked.GetComponent<Plate>();
        if (plate != null) plate.PutFood(resultFood);
        else Debug.LogWarning("[Cocina] El prefab no tiene Plate. Añádelo.");

        // 4. Dar al jugador — soltar lo que lleva si tiene algo
        if (pickup != null)
        {
            if (pickup.IsHoldingItem())
            {
                Debug.Log("[Cocina] Jugador tenía algo, soltándolo primero.");
                pickup.DropItem();
            }
            pickup.ForcePickup(cooked);
            Debug.Log("[Cocina] Comida entregada al jugador: " + cooked.name);
        }
        else
        {
            // Sin jugador: dejar la comida en el output
            Debug.LogWarning("[Cocina] No hay jugador. La comida quedó en: " + cooked.transform.position);
        }
    }

    public void OnCookingFailed()
    {
        Debug.Log("[Cocina] Ingredientes quemados.");
        ClearStation();
    }

    GameObject BuildCookedObject()
    {
        Vector3 spawnPos = outputPosition != null
            ? outputPosition.position
            : transform.position + Vector3.up * 0.6f;

        if (cookedFoodPrefab != null)
        {
            Debug.Log("[Cocina] Instanciando prefab: " + cookedFoodPrefab.name + " en " + spawnPos);
            return Instantiate(cookedFoodPrefab, spawnPos, Quaternion.identity);
        }

        // Fallback: usar primer ingrediente
        if (depositedItems.Count > 0 && depositedItems[0] != null)
        {
            Debug.LogWarning("[Cocina] Sin prefab, usando ingrediente como base.");
            GameObject obj = Instantiate(depositedItems[0], spawnPos, Quaternion.identity);
            obj.SetActive(true);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
            Collider col = obj.GetComponent<Collider>();
            if (col != null) { col.enabled = true; col.isTrigger = false; }
            if (obj.GetComponent<Plate>() == null) obj.AddComponent<Plate>();
            return obj;
        }

        return null;
    }

    void ClearStation()
    {
        foreach (var item in depositedItems) if (item != null) Destroy(item);
        depositedItems.Clear();
        foreach (var v in visualItems) if (v != null) Destroy(v);
        visualItems.Clear();
        currentIngredients = 0;
        isCooking          = false;
        _pendingPickup     = null;
        UpdateLabel();
    }
}