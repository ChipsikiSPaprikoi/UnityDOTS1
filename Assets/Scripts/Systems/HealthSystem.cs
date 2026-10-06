using Unity.Entities;

public partial struct HealthSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (health, entity) in
                 SystemAPI.Query<RefRO<Health>>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.Value <= 0f)
            {
                state.EntityManager.SetComponentEnabled<CanMove>(
                    entity,
                    false);
            }
        }
    }
}
