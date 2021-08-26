using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject loadingScreen;
    public SceneSwitchEvent sceneSwitchEvent;

    private Dictionary<SceneEnum, ChangeSceneEventParam> currentLoadings = new Dictionary<SceneEnum, ChangeSceneEventParam>();

    private void Awake()
    {
        var musicParam = new ChangeSceneEventParam(SceneEnum.MUSIC, SceneEnum.NULL, true);
        ChangeScene(musicParam);
        var sharedDataParam = new ChangeSceneEventParam(SceneEnum.SHARED_DATA, SceneEnum.NULL, false, LoadMainMenu);
        ChangeScene(sharedDataParam);
    }

    private void LoadMainMenu()
    {
        var titleParam = new ChangeSceneEventParam(SceneEnum.TITLE, SceneEnum.NULL, false);
        ChangeScene(titleParam);
    }

    public void ChangeScene(ChangeSceneEventParam param)
    {
        SetLoadingScreenActive(true);
        currentLoadings.Add(param.scene, param);
        var switchParam = new SceneSwitchEventParam(SceneSwitchEventParam.SceneLoadStateEnum.STARTED, param.scene, param.sceneToUnload);
        sceneSwitchEvent.Raise(switchParam);
        StartCoroutine(SwitchSceneCoroutine(param));
    }

    // Should be executed next frame!
    private IEnumerator SwitchSceneCoroutine(ChangeSceneEventParam param)
    {
        yield return null;
        if (param.sceneToUnload != SceneEnum.NULL) {
            SceneManager.UnloadSceneAsync((int)param.sceneToUnload);
        }
        var operation = SceneManager.LoadSceneAsync((int)param.scene, LoadSceneMode.Additive);
        operation.completed += (op) => UnitySceneLoaded(param);
    }

    public void SetLoadProgress(LoadProgressSceneEP param)
    {
        if (param.complete)
        {
            SceneLoadCompleted(currentLoadings[param.scene]);
        }
    }

    private void UnitySceneLoaded(ChangeSceneEventParam param)
    {
        if (param.showAfterLoad)
        {
            SceneLoadCompleted(param);
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)param.scene));
    }

    private void SceneLoadCompleted(ChangeSceneEventParam param)
    {
        var switchParam = new SceneSwitchEventParam(SceneSwitchEventParam.SceneLoadStateEnum.LOADED, param.scene, param.sceneToUnload);
        sceneSwitchEvent.Raise(switchParam);
        currentLoadings.Remove(param.scene);
        if (param.onLoad != null)
        {
            param.onLoad();
        }
        if (currentLoadings.Count == 0) {
            SetLoadingScreenActive(false);
        }
    }

    private void SetLoadingScreenActive(bool active)
    {
        loadingScreen.SetActive(active);
    }
}
