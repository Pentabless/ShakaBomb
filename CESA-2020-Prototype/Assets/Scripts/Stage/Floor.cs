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
        Generate,
        FloatFloor
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.transform.parent = this.transform;
            currentObj.OnRideFloor();
        }

        if(collision.transform.tag == "Player" || collision.transform.tag == "Balloon")
        {
            Rigidbody2D rig = collision.gameObject.GetComponent<Rigidbody2D>();
            if (!rig)
            {
                rig = collision.gameObject.GetComponentInParent<Rigidbody2D>();
            }
           
            // 中で止まったら通り抜けるようにする
            if(Mathf.Abs(Data.currentPlayerVel.y) < 0.05f && Data.currentPlayerVel.y < Data.prePlayerVel.y)
            {
                passable = true;
            }
           
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

    public void UpFloor()
    {
        floatFloor.OnFloat();
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

    protected bool CheckDifferences(float x, float y, float differences)
    {
        float temp = Mathf.Abs(x) - Mathf.Abs(y);

        if (temp <= differences)
            return true;
        else
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
    private int time;
    private SpriteRenderer spriteRenderer;
    public bool ActiveFlag { get; set; }

    public GenerateFloor(GameObject obj,int time)
    {
        thisObj = obj;
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1,1,1,0);
        thisObj.layer = 22;
        this.time = time;
    }

    protected override void Execute()
    {
        if (!ActiveFlag)
            return;

        time--;
        thisObj.layer = 0;
        spriteRenderer.color = new Color(1, 1, 1, 1);

        if (time == 0)
        {
            ActiveFlag = false;
            spriteRenderer.color = new Color(1, 1, 1, 0);
            thisObj.layer = 22;
            time = 60;
        }
    }
}

class RideOnFloor : Floor
{
    enum RideOnStatus
    {
        Stay,
        Move,
        Return
    }

    RideOnStatus status = RideOnStatus.Stay;
    float waitCount = 0.0f;
    float count = 0.0f;

    public RideOnFloor(GameObject obj, Vector3 start, Vector3 end, float second)
    {
        thisObj = obj;
        startPosition = start;
        endPosition = end;
        waitCount = second;
    }

    protected override void Execute()
    {
        if (thisObj.transform.childCount != 0)
        {
            count += Time.deltaTime;
            if (count >= waitCount)
            {
                status = RideOnStatus.Move;
            }
        }
        else
        {
            count = 0.0f;
            status = RideOnStatus.Return;
        }

        if(status == RideOnStatus.Move)
        {
            thisObj.transform.position = Vector3.Lerp(thisObj.transform.position, endPosition, 0.05f);
            if (CheckMove(thisObj.transform.position, endPosition))
            {
                status = RideOnStatus.Return;
            }
        }

        if(status == RideOnStatus.Return)
        {
            thisObj.transform.position = Vector3.Lerp(thisObj.transform.position, startPosition, 0.05f);
            if (CheckMove(thisObj.transform.position, startPosition))
            {
                status = RideOnStatus.Stay;
            }
        }
    }
}

class FallFloor:Floor
{
    enum FallStatus
    {
        Stay,
        Up,
        Down
    }

    float waitCount = 0.0f;
    float count = 0.0f;
    FallStatus status = FallStatus.Stay;

    public FallFloor(GameObject obj, Vector3 start, Vector3 end,float second)
    {
        thisObj = obj;
        startPosition = start;
        endPosition = end;
        waitCount = second;
    }

    protected override void Execute()
    {
        // 時間経過を見る
        if (thisObj.transform.childCount != 0)
        {
            count += Time.deltaTime;
            if (count >= waitCount)
            {
                status = FallStatus.Down;
            }
        }
        else
        {
            count = 0.0f;
        }

        // 時間が経ったら落下開始
        if(status == FallStatus.Down)
        { 
            thisObj.transform.position = Vector3.Lerp(thisObj.transform.position, endPosition, 0.08f);
            if(CheckMove(thisObj.transform.position, endPosition))
            {
                status = FallStatus.Up;
            }
        }

        // 落下後上昇
        if(status == FallStatus.Up)
        {
            thisObj.transform.position = Vector3.Lerp(thisObj.transform.position, startPosition, 0.03f);
            if (CheckMove(thisObj.transform.position, startPosition))
            {
                status = FallStatus.Stay;
            }

        }
    }
}

class FloatFloor : Floor
{
    public enum FloatStatus
    {
        Stay,
        Up,
        Down
    }

    readonly float upSecond = 0.3f;
    readonly int LimitCount = 3;
    readonly int UpCount = 6;

    public FloatStatus Status { get; set; }

    int balloonCount = 0;
    float downSecond = 0.0f;
    float difference;
    Vector3 velocity = Vector3.zero;

    bool fristDown = false;

    public FloatFloor(GameObject obj, Vector3 start, Vector3 end, float second)
    {
        thisObj = obj;
        startPosition = start;
        downSecond = second;
        endPosition = end;
    }

    protected override void Execute()
    {
        if (Status == FloatStatus.Down)
        {
            thisObj.transform.position = Vector3.SmoothDamp(thisObj.transform.position, startPosition, ref velocity, downSecond);
            if (CheckMove(thisObj.transform.position, startPosition))
            {
                Status = FloatStatus.Stay;
            }
        }

        if(Status == FloatStatus.Up)
        {
            thisObj.transform.position = Vector3.SmoothDamp(thisObj.transform.position, endPosition, ref velocity, upSecond);
            if (CheckMove(thisObj.transform.position, endPosition))
            {
                balloonCount = 0;
                Status = FloatStatus.Down;
            }
        }
    }

    public void OnFloat()
    {
        balloonCount++;
        if (balloonCount == UpCount)
            Status = FloatStatus.Up;
    }
}
