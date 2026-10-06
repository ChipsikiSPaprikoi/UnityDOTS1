using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(
                authoring.gameObject,
                TransformUsageFlags.Dynamic
            );

            AddComponent(entity, new PlayerMovementComponent
            {
                MoveSpeed = authoring.moveSpeed
            });
        }
    }
}

public struct PlayerMovementComponent : IComponentData
{
    public float MoveSpeed;
}
