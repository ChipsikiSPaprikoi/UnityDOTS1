using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ECSStatsUI : MonoBehaviour
{
    public Transform contentRoot;
    public TMP_Text rowPrefab;

    private readonly Dictionary<Entity, TMP_Text> rows = new();

    private void Update()
    {
        World world = World.DefaultGameObjectInjectionWorld;

        if (world == null || !world.IsCreated)
            return;

        EntityManager entityManager = world.EntityManager;

        EntityQuery query = entityManager.CreateEntityQuery(
            typeof(Health),
            typeof(Stamina),
            typeof(MoveSpeed)
        );

        NativeArray<Entity> entities =
            query.ToEntityArray(Allocator.Temp);

        HashSet<Entity> activeEntities = new();

        foreach (Entity entity in entities)
        {
            activeEntities.Add(entity);

            Health health =
                entityManager.GetComponentData<Health>(entity);

            Stamina stamina =
                entityManager.GetComponentData<Stamina>(entity);

            MoveSpeed speed =
                entityManager.GetComponentData<MoveSpeed>(entity);

            if (!rows.TryGetValue(entity, out TMP_Text row))
            {
                row = Instantiate(rowPrefab, contentRoot);
                rows.Add(entity, row);
            }

            row.text =
                $"Entity {entity.Index} " +
                $"Health: {health.Value:0} / 100 " +
                $"Stamina: {stamina.Current:0} / {stamina.Max:0} " +
                $"Speed: {speed.Value:0.0} ";
        }

        entities.Dispose();
        query.Dispose();

        List<Entity> removedEntities = new();

        foreach (var pair in rows)
        {
            if (!activeEntities.Contains(pair.Key))
                removedEntities.Add(pair.Key);
        }

        foreach (Entity entity in removedEntities)
        {
            Destroy(rows[entity].gameObject);
            rows.Remove(entity);
        }
    }
}
