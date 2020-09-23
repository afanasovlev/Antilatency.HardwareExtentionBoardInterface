//version 31.08.2020 23 30
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
        private Antilatency.DeviceNetwork.ILibrary dLibrary;
        private Antilatency.DeviceNetwork.INetwork inetwork;
        private Antilatency.DeviceNetwork.NodeHandle node;

        private Antilatency.HardwareExtensionInterface.ILibrary library;
        private Antilatency.HardwareExtensionInterface.ICotask cotask;
        private Antilatency.HardwareExtensionInterface.ICotaskConstructor cotaskConstructor;

       
        public GameObject obj;
        public float range = 5f;


        private void Awake()
        {
            init();
        }

        private void init()
        {

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

            var node = WaitForNode();

            cotask = cotaskConstructor.startTask(nw, node);

            if (cotask != null)
            {
                Controller.pwmPin1 = cotask.createPwmPin(Pins.IO1, 10000, 0.0f);
                Controller.pwmPin2 = cotask.createPwmPin(Pins.IO2, 10000, 0.0f);
                Controller.pwmPin5 = cotask.createPwmPin(Pins.IO5, 10000, 0.0f);
                Controller.pwmPin6 = cotask.createPwmPin(Pins.IO6, 10000, 0.0f);
                
                cotask.run();
            }
        }


        void Update()
        {

            float moveSpeed = 3f;
            float turnSpeed = 100f;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Controller.Forward();
                obj.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Controller.Back();
                obj.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Controller.Left();
                obj.transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                Controller.Right();
                obj.transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
            }

            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                Controller.Stop();
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
            var nodes = GetIdleNodesBySocketTag(socketTag);

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

    public class Controller
    {
        public static Antilatency.HardwareExtensionInterface.IPwmPin pwmPin1;
        public static Antilatency.HardwareExtensionInterface.IPwmPin pwmPin2;
        public static Antilatency.HardwareExtensionInterface.IPwmPin pwmPin5;
        public static Antilatency.HardwareExtensionInterface.IPwmPin pwmPin6;

        public static void Forward()
        {
            IncreaseSpeed(pwmPin2, pwmPin6, 0.7f, 0.86f);
        }
        public static void Back()
        {
            IncreaseSpeed(pwmPin1, pwmPin5, 0.7f, 0.84f);
        }
        public static void Left()
        {
            IncreaseSpeed(pwmPin1, pwmPin6, 0.8f, 0.75f);
        }
        public static void Right()
        {
            IncreaseSpeed(pwmPin2, pwmPin5, 0.8f, 0.75f);
        }
        public static void Stop()
        {
            pwmPin1.setDuty(0.0f);
            pwmPin2.setDuty(0.0f);
            pwmPin5.setDuty(0.0f);
            pwmPin6.setDuty(0.0f);
        }

        public static void IncreaseSpeed(IPwmPin pin1, IPwmPin pin2, float one, float two)
        {
            pin1.setDuty(one);
            pin2.setDuty(two);
        }

    }
}
