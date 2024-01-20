using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float _motorForce;
    [SerializeField] private float _motorReverseForce;
    [SerializeField] private float _frontBrakeForce;
    [SerializeField] private float _rearBrakeForce;
    [SerializeField] private float _maxSteerAngle;
    [SerializeField] private float _antiRoll;

    [Header("Object References")]
    [SerializeField] private WheelCollider _frontLeftWheelCollider;
    [SerializeField] private WheelCollider _frontRightWheelCollider;
    [SerializeField] private WheelCollider _backLeftWheelCollider;
    [SerializeField] private WheelCollider _backRightWheelCollider;

    [SerializeField] private Transform _frontLeftWheelTransform;
    [SerializeField] private Transform _frontRightWheelTransform;
    [SerializeField] private Transform _backLeftWheelTransform;
    [SerializeField] private Transform _backRightWheelTransform;

    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private Transform _centerOfMass;

    private float _z;
    private float _y;
    private bool _isBreaking;
    private Vector3 _localVelocity;

    private void Start()
    {
        _rigidbody.centerOfMass = _centerOfMass.position;
    }

    private void Update()
    {
        _localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);

        GetInput();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleBrake();
        HandleSteering();
        AddAntiRoll();
    }

    private void AddAntiRoll()
    {
        AddAntiRollPerAxle(_frontLeftWheelCollider, _frontRightWheelCollider);
        AddAntiRollPerAxle(_backLeftWheelCollider, _backRightWheelCollider);
    }

    private void AddAntiRollPerAxle(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        float travelL = 1f;
        float travelR = 1f;

        var groundedL = leftWheel.GetGroundHit(out WheelHit hit);
        if (groundedL)
        {
            travelL = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;
        }

        var groundedR = rightWheel.GetGroundHit(out hit);
        if (groundedR)
        {
            travelR = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;
        }

        float antiRollForce = (travelL - travelR) * _antiRoll;

        if (groundedL)
        {
            _rigidbody.AddForceAtPosition(leftWheel.transform.up * -antiRollForce,
                   leftWheel.transform.position);
        }

        if (groundedR)
        {
            _rigidbody.AddForceAtPosition(rightWheel.transform.up * antiRollForce,
                   rightWheel.transform.position);
        }
    }

    private void HandleSteering()
    {
        float steering = _maxSteerAngle * _z;
        _frontLeftWheelCollider.steerAngle = steering;
        _frontRightWheelCollider.steerAngle = steering;
    }

    private void HandleBrake()
    {
        if (_isBreaking 
            || (_y < 0 && _localVelocity.z > 0) 
            || (_y > 0 && _localVelocity.z < 0))
        {
            _frontLeftWheelCollider.brakeTorque = _frontBrakeForce;
            _frontRightWheelCollider.brakeTorque = _frontBrakeForce;
            _backLeftWheelCollider.brakeTorque = _rearBrakeForce;
            _backRightWheelCollider.brakeTorque = _rearBrakeForce;
        }
        else
        {
            _frontLeftWheelCollider.brakeTorque = 0;
            _frontRightWheelCollider.brakeTorque = 0;
            _backLeftWheelCollider.brakeTorque = 0;
            _backRightWheelCollider.brakeTorque = 0;
        }
    }

    private void HandleMotor()
    {
        float appliedTorque = 0;

        if (_y > 0 && _localVelocity.z > 0)
        {
            appliedTorque = _y * _motorForce;
        }
        else if (_y < 0 && _localVelocity.z < 0)
        {
            appliedTorque = _y * _motorReverseForce;
        }

        _frontLeftWheelCollider.motorTorque = appliedTorque;
        _frontRightWheelCollider.motorTorque = appliedTorque;
    }

    private void GetInput()
    {
        _y = Input.GetAxis("Vertical");
        _z = Input.GetAxis("Horizontal");
        _isBreaking = Input.GetKey(KeyCode.Space);
    }

    
}