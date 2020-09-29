//version 29.09.2020 10:23
// add slow turn right and left
using System;
using System.Linq;
using Antilatency.DeviceNetwork;
using Antilatency.HardwareExtensionInterface;
using Antilatency.HardwareExtensionInterface.Interop;
using UnityEngine;

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
                Controller.lBack = cotask.createPwmPin(Pins.IO1, 10000, 0.0f);
                Controller.lForw = cotask.createPwmPin(Pins.IO2, 10000, 0.0f);
                Controller.rBack = cotask.createPwmPin(Pins.IO5, 10000, 0.0f);
                Controller.rForw = cotask.createPwmPin(Pins.IO6, 10000, 0.0f);
                
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

            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                Controller.FLeft();
            }

            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                Controller.FRight();
            }

            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
                Controller.BLeft();
            }

            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
            {
                Controller.BRight();
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
        public static Antilatency.HardwareExtensionInterface.IPwmPin lBack; //pwm1
        public static Antilatency.HardwareExtensionInterface.IPwmPin lForw; //pwn2
        public static Antilatency.HardwareExtensionInterface.IPwmPin rBack; //pwm5
        public static Antilatency.HardwareExtensionInterface.IPwmPin rForw; //pwm6

        public static void Forward()
        {
            IncreaseSpeed(lForw, rForw, 0.7f, 0.86f);
        }
        public static void Back()
        {
            IncreaseSpeed(lBack, rBack, 0.7f, 0.84f);
        }
        public static void Left()
        {
            IncreaseSpeed(lBack, rForw, 0.8f, 0.75f);
        }
        public static void Right()
        {
            IncreaseSpeed(lForw, rBack, 0.8f, 0.75f);
        }
        public static void FRight()
        {
            IncreaseSpeed(lForw, rForw, 1.0f, 0.6f);
        }
        public static void FLeft()
        {
            IncreaseSpeed(lForw, rForw, 0.6f, 1.0f);
        }

        public static void BRight()
        {
            IncreaseSpeed(lBack, rBack, 0.6f, 1.0f);
        }
        public static void BLeft()
        {
            IncreaseSpeed(lBack, rBack, 1.0f, 0.6f);
        }

        public static void Stop()
        {
            lBack.setDuty(0.0f);
            lForw.setDuty(0.0f);
            rBack.setDuty(0.0f);
            rForw.setDuty(0.0f);
        }

        public static void IncreaseSpeed(IPwmPin pin1, IPwmPin pin2, float one, float two)
        {
            pin1.setDuty(one);
            pin2.setDuty(two);
        }

    }
}
