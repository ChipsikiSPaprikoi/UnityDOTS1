using Unity.Entities;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
    public class Baker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            GetEntity(TransformUsageFlags.Dynamic);
        }
    }
}
