using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        Mph,
        Kph
    }

    public class CarMover : MonoBehaviour
    {
        [SerializeField] private CarDriveType carDriveType = CarDriveType.FourWheelDrive;
        [SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] wheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 centreOfMassOffset;
        [SerializeField] public float maximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float steerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float tractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float fullTorqueOverAllWheels;
        [SerializeField] private float maxHandbrakeTorque;
        [SerializeField] private float downforce = 100f;
        [SerializeField] private SpeedType speedType;
        private const int NoOfGears = 5;
        [SerializeField] private float revRangeBoundary = 1f;
        [SerializeField] private float slipLimit;
        [SerializeField] public float brakeTorque;


        private Quaternion[] _wheelMeshLocalRotations;
        private Vector3 _prevPos, _pos;
        private float _steerAngle;
        private int _gearNum;
        private float _gearFactor;
        private float _oldRotation;
        private float _currentTorque;
        private Rigidbody _rigidbody;
        public float topSpeed = 200;
        
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle => _steerAngle;
        public float CurrentSpeed => _rigidbody.velocity.magnitude * 2.23693629f;
        public float MaxSpeed => topSpeed;
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        // Use this for initialization
        private void Start()
        {
            _wheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                _wheelMeshLocalRotations[i] = wheelMeshes[i].transform.localRotation;
            }
            wheelColliders[0].attachedRigidbody.centerOfMass = centreOfMassOffset;

            maxHandbrakeTorque = float.MaxValue;

            _rigidbody = GetComponent<Rigidbody>();
            _currentTorque = fullTorqueOverAllWheels - (tractionControl * fullTorqueOverAllWheels);
        }
        
        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
            float upgearlimit = (1 / (float)NoOfGears) * (_gearNum + 1);
            float downgearlimit = (1 / (float)NoOfGears) * _gearNum;

            if (_gearNum > 0 && f < downgearlimit)
            {
                _gearNum--;
            }

            if (f > upgearlimit && (_gearNum < (NoOfGears - 1)))
            {
                _gearNum++;
            }
        }

        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor) * (1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }


        private void CalculateGearFactor()
        {
            float f = (1 / (float)NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f * _gearNum, f * (_gearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
            _gearFactor = Mathf.Lerp(_gearFactor, targetGearFactor, Time.deltaTime * 5f);
        }


        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = _gearNum / (float)NoOfGears;
            var revsRangeMin = ULerp(0f, revRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(revRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, _gearFactor);
        }


        public void Move(float steering, float accel, float footBrake, float handbrake, bool isBreak)
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].GetWorldPose(out var position, out var quat);
                wheelMeshes[i].transform.position = position;
                wheelMeshes[i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = Mathf.Clamp(accel, 0, 1);
            accel = Mathf.Clamp(accel, -1, 1);
            BrakeInput = footBrake = -1 * Mathf.Clamp(footBrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            _steerAngle = steering * maximumSteerAngle;
            wheelColliders[0].steerAngle = _steerAngle;
            wheelColliders[1].steerAngle = _steerAngle;

            SteerHelper();
            ApplyDrive(accel, footBrake, isBreak);
            CapSpeed();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake * maxHandbrakeTorque;
                wheelColliders[2].brakeTorque = hbTorque;
                wheelColliders[3].brakeTorque = hbTorque;
            }


            CalculateRevs();
            GearChanging();

            AddDownForce();
            CheckForWheelSpin();
            TractionControl();
        }


        private void CapSpeed()
        {
            float speed = _rigidbody.velocity.magnitude;
            switch (speedType)
            {
                case SpeedType.Mph:

                    speed *= 2.23693629f;
                    if (speed > topSpeed)
                        _rigidbody.velocity = (topSpeed / 2.23693629f) * _rigidbody.velocity.normalized;
                    break;

                case SpeedType.Kph:
                    speed *= 3.6f;
                    if (speed > topSpeed)
                        _rigidbody.velocity = (topSpeed / 3.6f) * _rigidbody.velocity.normalized;
                    break;
            }
        }


        private void ApplyDrive(float accel, float footBrake, bool isBreak)
        {

            float thrustTorque;
            //Debug.Log(CurrentSpeed);
            switch (carDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    thrustTorque = accel * (_currentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        wheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case CarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (_currentTorque / 2f);
                    wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = thrustTorque;
                    break;

                case CarDriveType.RearWheelDrive:
                    thrustTorque = accel * (_currentTorque / 2f);
                    wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = thrustTorque;
                    break;

            }

            for (int i = 0; i < 4; i++)
            {
                if (isBreak)
                {
                    wheelColliders[i].brakeTorque = brakeTorque * footBrake;
                    
                }
                else if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, _rigidbody.velocity) < 50f)
                {
                    wheelColliders[i].brakeTorque = brakeTorque * footBrake;
                }
                
                else if (footBrake == 0 && Vector3.Magnitude(_rigidbody.velocity) < 1e-3)
                {
                    wheelColliders[i].brakeTorque = 0f;
                }
                if (!isBreak)
                {
                    wheelColliders[i].brakeTorque = 0f;
                }
            }
        }


        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].GetGroundHit(out var wheelHit);
                if (wheelHit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(_oldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnAdjust = (transform.eulerAngles.y - _oldRotation) * steerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
                _rigidbody.velocity = velRotation * _rigidbody.velocity;
            }
            _oldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            wheelColliders[0].attachedRigidbody.AddForce(downforce *
                                                         wheelColliders[0].attachedRigidbody.velocity.magnitude *
                                                         -transform.up);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tire skidding sounds
        // 3) leaves skid marks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                wheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= slipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= slipLimit)
                {
                    wheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        wheelEffects[i].PlayAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (wheelEffects[i].PlayingAudio)
                {
                    wheelEffects[i].StopAudio();
                }
                // end the trail generation
                wheelEffects[i].EndSkidTrail();
            }
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (carDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        wheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case CarDriveType.RearWheelDrive:
                    wheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    wheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case CarDriveType.FrontWheelDrive:
                    wheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    wheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= slipLimit && _currentTorque >= 0)
            {
                _currentTorque -= 10 * tractionControl;
            }
            else
            {
                _currentTorque += 10 * tractionControl;
                if (_currentTorque > fullTorqueOverAllWheels)
                {
                    _currentTorque = fullTorqueOverAllWheels;
                }
            }
        }


        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (wheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
