using UnityEngine;

public class CookingMinigame : MonoBehaviour
{
    [Header("Configuración Visual UI")]
    public RectTransform uiPointer; // La imagen de la aguja que va a rotar
    public GameObject minigameCanvas; // El Canvas que contiene el minijuego

    [Header("Ajustes del Minijuego")]
    public float pointerSpeed = 150f; // Qué tan rápido gira la aguja
    public int requiredSuccesses = 3; // Cuántos aciertos seguidos necesitas
    public KeyCode actionKey = KeyCode.Space; // La tecla para interactuar
    
    [Header("Zona de Éxito (Grados de 0 a 360)")]
    public float successMinAngle = 45f;
    public float successMaxAngle = 85f;

    private int currentSuccesses = 0;
    private float currentAngle = 0f;
    private bool isMinigameActive = false;

    void Update()
    {
        if (!isMinigameActive) return;

        // 1. Hacer girar la aguja con el tiempo
        currentAngle -= pointerSpeed * Time.deltaTime;
        
        // Mantener el ángulo en un ciclo de 0 a 360
        if (currentAngle <= -360f) currentAngle += 360f;
        
        // Aplicar la rotación visual a la aguja
        uiPointer.localRotation = Quaternion.Euler(0, 0, currentAngle);

        // 2. Detectar el input del jugador
        if (Input.GetKeyDown(actionKey))
        {
            CheckSkill();
        }
    }

    // Llama a esta función desde tu estufa/sartén para empezar a cocinar
    public void StartMinigame()
    {
        isMinigameActive = true;
        currentSuccesses = 0;
        currentAngle = 0f;
        minigameCanvas.SetActive(true);
        RandomizeSuccessZone();
    }

    private void CheckSkill()
    {
        // Convertimos el ángulo negativo a positivo para calcular fácilmente
        float checkAngle = Mathf.Abs(currentAngle);

        // Comprobamos si la aguja está dentro de los ángulos verdes
        if (checkAngle >= successMinAngle && checkAngle <= successMaxAngle)
        {
            currentSuccesses++;
            Debug.Log("¡Acierto! Llevas: " + currentSuccesses);

            if (currentSuccesses >= requiredSuccesses)
            {
                FinishCooking(true);
            }
            else
            {
                // Si acierta pero aún faltan, movemos la zona verde de lugar
                RandomizeSuccessZone();
            }
        }
        else
        {
            Debug.Log("¡Fallaste! La comida casi se quema.");
            // Aquí podrías reiniciar los aciertos o aplicar una penalización
            currentSuccesses = 0; 
        }
    }

    private void RandomizeSuccessZone()
    {
        // Elige un punto de inicio aleatorio para la zona verde
        successMinAngle = Random.Range(10f, 300f);
        // El tamaño de la zona verde será de 40 grados
        successMaxAngle = successMinAngle + 40f; 
        
        // TODO: Aquí actualizaremos la UI visual para que el jugador vea dónde está la nueva zona
    }

    private void FinishCooking(bool success)
    {
        isMinigameActive = false;
        minigameCanvas.SetActive(false);
        
        if (success)
        {
            Debug.Log("¡Comida terminada con éxito! Aparece el plato.");
            // TODO: Lógica para instanciar el plato final en la sartén
        }
    }
}