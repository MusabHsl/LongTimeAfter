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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController =GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveVector=joystickController.GetMovePosition() * MoveSpeed * Time.deltaTime / Screen.width;
        moveVector.z=moveVector.y; //burada yapılanın amacı joystikteki y değeri bizim karakterimizi z doğrultusunda etkilesin ve eylem yapsın.
        moveVector.y=0;
        
         // ---- HATIRLADIĞIN KOD BU OLABİLİR ----
        if (moveVector.magnitude > 0)
        {
            //transform.forward = moveVector; // Eski sert dönüş
            // Yumuşak dönüş:
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotationSpeed);
        }
        // --------------------------------------
        
        characterController.Move(moveVector);

        // Animasyon kontrolünü geri getirdik (Kurs mantığına uygun olarak)
        if (playerAnimator != null)
        {
            float speed = moveVector.magnitude;
            playerAnimator.ManageAnimations(speed);
        }
    }
}
