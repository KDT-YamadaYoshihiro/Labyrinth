using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData Data { get; private set; }

    public void Initialize(EnemyData data)
    {
        Data = data;
        name = data.Name;
        Debug.Log($"[Enemy] ¶¬: {data.Name} HP:{data.HP} U:{data.Attack}");
    }
}
