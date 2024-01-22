using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class VanController : MonoBehaviour
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
    [SerializeField] private NavMeshObstacle _navMeshObstacle;

    [SerializeField] private Transform _centerOfMass;
    [SerializeField] private bool _useCenterOfMass;

    [SerializeField] private AudioSource _engineAudio;

    private float _z;
    private float _y;
    private bool _isBreaking;
    private Vector3 _localVelocity;
    private bool _isBusted = false;

    public event Action Busted;
    public event Action Spotted;
    public event Action Stealed;

    public void Bust()
    {
        _isBusted = true;
        Busted?.Invoke();
    }

    public void HasBeenSpotted()
    {
        _navMeshObstacle.enabled = false;
        Spotted?.Invoke();
    }

    private void Start()
    {
        _engineAudio.Play();

        if (_useCenterOfMass)
        {
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
        }
        else
        {
            _rigidbody.automaticCenterOfMass = true;
        }
    }

    private void Update()
    {
        _localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);

        UpdateAudioPitch();
        GetInput();
    }

    private void UpdateAudioPitch()
    {
        _engineAudio.pitch = Remap(Mathf.Abs(_localVelocity.z) / 20, 0f, 1f, 0f, 2f);
    }

    private float Remap(float value, float x1, float x2, float y1, float y2)
    {
        return y1 + (value - x1) * (y2 - y1) / (x2 - x1);
    }

    private bool Near(float a, float b)
    {
        return Mathf.Abs(b - a) < 0.1f;
    }

    private void FixedUpdate()
    {
        if (_isBusted)
        {
            ApplyBrake();
        }
        else
        {
            HandleMotor();
            HandleBrake();
            HandleSteering();
            AddAntiRoll();
        }
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
            ApplyBrake();
        }
        else
        {
            _frontLeftWheelCollider.brakeTorque = 0;
            _frontRightWheelCollider.brakeTorque = 0;
            _backLeftWheelCollider.brakeTorque = 0;
            _backRightWheelCollider.brakeTorque = 0;
        }
    }

    private void ApplyBrake()
    {
        _frontLeftWheelCollider.brakeTorque = _frontBrakeForce;
        _frontRightWheelCollider.brakeTorque = _frontBrakeForce;
        _backLeftWheelCollider.brakeTorque = _rearBrakeForce;
        _backRightWheelCollider.brakeTorque = _rearBrakeForce;
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