using UnityEngine;
using UnityEngine.InputSystem;

public class AdaptiveRaySelector : MonoBehaviour
{
    [Header("Ray")]
    public Transform rayOrigin;
    public float maxDistance = 10f;
    public float snapRadius = 0.35f;
    public LayerMask targetLayer;

    [Header("Mode")]
    public bool adaptiveMode = false;

    [Header("Input")]
    public InputActionReference selectAction;
    public InputActionReference switchModeAction;

    private LineRenderer line;
    private TargetObject currentTarget;

    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.015f;
        line.endWidth = 0.015f;
        line.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        if (switchModeAction != null && switchModeAction.action.WasPressedThisFrame())
        {
            adaptiveMode = !adaptiveMode;
            Debug.Log("Mode changed. Adaptive mode = " + adaptiveMode);
        }

        if (currentTarget != null)
        {
            currentTarget.SetHover(false);
            currentTarget = null;
        }

        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;
        Vector3 endPoint = origin + direction * maxDistance;

        TargetObject foundTarget = null;

        if (adaptiveMode)
        {
            foundTarget = FindClosestTargetNearRay(origin, direction, out Vector3 snapPoint);

            if (foundTarget != null)
            {
                endPoint = snapPoint;
            }
        }
        else
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, targetLayer))
            {
                foundTarget = hit.collider.GetComponent<TargetObject>();
                endPoint = hit.point;
            }
        }

        if (foundTarget != null)
        {
            currentTarget = foundTarget;
            currentTarget.SetHover(true);
        }

        line.SetPosition(0, origin);
        line.SetPosition(1, endPoint);

        if (selectAction != null && selectAction.action.WasPressedThisFrame())
        {
            if (currentTarget != null)
            {
                currentTarget.Select();
                Debug.Log("Selected: " + currentTarget.name);
            }
            else
            {
                Debug.Log("Miss");
            }
        }
    }

    TargetObject FindClosestTargetNearRay(Vector3 origin, Vector3 direction, out Vector3 snapPoint)
    {
        snapPoint = origin + direction * maxDistance;

        TargetObject bestTarget = null;
        float bestDistance = snapRadius;

        TargetObject[] allTargets = FindObjectsByType<TargetObject>(FindObjectsSortMode.None);

        foreach (TargetObject target in allTargets)
        {
            Vector3 targetPosition = target.transform.position;

            float forwardDistance = Vector3.Dot(targetPosition - origin, direction);

            if (forwardDistance < 0 || forwardDistance > maxDistance)
                continue;

            Vector3 projectedPoint = origin + direction * forwardDistance;
            float distanceToRay = Vector3.Distance(targetPosition, projectedPoint);

            if (distanceToRay < bestDistance)
            {
                bestDistance = distanceToRay;
                bestTarget = target;
                snapPoint = targetPosition;
            }
        }

        return bestTarget;
    }
}