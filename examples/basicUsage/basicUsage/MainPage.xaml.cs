using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace basicUsage
{
    using displayI2C;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //Setup address
        private const string I2C_CONTROLLER_NAME = "I2C1"; //use for RPI2
        private const byte DEVICE_I2C_ADDRESS = 0x27; // 7-bit I2C address of the port expander

        //Setup pins
        private const byte EN = 0x02;
        private const byte RW = 0x01;
        private const byte RS = 0x00;
        private const byte D4 = 0x04;
        private const byte D5 = 0x05;
        private const byte D6 = 0x06;
        private const byte D7 = 0x07;
        private const byte BL = 0x03;

        public MainPage()
        {
            this.InitializeComponent();
            this.start();
        }

        private void start()
        {
            // Here is I2C bus and Display itself initialized.
            //
            //  I2C bus is initialized by library constructor. There is also defined PCF8574 pins 
            //  Default `DEVICE_I2C_ADDRESS` is `0x27` (you can change it by A0-2 pins on PCF8574 - for more info please read datasheet)
            //  `I2C_CONTROLLER_NAME` for Raspberry Pi 2 is `"I2C1"`
            //  For Arduino it should be `"I2C5"`, but I did't test it.
            //  Other arguments should be: RS = 0, RW = 1, EN = 2, D4 = 4, D5 = 5, D6 = 6, D7 = 7, BL = 3
            //  But it depends on your PCF8574.
            displayI2C lcd = new displayI2C(DEVICE_I2C_ADDRESS, I2C_CONTROLLER_NAME, RS, RW, EN, D4, D5, D6, D7, BL);
            
            //Initialization of HD44780 display do by init method.
            //By arguments you can turnOnDisplay, turnOnCursor, blinkCursor, cursorDirection and textShift (in thius order)
            lcd.init();
            
            
            // Here is created new symbol
            // Take a look at data - it's smile emoticon
            // 0x00 => 00000
            // 0x00 => 00000
            // 0x0A => 01010
            // 0x00 => 00000
            // 0x11 => 10001
            // 0x0E => 01110
            // 0x00 => 00000
            // 0x00 => 00000 
            
                                        // data of symbol by lines                          //address of symbol
            lcd.createSymbol(new byte[] { 0x00, 0x00, 0x0A, 0x00, 0x11, 0x0E, 0x00, 0x00 }, 0x00);
            
            // Here is printed string
            lcd.prints("Good morning,");
            
            // Navigation to second line
            lcd.gotoxy(0, 1);
            
            // Here is printed string
            lcd.prints("gentlemans");
            
            // Here is printed our new symbol (emoticon)
            lcd.printSymbol(0x00);
            
        }
    }
}
