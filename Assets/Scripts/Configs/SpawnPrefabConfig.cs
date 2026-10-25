using Unity.Entities;
using Unity.Mathematics;

public struct SpawnPrefabConfig : IComponentData
{
    public Entity Prefab;
    public Random Random;
}

public struct SpawnedEntityElement : IBufferElementData
{
    public Entity Value;
}
