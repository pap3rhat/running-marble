using System.Collections;
using TMPro;
using UnityEngine;

public class CentralMessageDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _centralMessageObject;
    [SerializeField] private TextMeshProUGUI _centralMessageText;

    private string _diedText = "You Died!";
    private string _gameOverText = "Game Over!";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;

        _gameManager.GameOver.AddListener(OnGameOver);
        _gameManager.StartCountdown.AddListener(StartDisplay);
        _gameManager.RespawnCountdown.AddListener(RespawnDisplay);
        _gameManager.RespawnMessageTime = 6f; // setting how long respawning text takes to be displayed
        _centralMessageObject.SetActive(false);
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* GAME OVER */
    private void OnGameOver(bool display)
    {
        if (display)
        {
            _centralMessageText.alpha = 1;
            _centralMessageText.text = _gameOverText;
        }

        _centralMessageObject.SetActive(display);
    }

    /* SPAWNING */
    private void StartDisplay()
    {
        StartCoroutine(DisplayStart());
    }

    private IEnumerator DisplayStart()
    {
        _centralMessageObject.SetActive(true);
        _centralMessageText.alpha = 1;
        _centralMessageText.SetText("Starting in...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("3...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("2...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("1...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("GO!");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageObject.SetActive(false);

        _gameManager.GameStarted(); // tell _gameManger spawning animation is done
    }

    /* RESPAWNING */
    private void RespawnDisplay()
    {
        StartCoroutine(DisplayRespawn());
    }

    private IEnumerator DisplayRespawn()
    {
        // You died message
        _centralMessageObject.SetActive(true);
        _centralMessageText.alpha = 1;
        _centralMessageText.text = _diedText;
        yield return new WaitForSeconds(1);

        // starting respawn messages 
        _centralMessageText.SetText("Respawning...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("3...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("2...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("1...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("GO!");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageObject.SetActive(false);
    }


    /* GENERAL */

    /*
     * Fades out text.
     */
    private IEnumerator FadeOut(TMP_Text text)
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime;
            text.alpha = 1 - t;
            yield return null;
        }
    }
}
