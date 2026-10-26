using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct TriggerDamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<SimulationSingleton>())
            return;

        var job = new TriggerDamageJob
        {
            HealthLookup = SystemAPI.GetComponentLookup<Health>(false),
            PlayerLookup = SystemAPI.GetComponentLookup<PlayerMovementComponent>(true),
            WallLookup = SystemAPI.GetComponentLookup<WallTag>(true)
        };

        state.Dependency = job.Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>(),
            state.Dependency);
    }

    [BurstCompile]
    public struct TriggerDamageJob : ITriggerEventsJob
    {
        public ComponentLookup<Health> HealthLookup;

        [ReadOnly]
        public ComponentLookup<PlayerMovementComponent> PlayerLookup;

        [ReadOnly]
        public ComponentLookup<WallTag> WallLookup;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool aIsPlayerOrWall =
                PlayerLookup.HasComponent(entityA) ||
                WallLookup.HasComponent(entityA);

            bool bIsPlayerOrWall =
                PlayerLookup.HasComponent(entityB) ||
                WallLookup.HasComponent(entityB);
            
            if (!aIsPlayerOrWall && !bIsPlayerOrWall)
                return;
            
            if (HealthLookup.HasComponent(entityA) &&
                (bIsPlayerOrWall || aIsPlayerOrWall))
            {
                Health health = HealthLookup[entityA];
                health.Value = 0f;
                HealthLookup[entityA] = health;
            }

            if (HealthLookup.HasComponent(entityB) &&
                (aIsPlayerOrWall || bIsPlayerOrWall))
            {
                Health health = HealthLookup[entityB];
                health.Value = 0f;
                HealthLookup[entityB] = health;
            }
        }
    }
}
