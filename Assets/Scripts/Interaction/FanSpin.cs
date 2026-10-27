using UnityEngine;

// Spins the fan blades while the fan is plugged in and the house has power.
// Unplugging it or a power cut lets the blades coast to a stop.
public class FanSpin : MonoBehaviour
{
    [SerializeField] private Transform blades;
    [SerializeField] private AppliancePlug plug;

    [Header("Speed")]
    [Tooltip("Degrees per second at full speed.")]
    [SerializeField] private float speed = 900f;
    [Tooltip("How fast the blades speed up and slow down, in degrees per second each second.")]
    [SerializeField] private float spinUp = 500f;

    private Vector3 axis = Vector3.up;
    private float current;

    private void Start()
    {
        if (plug == null) plug = GetComponent<AppliancePlug>();

        // The blades spin around their thinnest side.
        MeshFilter filter = blades != null ? blades.GetComponent<MeshFilter>() : null;
        if (filter != null && filter.sharedMesh != null)
        {
            Vector3 size = Vector3.Scale(filter.sharedMesh.bounds.size, blades.localScale);
            if (size.x < size.y && size.x < size.z) axis = Vector3.right;
            else if (size.z < size.y) axis = Vector3.forward;
        }
    }

    private void Update()
    {
        if (blades == null) return;

        bool running = PowerSupply.HasPower && (plug == null || plug.IsPluggedIn);
        current = Mathf.MoveTowards(current, running ? speed : 0f, spinUp * Time.deltaTime);
        if (current > 0f) blades.Rotate(axis, current * Time.deltaTime, Space.Self);
    }
}
