using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BubbleGenerator : MonoBehaviour
{
    [SerializeField]
    // プレハブ
    private GameObject bubblePrefab;
    [SerializeField]
    // 足元にできるエフェクト
    private ParticleSystem sweepPartcle;
    [SerializeField]
    // プレイヤー
    private Transform playerTransform;


    // 作っているか
    private bool isCreate;
    // 限度の大きさ
    private Vector3 limit_scale;
    // 色
    private Vector4 color;

    [SerializeField]
    // SE
    private AudioClip m_sound;

    void Start()
    {
        playerTransform = GameObject.Find(Player.NAME).transform;

        isCreate = false;
        limit_scale = new Vector3(5.0f, 5.0f, 5.0f);
        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //作ろうとしている状態にする
            isCreate = true;
        }

        if (isCreate)
        {
            //プレハブと同じオブジェクトを作る
            GameObject go = Instantiate(bubblePrefab) as GameObject;
            //座標を設定する
            go.transform.position = GameObject.Find(Player.NAME).transform.position;
            //生成した泡を子オブジェクトに登録する
            go.transform.parent = this.transform;
            //色を設定する
            go.GetComponent<Renderer>().material.color = color;
            //作っていない状態にする
            isCreate = false;

            // 足元にエフェクトを発生させる
            sweepPartcle.transform.position = playerTransform.position;
            sweepPartcle.Emit(1);
        }
    }

    //<自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    public void CreateBubble(bool create)
    {
        isCreate = create;
    }


    public void BubbleCreate()
    {
        SoundPlayer.Play(m_sound);
        limit_scale = new Vector3(5.0f, 5.0f, 5.0f);
        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        isCreate = true;
    }

    public void GroundBubbleCreate(Vector4 request_color, Vector3 request_limit_scale)
    {
        limit_scale = request_limit_scale;  //大きくなる限度
        color = request_color;              //色
        isCreate = true;
    }

    public void JumpBubbleCreate(Vector4 request_color, Vector3 request_limit_scale)
    {
        limit_scale = request_limit_scale;  //大きくなる限度
        color = request_color;              //色
        isCreate = true;
    }

    //------------------------------------------------------------------------------------------
    // ターゲットの追跡をやめる
    //------------------------------------------------------------------------------------------
    public void StopChase()
    {
        var children = GetComponentsInChildren<BubbleController>();
        int i = 0;
        foreach(var child in children)
        {
            i++;
            child.StopChase();
        }
    }
}
