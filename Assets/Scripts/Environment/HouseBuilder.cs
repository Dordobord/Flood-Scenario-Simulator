using UnityEngine;

public class HouseBuilder : MonoBehaviour
{
    [Header("Kit")]
    public HouseKitData kit;

    [Header("Size")]
    [Min(1)] public int widthInWalls = 3;
    [Min(1)] public int depthInWalls = 3;
    [Min(1)] public int storeys = 1;

    [Header("Openings")]
    public int doorSlot = 1;
    public bool frontWindows = true;
    public bool upperWindows = true;
    public bool sideWindows;

    [Header("Sides")]
    public bool leftWall = true;
    public bool rightWall = true;

    [Header("Paint")]
    public Material wallPaint;
    public Material roofPaint;

    [Header("Placement")]
    public float sinkDepth;
}
