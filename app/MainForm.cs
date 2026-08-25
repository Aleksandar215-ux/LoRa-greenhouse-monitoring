using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using MindFusion.Charting;
using Brush = MindFusion.Drawing.Brush;
using SolidBrush = MindFusion.Drawing.SolidBrush;
using MySql.Data.MySqlClient;
using MindFusion.Spreadsheet.Charts;



namespace DateTimeSeries
{
    public partial class MainForm : Form
    {
        Timer myTimer = new Timer();

        MyDateTimeSeries series1, series2;

        public MainForm()
        {
            InitializeComponent();

            // Kreiraj uzorak podataka
            ObservableCollection<MindFusion.Charting.Series> data = new ObservableCollection<MindFusion.Charting.Series>();

            lineChart.LicenseKey = "license key stays here";


            // Kreiranje dve serije: jednu za temperaturu, drugu za vlažnost
            series1 = new MyDateTimeSeries(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series1.DateTimeFormat = DateTimeFormat.LongTime;
            // series1.DateTimeFormat = DateTimeFormat.CustomDateTime;
            // series1.CustomDateTimeFormat = "mm:ss"; 
            series1.LabelInterval = 10;
            series1.MinValue = 0;
            series1.MaxValue = 100;  // Postavljanje odgovarajući opseg za temperaturu
            series1.Title = "Temperature (°C)";
            series1.SupportedLabels = LabelKinds.ToolTip;

            series2 = new MyDateTimeSeries(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series2.DateTimeFormat = DateTimeFormat.LongTime;
            series2.LabelInterval = 10;
            series2.MinValue = 0;
            series2.MaxValue = 100;  // Postavljanje odgovarajući opseg za vlažnost
            series2.Title = "Humidity (%)";
            series2.SupportedLabels = LabelKinds.XAxisLabel | LabelKinds.ToolTip;

            // Postavljanje dijagrama
            lineChart.Series.Add(series1);
            lineChart.Series.Add(series2);
            lineChart.Title = "Real-time Temperature and Humidity Data";
            lineChart.ShowXCoordinates = false; // Prikazivanje X koordinata
            lineChart.ShowLegendTitle = false;
            lineChart.LayoutPanel.Margin = new Margins(0, 0, 20, 0);

            lineChart.XAxis.Title = "Time"; // Postavljanje naziva X ose
            lineChart.XAxis.MinValue = 0; // Postavljanje opsega X ose
            lineChart.XAxis.MaxValue = 120;
            lineChart.XAxis.Interval = 1;

            lineChart.YAxis.MinValue = 0;
            lineChart.YAxis.MaxValue = 100;
            lineChart.YAxis.Interval = 10;
            lineChart.YAxis.Title = "Values";

            List<Brush> brushes = new List<Brush>()
            {
                new SolidBrush(Color.Red),
                new SolidBrush(Color.SeaGreen)
            };

            List<double> thicknesses = new List<double>() { 2 };

            PerSeriesStyle style = new PerSeriesStyle(brushes, brushes, thicknesses, null);
            lineChart.Plot.SeriesStyle = style;
            lineChart.Theme.PlotBackground = new SolidBrush(Color.White);
            lineChart.Theme.GridLineColor = Color.LightGray;
            lineChart.Theme.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            lineChart.TitleMargin = new Margins(10);
            lineChart.GridType = GridType.Horizontal;

            myTimer.Tick += new EventHandler(TimerEventProcessor);

            // Postavljanje intervala
            myTimer.Interval = 1000;
            myTimer.Start();
        }

        // Izvlačenje temperature i vlažnosti vazduha iz baze
        private (DateTime timestamp, double temperature, double humidity) FetchData()
        {
            string connectionString = "Server=localhost;Database=database_esp32;Uid=esp32;Pwd=microcontrollerslab@123;";
            DateTime timestamp = DateTime.MinValue;
            double temperature = 0, humidity = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT timestamp, message FROM temperatura_table ORDER BY timestamp DESC LIMIT 1";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        timestamp = reader.GetDateTime("timestamp");
                        string message = reader.GetString("message");
                        temperature = ExtractTemperatureFromMessage(message);
                        humidity = ExtractHumidityFromMessage(message);
                    }
                }
            }

            return (timestamp, temperature, humidity);
        }


        private double ExtractTemperatureFromMessage(string message)
        {
            try
            {
                Console.WriteLine($"Received message: {message}");  // Ispis poruke za debagovanje

                // Proveravanje da li poruka sadrži ključnu reč "Temperature:" i znak "*"
                if (string.IsNullOrEmpty(message) || !message.Contains("Temperature:") || !message.Contains("*C"))
                {
                    throw new FormatException("Temperature key or unit not found in the message.");
                }

                // Ekstrakcija temperature - prilagođeno za znak '*' umesto '°'
                int startIndex = message.IndexOf("Temperature:") + "Temperature:".Length;
                int endIndex = message.IndexOf("*C", startIndex);  // Zamena'°' sa '*C'

                if (endIndex == -1) endIndex = message.Length;  // Ako nema '*C', uzimamo do kraja

                string tempSubstring = message.Substring(startIndex, endIndex - startIndex).Trim();

                // Provera da li je parsiranje uspešno
                if (double.TryParse(tempSubstring, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out double temperature))
                {
                    // Provera da li temperatura nije prevelika
                    if (temperature < -100 || temperature > 100)  // Definišite odgovarajući opseg za temperaturu
                    {
                        throw new OverflowException("Temperature value is out of valid range.");
                    }
                    return temperature;
                }
                else
                {
                    throw new FormatException("Failed to parse temperature value.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing temperature: {ex.Message}");
                return -1;  // Signalna vrednost za grešku
            }
        }



        private double ExtractHumidityFromMessage(string message)
        {
            try
            {
                Console.WriteLine($"Received message: {message}");  // Ispis poruke za debagovanje

                if (string.IsNullOrEmpty(message) || !message.Contains("Humidity:") || !message.Contains("%"))
                {
                    throw new FormatException("Humidity key or unit not found in the message.");
                }

                int startIndex = message.IndexOf("Humidity:") + "Humidity:".Length;
                int endIndex = message.IndexOf("%", startIndex);
                string humSubstring = message.Substring(startIndex, endIndex - startIndex).Trim();

                // Provera da li je parsiranje uspešno
                if (double.TryParse(humSubstring, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out double humidity))
                {
                    // Provera da li je vlažnost u odgovarajućem opsegu
                    if (humidity < 0 || humidity > 100)  // Definišite opseg za vlažnost
                    {
                        throw new OverflowException("Humidity value is out of valid range.");
                    }
                    return humidity;
                }
                else
                {
                    throw new FormatException("Failed to parse humidity value.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing humidity: {ex.Message}");
                return -1;  // Signalna vrednost za grešku
            }
        }

        private void btnRealTimeMode_Click(object sender, EventArgs e)
        {
            // Zaustavljanje timer pre nego što očistite serije

            myTimer.Stop();


            Console.WriteLine("Switched to Real-Time Mode.");


            // Čiščenje postojećih podatke na grafiku
            // Kreiranje dve serije: jednu za temperaturu, drugu za vlažnost
            lineChart.Series.Clear();
            series1 = new MyDateTimeSeries(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series1.DateTimeFormat = DateTimeFormat.LongTime;
            // series1.DateTimeFormat = DateTimeFormat.CustomDateTime;
            // series1.CustomDateTimeFormat = "mm:ss"; 
            series1.LabelInterval = 10;
            series1.MinValue = 0;
            series1.MaxValue = 100;  // Postavljanje odgovarajućeg opsega za temperaturu
            series1.Title = "Temperature (°C)";
            series1.SupportedLabels = LabelKinds.ToolTip;

            series2 = new MyDateTimeSeries(DateTime.Now, DateTime.Now, DateTime.Now.AddMinutes(1));
            series2.DateTimeFormat = DateTimeFormat.LongTime;
            series2.LabelInterval = 10;
            series2.MinValue = 0;
            series2.MaxValue = 100;  // Postavljanje odgovarajućeg opsega za vlažnost
            series2.Title = "Humidity (%)";
            series2.SupportedLabels = LabelKinds.ToolTip | LabelKinds.XAxisLabel;

            // Setup chart
            lineChart.Series.Add(series1);
            lineChart.Series.Add(series2);
            lineChart.Title = "Real-time Temperature and Humidity Data";
            lineChart.ShowXCoordinates = false; // Prikazivanje X koordinata
            lineChart.ShowLegendTitle = false;
            lineChart.LayoutPanel.Margin = new Margins(0, 0, 20, 0);

            lineChart.XAxis.Title = "Time"; // Postavite naziv X ose
            lineChart.XAxis.MinValue = 0; // Postavite opseg X ose
            lineChart.XAxis.MaxValue = 120;
            lineChart.XAxis.Interval = 10;

            lineChart.YAxis.MinValue = 0;
            lineChart.YAxis.MaxValue = 100;
            lineChart.YAxis.Interval = 10;
            lineChart.YAxis.Title = "Values";

            List<Brush> brushes = new List<Brush>()
            {
                new SolidBrush(Color.Red),
                new SolidBrush(Color.SeaGreen)
            };

            List<double> thicknesses = new List<double>() { 2 };

            PerSeriesStyle style = new PerSeriesStyle(brushes, brushes, thicknesses, null);
            lineChart.Plot.SeriesStyle = style;
            lineChart.Theme.PlotBackground = new SolidBrush(Color.White);
            lineChart.Theme.GridLineColor = Color.LightGray;
            lineChart.Theme.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            lineChart.TitleMargin = new Margins(10);
            lineChart.GridType = GridType.Horizontal;

            myTimer.Tick += new EventHandler(TimerEventProcessor);

            // Postavljanje intervala
            myTimer.Interval = 2000;
            myTimer.Start();
        }

        // Metoda koja se pokreće kada se tajmer podigne
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            try
            {
                var (timestamp, temperature, humidity) = FetchData();

                Console.WriteLine($"Fetched data -> Timestamp: {timestamp}, Temperature: {temperature}, Humidity: {humidity}");

                // Ako je bilo greške u parsiranju, preskače se ažuriranje
                if (temperature == -1 || humidity == -1)
                {
                    Console.WriteLine("Invalid data received. Skipping update.");
                    return;
                }

                // Validacija temperature i vlažnosti pre nego što se dodaju u serije
                if (temperature < series1.MinValue || temperature > series1.MaxValue)
                {
                    Console.WriteLine($"Invalid temperature value: {temperature}. Skipping update.");
                    return;
                }

                if (humidity < series2.MinValue || humidity > series2.MaxValue)
                {
                    Console.WriteLine($"Invalid humidity value: {humidity}. Skipping update.");
                    return;
                }

                // Dodavanje vrednosti u serije
                series1.addValue(temperature);
                series2.addValue(humidity);

                Console.WriteLine($"Data updated: {timestamp} Temperature: {temperature} Humidity: {humidity}");
                lineChart.ChartPanel.InvalidateLayout();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TimerEventProcessor: {ex.Message}");
            }
        }


        private void startDateTimePicker_ValueChanged_1(object sender, EventArgs e)
        {
            // Kada se promeni vrednost, ažurira se grafikon
            UpdateChartForSelectedPeriod();
        }
        private void endDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateChartForSelectedPeriod();
        }


        private void EnableRealTimeMode()
        {
            // Postavljanje tajmera u real-time modu

            myTimer.Start();

            Console.WriteLine("Switched to Real-Time Mode.");
        }


        private void UpdateChartForSelectedPeriod()
        {
            // Preuzima se početni i krajnji datum
            DateTime startDate = startDateTimePicker.Value;
            DateTime endDate = endDateTimePicker.Value;

            // Pravilno inicijalizuj serije sa opsegom
            series1 = new MyDateTimeSeries(startDate, startDate, endDate);
            series2 = new MyDateTimeSeries(startDate, startDate, endDate);

            lineChart.Series.Clear();
            lineChart.Series.Add(series1);
            lineChart.Series.Add(series2);

            // Pomak za vlažnost
            TimeSpan humidityOffset = TimeSpan.FromMinutes(2);

            string connectionString = "Server=localhost;Database=database_esp32;Uid=esp32;Pwd=microcontrollerslab@123;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
SELECT timestamp, message
FROM temperatura_table
WHERE timestamp >= @startDate AND timestamp <= @endDate
ORDER BY timestamp";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime timestamp = reader.GetDateTime("timestamp");
                            string message = reader.GetString("message");

                            double temperature = ExtractTemperatureFromMessage(message);
                            double humidity = ExtractHumidityFromMessage(message);

                            if (temperature != -1 && humidity != -1)
                            {
                                // Dodavanje temperature
                                series1.addValue(temperature, timestamp);

                                // Dodavanje vlažnosti sa pomakom
                                DateTime adjustedTimestamp = timestamp.Add(humidityOffset);
                                series2.addValue(humidity, adjustedTimestamp);

                                Console.WriteLine($"Dodato: Temp={temperature} @ {timestamp}, Hum={humidity} @ {adjustedTimestamp}");
                            }
                        }
                    }
                }
            }

            // Format prikaza datuma i vremena


            // Skaliranje X ose (po satima)
            series1.LabelInterval = 6; // ili više, po potrebi
            series2.LabelInterval = 6;

            series1.SupportedLabels =  LabelKinds.ToolTip;
            series2.SupportedLabels =   LabelKinds.ToolTip;


            series1.DateTimeFormat = DateTimeFormat.CustomDateTime;
            series1.CustomDateTimeFormat = "dd.MM.\nHH:mm";

            series2.DateTimeFormat = DateTimeFormat.CustomDateTime;
            series2.CustomDateTimeFormat = "dd.MM.\nHH:mm";

            // Skaliranje X ose (po satima)
            lineChart.XAxis.MinValue = 0;
            lineChart.XAxis.MaxValue = 1.0; // ovo će biti zamenjeno dole

            double totalHours = (endDate - startDate).TotalHours;
            lineChart.XAxis.MaxValue = totalHours / 24.0; // jer 1.0 == 1 dan
            lineChart.XAxis.Interval = 0.125; // 3 
            double totalDays = (endDate - startDate).TotalDays;

            series1.MinValue = 0;
            series1.MaxValue = totalDays;

            series2.MinValue = 0;
            series2.MaxValue = totalDays;
            

            lineChart.XAxis.MinValue = 0;
            lineChart.XAxis.MaxValue = totalDays;
            lineChart.XAxis.Interval = 0.25; // svakih 6 sat
            // Redraw
            lineChart.ChartPanel.InvalidateLayout();
        }
    }
}