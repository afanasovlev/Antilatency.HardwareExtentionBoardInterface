//version 5.09.2020 9:18
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
        public string SoketTag;
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
        public Antilatency.HardwareExtensionInterface.IPwmPin pwmPin7;
        public Antilatency.HardwareExtensionInterface.IPwmPin pwmPin8;
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
            // var nodeTr = GetFirstIdleTrackerNodeBySocketTag(SoketTag);
            var node = WaitForNode();

            cotask = cotaskConstructor.startTask(nw, node);

            if (cotask != null)
            {
                outputPin1 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO1, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
                outputPin2 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO2, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
                outputPin5 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO5, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
                outputPin6 = cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO6, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
                pwmPin7 = cotask.createPwmPin(Pins.IO7, 10000, 0.0f);
                pwmPin8 = cotask.createPwmPin(Pins.IO8, 10000, 0.0f);

                cotask.run();
            }

        }


        void Update()
        {
           
            float moveSpeed = 3f;
            float turnSpeed = 100f;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Forward();
                obj.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Back();
                obj.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Left();
                obj.transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                Right();
                obj.transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
            }

            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                Stop();
            }
        }

        private void Forward()
        {
            IncreaseSpeed();
            outputPin2.setState(PinState.High);
            outputPin6.setState(PinState.High);
        }
        private void Back()
        {
            IncreaseSpeed();
            outputPin1.setState(PinState.High);
            outputPin5.setState(PinState.High);
        }
        private void Left()
        {
            IncreaseSpeed();
            outputPin1.setState(PinState.High);
            outputPin6.setState(PinState.High);
        }
        private void Right()
        {
            IncreaseSpeed();
            outputPin2.setState(PinState.High);
            outputPin5.setState(PinState.High);
        }
        private void Stop()
        {
            outputPin1.setState(PinState.Low);
            outputPin2.setState(PinState.Low);
            outputPin5.setState(PinState.Low);
            outputPin6.setState(PinState.Low);
        }

        private void IncreaseSpeed()
        {
            float timeToIncrease = 0.2f;

            float startTime;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            //if (Input.GetButton("Horizontal"))
            {
                startTime = Time.time;
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)  && Time.time - startTime > timeToIncrease)
                //if (Input.GetButton("Horizontal") && Time.time - startTime > timeToIncrease)
                {
                    pwmPin7.setDuty(pwmPin7.getDuty() + 0.2f);
                    pwmPin8.setDuty(pwmPin8.getDuty() + 0.2f);
                    Debug.Log((Time.time - startTime).ToString("00:00.00"));
                }
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
        protected NodeHandle[] GetIdleNodesBySocketTag(string socketTag)
        {
            var nativeNetwork = GetNativeNetwork();

            if (nativeNetwork == null)
            {
                return new NodeHandle[0];
            }

            using (var cotaskConstructor = library.getCotaskConstructor())
            {

                var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                        nativeNetwork.nodeGetStringProperty(v, "Tag") == socketTag &&
                        nativeNetwork.nodeGetStatus(v) == NodeStatus.Idle
                        ).ToArray();

                return nodes;
            }
        }

        protected NodeHandle GetFirstIdleNodeBySocketTag(string socketTag)
        {
            var nodes = GetIdleTrackerNodesBySocketTag(socketTag);

            if (nodes.Length == 0)
            {
                return new NodeHandle();
            }

            return nodes[0];
        }
        public Antilatency.DeviceNetwork.NodeHandle WaitForNode()
        {
            Console.WriteLine("Waiting for tracking node...");

            var node = new NodeHandle();
            var networkUpdateId = 0u;
            do
            {
                inetwork = GetNativeNetwork();

                var updateId = inetwork.getUpdateId();
                if (networkUpdateId != updateId)
                {
                    networkUpdateId = updateId;

                    Console.WriteLine("Network update id has been incremented, searching for available tracking node...");

                    node = GetFirstIdleNodeBySocketTag(SoketTag);

                    if (node == Antilatency.DeviceNetwork.NodeHandle.Null)
                    {
                        Console.WriteLine("Tracking node not found.");
                    }
                }
            } while (node == Antilatency.DeviceNetwork.NodeHandle.Null);

            Console.WriteLine("Tracking node found, serial number: " + inetwork.nodeGetStringProperty(node, Antilatency.DeviceNetwork.Interop.Constants.HardwareSerialNumberKey));

            return node;
        }
    }


}
