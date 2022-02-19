using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterWaveGenerator : MonoBehaviour
{
    // 水面のプレハブ
    [SerializeField]
    GameObject waterWave;

    // 対象とするタイルマップ
    [SerializeField]
    Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        // 全てのタイルを取得
        BoundsInt bound = tilemap.cellBounds;

        // 水面が繋がっている数
        int count = 0;

        // 左下から右上にかけて、全てのタイルを参照する
        for (int y = bound.min.y; y <= bound.max.y; y++)
        {
            for (int x = bound.min.x; x <= bound.max.x; x++)
            {
                // タイルの情報を取得する
                var pos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(pos);

                // 空白なら飛ばす
                if (!tile)
                {
                    // 水面を生成する
                    Generate(count, pos);
                    count = 0;
                    continue;
                }

                // 一つ上のマスが空白ならカウントする
                var up_pos = new Vector3Int(pos.x, pos.y + 1, 0);
                var up_tile = tilemap.GetTile(up_pos);
                if (!up_tile)
                {
                    count++;
                }
                else
                {
                    // 水面を生成する
                    Generate(count, pos);
                    count = 0;
                }
            }
            // 水面を生成する
            Generate(count, new Vector3Int(bound.max.x - 1, y, 0));
            count = 0;
        }
    }
    
    /// <summary>
    /// 水面を生成する
    /// </summary>
    /// <param name="count">水面の数</param>
    /// <param name="rightPos">一番右のセルの座標</param>
    void Generate(int count, Vector3Int rightPos)
    {
        // 数が0なら何もしない
        if (count == 0)
        {
            return;
        }

        // 中心座標を求める
        rightPos.x -= 1;
        var leftPos        = new Vector3Int(rightPos.x - (count-1), rightPos.y, rightPos.z);
        var leftPosWorld   = tilemap.GetCellCenterWorld(leftPos);
        var rightPosWorld  = tilemap.GetCellCenterWorld(rightPos);
        var centerPosWorld = (leftPosWorld + rightPosWorld) * 0.5f;

        // スケールを求める
        Vector3 scale = tilemap.transform.lossyScale;
        scale = new Vector3(scale.x * count, scale.y, scale.z);

        // 水面を生成する
        var obj = Instantiate(waterWave, centerPosWorld, Quaternion.identity, transform);
        obj.transform.localScale = scale;
    }
}
