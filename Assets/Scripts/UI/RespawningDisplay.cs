using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RespawningDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    // Respawn message
    [SerializeField] private TextMeshProUGUI _respawnText;
    [SerializeField] private GameObject _respawnObj;

    void Start()
    {
        _gameManager.RespawnCountdown.AddListener(RespawnDisplay);
        _respawnObj.SetActive(false);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void RespawnDisplay()
    {
        StartCoroutine(Display());
    }

    private IEnumerator Display()
    {
        yield return new WaitForSeconds(1);
        _respawnObj.SetActive(true);
        _respawnText.SetText("Respawning...");
        yield return new WaitForSeconds(1);
        _respawnText.SetText("3...");
        yield return new WaitForSeconds(1);
        _respawnText.SetText("2...");
        yield return new WaitForSeconds(1);
        _respawnText.SetText("1...");
        yield return new WaitForSeconds(1);
        _respawnObj.SetActive(false);
    }

}
