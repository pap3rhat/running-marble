using UnityEngine;

public class FallDeathDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    // Fall Death
    [SerializeField] private GameObject _fallDeathObj;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    void Start()
    {
        _gameManager.FallDeath.AddListener(on => _fallDeathObj.SetActive(on));
    }

}
