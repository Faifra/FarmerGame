using UnityEngine;

public class CornAI : MonoBehaviour
{

    [SerializeField] private GameObject cornPrefab;
    [SerializeField] private GameObject kernelPrefab;
    private Transform player;
    [SerializeField] private Transform kernelSP; //kernel spawn point
    [SerializeField] private float reloadTime = 5f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float attackRange = 15f;

    [SerializeField] private float reloadTimer;

    

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= reloadTime)
            {
                ThrowKernel();
                reloadTimer = 0f;
            }
        }
    }

    private void ThrowKernel()
    {
        Debug.Log("Throwing kernel!");
        GameObject kernel = Instantiate(kernelPrefab, kernelSP.position, Quaternion.identity);

        Vector3 direction = (player.position - kernelSP.position).normalized;

        Rigidbody rb = kernel.GetComponent<Rigidbody>();
        rb.AddForce(direction * throwForce, ForceMode.Impulse); //applies force to kernels rigidbody
    }
}
