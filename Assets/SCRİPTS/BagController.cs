using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class BagController : MonoBehaviour
{
    [SerializeField] private Transform bag;

    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            Debug.Log("Kube carpildi");
            AddProductToBag(other.gameObject); 
        }
    }

    public void AddProductToBag(GameObject cube)
    {
        cube.transform.SetParent(bag, true);
        cube.transform.localRotation = Quaternion.identity; // Sırtımızda yamuk durmaması için.
        cube.transform.localPosition = Vector3.zero; // Sırtımızda yamuk durmaması için.
    }
    

}
