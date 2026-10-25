using Unity.Burst;
using Unity.Entities;

public partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new HealthJob().ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct HealthJob : IJobEntity
    {
        private void Execute(in Health health, EnabledRefRW<CanMove> canMove)
        {
            if (health.Value <= 0f)
                canMove.ValueRW = false;
        }
    }
}
