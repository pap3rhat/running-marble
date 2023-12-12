using System.Collections;
using TMPro;
using UnityEngine;

public class ReSpawingDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    // (Re)spawn message
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private GameObject _countdownObj;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.StartCountdown.AddListener(StartDisplay);
        _gameManager.RespawnCountdown.AddListener(RespawnDisplay);
        _countdownObj.SetActive(false);

        _gameManager.RespawnMessageTime = 5f; // setting how long respawning text takes to be displayed
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* SPAWNING */
    private void StartDisplay()
    {
        StartCoroutine(DisplayStart());
    }

    private IEnumerator DisplayStart()
    {
        _countdownObj.SetActive(true);
        _countdownText.SetText("Starting in...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("3...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("2...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("1...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("GO!");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownObj.SetActive(false);

        _gameManager.GameStarted(); // tell _gameManger spawning animation is done
    }

    /* RESPAWNING */
    private void RespawnDisplay()
    {
        StartCoroutine(DisplayRespawn());
    }

    private IEnumerator DisplayRespawn()
    {
        yield return new WaitForSeconds(_gameManager.DiedMessageTime); // wait for died message to be displayed

        // starting respan messages 
        _countdownObj.SetActive(true);
        _countdownText.SetText("Respawning...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("3...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("2...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("1...");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownText.SetText("GO!");
        yield return StartCoroutine(FadeOut(_countdownText));
        _countdownObj.SetActive(false);
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
