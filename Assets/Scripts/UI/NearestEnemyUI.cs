using TMPro;
using Unity.Entities;
using UnityEngine;

public class NearestEnemyUI : MonoBehaviour
{
    public TMP_Text healthText;

    private void Update()
    {
        if (healthText == null)
            return;

        World world = World.DefaultGameObjectInjectionWorld;

        if (world == null || !world.IsCreated)
            return;

        EntityManager entityManager = world.EntityManager;

        EntityQuery query =
            entityManager.CreateEntityQuery(
                typeof(NearestEnemyHealth)
            );

        if (query.IsEmpty)
        {
            healthText.text = "Нет данных";
            query.Dispose();
            return;
        }

        NearestEnemyHealth data =
            query.GetSingleton<NearestEnemyHealth>();

        if (data.HasEnemy)
        {
            healthText.text =
                $"Ближайший Entity {data.EntityIndex}\n" +
                $"Health: {data.Value:0}";
        }
        else
        {
            healthText.text = "Противники отсутствуют";
        }

        query.Dispose();
    }
}
