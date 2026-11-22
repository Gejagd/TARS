# Design of the Hardware.


## Overview
So for the hardware design i created was quiete much of consideration, like for safety reason 
and fail-safe mechanism. And it comes from much of thought like :
- "What if the sensor failed?'.
- "What if the current is failing the sensor itself?"
- "What if there is some unexpected shorting?"
- And so on.
With much of that questions already, it comes to this design. For the component, please read 
on the documents [Bill Of Materials](./bom.md).


## Design
### Electrical Base Design
```
VCC -> CAP 470uF & 47uf -> DHT22 * 2, Potentiometer 10K * 2 -> 3 pin GND.
```
Why did i use all the 3 GND pinout? If you may ask me. It is because on of my assumption of 
"What if one GND pinout can't handle it all?" and that is it.

### Pinout Base Design
```
D4 -> RES 330ohm -> LED (1) Yellow -> GND.

D5 -> RES 330ohm -> LED (2) Green -> GND.

D6 <- Potentiometer (1).

D7 <- Potentiometer (2).

D15 <- RES 1K (Pull Up, VCC) <- DHT22 (2).

D16 <- DHT22 (1).
```