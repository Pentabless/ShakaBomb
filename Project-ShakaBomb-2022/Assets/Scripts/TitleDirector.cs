using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TitleDirector : MonoBehaviour
{
    public enum TitleScreenState
    {
        START,
        SELECT,
        DATA_SELECT,
        OPTION,
        LICENSE,
    }

    [SerializeField]
    RawImage blackBandPanel = default;
    public RawImage BlackBandPanel { get { return blackBandPanel; } }

    [SerializeField]
    Image titleLogo = default;

    public Image TitleLogo { get { return titleLogo; } }

    [SerializeField]
    TextMeshProUGUI copyrightText = default;

    public TextMeshProUGUI CopyrightText { get { return copyrightText; } }

    [SerializeField]
    List<GameObject> canvases = default;

    List<TitleState> states = default;

    TitleState currentState = default;

    void Start()
    {
        states = new List<TitleState>();
        states.Add(canvases[(int)TitleScreenState.START].GetComponent<TitleStartState>());
        states.Add(canvases[(int)TitleScreenState.SELECT].GetComponent<TitleSelectState>());
        states.Add(canvases[(int)TitleScreenState.DATA_SELECT].GetComponent<TitleDataSelectState>());
        states.Add(canvases[(int)TitleScreenState.OPTION].GetComponent<TitleOptionState>());
        states.Add(canvases[(int)TitleScreenState.LICENSE].GetComponent<TitleLicenseState>());

        currentState = states[(int)TitleScreenState.START];
        currentState.DirectorInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Execute();
    }

    public void ChangeState(TitleScreenState state)
    {
        currentState.SetActiveCanvas(false);
        currentState = states[(int)state];
        // currentState.Initialize();
        currentState.SetActiveCanvas(true);
    }
}
