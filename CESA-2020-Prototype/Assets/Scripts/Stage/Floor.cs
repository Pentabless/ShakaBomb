//==============================================================================================
/// File Name	: 
/// Summary		: 
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
//==============================================================================================
public partial class Floor
{
    public enum FloorStatus
    {
        Normal,
        Move,
        RidoOn,
        CircleRotation,
        Fall,
        Rotation,
        Generate
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.parent = this.transform;
            currentObj.OnRideFloor();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.parent = null;
            currentObj.OnDownFloor();
        }
    }


    private void ChangeState(Floor state)
    {
        currentObj = state;
    }

    protected virtual void Execute()
    {

    }

    public void StartFunction()
    {
        rotationFloor.Switch = true;
    }

    public void ActiveObject()
    {
        generateFloor.ActiveFlag = true;
    }

    /// <summary>
    /// ラープ完了したかどうか
    /// </summary>
    /// <param name="start">スタートポジション</param>
    /// <param name="end">エンドポジション</param>
    /// <returns>完了していればtrue</returns>
    protected bool CheckMove(Vector3 start, Vector3 end)
    {
        if (Mathf.Approximately(start.x, end.x) && Mathf.Approximately(start.y, end.y))
            return true;

        return false;
    }

    protected virtual void OnRideFloor()
    {

    }

    protected virtual void OnDownFloor()
    {

    }

}


class NormalFloor : Floor
{
    protected override void Execute()
    {

    }
}

class MoveFloor : Floor
{
    bool moveFlag = false;
    float reStartTime = 0.0f;
    float distance;

    public MoveFloor(GameObject obj,Vector3 start,Vector3 end)
    {
        thisObj = obj;
        startPosition = start;
        endPosition = end;
        distance = Vector3.Distance(start, end);
    }

    protected override void Execute()
    {
        // 現在の位置
        var percentage = ((Time.time - reStartTime) * speed) / distance;

        if (!moveFlag)
        {
            thisObj.transform.position = Vector3.Lerp(startPosition,endPosition, percentage);
            if (CheckMove(thisObj.transform.position, endPosition))
            {
                reStartTime = Time.time;
                moveFlag = true;
            }
        }
        else
        {
            thisObj.transform.position = Vector3.Lerp(endPosition, startPosition, percentage);
            if (CheckMove(thisObj.transform.position, startPosition))
            {
                reStartTime = Time.time;
                moveFlag = false;
            }
        }
    }
}

class CircleRotationFloor : Floor
{
    float radius;
    bool flag;

    public CircleRotationFloor(GameObject obj,float radius,float speed,bool right = true)
    {
        thisObj = obj;
        this.radius = radius;
        this.speed = speed;
        flag = right;
    }

    protected override void Execute()
    {
        var x = radius * Mathf.Sin(Time.time * (flag ? speed : speed * -1));
        var y = radius * Mathf.Cos(Time.time * (flag ? speed : speed * -1));
        var z = thisObj.transform.position.z;

        thisObj.transform.position = new Vector3(x, y, z);
    }
}

class RotationFloor : Floor
{
    float second;
    public bool Switch { get; set; }

    // コンストラクタ
    public RotationFloor(GameObject obj, float second)
    {
        this.thisObj = obj;
        this.second = second;
    }

    protected override void Execute()
    {
        if (Switch)
            return;

        // x軸を軸にして毎秒2度、回転させるQuaternionを作成（変数をrotとする）
        Quaternion rot = Quaternion.AngleAxis(second, Vector3.forward);
        // 現在の自信の回転の情報を取得する。
        Quaternion q = thisObj.transform.rotation;
        // 合成して、自身に設定
        thisObj.transform.rotation = q * rot;
    }

}

class GenerateFloor : Floor
{
    // ToDo:後で定数に返る
    private int time = 60;
    private SpriteRenderer spriteRenderer;
    public bool ActiveFlag { get; set; }

    public GenerateFloor(GameObject obj)
    {
        thisObj = obj;
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1,1,1,0);
    }

    protected override void Execute()
    {
        if (!ActiveFlag)
            return;

        time--;
        spriteRenderer.color = new Color(1, 1, 1, 1);

        if (time == 0)
        {
            ActiveFlag = false;
            spriteRenderer.color = new Color(1, 1, 1, 0);
            time = 60;
        }
    }
}

class RideOnFloor : Floor
{
    public float Percentage { get; set; }

    float distance;
    bool startFlag = false;
    bool direction = true;

    public RideOnFloor(GameObject obj, Vector3 start, Vector3 end)
    {
        thisObj = obj;
        startPosition = start;
        endPosition = end;
        thisObj.transform.position = start;
        Percentage = 0.0f;
        distance = Vector3.Distance(start, end);
    }

    protected override void Execute()
    {
        // trueが行き：falseが戻る
        if (!direction)
        {
            thisObj.transform.position = Vector3.Lerp(thisObj.transform.position, startPosition, Percentage);
            if (CheckMove(thisObj.transform.position, endPosition))
            {
                direction = true;
            }

        }
    }

    protected override void OnRideFloor()
    {
        thisObj.transform.position = Vector3.Lerp(thisObj.transform.position, endPosition, Percentage);
        if (CheckMove(thisObj.transform.position, endPosition))
        {
            direction = false;
        }
    }

    protected override void OnDownFloor()
    {
        direction = false;
    }
}
