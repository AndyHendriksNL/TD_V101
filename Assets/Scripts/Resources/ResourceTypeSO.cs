using UnityEngine;

public enum ResourceType
{
    Gold,
    Villagers
}

[CreateAssetMenu(menuName = "Scriptable Objects/Resource Type")]
public class ResourceTypeSO : ScriptableObject
{
    public ResourceType type;
}
