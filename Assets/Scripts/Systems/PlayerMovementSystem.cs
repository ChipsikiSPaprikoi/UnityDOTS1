using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<PlayerInputData>(
                out PlayerInputData input))
            return;

        float3 direction = input.MovementDirection;

        if (math.lengthsq(direction) < 0.001f)
            return;

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, movement) in
                 SystemAPI.Query<
                     RefRW<LocalTransform>,
                     RefRO<PlayerMovementComponent>>())
        {
            transform.ValueRW.Position +=
                direction *
                movement.ValueRO.MoveSpeed *
                deltaTime;
        }
    }
}
