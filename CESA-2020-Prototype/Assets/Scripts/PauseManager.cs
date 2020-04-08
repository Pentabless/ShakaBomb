using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

// ポーズ管理クラス
public class PauseManager : MonoBehaviour
{
    // Rigidbodyの速度を保存するクラス
    public class RigidbodyVelocity
    {
        public Vector2 velocity;          // 速度
        public float angularVeloccity;    // 角速度

        public RigidbodyVelocity(Rigidbody2D rigidbody)
        {
            velocity = rigidbody.velocity;
            angularVeloccity = rigidbody.angularVelocity;
        }
    }

    private static T[] GetComponentsInActiveScene<T>(bool includeInactive = true)
    {
        // ActiveなSceneのRootにあるGameObject[]を取得する
        var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        // 空の IEnumerable<T>
        IEnumerable<T> resultComponents = (T[])Enumerable.Empty<T>();
        foreach (var item in rootGameObjects)
        {
            // includeInactive = true を指定するとGameObjectが非活性なものからも取得する
            var components = item.GetComponentsInChildren<T>(includeInactive);
            resultComponents = resultComponents.Concat(components);
        }
        return resultComponents.ToArray();
    }


    [SerializeField]
    [Header("(全てを対象にする場合は空でいい)")]
    [Header("ポーズを適用するオブジェクトのrootオブジェクト")]
    private GameObject objectsWrapper;      // ポーズを適用するオブジェクトの範囲

    [SerializeField]
    private PlayDirector playDirector;     // プレイディレクター
    [SerializeField]
    private GameObject pauseMenu;            // ポーズメニューオブジェクト
    [SerializeField]
    private GameObject[] ignoreGameObjects;  // ポーズの影響を受けないオブジェクト

    private bool isPausing = false;                     //ポーズ中かどうか
    private RigidbodyVelocity[] rigidbodyVelocities;    //ポーズ前の速度の配列
    private Rigidbody2D[] pausingRigidbodies;           //ポーズ中のRigidbodyの配列
    private MonoBehaviour[] pausingMonoBehaviours;      //ポーズ中のMonoBehaviourの配列
    private Animator[] pausingAnimators;                //ポーズ中のAnimatorの配列
    private float[] animatorSpeeds;                     //ポーズ前のアニメーション速度の配列
    private ParticleSystem[] pausingParticleSystems;    //ポーズ中のParticleSystemの配列
    private bool[] particleEmittings;                   //ポーズ中の放出状態の配列

    private static Canvas pauseCanvas;   //ポーズ用Canvas
    private static Image pauseImage;     //ポーズ用Image

    //ポーズ用のCanvasとImage生成
    private void CreatePauseFilter()
    {
        //ポーズ用のCanvas生成
        GameObject FadeCanvasObject = new GameObject("CanvasPause");
        pauseCanvas = FadeCanvasObject.AddComponent<Canvas>();
        FadeCanvasObject.AddComponent<GraphicRaycaster>();
        pauseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //FadeCanvasObject.AddComponent<FadeManager>();

        //前面になるよう適当なソートオーダー設定
        pauseCanvas.sortingOrder = 50;

        //ポーズ用のImage生成
        GameObject imagePauseObject = new GameObject("ImagePause");
        pauseImage = imagePauseObject.AddComponent<Image>();
        pauseImage.transform.SetParent(pauseCanvas.transform, false);
        pauseImage.rectTransform.anchoredPosition = Vector3.zero;

        pauseImage.rectTransform.sizeDelta = new Vector2(9999, 9999);

        //色の設定
        pauseImage.color = new Color(0f, 0f, 0f, 0f);

        // ポーズ用オブジェクトとフェード用オブジェクト、影響を受けないオブジェクトの配列に追加する
        ignoreGameObjects = ignoreGameObjects.Concat(new GameObject[] { FadeCanvasObject, FadeManager.GetCanvas().gameObject }).ToArray();
    }

    private void Start()
    {
        CreatePauseFilter();
    }

    private void Update()
    {
        //ボタンが押されたら状態を変更する
        bool pressPause = (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7));
        if (pressPause && !FadeManager.isFadeOut && playDirector.canPause)
        {
            ChangePauseState();
        }

    }

    //ポーズ状態を変更する
    public void ChangePauseState()
    {
        isPausing = !isPausing;
        if (isPausing)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    //public void StartSwitchMovie(SwitchObject[] objects, Vector2 target)
    //{
    //    playingMovie = true;
    //    StartCoroutine(SwitchMovieCoroutine(objects, target));
    //}

    //private IEnumerator SwitchMovieCoroutine(SwitchObject[] objects, Vector2 target)
    //{
    //    GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
    //    CameraController cam_controller = camera.GetComponent<CameraController>();
    //    Vector2 start_pos = GameObject.FindGameObjectWithTag("Player").transform.position;

    //    Pause();

    //    //cam_controller.enabled = true;

    //    for (int i = 0; i <= 60; ++i)
    //    {
    //        Vector3 to = Vector2.Lerp(start_pos, target, i / 60f);
    //        cam_controller.MoveCamera(to);
    //        yield return new WaitForSeconds(1f / 60f);
    //    }

    //    foreach (var obj in objects)
    //    {
    //        obj.enabled = true;
    //        obj.OnTurnOn();
    //    }

    //    for (int i = 0; i <= 60; ++i)
    //    {
    //        yield return new WaitForSeconds(1f / 60f);
    //    }

    //    for (int i = 0; i <= 60; ++i)
    //    {
    //        Vector3 to = Vector2.Lerp(target, start_pos, i / 60f);
    //        cam_controller.MoveCamera(to);
    //        yield return new WaitForSeconds(1f / 60f);
    //    }

    //    Resume();
    //    playingMovie = false;
    //}

    //中断処理

    private void Pause()
    {
        if (isPausing)
        {
            //画面を暗くする
            pauseImage.color = new Color(0f, 0f, 0f, 0.6f);

            //ポーズメニューを起動する
            pauseMenu.SetActive(true);
        }

        //Rigidbodyの停止
        //子要素から、スリープ中でなく、IgnoreGameObjectsに含まれていないRigidbodyを抽出
        Predicate<Rigidbody2D> rigidbodyPredicate =
            obj => !obj.IsSleeping() &&
                   Array.FindIndex(ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        pausingRigidbodies = Array.FindAll(
                (objectsWrapper
                ? objectsWrapper.GetComponentsInChildren<Rigidbody2D>()
                : GetComponentsInActiveScene<Rigidbody2D>()),
                rigidbodyPredicate);
        rigidbodyVelocities = new RigidbodyVelocity[pausingRigidbodies.Length];
        for (int i = 0; i < pausingRigidbodies.Length; ++i)
        {
            //速度と角速度の保存
            rigidbodyVelocities[i] = new RigidbodyVelocity(pausingRigidbodies[i]);
            //Rigidbodyの停止
            pausingRigidbodies[i].Sleep();
        }

        //MonoBehaviourの停止
        //子要素から、有効かつこのインスタンスでないもの、IgnoreGameObjectsに含まれていないMonoBehaviourを抽出
        Predicate<MonoBehaviour> monoBehaviourPredicate =
            obj => obj.enabled &&
                   obj != this &&
                   Array.FindIndex(ignoreGameObjects, gameObject =>
                    Array.FindIndex(gameObject.GetComponentsInChildren<Transform>(), child => child == obj.transform) >= 0) < 0;
        pausingMonoBehaviours = Array.FindAll(
            (objectsWrapper
            ? objectsWrapper.GetComponentsInChildren<MonoBehaviour>()
            : GetComponentsInActiveScene<MonoBehaviour>()),
            monoBehaviourPredicate);
        foreach (var monoBehaviour in pausingMonoBehaviours)
        {
            //MonoBehaviourの停止
            monoBehaviour.enabled = false;
        }

        //Animatorの停止
        //子要素から、有効である、IgnoreGameObjectsに含まれていないAnimatorを抽出
        Predicate<Animator> animatorPredicate =
            obj => obj.enabled &&
                   Array.FindIndex(ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        pausingAnimators = Array.FindAll(
            (objectsWrapper
            ? objectsWrapper.GetComponentsInChildren<Animator>()
            : GetComponentsInActiveScene<Animator>()),
            animatorPredicate);
        animatorSpeeds = new float[pausingAnimators.Length];
        for (int i = 0; i < pausingAnimators.Length; ++i)
        {
            //速度の保存
            animatorSpeeds[i] = pausingAnimators[i].speed;
            //Animatorの停止
            pausingAnimators[i].speed = 0f;
        }

        //パーティクルの停止
        //子要素から、再生中である、IgnoreGameObjectsに含まれていないParticleSystemを抽出
        Predicate<ParticleSystem> particleSystemPredicate =
            obj => obj.isPlaying &&
                   Array.FindIndex(ignoreGameObjects, gameObject => gameObject == obj.gameObject) < 0;
        pausingParticleSystems = Array.FindAll(
            (objectsWrapper
            ? objectsWrapper.GetComponentsInChildren<ParticleSystem>()
             :GetComponentsInActiveScene<ParticleSystem>()),
            particleSystemPredicate);
        particleEmittings = new bool[pausingParticleSystems.Length];
        for (int i = 0; i < pausingParticleSystems.Length; ++i)
        {
            //放出状態の保存
            particleEmittings[i] = pausingParticleSystems[i].isEmitting;
            //ParticleSystemの停止
            pausingParticleSystems[i].Pause();
        }

    }


    //再開処理
    private void Resume()
    {
        //画面を元に戻す
        pauseImage.color = new Color(0f, 0f, 0f, 0f);

        //ポーズメニューを終了する
        pauseMenu.SetActive(false);

        //Rigidbodyの再開
        for (int i = 0; i < pausingRigidbodies.Length; i++)
        {
            pausingRigidbodies[i].WakeUp();
            pausingRigidbodies[i].velocity = rigidbodyVelocities[i].velocity;
            pausingRigidbodies[i].angularVelocity = rigidbodyVelocities[i].angularVeloccity;
        }

        //MonoBehaviourの再開
        foreach (var monoBehaviour in pausingMonoBehaviours)
        {
            monoBehaviour.enabled = true;
        }

        //Animatorの再開
        for (int i = 0; i < pausingAnimators.Length; ++i)
        {
            pausingAnimators[i].speed = animatorSpeeds[i];
        }

        //ParticleSystemの再開
        for (int i = 0; i < pausingParticleSystems.Length; ++i)
        {
            pausingParticleSystems[i].Play();
            if (!particleEmittings[i])
            {
                pausingParticleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
