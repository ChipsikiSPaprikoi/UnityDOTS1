using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float3 direction = float3.zero;

        if (Input.GetKey(KeyCode.W))
            direction.z += 1;

        if (Input.GetKey(KeyCode.S))
            direction.z -= 1;

        if (Input.GetKey(KeyCode.A))
            direction.x -= 1;

        if (Input.GetKey(KeyCode.D))
            direction.x += 1;

        if (math.lengthsq(direction) < 0.001f)
            return;

        direction = math.normalize(direction);

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach ((
                     RefRW<LocalTransform> transform,
                     RefRO<PlayerMovementComponent> movement
                 ) in SystemAPI.Query<
                     RefRW<LocalTransform>,
                     RefRO<PlayerMovementComponent>
                 >())
        {
            transform.ValueRW.Position +=
                direction * movement.ValueRO.MoveSpeed * deltaTime;
        }
    }
}
