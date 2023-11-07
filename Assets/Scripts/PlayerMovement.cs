using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody _playerRigidBody;
    [SerializeField, Range(0, 1000)] private float _speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementDierection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _playerRigidBody.AddForce(movementDierection * _speed * Time.deltaTime, ForceMode.Impulse);
    }
}
