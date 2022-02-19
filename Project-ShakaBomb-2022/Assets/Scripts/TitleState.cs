using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TitleState : MonoBehaviour
{
    public abstract void Initialize();
    public abstract void Execute();

    public void SetActiveCanvas(bool value)
    {
        this.gameObject.SetActive(value);
    }

    static protected TitleDirector directorInstance = default;
    public TitleDirector DirectorInstance { set { directorInstance = value; } }
}
