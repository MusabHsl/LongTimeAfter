using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickController : MonoBehaviour
{
    [SerializeField] RectTransform joystickOutline;
    [SerializeField] RectTransform joystickButton;
    [SerializeField] float MoveFactor;
    private Vector3 move;

    private bool CanControlJoystick;
    private Vector3 tapPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        HideJoystick();
    }

    public void TappedJoystickZone() //ekrana dokunduğunda algılayacak ve joystick ekranda gözükecek
    {
        if (Pointer.current != null)
        {
            tapPosition = Pointer.current.position.ReadValue(); //parmağının haraketini al
            joystickOutline.position = tapPosition; //Hareketi joystik gözüksün diye eşitle
        }
        Debug.Log("ekrana dokunuldu");
        ShowJoystick();
    }

    private void ShowJoystick()
    {
        joystickOutline.gameObject.SetActive(true);
        if (Pointer.current != null)
        {
            joystickOutline.position = Pointer.current.position.ReadValue();
        }
        CanControlJoystick=true;
    }

    private void HideJoystick()
    {
        joystickOutline.gameObject.SetActive(false);
        joystickButton.anchoredPosition = Vector2.zero; // Butonu merkeze al
        move = Vector3.zero; // Hareketi sıfırla (kayma dursun)
        CanControlJoystick=false;
    }

    public void ControlJoystick()
    {
        if (Pointer.current != null)
        {
            // Şu anki pozisyonu al
            Vector3 currentPosition = Pointer.current.position.ReadValue();
            Vector3 direction = currentPosition - tapPosition;

            // Hareket mesafesini hesapla ve ekran genişliğine göre normalize et
            float moveMagnitude = direction.magnitude * MoveFactor / Screen.width;
            
            // Joystick dairesinin yarıçapını geçmemesini sağla
            moveMagnitude = Mathf.Min(moveMagnitude, joystickOutline.rect.width / 2);

            // Yönü koru ama uzunluğu sınırla
            move = direction.normalized * moveMagnitude;
            
            // Joystick button'unu hareket ettir
            joystickButton.anchoredPosition = move;

            // Parmak kaldırıldığında joystick'i gizle
            if (Pointer.current.press.wasReleasedThisFrame)
            {
                HideJoystick();
            }
        }
    }

    // Joystick'in hareket yönünü döndür (PlayerMovement için)
    public Vector3 GetMovePosition()
    {
        return move;
    }


    // Update is called once per frame
    void Update()
    {
        if(CanControlJoystick)
        {
            ControlJoystick();
        }
       
    }
}
