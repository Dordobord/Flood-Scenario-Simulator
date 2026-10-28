using System.Collections;
using UnityEngine;

public class FloodWaterPhase : MonoBehaviour
{
    [Header("Water")]
    [SerializeField] private GameObject[] waterObjects;

    [Header("Rising")]
    [SerializeField] private float riseDepth = 0f;
    [SerializeField] private float riseSeconds = 0f;

    [Header("House")]
    [SerializeField] private Transform houseWater;
    [SerializeField] private SandbagWall sandbagWall;
    [SerializeField] private float floodedHeight = 0.3f;

    private void Awake()
    {
        GameEvents.PreparationEnded += ShowWater;
        SetWater(false);
        ApplyScenarioLook();
        if (houseWater != null) houseWater.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.PreparationEnded -= ShowWater;
    }

    private void ShowWater()
    {
        SetWater(true);
        if (riseDepth > 0f && riseSeconds > 0f) StartCoroutine(Rise());
        FloodHouse();
    }

    private void SetWater(bool show)
    {
        foreach (GameObject water in waterObjects)
        {
            if (water != null) water.SetActive(show);
        }
    }

    private void ApplyScenarioLook()
    {
        ScenarioData scenario = TaskManager.ChosenScenario;
        if (scenario == null || scenario.floodWaterMaterial == null) return;

        foreach (GameObject water in waterObjects)
        {
            if (water == null) continue;
            foreach (Renderer waterRenderer in water.GetComponentsInChildren<Renderer>(true))
                waterRenderer.sharedMaterial = scenario.floodWaterMaterial;
        }
    }

    private IEnumerator Rise()
    {
        float[] targets = new float[waterObjects.Length];
        for (int i = 0; i < waterObjects.Length; i++)
        {
            if (waterObjects[i] != null) targets[i] = waterObjects[i].transform.position.y;
        }

        for (float time = 0f; time < riseSeconds; time += Time.deltaTime)
        {
            float offset = riseDepth * (1f - time / riseSeconds);
            SetHeights(targets, offset);
            yield return null;
        }
        SetHeights(targets, 0f);
    }

    private void SetHeights(float[] targets, float offset)
    {
        for (int i = 0; i < waterObjects.Length; i++)
        {
            if (waterObjects[i] == null) continue;
            Vector3 position = waterObjects[i].transform.position;
            position.y = targets[i] - offset;
            waterObjects[i].transform.position = position;
        }
    }

    private void FloodHouse()
    {
        if (houseWater == null) return;

        bool sandbagged = sandbagWall != null && sandbagWall.IsFull;
        if (sandbagged) return;

        Vector3 position = houseWater.position;
        position.y = floodedHeight;
        houseWater.position = position;
        houseWater.gameObject.SetActive(true);
    }
}
