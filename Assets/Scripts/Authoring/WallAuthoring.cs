using Unity.Entities;
using UnityEngine;

public class WallAuthoring : MonoBehaviour
{
    private class WallBaker : Baker<WallAuthoring>
    {
        public override void Bake(WallAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<WallTag>(entity);
        }
    }
}
