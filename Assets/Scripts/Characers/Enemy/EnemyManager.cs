using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private Dictionary<int, EnemyData> enemyDataDict = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadEnemyCSV("EnemyData");
    }

    private void LoadEnemyCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{fileName}");
        if (csvFile == null)
        {
            Debug.LogError($"[EnemyManager] CSVファイルが見つかりません: {fileName}");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // ヘッダーを除く
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Split(',');
            if (cols.Length < 7) continue;

            EnemyData data = new EnemyData
            {
                ID = int.Parse(cols[0]),
                Name = cols[1],
                HP = int.Parse(cols[2]),
                Attack = int.Parse(cols[3]),
                Defense = int.Parse(cols[4]),
                Exp = int.Parse(cols[5]),
                PrefabName = cols[6].Trim()
            };

            enemyDataDict[data.ID] = data;
        }

        Debug.Log($"[EnemyManager] 敵データ読み込み完了: {enemyDataDict.Count}件");
    }

    public EnemyData GetEnemyData(int id)
    {
        if (enemyDataDict.TryGetValue(id, out EnemyData data))
            return data;

        Debug.LogWarning($"[EnemyManager] ID:{id} の敵データが存在しません");
        return null;
    }
}
