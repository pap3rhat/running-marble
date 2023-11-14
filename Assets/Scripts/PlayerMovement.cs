using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody _playerRigidBody;
    
    // planar movement
    [SerializeField, Range(0, 50)] private float _speed = 10f;

    // jumping
    private bool _isGrounded = false;
    [SerializeField] private float _jumpForce = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // planar movement
        Vector3 movementDierection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _playerRigidBody.AddForce(movementDierection * _speed * Time.deltaTime, ForceMode.Impulse);

        // jumping
        if(_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _playerRigidBody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        _isGrounded = true;
    }
}
