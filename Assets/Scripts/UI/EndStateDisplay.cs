using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndStateDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    // End state
    [SerializeField] private GameObject _endStateObj;
    [SerializeField] private TextMeshProUGUI _endStateText;
    private string _winText = "Winner!";
    private string _loseText = "Loser!";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    void Start()
    {
        _gameManager.EndState.AddListener(OnEndState);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void OnEndState(bool won)
    {
        _endStateObj.SetActive(true);
        if (won)
        {
            _endStateText.text = _winText;
        }
        else
        {
            _endStateText.text = _loseText;
        }
    }

}
