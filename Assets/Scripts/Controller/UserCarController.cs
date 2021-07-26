using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;


namespace Controller
{
    [RequireComponent(typeof (CarMover))]
    public class UserCarController : MonoBehaviour, IController
    {
        private CarMover _mCar; // the car controller we want to use
        private void Awake()
        {
            // get the car controller
            _mCar = GetComponent<CarMover>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            _mCar.Move(h, v, v, handbrake, false);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
