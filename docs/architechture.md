# Archictechture

## Overview
So for this project architechture, it was seperated by two main segments.
- Hardware side.
- Client side.

And as for general concept :
```
Software WinForms <- Client (PC) <- USB-OTG (UART) -> ESP32-S3 Devvkit < - > Sensor, LED, Potentiometer
```

### Hardware side
Overal as for hardware side, the logic is quite to be like this. 2 DHT would be compared and 
the difference would be divisioned by ``2.0`` for the best result if those 2 sensor are 
available, if nor it will be one at the time that would be used to extract the data.
As for LED it was quite simple as is just following the potentiometer value to be the 
threshold and compared by the data value, and when it was above the threshold it will be 
``on``, otherwise it was ``off``.

for the pseudocode can be seen in the documents [harware_pseudocode](./hardware_pseudocode.md).