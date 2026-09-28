using UnityEngine;

public class TrapPlacementScript : MonoBehaviour
{
    [SerializeField] private float placementDistance = 2f;
    [SerializeField] private float placementHeight = 2f;
    [SerializeField] private LayerMask groundLayers;

    public void PlaceTrap(GameObject trapPrefab)
    {
        if (trapPrefab == null)
            return;

        Vector3 startPosition = transform.position + transform.forward * placementDistance + Vector3.up * placementHeight;

        if (Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, placementHeight * 2f, groundLayers))
        {
            Vector3 flatForward = transform.forward;
            flatForward.y = 0f;
            flatForward.Normalize();

            Quaternion rotation = Quaternion.LookRotation(flatForward);

            GameObject trap = Instantiate(trapPrefab, hit.point, rotation);

            Collider collider = trap.GetComponent<Collider>();

            if (collider != null)
            {
                float bottom = collider.bounds.min.y;
                float offset = hit.point.y - bottom;

                trap.transform.position += Vector3.up * offset;
            }
        }
    }
}