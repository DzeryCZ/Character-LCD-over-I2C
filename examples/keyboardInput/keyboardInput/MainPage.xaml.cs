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

namespace keyboardInput
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

        private displayI2C _lcd;



        private bool backLight = false;

        public MainPage()
        {
            this.InitializeComponent();
            this.start();
            Window.Current.CoreWindow.KeyDown += inputKeyDown;
        }

        private void start()
        {
            _lcd = new displayI2C(DEVICE_I2C_ADDRESS, I2C_CONTROLLER_NAME, RS, RW, EN, D4, D5, D6, D7, BL);
            _lcd.init();
            _lcd.prints("Hello,");
        }


        void inputKeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey.ToString()){
                case "Space": _lcd.prints(" ");
                    break;
                case "Enter": _lcd.gotoSecondLine();
                    break;
                case "Back": _lcd.clrscr();
                    break;
                case "Control":
                    if (this.backLight)
                    {
                        _lcd.turnOffBacklight();
                    }
                    else
                    {
                        _lcd.turnOnBacklight();
                    }
                    this.backLight = !this.backLight;
                    break;
                case "Shift": //do nothing
                case "Menu": //do nothing
                    break;
                case "Number1": _lcd.prints("1");
                    break;
                case "Number2": _lcd.prints("2");
                    break;
                case "Number3": _lcd.prints("3");
                    break;
                default: _lcd.prints(args.VirtualKey.ToString());
                    break;
            }
            
        }

    }
}
