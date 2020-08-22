using UnityEngine;
using Antilatency.DeviceNetwork;
using System;
using Antilatency.HardwareExtensionInterface.Interop;
using Antilatency.HardwareExtensionInterface;

public class ExtentionBoardExample : MonoBehaviour
{
    public GameObject obj;
    public float range = 5f;

    public Antilatency.HardwareExtensionInterface.IOutputPin[] oPins = new IOutputPin[4];

    void Start()
    {
       

       
    }
    void Update()
    {
        var eBoard = new Board();
        oPins[0] = eBoard.cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO1, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
        oPins[1] = eBoard.cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO2, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
        oPins[2] = eBoard.cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO5, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);
        oPins[3] = eBoard.cotask.createOutputPin(Antilatency.HardwareExtensionInterface.Interop.Pins.IO6, Antilatency.HardwareExtensionInterface.Interop.PinState.Low);

        eBoard.cotask.run();

        float moveSpeed = 3f;
        float turnSpeed = 100f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            obj.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            oPins[1].setState(PinState.High);
            oPins[3].setState(PinState.High);

        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            obj.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
            oPins[0].setState(PinState.High);
            oPins[2].setState(PinState.High);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            obj.transform.Rotate(Vector3.up * -turnSpeed * Time.deltaTime);
            oPins[1].setState(PinState.High);
            oPins[2].setState(PinState.High);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            obj.transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
            oPins[0].setState(PinState.High);
            oPins[3].setState(PinState.High);
        }
    }



}
public class Board
{
    public Antilatency.Integration.DeviceNetwork network;
    public Antilatency.DeviceNetwork.ILibrary dLibrary;
    public Antilatency.DeviceNetwork.INetwork inetwork;
    public Antilatency.DeviceNetwork.NodeHandle nodeHandle;

    public Antilatency.HardwareExtensionInterface.ILibrary library;
    public Antilatency.HardwareExtensionInterface.ICotask cotask;
    public Antilatency.HardwareExtensionInterface.ICotaskConstructor cotaskConstructor;

    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin1;
    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin2;
    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin5;
    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin6;
    
    public Board()
    {
        dLibrary = Antilatency.DeviceNetwork.Library.load();
        library = Antilatency.HardwareExtensionInterface.Library.load();

        if (library != null)
        {
            System.Console.WriteLine("HW Lib is ok ");
        }
        if (dLibrary != null)
        {
            System.Console.WriteLine("DN Lib is ok");
        }

        cotaskConstructor = library.getCotaskConstructor();

        inetwork = dLibrary.createNetwork(new[] { new UsbDeviceType { vid = UsbVendorId.Antilatency, pid = 0x0000 } });

        if (inetwork == null)
        {
            Debug.LogError("Network is null!!!!");
            return;
        }

        nodeHandle = new NodeHandle();

        cotask = cotaskConstructor.startTask(inetwork, nodeHandle);
    }

  
}
