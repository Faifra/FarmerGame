using Unity.Cinemachine;
using UnityEngine;

public class KernelCollision : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 10;
    [SerializeField] private float maxLifetime = 7f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided with something!");
        PopKernel();
        Destroy(gameObject);
    }

    private void PopKernel()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                GameManagerScript.SetHP(GameManagerScript.GetHP() - explosionDamage);
                Debug.Log($"popped kernel: {GameManagerScript.GetHP()}");
                break;
            }
        }
    }
}
