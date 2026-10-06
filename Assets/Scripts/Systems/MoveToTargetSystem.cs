using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PlayerMovementSystem))]
public partial struct MoveToTargetSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, speed, target) in
                 SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<MoveSpeed>,
                         RefRO<MoveTarget>>()
                     .WithAll<CanMove>())
        {
            Entity targetEntity = target.ValueRO.Value;

            if (!SystemAPI.HasComponent<LocalTransform>(targetEntity))
                continue;

            LocalTransform targetTransform =
                SystemAPI.GetComponent<LocalTransform>(targetEntity);

            float3 direction =
                targetTransform.Position -
                transform.ValueRO.Position;

            float distance = math.length(direction);

            if (distance < 0.1f)
                continue;

            direction /= distance;

            transform.ValueRW.Position +=
                direction *
                speed.ValueRO.Value *
                deltaTime;
        }
    }
}
