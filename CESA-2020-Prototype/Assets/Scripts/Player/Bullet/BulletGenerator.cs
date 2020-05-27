using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    // Bulletの種類
    enum BulletType
    {
        Normal,
    }

    [SerializeField]
    // Bulletプレハブ
    private List<GameObject> bulletPrefab = new List<GameObject>();
    // 使用するプレハブのインデックス
    private BulletType usePrefabIndex = BulletType.Normal;

    [SerializeField]
    // ガイドラインプレハブ
    private GameObject guideLine = null;
    // ガイドライン用オブジェクト
    private List<GameObject> guideLines = new List<GameObject>();
    [SerializeField]
    // ガイドライン生成数
    private int guideLineNum = 5;
    [SerializeField]
    // ガイドライン生成間隔
    private float guideLineInterval = 0.2f;

    [SerializeField]
    // 生成できない時間
    private float cantCreateTime = 0.1f;
    // 残り生成できない時間
    private float remainingTime = 0.0f;
    [SerializeField]
    // 発射方向に応じて位置をずらす量
    private float offsetlength = 0.5f;

    // 使用するプレハブの発射時の力
    private Vector2 currentPower = Vector2.zero;


    //------------------------------------------------------------------------------------------
    // Update
    //------------------------------------------------------------------------------------------
    void Update()
    {
        remainingTime -= Time.deltaTime;
    }

    //------------------------------------------------------------------------------------------
    // ガイドラインを有効にする
    //------------------------------------------------------------------------------------------
    public void EnableGuideLines(Vector3 playerPos, float angle)
    {
        var shotPos = CalcShotPos(playerPos, angle);

        // ↓修正したい
        var go = Instantiate(bulletPrefab[(int)usePrefabIndex]);
        var shotForce = go.GetComponent<BulletController>().CalcShotForce(angle);
        var mass = go.GetComponent<Rigidbody2D>().mass;
        var gravity = go.GetComponent<Rigidbody2D>().gravityScale;
        Destroy(go);
        
        // 多い分を削除する
        for(int i= guideLines.Count - 1; i >= guideLineNum; i--)
        {
            guideLines.RemoveAt(i);
        }

        // 位置を決める
        for(int i = 0; i < guideLineNum; i++)
        {
            // 足りない分を追加する
            if (guideLines.Count == i)
            {
                guideLines.Add(Instantiate(guideLine));
            }
            guideLines[i].SetActive(true);
            // 位置を設定する
            guideLines[i].transform.parent = transform;
            guideLines[i].transform.position = CalcPositionFromForce((i + 1) * guideLineInterval, mass, shotPos,
                shotForce, (Vector3)Physics2D.gravity * gravity);
        }
    }

    //------------------------------------------------------------------------------------------
    // ガイドラインを無効にする
    //------------------------------------------------------------------------------------------
    public void DisableGuideLines()
    {
        foreach(var guide in guideLines)
        {
            guide.SetActive(false);
        }
    }

    //------------------------------------------------------------------------------------------
    // Bulletを生成する
    //------------------------------------------------------------------------------------------
    public bool BulletCreate(Vector3 playerPos, float angle)
    {
        if (remainingTime > 0.0f)
        {
            return false;
        }

        // プレハブと同じオブジェクトを作る
        GameObject go = Instantiate(bulletPrefab[(int)usePrefabIndex], CalcShotPos(playerPos, angle), Quaternion.identity);
        go.GetComponent<BulletController>().SetAngle(angle);

        remainingTime = cantCreateTime;
        return true;
    }

    //------------------------------------------------------------------------------------------
    // 発射位置を計算する
    //------------------------------------------------------------------------------------------
    private Vector3 CalcShotPos(Vector3 startPos, float angle)
    {
        // 位置を発射方向にずらす
        return startPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f) * offsetlength;
    }

    //------------------------------------------------------------------------------------------
    // 物理挙動を計算する
    //------------------------------------------------------------------------------------------
    Vector3 CalcPositionFromForce(float time, float mass, Vector3 startPosition, Vector3 force, Vector3 gravity)
    {
        Vector3 speed = (force / mass);
        Vector3 position = (speed * time) + (gravity * 0.5f * Mathf.Pow(time, 2));

        return startPosition + position;
    }
}
