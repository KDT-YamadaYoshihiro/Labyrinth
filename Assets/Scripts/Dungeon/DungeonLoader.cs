using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 複数CSVからダンジョンデータを読み込むマネージャ
/// エネミーデータ読み込みにも対応
/// </summary>
public class DungeonLoader : MonoBehaviour
{
    [Header("読み込むCSVファイル名リスト（拡張子不要）")]
    [SerializeField] private List<string> csvFileNames = new List<string>();

    [Header("生成時の親オブジェクト")]
    [SerializeField] private Transform dungeonParent;

    // ファイル名ごとに2次元データを保持
    private Dictionary<string, List<List<string>>> dungeonDataMap = new Dictionary<string, List<List<string>>>();

    void Start()
    {
        LoadAllDungeonCSV();
    }

    /// <summary>
    /// 指定された全CSVを読み込む
    /// </summary>
    void LoadAllDungeonCSV()
    {
        if (csvFileNames.Count == 0)
        {
            Debug.LogError("読み込むCSVファイル名が指定されていません。");
            return;
        }

        foreach (string fileName in csvFileNames)
        {
            LoadDungeonCSV(fileName);
        }

        Debug.Log($"[DungeonLoader] 全CSV読み込み完了 ({dungeonDataMap.Count}ファイル)");
    }

    /// <summary>
    /// 個別のCSVを読み込む
    /// </summary>
    void LoadDungeonCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{fileName}");

        if (csvFile == null)
        {
            Debug.LogError($"[DungeonLoader] CSVファイルが見つかりません: {fileName}");
            return;
        }

        try
        {
            string[] lines = csvFile.text.Split('\n');
            List<List<string>> dungeonData = new();

            for (int y = 0; y < lines.Length; y++)
            {
                if (string.IsNullOrWhiteSpace(lines[y])) continue;
                string[] cells = lines[y].Trim().Split(',');
                List<string> row = new();

                for (int x = 0; x < cells.Length; x++)
                {
                    string cell = cells[x].Trim();
                    row.Add(cell);

                    // === 敵セルの検出 ===
                    if (cell.StartsWith("E"))
                    {
                        int enemyID = 0;
                        if (cell.Length > 1 && int.TryParse(cell.Substring(1), out enemyID))
                        {
                            EnemyData data = EnemyManager.Instance.GetEnemyData(enemyID);
                            if (data != null)
                            {
                                GameObject prefab = Resources.Load<GameObject>($"Prefabs/{data.PrefabName}");
                                if (prefab != null)
                                {
                                    Vector3 pos = new Vector3(x, 0, -y);
                                    GameObject enemyObj = Instantiate(prefab, pos, Quaternion.identity, dungeonParent);

                                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                                    if (enemy != null)
                                        enemy.Initialize(data);

                                    Debug.Log($"[DungeonLoader] 敵生成: {data.Name} (ID:{enemyID}) at ({x},{y})");
                                }
                                else
                                {
                                    Debug.LogWarning($"[DungeonLoader] 敵プレハブが見つかりません: {data.PrefabName}");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[DungeonLoader] 敵IDが不明なセル: {cell}");
                        }
                    }
                }

                dungeonData.Add(row);
            }

            dungeonDataMap[fileName] = dungeonData;

            Debug.Log($"[DungeonLoader] CSV読み込み成功: {fileName} (行:{dungeonData.Count}, 列:{dungeonData[0].Count})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DungeonLoader] CSV『{fileName}』読み込み中にエラー: {e.Message}");
        }
    }

    public List<List<string>> GetDungeonData(string fileName)
    {
        if (dungeonDataMap.ContainsKey(fileName))
            return dungeonDataMap[fileName];
        else
        {
            Debug.LogError($"[DungeonLoader] ダンジョンデータ『{fileName}』は読み込まれていません。");
            return null;
        }
    }

    public List<string> GetLoadedFileNames()
    {
        return new List<string>(dungeonDataMap.Keys);
    }
}
