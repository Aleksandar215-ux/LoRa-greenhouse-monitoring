End-to-end system for remote monitoring of air temperature and humidity inside a greenhouse. A battery-powered sensor node transmits readings over a long-range LoRa link to a receiver in the house, which stores them in a MySQL database. A desktop application reads from that database and displays the measurements.

Built as a complete system — hardware, firmware, wireless link, database and application — rather than as an isolated exercise.
Why LoRa

The greenhouse sits well outside Wi-Fi range of the house, with no mains power at the sensor location. LoRa was chosen because it covers that distance at very low power consumption, requires no subscription or network infrastructure, and penetrates obstacles well at sub-GHz frequencies — where Wi-Fi would not reach and GSM would mean a recurring cost and higher current draw.

System architecture
┌────────────────────┐              ┌────────────────────┐
│   GREENHOUSE       │              │   HOUSE            │
│                    │    LoRa      │                    │
│   MCU + sensor     │ ───────────► │   MCU + LoRa       │
│   reads T and RH   │              │   receiver         │
│   sleeps between   │    metres    │   parses packet    │
│   readings         │              │                    │
└────────────────────┘              └─────────┬──────────┘
                                              │
                                              ▼
                                     ┌────────────────────┐
                                     │   MySQL database   │
                                     │   timestamped      │
                                     │   readings         │
                                     └─────────┬──────────┘
                                               │
                                               ▼
                                     ┌────────────────────┐
                                     │   Desktop app (C#) │
                                     │   view and track   │
                                     └────────────────────┘
Repository structure
transmitter/   Sensor node firmware — reads the sensor, sends over LoRa
receiver/      Receiver firmware — listens, parses, writes to the database
app/           Desktop application (C#) for viewing the measurements
docs/          Photos, wiring diagrams and screenshots
Hardware

Both ends are built around a microcontroller paired with a LoRa transceiver module. The sensor node adds a digital temperature and humidity sensor and runs on battery; the receiver is connected to a machine in the house, which handles the database write.


1 — Measurement. The sensor node wakes, reads temperature and relative humidity, and formats both values into a single plain-text message:

Temperature:25.67 °C , Humidity:52.33 %

The ASCII string format was a deliberate choice: readings are legible straight off a serial monitor, which made the radio link easy to debug during development, and it keeps the parsing logic on the receiving side simple.

2 — Transmission. The packet is sent over LoRa on a fixed frequency and sync word. The node then returns to low-power mode until the next interval, which is what makes battery operation practical.

3 — Reception. The receiver listens continuously and parses the incoming string, extracting the numeric temperature and humidity values from it before they are stored.

4 — Storage. Each reading is written to MySQL with a timestamp, giving a continuous history rather than only the current value.

5 — Display. The desktop application queries the database and presents the measurements to the user.

Database

Each received message is stored as its own row, with the database assigning the timestamp on insert:

sql
CREATE TABLE readings (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    message     VARCHAR(100),
    created_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

The received string is stored as-is in message, which keeps the receiver simple and preserves exactly what came over the air. The trade-off is described under Limitations below.

Desktop application
<!-- ![Application window](docs/app.png) --> <!-- FILL: 2-3 rečenice — u čemu je pisana (WinForms? WPF?), šta prikazuje -->
Building and running

Firmware (transmitter/ and receiver/)

Toolchain: Arduino IDE / PlatformIO -->
Libraries:  Spi,RadioLib , BME280 sensor library 

The LoRa frequency and sync word must match on both sides before flashing.

Application (app/)



Database credentials are set in 

Limitations and next steps
Point-to-point link. Moving to LoRaWAN with a gateway would add addressing, encryption and support for multiple nodes across the site.
No acknowledgements. Lost packets are currently not retransmitted; adding ACK and retry would make the link robust in poor conditions.
Text-based packets. The plain-text format is easy to read and debug, but it costs airtime and energy compared to sending the two values as raw bytes. Binary encoding would be the next optimisation for a battery-powered node.
Readings stored as text. Because the message is kept as a single string, the database cannot aggregate or chart the values directly. Parsing on insert into separate temperature and humidity FLOAT columns would allow averages, minima and maxima to be computed in SQL rather than in the application.
Raw logging only. The natural extension is anomaly detection on the node itself, so it reports when something is wrong rather than streaming every reading — the direction I took further in my master's thesis.
Credits:https://github.com/Xinyuan-LilyGO
Author

Aleksandar Popović — MSc Mechatronics, Faculty of Technical Sciences, University of Novi Sad

LinkedIn · apopovic915@gmail.com
