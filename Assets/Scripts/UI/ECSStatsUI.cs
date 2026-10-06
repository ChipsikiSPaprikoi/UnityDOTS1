using TMPro;
using Unity.Entities;
using UnityEngine;

public class ECSStatsUI : MonoBehaviour
{
    public TMP_Text statsText;

    private void Update()
    {
        if (!World.DefaultGameObjectInjectionWorld.IsCreated)
            return;

        EntityManager entityManager =
            World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query =
            entityManager.CreateEntityQuery(
                typeof(Health),
                typeof(Stamina),
                typeof(MoveSpeed)
            );

        if (query.IsEmpty)
            return;

        Entity entity = query.GetSingletonEntity();

        Health health =
            entityManager.GetComponentData<Health>(entity);

        Stamina stamina =
            entityManager.GetComponentData<Stamina>(entity);

        MoveSpeed speed =
            entityManager.GetComponentData<MoveSpeed>(entity);

        statsText.text =
            $"Health: {health.Value:0} / 100\n" +
            $"Stamina: {stamina.Current:0} / {stamina.Max:0}\n" +
            $"Speed: {speed.Value:0.0}";
    }
}
