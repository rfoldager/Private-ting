/*********************************************************
 * Netduino test
 * 
 * Rasmus Foldager Andersen
 * 15-9-2011
 * 
 * Specs:
 * 1: The LED should toggle on/off with 1 Hz frequency
 * 2: On the reception of a button even the toggle frequency
 * should increase to 10Hz.
 * 3: On the reception of another button event the frequency
 * should reverse to 1 Hz.
 * 
 * 4: Each time the button is pressed a character is
 * transmitted on the UART0 (Com1)
 * 5: The Baud rate should be set up to 115.2k, 8 bit char
 * size, Parity = none.
 * 6: PB30 (AD3 (GP_PIN_A3)) is used at direction pin.
 * RX = 0 (Low), TX = 1 (High) - should be set default to RX
 * When a character is transmitted TX direction is set.
 * When character has been sent it defalt to RX again.
 * 
 * *******************************************************/



using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.IO.Ports;
using System.Text;

namespace BlinkingLED
{
    public class Program
    {

        static int SleepTime = 1000; // Initial refresh rate of 1 sec.
        static OutputPort transmit = new OutputPort(Pins.GPIO_PIN_A3, false); // Direction pin. Set as Low per default
        static SerialPort serialPort = new SerialPort("COM2", 115200, Parity.None, 8, StopBits.One);
        
        public static void Main()
        {
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            InterruptPort button= new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);

            serialPort.Open();

            button.OnInterrupt += new NativeEventHandler(Button_OnInterrupt);

            while (true)
            {
                led.Write(true);
                Thread.Sleep(SleepTime);
                led.Write(false);
                Thread.Sleep(SleepTime);

            }
        }

        static void Button_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (data2 == 0)
            {
                SleepTime = 1000;
            }

            if (data2 == 1)
            {
                SleepTime = 100;
                Print("e");
            }
        }

        static void Print(string line)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            byte[] bytesToWrite = encoder.GetBytes(line);
            transmit.Write(true); // Set direction HIGH, send the input and reset the direction to LOW again.
            serialPort.Write(bytesToWrite, 0, bytesToWrite.Length);
            transmit.Write(false);
        }
       
    }
}
