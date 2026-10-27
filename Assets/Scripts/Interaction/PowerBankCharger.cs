using TMPro;
using UnityEngine;

public class PowerBankCharger : MonoBehaviour, IInteractable
{
    [Header("Task")]
    [SerializeField] private TaskData task;

    [Header("Charging")]
    [SerializeField] private float chargeSeconds = 30f;

    [Header("Label")]
    [SerializeField] private TMP_Text label;

    [Header("Cable")]
    [SerializeField] private Transform port;
    [SerializeField] private Transform outlet;
    [SerializeField] private GameObject cableModel;
    [Tooltip("Distance from the model's origin to the back and the front of the plug body.")]
    [SerializeField] private float plugBack = 0.015f;
    [SerializeField] private float plugFront = 0.015f;
    [SerializeField] private float cableSag = 0.15f;
    [Tooltip("How far the cable leaves the outlet and the power bank before it starts to hang.")]
    [SerializeField] private float cableLead = 0.08f;
    [Tooltip("Keeps the waves of the modelled cable. 0 makes it a clean curve.")]
    [SerializeField] private float cableWiggle = 0.5f;
    [Tooltip("Size of the plug and the cable thickness compared to the model.")]
    [SerializeField] private float cableScale = 1.25f;

    private bool pluggedIn;
    private float charge;
    private float plugTime;
    private GameObject cable;

    public string Prompt
    {
        get
        {
            GameSettings settings = GameSettings.Current;
            if (IsFull()) return settings.powerBankTakePrompt;
            if (!PowerSupply.HasPower) return settings.noPowerPrompt;
            if (pluggedIn) return string.Format(settings.powerBankChargingFormat, Mathf.FloorToInt(charge * 100f));
            return settings.powerBankPlugPrompt;
        }
    }

    public bool CanInteract
    {
        get { return task != null && (TaskManager.Instance == null || !TaskManager.Instance.IsCompleted(task)); }
    }

    private void Update()
    {
        if (pluggedIn && PowerSupply.HasPower && !IsFull())
        {
            charge = Mathf.Min(1f, charge + Time.deltaTime / Mathf.Max(0.01f, chargeSeconds));
        }

        if (label != null && pluggedIn)
        {
            label.text = string.Format(GameSettings.Current.powerBankLabelFormat, Mathf.FloorToInt(charge * 100f));
        }
    }

    public void Interact()
    {
        if (!CanInteract) return;
        GameSettings settings = GameSettings.Current;

        if (IsFull())
        {
            if (TaskManager.Instance != null) TaskManager.Instance.RecordAttempt(task, true, Time.time - plugTime);
            GameEvents.RaiseMessage(settings.powerBankPackedMessage);
            gameObject.SetActive(false);
            return;
        }

        if (!PowerSupply.HasPower)
        {
            GameEvents.RaiseMessage(settings.noPowerMessage);
            return;
        }

        if (pluggedIn)
        {
            GameEvents.RaiseMessage(settings.powerBankStillChargingMessage);
            return;
        }

        pluggedIn = true;
        plugTime = Time.time;
        ShowCable();
    }

    private bool IsFull()
    {
        return charge >= 1f;
    }

    // Places the cable model with its plug in the outlet and bends the cable along a hanging curve
    // to the power bank's port. The model is laid out straight along one axis with the plug at its origin.
    // The cable is a child of the power bank, so it disappears when the power bank is packed.
    private void ShowCable()
    {
        if (outlet == null || cableModel == null) return;

        if (cable == null)
        {
            cable = Instantiate(cableModel, transform);
            cable.name = "Cable";
            BendCable();
        }

        cable.SetActive(true);
    }

    private void BendCable()
    {
        MeshFilter[] filters = cable.GetComponentsInChildren<MeshFilter>();
        Transform root = cable.transform;

        // Find the long axis of the model and which way the cable runs from the plug.
        Bounds bounds = new Bounds();
        bool first = true;
        foreach (MeshFilter filter in filters)
        {
            foreach (Vector3 vertex in filter.sharedMesh.vertices)
            {
                Vector3 point = root.InverseTransformPoint(filter.transform.TransformPoint(vertex)) * cableScale;
                if (first) bounds = new Bounds(point, Vector3.zero);
                else bounds.Encapsulate(point);
                first = false;
            }
        }

        Vector3 size = bounds.size;
        Vector3 axis = size.x >= size.y && size.x >= size.z ? Vector3.right : (size.z >= size.y ? Vector3.forward : Vector3.up);
        float max = Vector3.Dot(bounds.max, axis);
        float min = Vector3.Dot(bounds.min, axis);
        if (Mathf.Abs(min) > Mathf.Abs(max))
        {
            axis = -axis;
            max = -min;
        }
        Vector3 modelSide = Vector3.Cross(Vector3.up, axis).normalized;
        float plugBack = this.plugBack * cableScale;
        float plugFront = this.plugFront * cableScale;
        float length = Mathf.Max(0.001f, max - plugFront);

        // Curve from the front of the plug to the port.
        Vector3 outward = outlet.forward;
        Vector3 plugOrigin = outlet.position + outward * plugBack;
        Vector3 start = plugOrigin + outward * plugFront;
        Vector3 end = port != null ? port.position : transform.position;
        Vector3 toOutlet = start - end;
        toOutlet.y = 0f;
        Vector3 portOut = toOutlet.sqrMagnitude > 0.0001f ? toOutlet.normalized : -outward;
        Vector3 control1 = start + outward * cableLead;
        Vector3 control2 = end + portOut * cableLead;
        float floor = Mathf.Max(Mathf.Min(start.y, end.y) - cableSag, FloorHeight(end) + 0.003f);

        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = Instantiate(filter.sharedMesh);
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 point = root.InverseTransformPoint(filter.transform.TransformPoint(vertices[i])) * cableScale;
                float along = Vector3.Dot(point, axis);
                float side = Vector3.Dot(point, modelSide);
                float up = point.y;
                Vector3 world;

                if (along <= plugFront)
                {
                    // The plug stays rigid, pushed into the outlet.
                    Vector3 wallSide = Vector3.Cross(Vector3.up, outward).normalized;
                    world = plugOrigin + outward * along + wallSide * side + Vector3.up * up;
                }
                else
                {
                    float t = Mathf.Clamp01((along - plugFront) / length);
                    Vector3 centre = Bezier(start, control1, control2, end, t);
                    float hang = 16f * t * t * (1f - t) * (1f - t);
                    centre.y = Mathf.Max(centre.y - cableSag * hang, floor);

                    Vector3 tangent = BezierTangent(start, control1, control2, end, t);
                    if (tangent.sqrMagnitude < 0.000001f) tangent = -outward;
                    tangent.Normalize();
                    Vector3 curveSide = Vector3.Cross(Vector3.up, tangent);
                    curveSide = curveSide.sqrMagnitude > 0.000001f ? curveSide.normalized : Vector3.Cross(Vector3.up, outward).normalized;
                    Vector3 normal = Vector3.Cross(tangent, curveSide);

                    // Thickness stays the same; only the waves of the model are scaled.
                    float wave = Mathf.Lerp(1f, cableWiggle, Mathf.Clamp01((along - plugFront) / (0.02f * cableScale)));
                    Vector2 offset = new Vector2(side, up);
                    float radius = 0.004f * cableScale;
                    if (offset.magnitude > radius)
                    {
                        Vector2 centreOffset = offset.normalized * (offset.magnitude - radius);
                        offset = centreOffset * wave + offset.normalized * radius;
                    }
                    world = centre + curveSide * offset.x + normal * offset.y;
                    world.y = Mathf.Max(world.y, floor - 0.003f);
                }

                vertices[i] = filter.transform.InverseTransformPoint(world);
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            filter.sharedMesh = mesh;
        }
    }

    // The cable lies on the floor under the power bank instead of sinking through it.
    private float FloorHeight(Vector3 point)
    {
        float floor = point.y - 1f;
        foreach (RaycastHit hit in Physics.RaycastAll(point + Vector3.up * 0.05f, Vector3.down, 1.05f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.IsChildOf(transform)) continue;
            floor = Mathf.Max(floor, hit.point.y);
        }
        return floor;
    }

    private static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1f - t;
        return u * u * u * a + 3f * u * u * t * b + 3f * u * t * t * c + t * t * t * d;
    }

    private static Vector3 BezierTangent(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1f - t;
        return 3f * u * u * (b - a) + 6f * u * t * (c - b) + 3f * t * t * (d - c);
    }
}
