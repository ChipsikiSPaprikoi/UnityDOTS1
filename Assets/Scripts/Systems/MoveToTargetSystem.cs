using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerMovementSystem))]
public partial struct MoveToTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<PhysicsWorldSingleton>())
            return;

        Entity targetEntity = Entity.Null;

        foreach (var target in SystemAPI.Query<RefRO<MoveTarget>>())
        {
            if (target.ValueRO.Value != Entity.Null)
            {
                targetEntity = target.ValueRO.Value;
                break;
            }
        }

        if (targetEntity == Entity.Null ||
            !SystemAPI.HasComponent<LocalTransform>(targetEntity))
            return;

        float3 targetPosition =
            SystemAPI.GetComponent<LocalTransform>(targetEntity).Position;

        var job = new MoveToTargetJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            TargetPosition = targetPosition,
            TargetEntity = targetEntity,
            CollisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld,
            WallTags = SystemAPI.GetComponentLookup<WallTag>(true)
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct MoveToTargetJob : IJobEntity
    {
        public float DeltaTime;
        public float3 TargetPosition;
        public Entity TargetEntity;

        [ReadOnly]
        public CollisionWorld CollisionWorld;

        [ReadOnly]
        public ComponentLookup<WallTag> WallTags;

        private void Execute(
            Entity entity,
            ref LocalTransform transform,
            in MoveSpeed speed,
            in MoveTarget target,
            EnabledRefRO<CanMove> canMove)
        {
            if (!canMove.ValueRO ||
                target.Value != TargetEntity ||
                target.Value == Entity.Null)
                return;

            float3 direction = TargetPosition - transform.Position;
            float distance = math.length(direction);

            if (distance < 0.1f)
                return;

            direction /= distance;

            var raycastInput = new RaycastInput
            {
                Start = transform.Position + direction * 0.6f,
                End = TargetPosition,
                Filter = CollisionFilter.Default
            };

            if (CollisionWorld.CastRay(raycastInput, out RaycastHit hit))
            {
                Entity hitEntity =
                    CollisionWorld.Bodies[hit.RigidBodyIndex].Entity;
                
                if (hitEntity != entity &&
                    hitEntity != TargetEntity &&
                    WallTags.HasComponent(hitEntity))
                {
                    return;
                }
            }

            transform.Position += direction * speed.Value * DeltaTime;
        }
    }
}
