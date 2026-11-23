using UnityEngine;

public class VineClimbAnchorScript : MonoBehaviour
{
    [Header("Climb Points")]
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform outwardDirection;

    [Header("Overhang Clearance")]
    [SerializeField, Min(0f)] private float overhangClearance = 1f;
    [SerializeField, Min(0f)] private float curveHeight = 0.75f;

    private VineClimbLinkScript activeVine;

    public Transform BottomPoint => bottomPoint;
    public Transform TopPoint => topPoint;

    public bool CanGrowVine => bottomPoint != null && topPoint != null && activeVine == null;

    public Vector3 GetOutwardDirection()
    {
        if (outwardDirection != null)
        {
            Vector3 direction = outwardDirection.forward;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
                return direction.normalized;
        }

        Vector3 fallback = transform.forward;
        fallback.y = 0f;

        return fallback.sqrMagnitude > 0.001f ? fallback.normalized : Vector3.forward;
    }

    public Vector3 GetFirstControlPoint()
    {
        Vector3 start = bottomPoint.position;
        float height = Vector3.Distance(bottomPoint.position, topPoint.position);

        return start + Vector3.up * (height * 0.35f + curveHeight) + GetOutwardDirection() * overhangClearance;
    }

    public Vector3 GetSecondControlPoint()
    {
        Vector3 end = topPoint.position;
        float height = Vector3.Distance(bottomPoint.position, topPoint.position);

        return end - Vector3.up * (height * 0.2f) + GetOutwardDirection() * overhangClearance;
    }

    public bool TryGrowVine(GameObject vinePrefab)
    {
        if (!CanGrowVine || vinePrefab == null)
            return false;

        GameObject vineObject = Instantiate(vinePrefab, bottomPoint.position, Quaternion.identity);

        VineClimbLinkScript vine = vineObject.GetComponent<VineClimbLinkScript>();

        if (vine == null)
        {
            Debug.LogError("Vine prefab is missing VineClimbLinkScript.", vineObject);

            Destroy(vineObject);
            return false;
        }

        if (!vine.Configure(this))
        {
            Destroy(vineObject);
            return false;
        }

        activeVine = vine;
        return true;
    }

    public void NotifyVineDestroyed(VineClimbLinkScript vine)
    {
        if (activeVine == vine)
            activeVine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (bottomPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(bottomPoint.position, 0.2f);
        }

        if (topPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(topPoint.position, 0.2f);
        }

        if (bottomPoint != null && topPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(bottomPoint.position, topPoint.position);
        }
    }
}