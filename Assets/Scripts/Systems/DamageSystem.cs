using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

public partial struct DamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new DamageJob
        {
            Damage = 5f * SystemAPI.Time.DeltaTime
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    public partial struct DamageJob : IJobEntity
    {
        public float Damage;

        private void Execute(ref Health health)
        {
            health.Value = math.max(0f, health.Value - Damage);
        }
    }
}
