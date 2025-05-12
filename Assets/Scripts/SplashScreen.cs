using Steamworks;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private AsyncOperation _asyncLoad;
    private bool _finishedShowingLogos;
    
  
    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        _asyncLoad = SceneManager.LoadSceneAsync(GameMasterManager.GetStartSceneString(), LoadSceneMode.Single);
        _asyncLoad.allowSceneActivation = false;
        //wait until the asynchronous scene fully loads
        while (!_asyncLoad.isDone)
        {
            //scene has loaded as much as possible,
            // the last 10% can't be multi-threaded
            if (_asyncLoad.progress >= 0.9f)
            {
                if (_finishedShowingLogos)
                {
                    _asyncLoad.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

    public void FinishedShowingLogos()
    {
        _finishedShowingLogos = true;
    }
}