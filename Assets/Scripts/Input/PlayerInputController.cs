using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private World world;
    private EntityManager entityManager;
    private Entity inputEntity;

    private void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;

        if (world == null || !world.IsCreated)
            return;

        entityManager = world.EntityManager;

        EntityQuery query =
            entityManager.CreateEntityQuery(
                typeof(PlayerInputData)
            );

        if (query.IsEmpty)
        {
            inputEntity = entityManager.CreateEntity(
                typeof(PlayerInputData)
            );
        }
        else
        {
            inputEntity = query.GetSingletonEntity();
        }

        query.Dispose();
    }

    private void Update()
    {
        if (world == null || !world.IsCreated ||
            !entityManager.Exists(inputEntity))
            return;

        float3 direction = float3.zero;

        if (Input.GetKey(KeyCode.W))
            direction.z += 1f;

        if (Input.GetKey(KeyCode.S))
            direction.z -= 1f;

        if (Input.GetKey(KeyCode.A))
            direction.x -= 1f;

        if (Input.GetKey(KeyCode.D))
            direction.x += 1f;

        if (math.lengthsq(direction) > 0f)
            direction = math.normalize(direction);

        entityManager.SetComponentData(
            inputEntity,
            new PlayerInputData
            {
                MovementDirection = direction
            }
        );
    }

    private void OnDisable()
    {
        if (world != null && world.IsCreated &&
            entityManager.Exists(inputEntity))
        {
            entityManager.SetComponentData(
                inputEntity,
                new PlayerInputData
                {
                    MovementDirection = float3.zero
                }
            );
        }
    }
}
