using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RainShelter : MonoBehaviour
{
    public Collider Area
    {
        get { return GetComponent<Collider>(); }
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
}
