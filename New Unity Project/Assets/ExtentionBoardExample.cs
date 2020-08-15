using System.Collections;
using System.Collections.Generic;
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

    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin1;
    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin2;
    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin5;
    public Antilatency.HardwareExtensionInterface.IOutputPin outputPin6;

    public Antilatency.HardwareExtensionInterface.IOutputPin[] oPins;
    protected INetwork GetNativeNetwork()
    {
        if (network == null)
        {
            Debug.LogError("Network is null??????");
            return null;
        }

        if (network.NativeNetwork == null)
        {
            Debug.LogError("Native network is null");
            return null;
        }

        return network.NativeNetwork;
    }


    void Update()
        {

        Console.Write("hi from awake");
        inetwork = GetNativeNetwork();

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


        float h = Input.GetAxis("Horizontal");
            float xPos = h * range;
            float moveSpeed = 3f;
            float turnSpeed = 100f;

            float g = Input.GetAxis("Vertical");
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
