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

    void Start()
    {
       
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
        yield return new WaitForSeconds(1);
        _countdownText.SetText("3...");
        yield return new WaitForSeconds(1);
        _countdownText.SetText("2...");
        yield return new WaitForSeconds(1);
        _countdownText.SetText("1...");
        yield return new WaitForSeconds(1);
        _countdownText.SetText("GO!");
        yield return new WaitForSeconds(1);
        _countdownObj.SetActive(false);
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
        yield return new WaitForSeconds(1);
        _countdownText.SetText("3...");
        yield return new WaitForSeconds(1);
        _countdownText.SetText("2...");
        yield return new WaitForSeconds(1);
        _countdownText.SetText("1...");
        yield return new WaitForSeconds(1);
        _countdownText.SetText("GO!");
        yield return new WaitForSeconds(1);
        _countdownObj.SetActive(false);
    }

}
