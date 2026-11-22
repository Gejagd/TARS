// LIBRARIES
#include <DHT.h>
#include <math.h>


// I/O AREAS
#define DHTPIN_1 15
#define DHTPIN_2 16

#define Potentiometer_1 6
#define Potentiometer_2 7

#define LEDPIN_1 5
#define LEDPIN_2 4

#define DHT_TYPE DHT22


// DECLARATION AREAS
DHT dht_1(DHTPIN_1, DHT_TYPE);
DHT dht_2(DHTPIN_2, DHT_TYPE);

// Sensor comparison tolerance
float temperature_tolerance;
float humidity_tolerance;


// Group sensor readings into one data structure
struct SensorData {
  float temperature;
  float humidity;
  bool valid;
};

// Individual sensor data
SensorData sensor_a;
SensorData sensor_b;


// FUNCTION AREAS
// Temperature reading
float tempRead(DHT &sensor) {
  float temperature = sensor.readTemperature();

  return temperature;
}

// Humidity reading
float humRead(DHT &sensor) {
  float humidity = sensor.readHumidity();

  return humidity;
}

// Validating logic
bool isValid(float temp, float hum) {
  if (isnan(temp) || isnan(hum)) {
    return false;
  }

  if (temp < -40 || temp > 80) {
    return false;
  }

  if (hum < 0 || hum > 100) {
    return false;
  }

  return true;
}

// Validation reading
void validRead(SensorData &sensor) {
  sensor.valid = isValid(
    sensor.temperature,
    sensor.humidity
  );
}

// Potentiometer reading
float potRead(int pin, float minValue, float maxValue) {
  int rawValue = analogRead(pin);

  float result = minValue + (rawValue / 4095.0) * (maxValue - minValue);

  return result;
}

// LED control
void LEDControl(
  float temperature, float humidity,
  bool sensorValid, float temperatureLimit,
  float humidityLimit) {

  if (!sensorValid) {
    digitalWrite(LEDPIN_1, LOW);
    digitalWrite(LEDPIN_2, LOW);
    return;
  }

  // Temperature LED
  if (temperature > temperatureLimit) {
      digitalWrite(LEDPIN_1, HIGH);
  } else {
    digitalWrite(LEDPIN_1, LOW);
  }

  // Humidity LED
  if (humidity > humidityLimit) {
    digitalWrite(LEDPIN_2, HIGH);
  } else {
    digitalWrite(LEDPIN_2, LOW);
  }
}

// Comparison logic
SensorData ComparisonSelection(
  const SensorData &sensor_a,
  const SensorData &sensor_b) {
  SensorData selected = {NAN, NAN, false};

  if (sensor_a.valid && sensor_b.valid) {
    float temperature_differences = fabsf(sensor_a.temperature - sensor_b.temperature);
    float humidity_differences = fabsf(sensor_a.humidity - sensor_b.humidity);

    if (temperature_differences <= temperature_tolerance &&
        humidity_differences <= humidity_tolerance) {
      selected.temperature = (sensor_a.temperature + sensor_b.temperature) / 2.0;
      selected.humidity = (sensor_a.humidity + sensor_b.humidity) / 2.0;
      selected.valid = true;
    } else {
      // Conflict: temporary priority to Sensor A
      selected = sensor_a;
    }
  } else if (sensor_a.valid) {
    selected = sensor_a;
  } else if (sensor_b.valid) {
    selected = sensor_b;
  }

  return selected;
}


// SETUP
void setup() {
  Serial.begin(115200);

  dht_1.begin();
  dht_2.begin();

  pinMode(Potentiometer_1, INPUT);
  pinMode(Potentiometer_2, INPUT);

  pinMode(LEDPIN_1, OUTPUT);
  pinMode(LEDPIN_2, OUTPUT);

  analogReadResolution(12);

  Serial.println("TARS Dual Sensor System Started");
}


// MAIN LOOP

void loop() {
  // SENSOR READING
  sensor_a.temperature = tempRead(dht_1);
  sensor_a.humidity = humRead(dht_1);

  sensor_b.temperature = tempRead(dht_2);
  sensor_b.humidity = humRead(dht_2);

  // SENSOR VALIDATION
  validRead(sensor_a);
  validRead(sensor_b);

  // POTENTIOMETER READING
  float temperatureLimit = potRead(Potentiometer_1, 15.0, 40.0);

  float humidityLimit = potRead(Potentiometer_2, 30.0, 90.0);

  // SENSOR COMPARISON
  SensorData finalData = ComparisonSelection(
    sensor_a,
    sensor_b
  );

  // LED CONTROL
  LEDControl(
    finalData.temperature,
    finalData.humidity,
    finalData.valid,
    temperatureLimit,
    humidityLimit
  );

  // SERIAL OUTPUT
  if (finalData.valid) {
    Serial.print(finalData.temperature);
    Serial.print(";");
    Serial.print(finalData.humidity);
    Serial.print(";");
    Serial.print(temperatureLimit);
    Serial.print(";");
    Serial.println(humidityLimit);
  } else {
    Serial.println("SENSOR FAILURE");
  }

  delay(1000);
}