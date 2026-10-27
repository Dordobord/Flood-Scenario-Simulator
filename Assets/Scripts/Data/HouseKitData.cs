using UnityEngine;

[CreateAssetMenu(fileName = "HouseKit_Data", menuName = "Storm Waits/House Kit")]
public class HouseKitData : ScriptableObject
{
    [Header("Walls")]
    public GameObject wallPlain;
    public GameObject wallWindow;
    public GameObject wallDoor;

    [Header("Trim")]
    public GameObject cornerPost;
    public GameObject floorBand;

    [Header("Roof")]
    public GameObject roof;
    public GameObject roofEdge;

    [Header("Openings")]
    public GameObject door;
    public GameObject window;

    [Header("Fit")]
    public float doorInset = 0.05f;
    public float windowCenterHeight = 1.5f;
    public Vector2 windowHoleSize = new Vector2(1.5f, 1.2f);

    [Header("Paint")]
    public Material[] wallPaints;
    public Material[] roofPaints;
}
