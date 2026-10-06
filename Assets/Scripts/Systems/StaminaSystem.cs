using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct StaminaSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (stamina, transform, target) in
                 SystemAPI.Query<
                     RefRW<Stamina>,
                     RefRO<LocalTransform>,
                     RefRO<MoveTarget>>())
        {
            if (!SystemAPI.HasComponent<LocalTransform>(
                    target.ValueRO.Value))
                continue;

            LocalTransform targetTransform =
                SystemAPI.GetComponent<LocalTransform>(
                    target.ValueRO.Value);

            float distance = math.distance(
                transform.ValueRO.Position,
                targetTransform.Position);

            bool isMoving = distance > 0.1f;

            if (isMoving && stamina.ValueRO.Current > 0f)
            {
                stamina.ValueRW.Current -=
                    stamina.ValueRO.DrainPerSecond * deltaTime;
            }
            else if (!isMoving)
            {
                stamina.ValueRW.Current +=
                    stamina.ValueRO.RecoveryPerSecond * deltaTime;
            }

            stamina.ValueRW.Current = math.clamp(
                stamina.ValueRO.Current,
                0f,
                stamina.ValueRO.Max);
        }

        foreach (var (stamina, entity) in
                 SystemAPI.Query<RefRO<Stamina>>()
                     .WithEntityAccess())
        {
            if (stamina.ValueRO.Current <= 0f)
            {
                state.EntityManager.SetComponentEnabled<CanMove>(
                    entity,
                    false);
            }
            else if (stamina.ValueRO.Current >= 20f)
            {
                state.EntityManager.SetComponentEnabled<CanMove>(
                    entity,
                    true);
            }
        }
    }
}
