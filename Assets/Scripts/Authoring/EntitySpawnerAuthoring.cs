using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EntitySpawnerAuthoring : MonoBehaviour
{
    public GameObject entityPrefab;

    public class Baker : Baker<EntitySpawnerAuthoring>
    {
        public override void Bake(EntitySpawnerAuthoring authoring)
        {
            if (authoring.entityPrefab == null)
                return;

            Entity entity = GetEntity(TransformUsageFlags.None);
            Entity prefabEntity = GetEntity(
                authoring.entityPrefab,
                TransformUsageFlags.Dynamic
            );

            uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            AddComponent(entity, new SpawnPrefabConfig
            {
                Prefab = prefabEntity,
                Random = new Unity.Mathematics.Random(seed)
            });

            AddBuffer<SpawnedEntityElement>(entity);
        }
    }
}
