using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start()
    {
        EventsManager.Instance.OnPlayerDead += RestartLevel;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.OnPlayerDead -= RestartLevel;
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartLevelAfterDelay());
    }

    IEnumerator RestartLevelAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

}
