using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        // Geliştirici Tuşu: R (Restart & Reset Data)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetGameProgress();
        }
    }

    public void ResetGameProgress()
    {
        Debug.Log("Geliştirici Modu: Tüm PlayerPrefs verileri siliniyor ve sahne yeniden başlatılıyor...");
        
        // Tüm kayıtlı verileri (coinler, üniteler vb.) siler
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Aktif sahneyi yeniden yükle
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
