using UnityEngine;

using System;
using Antilatency.HardwareExtensionInterface.Interop;
using Antilatency.HardwareExtensionInterface;
using System.Runtime.InteropServices;
using DLLTest;

namespace Antilatency.DeviceNetwork
{
    public class ExtentionBoardExample : MonoBehaviour
    {

        public Antilatency.HardwareExtensionInterface.IOutputPin[] oPins = new IOutputPin[4];

      
        public GameObject obj;
        public float range = 5f;

        private void Start()
        {


        }
        void Update()
        {
            
            var eBoard = new Board();
            oPins = eBoard.GetOutputPin();

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
        public Antilatency.HardwareExtensionInterface.ILibrary library;
        public Antilatency.HardwareExtensionInterface.ICotask cotask;
        public Antilatency.HardwareExtensionInterface.ICotaskConstructor cotaskConstructor;

        public Antilatency.Integration.DeviceNetwork network;
        public Antilatency.DeviceNetwork.ILibrary _library;
        public INetwork inetwork;
        public NodeHandle nodeHandle;


        public IOutputPin outputPin1;
        public IOutputPin outputPin2;
        public IOutputPin outputPin5;
        public IOutputPin outputPin6;



        public Board()
        {
          
          
            _library = Library.load();
            library = Antilatency.HardwareExtensionInterface.Library.load();

            if (_library == null)
            {
                Debug.LogError("Failed to load Antilatency Device Network library");
                return;
            }

            if (library == null)
            {
                throw new Exception("Hardware extention library is null");
            }

            _library.setLogLevel(LogLevel.Info);
            Console.WriteLine("Antilatency Device Network version: " + _library.getVersion());

            cotaskConstructor = library.getCotaskConstructor();

            inetwork = _library.createNetwork(new[] { new UsbDeviceType { vid = UsbVendorId.AntilatencyLegacy, pid = 0x3237 } });

            nodeHandle = new NodeHandle();
            cotask = cotaskConstructor.startTask(inetwork, nodeHandle);


        }

        public IOutputPin[] GetOutputPin()
        {
            IOutputPin[] oPins = new IOutputPin[4];
            oPins[0] = cotask.createOutputPin(Pins.IO1, PinState.Low);
            oPins[1] = cotask.createOutputPin(Pins.IO2, PinState.Low);
            oPins[2] = cotask.createOutputPin(Pins.IO5, PinState.Low);
            oPins[3] = cotask.createOutputPin(Pins.IO6, PinState.Low);

            cotask.run();

            return oPins;
        }
        public static class Library
        {
            [DllImport("AntilatencyDeviceNetwork")]
            private static extern Antilatency.InterfaceContract.ExceptionCode getLibraryInterface(System.IntPtr unloader, out System.IntPtr result);
            public static ILibrary load()
            {
                System.IntPtr libraryAsIInterfaceIntermediate;
                getLibraryInterface(System.IntPtr.Zero, out libraryAsIInterfaceIntermediate);
                Antilatency.InterfaceContract.IInterface libraryAsIInterface = new Antilatency.InterfaceContract.Details.IInterfaceWrapper(libraryAsIInterfaceIntermediate);
                var library = libraryAsIInterface.QueryInterface<ILibrary>();
                libraryAsIInterface.Dispose();
                return library;
            }
        }
    }
}

