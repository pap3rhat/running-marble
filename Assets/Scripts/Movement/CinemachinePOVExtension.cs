using Cinemachine;
using UnityEngine;

public class CinemachinePOVExtension : CinemachineExtension
{
    private InputManager _inputManager;
    private Vector3 _startingRotation;

    [SerializeField] private float _horizontalClampAngle = 180f;
    [SerializeField] private float _verticalClampAngle = 45f;
    [SerializeField] private float _horizontalSpeed = 10f;
    [SerializeField] private float _verticalSpeed = 10f;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    protected override void Awake()
    {
        _inputManager = InputManager.Instance;
        if (_startingRotation == null)
        {
            _startingRotation = transform.localRotation.eulerAngles;
        }
        base.Awake();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                Vector2 deltaInput = _inputManager.GetMouseData();
                _startingRotation.x += deltaInput.x * _verticalSpeed * Time.deltaTime;
                _startingRotation.y += deltaInput.y * _horizontalSpeed * Time.deltaTime;

                _startingRotation.x = Mathf.Clamp(_startingRotation.x, -_horizontalClampAngle, _horizontalClampAngle);
                _startingRotation.y = Mathf.Clamp(_startingRotation.y, -_verticalClampAngle, _verticalClampAngle);

                state.RawOrientation = Quaternion.Euler(-_startingRotation.y, _startingRotation.x, 0f);
            }
        }
    }
}
