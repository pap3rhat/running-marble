using TMPro;
using UnityEngine;
public class LevelDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    // Level Display Text -> Never hidden, because menu just appears above it and blocks it
    [SerializeField] private TextMeshProUGUI _levelText;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.LevelUpdate.AddListener(OnLevelUpdate);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     *  Handles displaying current level.
     */
    private void OnLevelUpdate(int lvl)
    {
        _levelText.text = $"Level: {lvl}";
    }
}
