using System;
using UnityEngine;
using Antilatency.HardwareExtensionInterface.Interop;
using Antilatency.HardwareExtensionInterface;
using Antilatency.DeviceNetwork;

using UnityEngine.Events;



namespace Antilatency.Integration
{
    public class ExtentionBoardExample : MonoBehaviour
    {
        
        public DeviceNetwork Network;
        public Antilatency.DeviceNetwork.ILibrary dLibrary;
        public Antilatency.DeviceNetwork.INetwork inetwork;
        public Antilatency.DeviceNetwork.NodeHandle node;

        public Antilatency.HardwareExtensionInterface.ILibrary library;
        public Antilatency.HardwareExtensionInterface.ICotask cotask;
        public Antilatency.HardwareExtensionInterface.ICotaskConstructor cotaskConstructor;

        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin1;
        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin2;
        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin5;
        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin6;
        public GameObject obj;
        public float range = 5f;

        private void Awake()
        {
            init();
        }

        private void init()
        {
            node = new NodeHandle(); 

            if (Network == null)
            {
                Debug.LogError("Network is null, from Init");
                return;
            }
            dLibrary = Antilatency.DeviceNetwork.Library.load();
            library = Antilatency.HardwareExtensionInterface.Library.load();

            if (library == null)
            {
                Debug.LogError("HW Lib is null");
            }
            if (dLibrary == null)
            {
                Debug.LogError("DN Lib is null");
            }
            dLibrary.setLogLevel(LogLevel.Info);

            cotaskConstructor = library.getCotaskConstructor();

          
            var nw = GetNativeNetwork();
            cotask = cotaskConstructor.startTask(nw, new NodeHandle()); 

        }


        private void Start()
        {
       
            outputPin1 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO1, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
            outputPin2 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO2, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
            outputPin5 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO5, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
            outputPin6 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO6, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);

            cotask.run();
        }
        void Update()
        {

            float moveSpeed = 3f;
            float turnSpeed = 100f;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                obj.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                outputPin2.setState(PinState.High);
                outputPin6.setState(PinState.High);

            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                obj.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
                outputPin1.setState(PinState.High);
                outputPin5.setState(PinState.High);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                obj.transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
                outputPin2.setState(PinState.High);
                outputPin5.setState(PinState.High);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                obj.transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
                outputPin1.setState(PinState.High);
                outputPin6.setState(PinState.High);
            }
        }
        public INetwork GetNativeNetwork()
        {
            if (Network == null)
            {
                Debug.LogError("Network is null");
                return null;
            }

            if (Network.NativeNetwork == null)
            {
                Debug.LogError("Native network is null");
                return null;
            }

            return Network.NativeNetwork;
        }
    }
    
}
