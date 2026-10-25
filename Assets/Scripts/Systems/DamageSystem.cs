using Unity.Entities;
using Unity.Mathematics;

public partial struct DamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float damage = 5f * SystemAPI.Time.DeltaTime;

        foreach (var health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.Value = math.max(
                0f,
                health.ValueRO.Value - damage
            );
        }
    }
}
