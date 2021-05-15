using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Scene]
    public string m_SceneName;

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
    }

}