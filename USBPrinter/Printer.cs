using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;


namespace USBPrinter
{
    class Printer
    {

        public static UsbDevice MyUsbDevice;


        public static UsbDeviceFinder MyUsbFinder =
            new UsbDeviceFinder(0x6868, 0x0600);
        public void Print(String print)
        {
            //t();
            ErrorCode ec = ErrorCode.None;
            try
            {
                // Find and open the usb device.
                MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

                // If the device is open and ready
                if (MyUsbDevice == null) throw new Exception("Device Not Found.");

                // If this is a "whole" usb device (libusb-win32, linux libusb)
                // it will have an IUsbDevice interface. If not (WinUSB) the
                // variable will be null indicating this is an interface of a
                // device.
                IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used,
                    // the desired configuration and interface must be selected.

                    // Select config
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface
                    wholeUsbDevice.ClaimInterface(1);
                }
                int count = 0;
                byte[] data = new byte[256];


                //var controlPacket = new UsbSetupPacket();
                //controlPacket.Request = 0xA0;
                //controlPacket.RequestType = 64;
                //controlPacket.Value =  0x07F92;
                //controlPacket.Index = 0;

                //int written;

                // MyUsbDevice.ControlTransfer(ref controlPacket, data, data.Length, out written);


                UsbEndpointWriter writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep03);
                int bytesWritten;
                writer.Write(new byte[] { 0x1B, 0x40 }, 2000, out bytesWritten);

                //ec = writer.Write(new byte[] { 0x1D, 0x68, 0x70 }, 2000, out bytesWritten);
                //ec = writer.Write(new byte[] { 0x1D, 0x77, 0x02, 0x1D, 0x48, 0x03, 0x1D, 0x6B, 0x06 }, 2000, out bytesWritten);
                //ec = writer.Write(Encoding.Default.GetBytes("A1234567890A"), 2000, out bytesWritten); // надпись штрихкода
                //ec = writer.Write(new byte[] { 0x00, 0x56, 0x30 }, 2000, out bytesWritten);
                //ec = writer.Write(new byte[] { 0x09 }, 2000, out bytesWritten); ec = writer.Write(new byte[] { 0x09 }, 2000, out bytesWritten);


                ec = writer.Write(Encoding.Default.GetBytes(print + "\x0A"), 2000, out bytesWritten);

                

                //ec = writer.Write(new byte[] { 0x09 }, 2000, out bytesWritten);
                if (ec != ErrorCode.None)
                {
                }


                UsbSetupPacket packet = new UsbSetupPacket((byte)UsbRequestType.TypeVendor, (byte)1, (short)0, 0, 0);
                int temp1;
                byte[] temp2 = Encoding.Default.GetBytes("Hello");
                MyUsbDevice.ControlTransfer(ref packet, temp2, 0, out temp1);


                // open read endpoint
                using (var write = wholeUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep04, EndpointType.Interrupt))
                {
                    write.Write(new byte[] { 0x10, 0x04, 0x01 }, 2000, out count);
                }
                //using(var read = wholeUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01, 64, EndpointType.Bulk))
                //      {
                //          read.Read(data, 50, out count);
                //      }

                Console.WriteLine("\r\nDone!\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
            }
            finally
            {
                if (MyUsbDevice != null)
                {
                    if (MyUsbDevice.IsOpen)
                    {
                        // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                        // it exposes an IUsbDevice interface. If not (WinUSB) the
                        // 'wholeUsbDevice' variable will be null indicating this is
                        // an interface of a device; it does not require or support
                        // configuration and interface selection.
                        IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null))
                        {
                            // Release interface
                            wholeUsbDevice.ReleaseInterface(0);
                        }

                        MyUsbDevice.Close();
                    }
                    MyUsbDevice = null;

                    // Free usb resources
                    UsbDevice.Exit();

                }

                // Wait for user input..
           //     Console.ReadKey();
            }
        }


        private static void t()
        {
            int VendorID = 0x6868;
            int ProductID = 0x0600;

            UsbDevice usbDevice = null;

            UsbRegDeviceList allDevices = UsbDevice.AllDevices;

            Console.WriteLine("Found {0} devices", allDevices.Count);

            foreach (UsbRegistry usbRegistry in allDevices)
            {
                Console.WriteLine("Got device: {0}\r\n", usbRegistry.FullName);

                if (usbRegistry.Open(out usbDevice))
                {
                    Console.WriteLine("Device Information\r\n------------------");

                    Console.WriteLine("{0}", usbDevice.Info.ToString());

                    Console.WriteLine("VID & PID: {0} {1}", usbDevice.Info.Descriptor.VendorID, usbDevice.Info.Descriptor.ProductID);

                    Console.WriteLine("\r\nDevice configuration\r\n--------------------");
                    foreach (UsbConfigInfo usbConfigInfo in usbDevice.Configs)
                    {
                        Console.WriteLine("{0}", usbConfigInfo.ToString());

                        Console.WriteLine("\r\nDevice interface list\r\n---------------------");
                        IReadOnlyCollection<UsbInterfaceInfo> interfaceList = usbConfigInfo.InterfaceInfoList;
                        foreach (UsbInterfaceInfo usbInterfaceInfo in interfaceList)
                        {
                            Console.WriteLine("{0}", usbInterfaceInfo.ToString());

                            Console.WriteLine("\r\nDevice endpoint list\r\n--------------------");
                            IReadOnlyCollection<UsbEndpointInfo> endpointList = usbInterfaceInfo.EndpointInfoList;
                            foreach (UsbEndpointInfo usbEndpointInfo in endpointList)
                            {
                                Console.WriteLine("{0}", usbEndpointInfo.ToString());
                            }
                        }
                    }
                    usbDevice.Close();
                }
                Console.WriteLine("\r\n----- Device information finished -----\r\n");
            }





            Console.WriteLine("Trying to find our device: {0} {1}", VendorID, ProductID);
            UsbDeviceFinder usbDeviceFinder = new UsbDeviceFinder(VendorID, ProductID);

            // This does not work !!! WHY ?
            usbDevice = UsbDevice.OpenUsbDevice(usbDeviceFinder);

            if (usbDevice != null)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("FAIL");
            }

            UsbDevice.Exit();

            Console.Write("Press anything to close");
            Console.ReadKey();
        }





    }
}
