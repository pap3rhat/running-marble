using TMPro;
using UnityEngine;
public class LevelDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    // Level Display Text -> Never hidden
    [SerializeField] private TextMeshProUGUI _levelText;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.LevelUpdate.AddListener(OnLevelUpdate);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     *  Handles displaying remaining time.
     */
    private void OnLevelUpdate(int lvl)
    {
        _levelText.text = $"Level: {lvl}";
    }
}
