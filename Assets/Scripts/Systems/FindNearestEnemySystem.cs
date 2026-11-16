using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerMovementSystem))]
[UpdateAfter(typeof(MoveToTargetSystem))]
public partial struct FindNearestEnemySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.CreateEntity(
            typeof(NearestEnemyHealth)
        );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float3 playerPosition = float3.zero;
        bool playerFound = false;

        foreach (var transform in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<PlayerMovementComponent>())
        {
            playerPosition = transform.ValueRO.Position;
            playerFound = true;
            break;
        }

        if (!playerFound)
            return;

        float nearestDistance = float.MaxValue;
        float nearestHealth = 0f;
        int nearestIndex = -1;
        bool enemyFound = false;

        foreach (var (health, transform, entity) in
                 SystemAPI.Query<
                         RefRO<Health>,
                         RefRO<LocalTransform>>()
                     .WithEntityAccess())
        {
            float distance = math.distancesq(
                playerPosition,
                transform.ValueRO.Position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestHealth = health.ValueRO.Value;
                nearestIndex = entity.Index;
                enemyFound = true;
            }
        }

        SystemAPI.SetSingleton(new NearestEnemyHealth
        {
            Value = nearestHealth,
            EntityIndex = nearestIndex,
            HasEnemy = enemyFound
        });
    }
}
