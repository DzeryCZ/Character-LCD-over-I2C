Character LCD display over I2C 
=============================
Connect HD44780 LCD character display to Windows 10 IoT devices via I2C and PCF8574

    Author: Jaroslav Zivny
    Version: 1.1
    Keywords: Windows IoT, LCD, HD44780, PCF8574, I2C bus, Raspberry Pi 2
    Git: https://github.com/DzeryCZ/Character-LCD-over-I2C

Connect
==========

![Connection](https://cloud.githubusercontent.com/assets/4294781/9507211/d25680f4-4c4c-11e5-8aa1-9aada70caa25.jpg)


Basic usage (tl;dr)
===============

Initialization
--------------

I2C bus is initialized by library constructor. You have to define there also PCF8574 pins.


    displayI2C lcd = new displayI2C(DEVICE_I2C_ADDRESS, I2C_CONTROLLER_NAME, RS, RW, EN, D4, D5, D6, D7, BL);


Default `DEVICE_I2C_ADDRESS` is `0x27` (you can change it by A0-2 pins on PCF8574 - for more info please read datasheet)

`I2C_CONTROLLER_NAME` for Raspberry Pi 2 is `"I2C1"` (For Arduino it should be `"I2C5"`, but I did't test it.)

Other arguments are: RS = 0, RW = 1, EN = 2, D4 = 4, D5 = 5, D6 = 6, D7 = 7, BL = 3 (by default)

But the number of pins depends on your PCF8574.

------------------------------------------------------

Initialization of HD44780 display do by init method. [More info](#init)

    lcd.init();
    
Print string
-------------

Print string just by prints method e.g.

    lcd.prints("Good morning");
    


------------------------------------------------------

------------------------------------------------------


Methods
=========

init
----

Initialize HD44780 display

Arguments: 
* bool turnOnDisplay (default: true)
* bool turnOnCursor (default: false)
* bool blinkCursor (default: false)
* bool cursorDirection (default: true)
* bool textShift (default: false)



turnOnBacklight
---------------

Turn the Back light ON

No arguments 



turnOffBacklight
---------------

Turn the Back light OFF

No arguments 



prints
------

Print string to display

Arguments:

* string text



printc
------

Print char to display

Arguments:
* char letter



gotoSecondLine
--------------

Move cursor to start of the second line

No arguments



gotoxy
-------

Move cursor to X and Y coordinates

Arguments:
* byte x
* byte y


clrscr
------

Clear screen and move cursor to start

No arguments



createSymbol
------------

Create custom symbol

Arguments:
* byte[] data
* byte address



printSymbol
----------

Print custom symbol

Arguments:
* byte address


Changelog
==========

v 1.1
------
* faster rendering of characters

v 1.0 
------
* vanilla version


Contribute
===========

If you find mistakes, things that could be done better, feel free to contribute!
    
    
Copyright
=========

Copyright (©) 2015 Jaroslav Živný.

Distributed under the MIT License.
