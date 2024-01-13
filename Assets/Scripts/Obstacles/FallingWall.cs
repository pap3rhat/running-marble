using UnityEngine;

public class FallingWall : MonoBehaviour
{
    private float _fallTime = 0.5f;
    private float _getUpTime = 3f;
    private float _passedTime = 0f;
    private bool _falling = false;

    [SerializeField] private Transform _posOneTransform;
    [SerializeField] private Transform _posTwoTransform;

    /*----------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    void Update()
    {
        _passedTime += Time.deltaTime;

        if (_falling)
        {

            if (_passedTime >= _fallTime)
            {
                this.transform.position = Vector3.Lerp(_posOneTransform.position, _posTwoTransform.position, 1);
                _passedTime = 0f;
                _falling = false;
            }
            else
            {
                this.transform.position = Vector3.Lerp(_posOneTransform.position, _posTwoTransform.position, _passedTime / _fallTime);
            }
        }
        else
        {
            if (_passedTime >= _getUpTime)
            {
                this.transform.position = Vector3.Lerp(_posTwoTransform.position, _posOneTransform.position, 1);
                _passedTime = 0f;
                _falling = true;
            }
            else
            {
                this.transform.position = Vector3.Lerp(_posTwoTransform.position, _posOneTransform.position, _passedTime / _getUpTime);
            }
        }

    }

    /*----------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void SetTimes(float fall, float up)
    {
        _fallTime = fall;
        _getUpTime = up;
    }

    public void SetFalling(bool falls)
    {
        _falling = falls;
    }
}

