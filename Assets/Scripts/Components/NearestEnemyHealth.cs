using Unity.Entities;

public struct NearestEnemyHealth : IComponentData
{
    public float Value;
    public int EntityIndex;
    public bool HasEnemy;
}
