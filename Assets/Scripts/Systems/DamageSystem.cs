using Unity.Entities;
using Unity.Mathematics;

public partial struct DamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonRW<Health>(out var health))
            return;

        float damage = 5f * SystemAPI.Time.DeltaTime;

        health.ValueRW.Value = math.max(
            0f,
            health.ValueRO.Value - damage
        );
    }
}
