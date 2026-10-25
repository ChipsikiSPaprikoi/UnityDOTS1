using Unity.Entities;
using UnityEngine;

public class EntityAuthoring : MonoBehaviour
{
    public float moveSpeed = 2f;

    public float maxHealth = 100f;

    public float maxStamina = 100f;
    public float staminaDrain = 20f;
    public float staminaRecovery = 10f;

    public GameObject player;

    public class Baker : Baker<EntityAuthoring>
    {
        public override void Bake(EntityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MoveSpeed
            {
                Value = authoring.moveSpeed
            });

            AddComponent(entity, new Health
            {
                Value = authoring.maxHealth
            });

            AddComponent(entity, new Stamina
            {
                Current = authoring.maxStamina,
                Max = authoring.maxStamina,
                DrainPerSecond = authoring.staminaDrain,
                RecoveryPerSecond = authoring.staminaRecovery
            });

            AddComponent(entity, new CanMove());

            Entity targetEntity = Entity.Null;

            if (authoring.player != null)
            {
                targetEntity = GetEntity(
                    authoring.player,
                    TransformUsageFlags.Dynamic
                );
            }
            
            AddComponent(entity, new MoveTarget
            {
                Value = targetEntity
            });
        }
    }
}
