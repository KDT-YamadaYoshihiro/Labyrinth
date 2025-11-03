using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 複数CSVからダンジョンデータを読み込むマネージャ
/// </summary>
public class DungeonLoader : MonoBehaviour
{
    [Header("読み込むCSVファイル名リスト（拡張子不要）")]
    [SerializeField] private List<string> csvFileNames = new List<string>();

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

        Debug.Log($" 全CSVファイルの読み込み完了！ 読み込んだファイル数: {dungeonDataMap.Count}");
    }

    /// <summary>
    /// 個別のCSVを読み込む
    /// </summary>
    void LoadDungeonCSV(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, $"Data/CSV/{fileName}.csv");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSVファイルが見つかりません: {filePath}");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            List<List<string>> dungeonData = new List<List<string>>();

            for (int y = 0; y < lines.Length; y++)
            {
                string[] cells = lines[y].Split(',');
                List<string> row = new List<string>();

                for (int x = 0; x < cells.Length; x++)
                {
                    string cell = cells[x].Trim();
                    row.Add(cell);

                    if (cell.StartsWith("E"))
                    {
                        string enemyType = (cell.Length > 1) ? cell.Substring(1) : "不明";
                        Debug.Log($"[{fileName}] 敵({enemyType})が発見 [x:{x}, y:{y}]");
                    }
                }

                dungeonData.Add(row);
            }

            dungeonDataMap[fileName] = dungeonData;

            Debug.Log($"CSV読み込み成功: {fileName}（行数: {dungeonData.Count}, 列数: {dungeonData[0].Count}）");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CSV『{fileName}』読み込み中にエラー: {e.Message}");
        }
    }

    /// <summary>
    /// 特定のダンジョンデータを取得
    /// </summary>
    public List<List<string>> GetDungeonData(string fileName)
    {
        if (dungeonDataMap.ContainsKey(fileName))
            return dungeonDataMap[fileName];
        else
        {
            Debug.LogError($"ダンジョンデータ『{fileName}』は読み込まれていません。");
            return null;
        }
    }

    /// <summary>
    /// 読み込んだ全ファイル名を取得
    /// </summary>
    public List<string> GetLoadedFileNames()
    {
        return new List<string>(dungeonDataMap.Keys);
    }
}
