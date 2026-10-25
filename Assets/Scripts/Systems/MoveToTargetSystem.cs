using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerMovementSystem))]
public partial struct MoveToTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
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
        {
            return;
        }
        
        float3 targetPosition =
            SystemAPI.GetComponent<LocalTransform>(targetEntity).Position;

        var job = new MoveToTargetJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            TargetPosition = targetPosition
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct MoveToTargetJob : IJobEntity
    {
        public float DeltaTime;
        public float3 TargetPosition;

        private void Execute(
            ref LocalTransform transform,
            in MoveSpeed speed,
            in MoveTarget target)
        {
            if (target.Value == Entity.Null)
                return;

            float3 direction = TargetPosition - transform.Position;
            float distance = math.length(direction);

            if (distance < 0.1f)
                return;

            direction /= distance;
            transform.Position += direction * speed.Value * DeltaTime;
        }
    }
}
