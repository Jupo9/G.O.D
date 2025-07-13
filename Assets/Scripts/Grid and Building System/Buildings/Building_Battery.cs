using UnityEngine;

public class Building_Battery : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material emptySlot;
    [SerializeField] private Material fireSlot;
    [SerializeField] private Material lightSlot;

    private MeshRenderer meshRenderer;
    private Material[] slotMaterials;

    private string[] slotStates = new string[4];

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        slotMaterials = meshRenderer.materials;

        for (int i = 1; i <= 4; i++)
        {
            slotMaterials[i] = emptySlot;
            slotStates[i - 1] = "";
        }

        meshRenderer.materials = slotMaterials;
    }

    public void AddSlot(string type)
    {
        for (int i = 0; i < 4; i++)
        {
            if (slotStates[i] == "" )
            {
                slotStates[i] = type;
                slotMaterials[i + 1] = GetMaterialByType(type);
                meshRenderer.materials = slotMaterials;
                return;
            }
        }

       Debug.LogWarning("All battery slots are already filled");
    }

    public void RemoveSlot(string type) 
    {
        for (int i = 3; i >= 0; i--)
        {
            if (slotStates[i] == type)
            {
                slotStates[i] = "";
                slotMaterials[i + 1] = emptySlot;
                meshRenderer.materials = slotMaterials;
                return;
            }
        }

       Debug.LogWarning($"No {type} slot found to remove.");
    }

    private Material GetMaterialByType(string type)
    {
        switch (type) 
        {
            case "Fire": return fireSlot;
            case "Light": return lightSlot;
            default: return emptySlot;
        }
    }

    public int GetCurrentSlotCount(string type)
    {
        int count = 0;
        foreach (var slot in slotStates)
        {
            if (slot == type)
                count++;
        }
        return count;
    }

    public int GetMaxSlotCount(string type)
    {
        return 4;
    }
}
