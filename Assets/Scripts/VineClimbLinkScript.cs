
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class VineClimbLinkScript : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshLink navMeshLink;
    [SerializeField] private LineRenderer vineRenderer;

    [Header("Visual")]
    [SerializeField, Range(4, 64)] private int curveSegments = 24;

    private VineClimbAnchorScript anchor;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 controlPoint1;
    private Vector3 controlPoint2;

    private bool configured;

    public Vector3 StartPoint => startPoint;
    public Vector3 EndPoint => endPoint;

    private void Awake()
    {
        if (navMeshLink == null)
            navMeshLink = GetComponent<NavMeshLink>();

        if (vineRenderer == null)
            vineRenderer = GetComponent<LineRenderer>();
    }

    public bool Configure(VineClimbAnchorScript buildingAnchor)
    {
        if (buildingAnchor == null || navMeshLink == null)
            return false;

        anchor = buildingAnchor;

        startPoint = anchor.BottomPoint.position;
        endPoint = anchor.TopPoint.position;

        controlPoint1 = anchor.GetFirstControlPoint();
        controlPoint2 = anchor.GetSecondControlPoint();

        navMeshLink.startPoint = navMeshLink.transform.InverseTransformPoint(startPoint);

        navMeshLink.endPoint = navMeshLink.transform.InverseTransformPoint(endPoint);

        navMeshLink.bidirectional = false;
        navMeshLink.activated = true;
        navMeshLink.enabled = true;

        DrawVine();

        configured = true;
        return true;
    }

    public Vector3 GetPoint(float t)
    {
        t = Mathf.Clamp01(t);

        float inverseT = 1f - t;

        return inverseT * inverseT * inverseT * startPoint + 3f * inverseT * inverseT * t * controlPoint1 + 3f * inverseT * t * t * controlPoint2 + t * t * t * endPoint;
    }

    private void DrawVine()
    {
        if (vineRenderer == null)
            return;

        vineRenderer.useWorldSpace = true;
        vineRenderer.positionCount = curveSegments + 1;

        for (int i = 0; i <= curveSegments; i++)
        {
            float t = i / (float)curveSegments;
            vineRenderer.SetPosition(i, GetPoint(t));
        }
    }

    private void OnDestroy()
    {
        if (anchor != null)
            anchor.NotifyVineDestroyed(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (!configured)
            return;

        Gizmos.color = Color.cyan;

        Vector3 previousPoint = GetPoint(0f);

        for (int i = 1; i <= curveSegments; i++)
        {
            Vector3 currentPoint = GetPoint(i / (float)curveSegments);

            Vector3 currentPointOnCurve = currentPoint;

            Gizmos.DrawLine(previousPoint, currentPointOnCurve);
            previousPoint = currentPointOnCurve;
        }
    }
}