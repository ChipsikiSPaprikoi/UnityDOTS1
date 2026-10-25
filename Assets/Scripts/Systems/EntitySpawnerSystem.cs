using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EntitySpawnerSystem : ISystem
{
    private EntityQuery _playerQuery;

    public void OnCreate(ref SystemState state)
    {
        _playerQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<PlayerMovementComponent>(),
            ComponentType.ReadOnly<LocalTransform>()
        );

        state.RequireForUpdate<SpawnPrefabConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (_playerQuery.IsEmpty)
            return;

        if (!Input.GetKeyDown(KeyCode.Space) &&
            !Input.GetKeyDown(KeyCode.X))
            return;

        Entity playerEntity = _playerQuery.GetSingletonEntity();
        LocalTransform playerTransform =
            SystemAPI.GetComponent<LocalTransform>(playerEntity);

        Entity singletonEntity =
            SystemAPI.GetSingletonEntity<SpawnPrefabConfig>();

        RefRW<SpawnPrefabConfig> config =
            SystemAPI.GetSingletonRW<SpawnPrefabConfig>();

        DynamicBuffer<SpawnedEntityElement> spawnedEntities =
            SystemAPI.GetBuffer<SpawnedEntityElement>(singletonEntity);

        EntityCommandBuffer ecb =
            SystemAPI.GetSingleton<
                EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Unity.Mathematics.Random random = config.ValueRO.Random;

            float3 offset = new float3(
                random.NextFloat(-6f, 6f),
                0f,
                random.NextFloat(-6f, 6f)
            );

            Entity newEntity = ecb.Instantiate(config.ValueRO.Prefab);

            ecb.SetComponent(
                newEntity,
                LocalTransform.FromPosition(
                    playerTransform.Position + offset
                )
            );

            ecb.SetComponent(
                newEntity,
                new MoveTarget { Value = playerEntity }
            );
            
            ecb.AppendToBuffer(
                singletonEntity,
                new SpawnedEntityElement { Value = newEntity }
            );

            random.NextUInt();
            config.ValueRW.Random = random;
        }
        else if (Input.GetKeyDown(KeyCode.X) && spawnedEntities.Length > 0)
        {
            int lastIndex = spawnedEntities.Length - 1;
            Entity entityToDestroy = spawnedEntities[lastIndex].Value;

            ecb.DestroyEntity(entityToDestroy);
            spawnedEntities.RemoveAt(lastIndex);
        }
    }
}
