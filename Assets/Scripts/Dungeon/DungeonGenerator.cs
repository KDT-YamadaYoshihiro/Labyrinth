using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CSVデータから3Dダンジョンを生成するクラス
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    [Header("参照するDungeonLoader")]
    [SerializeField] private DungeonLoader dungeonLoader;

    [Header("生成するPrefab")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject chestPrefab;
    public GameObject stairUpPrefab;
    public GameObject stairDownPrefab;
    public GameObject[] enemyPrefabs; // Eの数字で指定

    [Header("配置設定")]
    public float cellSize = 1.0f; // マス1つ分の距離
    public string targetDungeonName = "Dungeon01"; // 読み込むCSV名

    private Transform dungeonParent;

    void Start()
    {
        GenerateDungeon();
    }

    /// <summary>
    /// CSV内容をもとにPrefabを配置
    /// </summary>
    public void GenerateDungeon()
    {
        if (dungeonLoader == null)
        {
            Debug.LogError("❌ DungeonLoader がアタッチされていません。");
            return;
        }

        var data = dungeonLoader.GetDungeonData(targetDungeonName);
        if (data == null)
        {
            Debug.LogError($"❌ 指定したダンジョンデータ({targetDungeonName})が読み込まれていません。");
            return;
        }

        // 既存のマップ削除
        if (dungeonParent != null)
        {
            DestroyImmediate(dungeonParent.gameObject);
        }

        dungeonParent = new GameObject($"Generated_{targetDungeonName}").transform;
        dungeonParent.position = Vector3.zero;

        for (int y = 0; y < data.Count; y++)
        {
            for (int x = 0; x < data[y].Count; x++)
            {
                string cell = data[y][x];
                Vector3 pos = new Vector3(x * cellSize, 0, -y * cellSize);

                GameObject prefab = null;

                switch (cell)
                {
                    case "0": 
                        prefab = floorPrefab;
                        break;
                    case "1":
                        prefab = wallPrefab;
                        break;
                    case "2":
                        prefab = chestPrefab;
                        break;
                    case "3":
                        prefab = stairUpPrefab;
                        break;
                    case "4":
                        prefab = stairDownPrefab;
                        break;
                    default:
                        if (cell.StartsWith("E"))
                        {
                            int enemyId = 0;
                            if (int.TryParse(cell.Substring(1), out enemyId))
                            {
                                if (enemyId - 1 >= 0 && enemyId - 1 < enemyPrefabs.Length)
                                {
                                    prefab = enemyPrefabs[enemyId - 1];
                                }
                                else
                                {
                                    Debug.LogWarning($"敵ID{enemyId}に対応するPrefabが見つかりません。");
                                }
                            }
                        }
                        break;
                }

                if (prefab != null)
                {
                    GameObject obj = Instantiate(prefab, pos, Quaternion.identity, dungeonParent);
                    obj.name = $"{prefab.name}_({x},{y})";
                }
            }
        }

        Debug.Log($"ダンジョン生成完了: {targetDungeonName}（{data.Count}行）");
    }
}
