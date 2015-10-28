/**
 *  Character-LCD-over-I2C 
 *  ===================
 *  Connect HD44780 LCD character display to Windows 10 IoT devices via I2C and PCF8574
 *
 *  Author: Jaroslav Zivny
 *  Version: 1.1
 *  Keywords: Windows IoT, LCD, HD44780, PCF8574, I2C bus, Raspberry Pi 2
 *  Git: https://github.com/DzeryCZ/Character-LCD-over-I2C
**/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Gpio;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

using System.Threading.Tasks;

namespace displayI2C
{
    class displayI2C
    {

        private const byte LCD_WRITE = 0x07;

        private byte _D4;
        private byte _D5;
        private byte _D6;
        private byte _D7;
        private byte _En;
        private byte _Rw;
        private byte _Rs;
        private byte _Bl;

        private byte[] _LineAddress = new byte[] { 0x00, 0x40 };

        private byte _backLight = 0x01;

        private I2cDevice _i2cPortExpander;


        public displayI2C(byte deviceAddress, string controllerName, byte Rs, byte Rw, byte En, byte D4, byte D5, byte D6, byte D7, byte Bl, byte[] LineAddress) : this(deviceAddress, controllerName, Rs, Rw, En, D4, D5, D6, D7, Bl)
        {
            this._LineAddress = LineAddress;
        }


        public displayI2C(byte deviceAddress, string controllerName, byte Rs, byte Rw, byte En, byte D4, byte D5, byte D6, byte D7, byte Bl) 
        {
            // Configure pins
            this._Rs = Rs;
            this._Rw = Rw;
            this._En = En;
            this._D4 = D4;
            this._D5 = D5;
            this._D6 = D6;
            this._D7 = D7;
            this._Bl = Bl;

            // It's async method, so we have to wait
            Task.Run(() => this.startI2C(deviceAddress, controllerName)).Wait();
        }
               


        /**
        * Start I2C Communication
        **/
        public async void startI2C(byte deviceAddress, string controllerName)
        {
            try
            {
                var i2cSettings = new I2cConnectionSettings(deviceAddress);
                i2cSettings.BusSpeed = I2cBusSpeed.FastMode;
                string deviceSelector = I2cDevice.GetDeviceSelector(controllerName);
                var i2cDeviceControllers = await DeviceInformation.FindAllAsync(deviceSelector);
                this._i2cPortExpander = await I2cDevice.FromIdAsync(i2cDeviceControllers[0].Id, i2cSettings);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
                return;
            }
        }


        /**
        * Initialization
        **/
        public void init(bool turnOnDisplay = true, bool turnOnCursor = false, bool blinkCursor = false, bool cursorDirection = true, bool textShift = false)
        {
            //Task.Delay(100).Wait();
            //pulseEnable(Convert.ToByte((turnOnBacklight << 3)));

            /* Init sequence */
            Task.Delay(100).Wait();
            pulseEnable(Convert.ToByte((1 << this._D5) | (1 << this._D4)));
            Task.Delay(5).Wait();
            pulseEnable(Convert.ToByte((1 << this._D5) | (1 << this._D4)));
            Task.Delay(5).Wait();
            pulseEnable(Convert.ToByte((1 << this._D5) | (1 << this._D4)));

            /*  Init 4-bit mode */
            pulseEnable(Convert.ToByte((1 << this._D5)));

            /* Init 4-bit mode + 2 line */
            pulseEnable(Convert.ToByte((1 << this._D5)));
            pulseEnable(Convert.ToByte((1 << this._D7)));

            /* Turn on display, cursor */
            pulseEnable(0);
            pulseEnable(Convert.ToByte((1 << this._D7) | (Convert.ToByte(turnOnDisplay) << this._D6) | (Convert.ToByte(turnOnCursor) << this._D5) | (Convert.ToByte(blinkCursor) << this._D4)));

            this.clrscr();

            pulseEnable(0);
            pulseEnable(Convert.ToByte((1 << this._D6) | (Convert.ToByte(cursorDirection) << this._D5) | (Convert.ToByte(textShift) << this._D4)));
        }


        /**
        * Turn the backlight ON.
        **/
        public void turnOnBacklight()
        {
            this._backLight = 0x01;
            this.sendCommand(0x00);
        }


        /**
        * Turn the backlight OFF.
        **/
        public void turnOffBacklight()
        {
            this._backLight = 0x00;
            this.sendCommand(0x00);
        }


        /**
        * Can print string onto display
        **/
        public void prints(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                this.printc(text[i]);
            }
        }


        /**
        * Print single character onto display
        **/
        public void printc(char letter)
        {
            try {
                this.write(Convert.ToByte(letter), 1);
            }
            catch (Exception e)
            {

            }
        }


        /**
        * skip to second line
        **/
        public void gotoSecondLine()
        {
            this.sendCommand(0xc0);
        }


        /**
        * goto X and Y 
        **/
        public void gotoxy(byte x, byte y)
        {
            this.sendCommand( Convert.ToByte(x | _LineAddress[y] | (1 << LCD_WRITE)) );
        }


        /**
        * Send data to display
        **/
        public void sendData(byte data)
        {
            this.write(data, 1);
        }


        /**
        * Send command to display
        **/
        public void sendCommand(byte data)
        {
            this.write(data, 0);
        }


        /**
        * Clear display and set cursor at start of the first line
        **/
        public void clrscr()
        {
            pulseEnable(0);
            pulseEnable(Convert.ToByte((1 << this._D4)));
            Task.Delay(5).Wait();
        }


        /**
        * Send pure data to display
        **/
        public void write(byte data, byte Rs)
        {
            pulseEnable(Convert.ToByte((data & 0xf0) | (Rs << this._Rs)));
            pulseEnable(Convert.ToByte((data & 0x0f) << 4 | (Rs << this._Rs)));
            //Task.Delay(5).Wait(); //In case of problem with displaying wrong characters uncomment this part
        }


        /**
        * Create falling edge of "enable" pin to write data/inctruction to display
        */
        private void pulseEnable(byte data)
        {
            this._i2cPortExpander.Write(new byte[] { Convert.ToByte(data | (1 << this._En) | (this._backLight << this._Bl)) }); // Enable bit HIGH
            this._i2cPortExpander.Write(new byte[] { Convert.ToByte(data | (this._backLight << this._Bl)) }); // Enable bit LOW
            //Task.Delay(2).Wait(); //In case of problem with displaying wrong characters uncomment this part
        }


        /**
        * Save custom symbol to CGRAM
        **/
        public void createSymbol(byte[] data, byte address)
        {
            this.sendCommand( Convert.ToByte(0x40 | (address << 3)));
            for(var i = 0; i < data.Length; i++)
            {
                this.sendData(data[i]);
            }
            this.clrscr();
        }


        /**
        * Print custom symbol
        **/
        public void printSymbol(byte address)
        {
            this.sendData(address);
        }


    }
}
