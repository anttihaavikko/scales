using AnttiStarterKit.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace
public class SceneChanger : MonoBehaviour
{
    public Blinders blinders;
    public Transform spinner;

    public string sceneToLoadAtStart;

    //public GameCursor cursor;
    public Canvas canvas;
    public GameObject startCam;
    private AsyncOperation operation;

    private string sceneToLoad;

    public static SceneChanger Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(Instance.gameObject);
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(sceneToLoadAtStart))
            ChangeScene(sceneToLoadAtStart, true);
    }

    private void Update()
    {
        if (operation != null && operation.isDone)
        {
            operation = null;
            Invoke("After", 0.1f);
        }
    }

    public void AttachCamera()
    {
        canvas.worldCamera = Camera.main;
    }

    private void After()
    {
        blinders.Open();
        Tweener.Instance.ScaleTo(spinner, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseIn);
    }

    public void ChangeScene(string sceneName, bool silent = false, bool closeBlinders = true)
    {
        if (!silent)
        {
            //AudioManager.Instance.DoButtonSound();
        }

        if (startCam)
            startCam.SetActive(true);

        if (closeBlinders)
        {
            blinders.Close();
            Tweener.Instance.ScaleTo(spinner, Vector3.one, 0.2f, 0f, TweenEasings.BounceEaseOut);
        }

        sceneToLoad = sceneName;
        CancelInvoke("DoChangeScene");
        Invoke("DoChangeScene", blinders.GetDuration());
    }

    private void DoChangeScene()
    {
        operation = SceneManager.LoadSceneAsync(sceneToLoad);
    }
}