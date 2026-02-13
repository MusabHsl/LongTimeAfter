using System.Collections;
using UnityEngine;

public 
class ProductPlantController : MonoBehaviour
{
    private bool isReadyToPick;
    private Vector3 originalScale;

    [SerializeField] private GameObject boxGO;
    private BagController bagController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isReadyToPick=true;
        originalScale=transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isReadyToPick)
        {
            Debug.Log("Fideye dokunuldu");
            isReadyToPick = false;
            bagController=other.GetComponent<BagController>();
            bagController.AddProductToBag(boxGO);

        }
        StartCoroutine(ProductPicked());
    }

    IEnumerator ProductPicked()
    {
    Vector3 targetscale=originalScale/3;
    transform.gameObject.LeanScale(targetscale,1f);
    yield return new WaitForSeconds(5f);
    transform.gameObject.LeanScale(originalScale,1f).setEase(LeanTweenType.easeOutBack);
    isReadyToPick=true;
    }
     /* //BURADA KÜTÜPHANE EKLEMEDEN YAPILAN ANİMASYONSUZ VE POPUP OLMADAN YAPILAN FİDENİN BÜYÜME KÜÇÜME İŞLEMİ
    IEnumerator ProductPicked()
    {
        float duration = 1f;
        float timer = 0;

        Vector3 targetscale = originalScale / 3;

        while (timer < duration)
        {

            float t = timer / duration;
            Vector3 newscale = Vector3.Lerp(originalScale, targetscale, t);
            transform.localScale = newscale;
            timer += Time.deltaTime;
            yield return null;
        }

        //artık fidemiz küçüldi - şimdi 5 saniye içinde smooth büyüme başlasın
        timer = 0f; // Timer'ı sıfırla
        float growDuration = 5f; // Büyüme süresi 5 saniye

        while (timer < growDuration)
        {
            float t = timer / growDuration; // İlerleme yüzdesi (0 -> 1)
            Vector3 newScale = Vector3.Lerp(targetscale, originalScale, t); // Küçükten -> Orijinale
            transform.localScale = newScale;
            timer += Time.deltaTime;
            yield return null;
        }

        // Tam olarak orijinal boyuta ayarla
        transform.localScale = originalScale;

        // Tekrar toplanabilir yap
        isReadyToPick = true;
        yield return null;

    }
    */
    
}
