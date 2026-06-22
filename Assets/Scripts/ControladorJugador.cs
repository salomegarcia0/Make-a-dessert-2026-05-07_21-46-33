using UnityEngine;


public class KinematicMovement : MonoBehaviour
{
    public float speed = 5f;
    public float turnSpeed = 10f;
    
    private CharacterController _controller;
    private Animator _animator;
    private Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        //se busca el componente Animator en los hijos del objeto, asumiendo que el Animator está en un hijo 
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        // Capturamos el input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        // Crea una matriz que rota -90 grados en el eje Y
        _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, -90, 0));


        // Transformamos el input para que coincida con la cámara isométrica (45°)
        Vector3 skewedInput = _isoMatrix.MultiplyPoint3x4(input.normalized);

        if (_animator != null)
        {
            // Actualiza el parámetro de velocidad en el Animator
            _animator.SetFloat("Speed", input.magnitude*speed);
        }

        if (input != Vector3.zero)
        {
            // Movimiento
            _controller.Move(skewedInput * speed * Time.deltaTime);

            // Rotación fluida hacia la dirección del movimiento
            Quaternion targetRotation = Quaternion.LookRotation(skewedInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    public void TriggerInteraction()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Interact");
        }
    }
}