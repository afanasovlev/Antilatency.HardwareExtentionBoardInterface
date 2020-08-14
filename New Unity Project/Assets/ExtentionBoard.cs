using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using Antilatency;
using Antilatency.DeviceNetwork;
using Antilatency.HardwareExtensionInterface;
using Antilatency.HardwareExtensionInterface.Interop;




// 
/*class Program
{
    static void Main(string[] args)
    {
        var example = new Example();
        while (true)
        {


        }
    }
}*/


class Example
{
    private Antilatency.DeviceNetwork.ILibrary _adnLibrary;
    private Antilatency.DeviceNetwork.INetwork _deviceNetwork;
    private Antilatency.DeviceNetwork.NodeHandle _nodeHandle;
    private Antilatency.Integration.DeviceNetwork Network;

    private Antilatency.HardwareExtensionInterface.ICotaskConstructor _cotaskConstructor;
    private Antilatency.HardwareExtensionInterface.ICotask _cotask;
    private Antilatency.HardwareExtensionInterface.IOutputPin outputPin1;
    private Antilatency.HardwareExtensionInterface.IOutputPin outputPin2;
    private Antilatency.HardwareExtensionInterface.IOutputPin outputPin5;
    private Antilatency.HardwareExtensionInterface.IOutputPin outputPin6;
    private Antilatency.HardwareExtensionInterface.ILibrary _library;

    protected virtual void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        if (Network == null)
        {
            Debug.LogError("Network is null");
            return;
        }

        _library = Antilatency.HardwareExtensionInterface.Library.load();


        if (_library == null)
        {
            Debug.LogError("Failed to create hardware extension interface library");
            return;
        }
    }

    public Example()
    {
        _adnLibrary = Antilatency.DeviceNetwork.Library.load();
        

        _nodeHandle = WaitForNode();
        _cotaskConstructor = _library.getCotaskConstructor();
        _deviceNetwork = _adnLibrary.createNetwork(new[] { new UsbDeviceType { vid = UsbVendorId.Antilatency, pid = 0x0000 } });
        _cotask = _cotaskConstructor.startTask(_deviceNetwork, _nodeHandle);



        outputPin1 = _cotask.createOutputPin(Pins.IO1, PinState.Low);
        outputPin2 = _cotask.createOutputPin(Pins.IO2, PinState.Low);
        outputPin5 = _cotask.createOutputPin(Pins.IO5, PinState.Low);
        outputPin6 = _cotask.createOutputPin(Pins.IO6, PinState.Low);
        

        _cotask.run();

        Update();

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                
                outputPin2.setState(PinState.High);
                outputPin6.setState(PinState.High);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                outputPin1.setState(PinState.High);
                outputPin5.setState(PinState.High);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                outputPin2.setState(PinState.High);
                outputPin5.setState(PinState.High);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                outputPin6.setState(PinState.High);
                outputPin1.setState(PinState.High);
            }
        }
    }

    public NodeHandle WaitForNode()
    {
        Console.WriteLine("Waiting for node...");

        var node = new NodeHandle();
        var networkUpdateId = 0u;
        do
        {
            //Every time any node is connected, disconnected or node status is changed, network update id is incremented.
            var updateId = _deviceNetwork.getUpdateId();
            if (networkUpdateId != updateId)
            {
                networkUpdateId = updateId;

                Console.WriteLine("Network update id has been incremented, searching for available node...");

                node = new NodeHandle();

                if (node == Antilatency.DeviceNetwork.NodeHandle.Null)
                {
                    Console.WriteLine("Tracking node not found.");
                }
            }
        } while (node == Antilatency.DeviceNetwork.NodeHandle.Null);

        Console.WriteLine("Tracking node found, serial number: " + _deviceNetwork.nodeGetStringProperty(node, Antilatency.DeviceNetwork.Interop.Constants.HardwareSerialNumberKey));

        return node;
    }
    public class ExtentionBoard : MonoBehaviour
    {

        void Start()
        {

        }


        
    }
}
