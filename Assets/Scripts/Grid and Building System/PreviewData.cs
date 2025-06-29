using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuilderType
{
    Angel,
    Devil
}

[CreateAssetMenu(menuName = "Preview/Building")]
public class PreviewData : ScriptableObject
{
    public string buildingName;

    public BuilderType builderType;

    public GameObject previewPrefab;
    public GameObject buildingPrefab;

    public float buildTime = 3f;

    public int buildFireCosts = 3;
    public int buildLightCosts = 3;
}
