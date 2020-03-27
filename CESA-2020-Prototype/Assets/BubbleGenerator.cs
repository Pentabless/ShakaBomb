using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    // プレファブ
    public GameObject bubblePrefab;
    // 作っているか
    bool isCreate;
    // 限度の大きさ
    Vector3 limit_scale;
    // 色
    Vector4 color;

    // SE
    public AudioClip m_sound;
    AudioSource m_audioSource;

    void Start()
    {
        isCreate = false;
        limit_scale = new Vector3(5.0f, 5.0f, 5.0f);
        color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        //Componentを取得
        m_audioSource = GetComponent<AudioSource>();
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
            //プレファブと同じオブジェクトを作る
            GameObject go = Instantiate(bubblePrefab) as GameObject;
            //座標を設定する
            go.transform.position = GameObject.Find("Player").transform.position;
            //色を設定する
            go.GetComponent<Renderer>().material.color = color;
            //作っていない状態にする
            isCreate = false;
        }
    }

    //<自作関数>-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    public void CreateBubble(bool create)
    {
        isCreate = create;
    }


    public void BubbleCreate()
    {
        m_audioSource.PlayOneShot(m_sound);
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
}
