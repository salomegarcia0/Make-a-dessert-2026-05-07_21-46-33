using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControladorJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;
<<<<<<< Updated upstream
    public float turnSpeed = 10f;
    
    private CharacterController _controller;
    private Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    void Start()
    {
        _controller = GetComponent<CharacterController>();
=======
    public float turnSpeed = 15f;

    private CharacterController controller;
    private Camera camaraPrincipal;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camaraPrincipal = Camera.main; // Busca la cámara principal del escenario
>>>>>>> Stashed changes
    }

    void Update()
    {
        // 1. Inputs secos para no patinar
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 2. Direcciones basadas en hacia dónde mira la cámara
        Vector3 camForward = camaraPrincipal.transform.forward;
        Vector3 camRight = camaraPrincipal.transform.right;

<<<<<<< Updated upstream
        // Transformamos el input para que coincida con la cámara isométrica (45°)
        Vector3 skewedInput = _isoMatrix.MultiplyPoint3x4(input.normalized);
        
        //Vector3 skewedInput = input.normalized; // Sin transformación para movimiento directo

        if (input != Vector3.zero)
=======
        // Aplanamos las direcciones para no volar
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Dirección final hacia donde el jugador quiere ir
        Vector3 direction = (camForward * vertical + camRight * horizontal).normalized;

        if (direction.magnitude >= 0.1f)
>>>>>>> Stashed changes
        {
            // --- ROTACIÓN PERFECTA ---
            // Le decimos que mire hacia la dirección en la que nos movemos
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // --- MOVIMIENTO ---
            controller.Move(direction * speed * Time.deltaTime);
        }
        
        // Gravedad
        if (!controller.isGrounded)
        {
            controller.Move(new Vector3(0, -9.81f * Time.deltaTime, 0));
        }
    }
}