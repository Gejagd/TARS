# Pseudocode of the hardware


```
ALGORITHM TARS_DUAL_SENSOR_SYSTEM

CONSTANT:
    DHT_PIN_1 = 15
    DHT_PIN_2 = 16

    POTENTIOMETER_PIN_1 = 6
    POTENTIOMETER_PIN_2 = 7

    LED_PIN_1 = 5
    LED_PIN_2 = 4

    DHT_TYPE = DHT22

VARIABLE:
    temperature_tolerance
    humidity_tolerance

STRUCTURE SensorData:
    temperature
    humidity
    valid

DECLARE:
    sensor_a AS SensorData
    sensor_b AS SensorData


FUNCTION tempRead(sensor):
    READ temperature FROM sensor
    RETURN temperature
END FUNCTION


FUNCTION humRead(sensor):
    READ humidity FROM sensor
    RETURN humidity
END FUNCTION


FUNCTION isValid(temp, hum):
    IF temp IS NOT A NUMBER OR hum IS NOT A NUMBER THEN
        RETURN FALSE
    END IF

    IF temp < -40 OR temp > 80 THEN
        RETURN FALSE
    END IF

    IF hum < 0 OR hum > 100 THEN
        RETURN FALSE
    END IF

    RETURN TRUE
END FUNCTION


FUNCTION validRead(sensor):
    sensor.valid = isValid(
        sensor.temperature,
        sensor.humidity
    )
END FUNCTION


FUNCTION potRead(pin, minValue, maxValue):
    READ rawValue FROM potentiometer

    CONVERT rawValue INTO result
        USING minValue AND maxValue

    RETURN result
END FUNCTION


FUNCTION LEDControl(
    temperature,
    humidity,
    sensorValid,
    temperatureLimit,
    humidityLimit
):
    IF sensorValid IS FALSE THEN
        TURN OFF LED_PIN_1
        TURN OFF LED_PIN_2
        RETURN
    END IF

    IF temperature > temperatureLimit THEN
        TURN ON LED_PIN_1
    ELSE
        TURN OFF LED_PIN_1
    END IF

    IF humidity > humidityLimit THEN
        TURN ON LED_PIN_2
    ELSE
        TURN OFF LED_PIN_2
    END IF
END FUNCTION


FUNCTION ComparisonSelection(sensor_a, sensor_b):

    INITIALIZE selected AS INVALID SensorData

    IF sensor_a.valid AND sensor_b.valid THEN

        CALCULATE temperatureDifference =
            ABS(sensor_a.temperature - sensor_b.temperature)

        CALCULATE humidityDifference =
            ABS(sensor_a.humidity - sensor_b.humidity)

        IF temperatureDifference <= temperature_tolerance
           AND humidityDifference <= humidity_tolerance THEN

            CALCULATE selected.temperature =
                (sensor_a.temperature + sensor_b.temperature) / 2

            CALCULATE selected.humidity =
                (sensor_a.humidity + sensor_b.humidity) / 2

            SET selected.valid = TRUE

        ELSE
            SELECT sensor_a AS PRIORITY SENSOR
        END IF

    ELSE IF sensor_a.valid THEN
        SELECT sensor_a

    ELSE IF sensor_b.valid THEN
        SELECT sensor_b

    END IF

    RETURN selected

END FUNCTION


PROCEDURE SETUP:

    INITIALIZE SERIAL COMMUNICATION AT 115200 BAUD

    INITIALIZE DHT SENSOR 1
    INITIALIZE DHT SENSOR 2

    SET POTENTIOMETER_PIN_1 AS INPUT
    SET POTENTIOMETER_PIN_2 AS INPUT

    SET LED_PIN_1 AS OUTPUT
    SET LED_PIN_2 AS OUTPUT

    SET ANALOG INPUT RESOLUTION TO 12 BITS

    DISPLAY "TARS Dual Sensor System Started"

END PROCEDURE


PROCEDURE MAIN_LOOP:

    REPEAT FOREVER:

        // 1. SENSOR READING
        sensor_a.temperature = tempRead(DHT_SENSOR_1)
        sensor_a.humidity = humRead(DHT_SENSOR_1)

        sensor_b.temperature = tempRead(DHT_SENSOR_2)
        sensor_b.humidity = humRead(DHT_SENSOR_2)


        // 2. SENSOR VALIDATION
        validRead(sensor_a)
        validRead(sensor_b)


        // 3. POTENTIOMETER READING
        temperatureLimit = potRead(
            POTENTIOMETER_PIN_1,
            15.0,
            40.0
        )

        humidityLimit = potRead(
            POTENTIOMETER_PIN_2,
            30.0,
            90.0
        )


        // 4. SENSOR COMPARISON
        finalData = ComparisonSelection(
            sensor_a,
            sensor_b
        )


        // 5. LED CONTROL
        LEDControl(
            finalData.temperature,
            finalData.humidity,
            finalData.valid,
            temperatureLimit,
            humidityLimit
        )


        // 6. SERIAL OUTPUT
        IF finalData.valid IS TRUE THEN

            DISPLAY finalData.temperature
            DISPLAY ";"

            DISPLAY finalData.humidity
            DISPLAY ";"

            DISPLAY temperatureLimit
            DISPLAY ";"

            DISPLAY humidityLimit

        ELSE
            DISPLAY "SENSOR FAILURE"
        END IF


        WAIT 1000 MILLISECONDS

    END REPEAT

END PROCEDURE


END ALGORITHM
```