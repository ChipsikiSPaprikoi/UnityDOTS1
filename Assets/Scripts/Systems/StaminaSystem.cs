using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct StaminaSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new StaminaJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            LocalTransforms = SystemAPI.GetComponentLookup<LocalTransform>(true)
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct StaminaJob : IJobEntity
    {
        public float DeltaTime;

        [ReadOnly]
        public ComponentLookup<LocalTransform> LocalTransforms;

        private void Execute(
            ref Stamina stamina,
            EnabledRefRW<CanMove> canMove,
            in LocalTransform transform,
            in MoveTarget target)
        {
            Entity targetEntity = target.Value;
            if (targetEntity == Entity.Null ||
                !LocalTransforms.HasComponent(targetEntity))
                return;

            float distance = math.distance(
                transform.Position,
                LocalTransforms[targetEntity].Position);

            bool isMoving = distance > 0.1f;

            if (isMoving && stamina.Current > 0f)
                stamina.Current -= stamina.DrainPerSecond * DeltaTime;
            else if (!isMoving)
                stamina.Current += stamina.RecoveryPerSecond * DeltaTime;

            stamina.Current = math.clamp(stamina.Current, 0f, stamina.Max);

            if (stamina.Current <= 0f)
                canMove.ValueRW = false;
            else if (stamina.Current >= 20f)
                canMove.ValueRW = true;
        }
    }
}
