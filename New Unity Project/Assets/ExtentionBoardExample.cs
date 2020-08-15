using UnityEngine;
using Antilatency.DeviceNetwork;
using System;
using Antilatency.HardwareExtensionInterface.Interop;
using Antilatency.HardwareExtensionInterface;

public class ExtentionBoardExample : MonoBehaviour
{
    public Antilatency.HardwareExtensionInterface.ILibrary library;
    public Antilatency.HardwareExtensionInterface.ICotask cotask;
    public Antilatency.HardwareExtensionInterface.ICotaskConstructor cotaskConstructor;

    public Antilatency.Integration.DeviceNetwork network;
    public Antilatency.DeviceNetwork.ILibrary dLibrary;
    public INetwork inetwork;
    public NodeHandle nodeHandle;

    public GameObject obj;
    public float range = 5f;

    public IOutputPin outputPin1;
    public IOutputPin outputPin2;
    public IOutputPin outputPin5;
    public IOutputPin outputPin6;

    public Antilatency.HardwareExtensionInterface.IOutputPin[] oPins = new IOutputPin[4];
   

    void Update()
        {

        inetwork = dLibrary.createNetwork(new[] { new UsbDeviceType { vid = UsbVendorId.Antilatency, pid = 0x0000 } });

        if (inetwork == null)
        {
            Debug.LogError("Network is null!!!!");
            return;
        }
        dLibrary = Antilatency.DeviceNetwork.Library.load();
        library = Antilatency.HardwareExtensionInterface.Library.load();

        if (library == null)
        {
            Debug.LogError("Failed to create hardware extension interface library");
            return;
        }

        nodeHandle = new NodeHandle();
        cotaskConstructor = library.getCotaskConstructor();


        cotask = cotaskConstructor.startTask(inetwork, nodeHandle);

        oPins[0] = cotask.createOutputPin(Pins.IO1, PinState.Low);
        oPins[1] = cotask.createOutputPin(Pins.IO2, PinState.Low);
        oPins[2] = cotask.createOutputPin(Pins.IO5, PinState.Low);
        oPins[3] = cotask.createOutputPin(Pins.IO6, PinState.Low);

        cotask.run();
                            
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
