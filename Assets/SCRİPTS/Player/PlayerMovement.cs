using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Objects")]
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PlayerAnimator playerAnimator;
    private CharacterController characterController;
    Vector3 moveVector;

    [Header("Settings")]
    [SerializeField] private int MoveSpeed;
    [SerializeField] private float rotationSpeed = 10f;

    private float gravity = -9.81f;
    private float verticalVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (joystickController == null)
        {
            return;
        }

        Vector3 joystickDirection = joystickController.GetMovePosition();
        
        // Yerçekimi hesaplama
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f; // Yere yapışık kalması için küçük bir kuvvet
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (joystickDirection.magnitude > 0)
        {
            moveVector = joystickDirection * MoveSpeed * Time.deltaTime / Screen.width;
            moveVector.z = moveVector.y; 
            moveVector.y = 0;
            
            // Dönüş mantığı
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotationSpeed);
        }
        else
        {
            moveVector = Vector3.zero;
        }

        // Yerçekimini ekle
        moveVector.y = verticalVelocity;
        
        characterController.Move(moveVector);

        // Animasyon kontrolü
        if (playerAnimator != null)
        {
            float speed = joystickDirection.magnitude;
            playerAnimator.ManageAnimations(speed);
        }
    }
}
