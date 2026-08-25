#include "LoRaBoards.h"
#include <RadioLib.h>
#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#define SEALEVELPRESSURE_HPA (1013.25)
#if   defined(USING_SX1280)
#ifndef CONFIG_RADIO_FREQ
#define CONFIG_RADIO_FREQ           2400.0
#endif
#ifndef CONFIG_RADIO_OUTPUT_POWER
#define CONFIG_RADIO_OUTPUT_POWER   13
#endif
#ifndef CONFIG_RADIO_BW
#define CONFIG_RADIO_BW             203.125
#endif
SX1280 radio = new Module(RADIO_CS_PIN, RADIO_DIO1_PIN, RADIO_RST_PIN, RADIO_BUSY_PIN);
#endif
Adafruit_BME280 bme; // I2C komunikacija
unsigned long delayTime;
// čuva stanje prenosa između petlji
static int transmissionState = RADIOLIB_ERR_NONE;
//Zastavica koja ukazuje da je paket primljen.
static volatile bool transmittedFlag = false;
static uint32_t counter = 0;
static String payload;
String message = "Poruka je poslata";
// Ova funkcija se poziva kada je paket poslat sa modula
void setFlag(void)
{
    // šalje se paket, zastava se postavlja
    transmittedFlag = true;
}

void setup()
{
   Serial.begin(115200);
    while (!Serial);   
    Serial.println(F("BME280 test"));

    setupBoards();
    Wire.begin(46,42); // inicijalizacija SDA i SCL pinova
    unsigned status;
     status = bme.begin(0x76);
    if (!status) {
        Serial.println("Could not find a valid BME280 sensor, check wiring, address, sensor ID!");
        Serial.print("SensorID was: 0x"); Serial.println(bme.sensorID(), 16);
        Serial.print("        ID of 0xFF probably means a bad address, a BMP 180 or BMP 085\n");
        Serial.print("   ID of 0x56-0x58 represents a BMP 280,\n");
        Serial.print("        ID of 0x60 represents a BME 280.\n");
        Serial.print("        ID of 0x61 represents a BME 680.\n");
        if (u8g2) {
            u8g2->setFont(u8g2_font_ncenB08_tr);
            u8g2->clearBuffer();
            u8g2->setCursor(0, 16);
            u8g2->print("BME280 Could not find");
            u8g2->sendBuffer();
        }
        while (1) delay(10);
    }

    Serial.println("-- Default Test --");
    delayTime = 1000;

    Serial.println();


    setupBoards();

    
    delay(1500);

#ifdef  RADIO_TCXO_ENABLE
    pinMode(RADIO_TCXO_ENABLE, OUTPUT);
    digitalWrite(RADIO_TCXO_ENABLE, HIGH);
#endif

    // inicijalizacija radija podrazumevanim podešavanjima
    int state = radio.begin();

    printResult(state == RADIOLIB_ERR_NONE);

    Serial.print(F("Radio Initializing ... "));
    if (state == RADIOLIB_ERR_NONE) {
        Serial.println(F("success!"));
    } else {
        Serial.print(F("failed, code "));
        Serial.println(state);
        while (true);
    }

    //funkcija koja se postavlja kada je paket poslat
    radio.setPacketSentAction(setFlag);

  //Postavlja se noseća frekvencija
    if (radio.setFrequency(CONFIG_RADIO_FREQ) == RADIOLIB_ERR_INVALID_FREQUENCY) {
        Serial.println(F("Selected frequency is invalid for this module!"));
        while (true);
    }

  
    // Postavlja se propusni opseg
    if (radio.setBandwidth(CONFIG_RADIO_BW) == RADIOLIB_ERR_INVALID_BANDWIDTH) {
        Serial.println(F("Selected bandwidth is invalid for this module!"));
        while (true);
    }


//Postavlja se faktor širenja
    if (radio.setSpreadingFactor(12) == RADIOLIB_ERR_INVALID_SPREADING_FACTOR) {
        Serial.println(F("Selected spreading factor is invalid for this module!"));
        while (true);
    }

    
    // Postavlja se kodni odnos
    if (radio.setCodingRate(6) == RADIOLIB_ERR_INVALID_CODING_RATE) {
        Serial.println(F("Selected coding rate is invalid for this module!"));
        while (true);
    }

   // Postavlja se sinhronizovana reč
    if (radio.setSyncWord(0xAB) != RADIOLIB_ERR_NONE) {
        Serial.println(F("Unable to set sync word!"));
        while (true);
    }

    // Postavlja se snaga prenosa
    if (radio.setOutputPower(CONFIG_RADIO_OUTPUT_POWER) == RADIOLIB_ERR_INVALID_OUTPUT_POWER) {
        Serial.println(F("Selected output power is invalid for this module!"));
        while (true);
    }
   //Postavlja dužinu preambule za LoRa ili FSK modem
    if (radio.setPreambleLength(16) == RADIOLIB_ERR_INVALID_PREAMBLE_LENGTH) {
        Serial.println(F("Selected preamble length is invalid for this module!"));
        while (true);
    }
    // Omogućava ili onemogućava CRC proveru primljenih paketa.
    if (radio.setCRC(false) == RADIOLIB_ERR_INVALID_CRC_CONFIGURATION) {
        Serial.println(F("Selected CRC is invalid for this module!"));
        while (true);
    }
    // Šalje se prvi paket
    Serial.print(F("Radio Sending first packet ... "));
    transmissionState = radio.startTransmit(String(counter).c_str());
    delay(1000);
}
void loop()
{
    // provera da li je prethodni paket poslat
    if (transmittedFlag) {
        //slanje temperature i vlažnosti vazduha
         payload = String(counter++) + "," + "Temperature:" + bme.readTemperature() + "*C " + "Humidity:" +  bme.readHumidity() + " %" ;
        // resetovanje zastavice
        transmittedFlag = false;
        //pali se led svetlo
        flashLed();
        if (transmissionState == RADIOLIB_ERR_NONE) {
            // paket uspešno poslat
            Serial.println(F("transmission finished!"));
           
        } else {
            Serial.print(F("failed, code "));
            Serial.println(transmissionState);
        }
        // čeka sekundu pre nego što pošalje opet
        delay(1000);
        // slanje novog paketa
        Serial.print(F("Radio Sending another packet ... "));
        transmissionState = radio.startTransmit(payload);
        

    }
    printValues();
    delay(delayTime);
}
//Ispisivanje vrednosti temperature i vlažnosti vazduga u serijskom monitoru
void printValues()
{   
    Serial.print("Temperature = ");
    Serial.print(bme.readTemperature());
    Serial.println(" °C");
    Serial.print("Humidity = ");
    Serial.print(bme.readHumidity());
    Serial.println(" %");

    Serial.println();
}
    
