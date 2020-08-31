using System;
using UnityEngine;
using Antilatency.HardwareExtensionInterface.Interop;
using Antilatency.HardwareExtensionInterface;
using Antilatency.DeviceNetwork;

using UnityEngine.Events;
using System.Linq;
using System.Collections;

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
        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin3;
        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin4;
        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin7;
        public Antilatency.HardwareExtensionInterface.IOutputPin outputPin8;
        public GameObject obj;
        public float range = 5f;
        public class BoolEvent : UnityEvent<bool> { }

       
       
       

        protected Alt.Tracking.ILibrary _trackingLibrary;
        protected UnityEngine.Pose _placement;
        private Alt.Tracking.ITrackingCotask _trackingCotask;
        protected NodeHandle _trackingNode;
        private void Awake()
        {
            init();
        }

        private void init()
        {
            //node = new NodeHandle(); 

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
            node = GetFirstIdleHardwareExtensionInterfaceNode();
            cotask = cotaskConstructor.startTask(nw, node);
            if (cotask != null)
            {
                outputPin1 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO1, Antilatency.HardwareExtensionInterface.Interop.PinState.High);
                outputPin2 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO2, Antilatency.HardwareExtensionInterface.Interop.PinState.High);
                outputPin5 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO5, Antilatency.HardwareExtensionInterface.Interop.PinState.High);
                outputPin6 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO6, Antilatency.HardwareExtensionInterface.Interop.PinState.High);
               outputPin3 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IOA3, Antilatency.HardwareExtensionInterface.Interop.PinState.High);
                outputPin4 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IOA4, Antilatency.HardwareExtensionInterface.Interop.PinState.High);
                outputPin7 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO7, Antilatency.HardwareExtensionInterface.Interop.PinState.High);
               outputPin8 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO8, Antilatency.HardwareExtensionInterface.Interop.PinState.High);

                cotask.run();
            }
            
        }


        void Update()
        {
            outputPin1.setState(PinState.Low);
            outputPin2.setState(PinState.Low);
            outputPin5.setState(PinState.Low);
            outputPin6.setState(PinState.Low);
            float moveSpeed = 3f;
            float turnSpeed = 100f;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                
                outputPin2.setState(PinState.High);
                //outputPin6.setState(PinState.High);
                obj.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                
                outputPin1.setState(PinState.High);
                //outputPin5.setState(PinState.High);
                obj.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                
                outputPin2.setState(PinState.High);
                //outputPin5.setState(PinState.High);
                obj.transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                
                outputPin1.setState(PinState.High);
                //outputPin6.setState(PinState.High);
                obj.transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
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
       
       

        
        
        protected NodeHandle GetFirstIdleHardwareExtensionInterfaceNode()
        {
            var nodes = GetFirstIdleHardwareExtensionInterfaceNodes();
            if (nodes.Length == 0)
            {
                return new NodeHandle();
            }
            return nodes[0];
        }
        protected NodeHandle[] GetFirstIdleHardwareExtensionInterfaceNodes()
        {
            var nativeNetwork = GetNativeNetwork();

            if (nativeNetwork == null)
            {
                return new NodeHandle[0];
            }

            using (var cotaskConstructor = library.getCotaskConstructor())
            {
                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetStatus(v) == NodeStatus.Idle
                    ).ToArray();

                return nodes;
            }
        }









       
        public IEnumerator Test(IOutputPin output1, IOutputPin output2)
        {

            

            Debug.Log("Output");
            output1.setState(PinState.High);
            output2.setState(PinState.High);

            yield return new WaitForSeconds(0.1f);
        }
    }


}