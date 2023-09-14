using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text restartText;

    private bool _playerAlive;

    // Start is called before the first frame update
    void Start()
    {
        _playerAlive = true;

        gameOverText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerAlive && Input.GetKeyDown(KeyCode.Return))
        {
            _playerAlive = true;
            ResetGame();
        }
    }

    /// <summary>
    /// Call this when game is over!
    /// </summary>
    public void GameOver()
    {
        // TODO: Activate the GameOver UI (flashing 'GAME OVER!!!')
        // TODO: Stop spawning monsters?
        Debug.Log("GameOver!");
        _playerAlive = false;
        
        // Display 'Game Over' text
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        
        // Make the restart text blinking
        StartCoroutine(RestartTextFlickerRoutine());
    }

    /// <summary>
    /// Make the restart text blinking
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartTextFlickerRoutine()
    {
        while (true)
        {
            restartText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            restartText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Reload the entire scene.
    /// </summary>
    private void ResetGame()
    {
        SceneManager.LoadScene("AOSS");
        Time.timeScale = 1.0f;
    }
    
    public void OnKillPause()
    {
        StartCoroutine(SlowMotionEffect(0.1f, 0.07f));
    }

    public void GameOverSloMo()
    {
        StartCoroutine(SlowMotionEffect(0.2f, 3f));
    }

    public bool PlayerIsAlive()
    {
        return _playerAlive;
    }
    
    IEnumerator SlowMotionEffect(float slowFactor, float slowMoPeriod)
    {
        Time.timeScale = slowFactor;
        yield return new WaitForSecondsRealtime(slowMoPeriod);
        Time.timeScale = 1f;
    }
}
