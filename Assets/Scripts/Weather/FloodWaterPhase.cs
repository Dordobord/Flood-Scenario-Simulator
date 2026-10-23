using UnityEngine;

public class FloodWaterPhase : MonoBehaviour
{
    [Header("Water")]
    [SerializeField] private GameObject[] waterObjects;

    [Header("House")]
    [SerializeField] private Transform houseWater;
    [SerializeField] private SandbagWall sandbagWall;
    [SerializeField] private float floodedHeight = 0.3f;
    [SerializeField] private float seepageHeight = 0.07f;

    private void Awake()
    {
        GameEvents.PreparationEnded += ShowWater;
        SetWater(false);
        if (houseWater != null) houseWater.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.PreparationEnded -= ShowWater;
    }

    private void ShowWater()
    {
        SetWater(true);
        FloodHouse();
    }

    private void SetWater(bool show)
    {
        foreach (GameObject water in waterObjects)
        {
            if (water != null) water.SetActive(show);
        }
    }

    private void FloodHouse()
    {
        if (houseWater == null) return;

        bool sandbagged = sandbagWall != null && sandbagWall.IsFull;
        Vector3 position = houseWater.position;
        position.y = sandbagged ? seepageHeight : floodedHeight;
        houseWater.position = position;
        houseWater.gameObject.SetActive(true);
    }
}
