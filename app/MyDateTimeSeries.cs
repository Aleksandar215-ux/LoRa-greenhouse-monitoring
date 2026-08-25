using System;
using System.Collections;
using System.Collections.Generic;
using MindFusion.Charting;

namespace DateTimeSeries
{
    
    class MyDateTimeSeries : Series
    {
        ///<>
        /// Inicijalizacija nove istance prilagođene klase TimeSeries.
        /// <>
        /// <param name="start"
        /// <param name="minDate"
        /// <param name="maxDate"
        /// <param name="values"
        public MyDateTimeSeries(DateTime start, DateTime minDate, DateTime maxDate)
        {
            this.start = start;

            this.minDate = minDate;
            this.maxDate = maxDate;
            minValue = 0;
            maxValue = (maxDate - minDate).TotalDays; // ⬅️ KLJUČNO!

            dateTimeFormat = DateTimeFormat.ShortTime;
            customDateTimeFormat = "";
            labelInterval = 10;

            dates = new List<long>();
            values = new List<double>();
        }

        /// <>
        /// Dobija vrednost na određenom indeksu iz određene dimenzije.
        /// <>
        /// <param name="index">Vrednost indeksa</param>
        /// <param name="dimension">Dinmenzija</param>
        /// <returns></returns>
        public double GetValue(int index, int dimension)
        {
            if (dimension == 0)
            {
                if (index < dates.Count && index >= 0)
                {
                    long currValue = dates[index];

                    var p = (currValue - (double)minDate.Ticks) / ((double)maxDate.Ticks - (double)minDate.Ticks);

                    return minValue + ((maxValue - minValue) * p);
                }
            }

            if (dimension == 1)
                return values[index];

            return 0;
        }

        /// <>
        /// Dodaje određenu vrednost na kraj liste podataka.
        /// </>
        /// <param name="value"></param>
        public void addValue(double value, bool isHumidity = false)
        {
            this.values.Add(value);
            long currTime = DateTime.Now.Ticks;
            if (isHumidity)
            {
                // Pomeranje vremena za 2 minuta (na primer)
                TimeSpan offset = TimeSpan.FromMinutes(2);
                currTime += offset.Ticks;
            }
            dates.Add(currTime);
        }
        /// <>
        /// Vraća određenu vrstu oznake na zadatom indeksu.
        /// </summary>
        /// <param name="index">Indeks oznake</param>
        /// <param name="kind">Vrsta oznake</param>
        /// <returns></returns>
        public string GetLabel(int index, LabelKinds kind)
        {
            if (kind == LabelKinds.XAxisLabel)
            {
                return formatDateTime(index, labelInterval);
            }

            if (kind == LabelKinds.ToolTip)
            {
                string date = formatDateTime(index, 1);

                if (values.Count > index)
                    return "Time: " + date + " Value: " + values[index].ToString("F2");

            }

            return string.Empty;
        }
        public void addValue(double value, DateTime timestamp)
        {
            this.values.Add(value);
            this.dates.Add(timestamp.Ticks);
        }
        private string formatDateTime(int index, int lInterval)
        {
            if (index < values.Count && index % lInterval == 0)
            {
                DateTime dateTime = new DateTime(dates[index]);

                SortedList dateTimeFormats = new SortedList(9);
                dateTimeFormats.Add("d", DateTimeFormat.ShortDate);
                dateTimeFormats.Add("D", DateTimeFormat.LongDate);
                dateTimeFormats.Add("t", DateTimeFormat.ShortTime);
                dateTimeFormats.Add("T", DateTimeFormat.LongTime);
                dateTimeFormats.Add("M", DateTimeFormat.MonthDateTime);
                dateTimeFormats.Add("Y", DateTimeFormat.YearDateTime);
                dateTimeFormats.Add("f", DateTimeFormat.FullDateTime);
                dateTimeFormats.Add("*", DateTimeFormat.CustomDateTime);
                dateTimeFormats.Add("", DateTimeFormat.None);

                string format = customDateTimeFormat;

                if (dateTimeFormat != DateTimeFormat.None &&
                    dateTimeFormat != DateTimeFormat.CustomDateTime)
                {
                    int fIndex = dateTimeFormats.IndexOfValue(dateTimeFormat);
                    format = dateTimeFormats.GetKey(fIndex).ToString();

                }

                return dateTime.ToString(format);
            }

            return "";
        }
        public bool IsEmphasized(int index)
        {
            return false;
        }

        /// <>
        /// Serija podržava oznake na X-osi.
        /// <>
        /// 
  

        public LabelKinds SupportedLabels
        {
            get { return labelKinds; }
            set { labelKinds = value; }
        }

        /// <>
        /// Prva dimenzija je sortirana.
        /// <>
        /// <param name="dimension">Dimenzija>
        /// <returns></returns>
        public bool IsSorted(int dimension)
        {
            return dimension == 0;
        }

        /// <>
        /// Postoje dve dimenzije
        /// <>
        public int Dimensions
        {
            get { return 2; }
        }

        /// <>
        /// Veličina serije je ekvivalentna broju vrednosti.
        /// <>
        public int Size
        {
            get { return values.Count; }
        }

        public string Title { get; set; }

        /// <>
        /// Postavlja kordinate na odgovarajući najmanji datum.
        /// <>
        public double MinValue
        {
            get { return minValue; }
            set
            {
                if (minValue == value)
                    return;

                minValue = value;
                OnDataChanged();
            }
        }

        /// <>
        /// Postavlja kordinate na odgovarajući najveći datum.
        /// <>
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                if (maxValue == value)
                    return;

                maxValue = value;
                OnDataChanged();
            }
        }


        /// <>
        /// Postavlja ili dobija vreme početka na osi.
        /// <>
        public DateTime MinDate
        {
            get { return minDate; }
            set
            {
                if (minDate == value)
                    return;

                minDate = value;
                OnDataChanged();
            }
        }

        /// <>
        /// Postavlja ili dobija krajnje vreme na osi.
        /// <>
        public DateTime MaxDate
        {
            get { return maxDate; }
            set
            {
                if (maxDate == value)
                    return;

                maxDate = value;
                OnDataChanged();
            }
        }

        /// <>
        /// Dobija ili postavlja vrednost koja označava kako formatirati DateTime vrednosti kao oznake..
        /// <>
        public DateTimeFormat DateTimeFormat
        {
            get { return dateTimeFormat; }
            set
            {
                if (dateTimeFormat == value)
                    return;

                dateTimeFormat = value;
                OnDataChanged();
            }
        }

        /// <>
        /// Dobija ili postavlja koliko vrednosti će biti
        /// preskočeno pre nego što se vremenska oznaka unosa prikaže kao oznaka na X-osi.
        /// <>
        public int LabelInterval
        {
            get { return labelInterval; }
            set
            {
                if (labelInterval == value)
                    return;

                labelInterval = value;
                OnDataChanged();
            }
        }

        /// <>
        /// Dobija ili postavlja prilagođeni format stringa za DateTime oznak
        /// <>
        public string CustomDateTimeFormat
        {
            get { return customDateTimeFormat; }
            set
            {
                if (customDateTimeFormat == value)
                    return;

                customDateTimeFormat = value;
                OnDataChanged();
            }
        }

        public event EventHandler DataChanged;

        /// <>
        /// Pokreće događaj DataChanged.
        /// <>
        protected virtual void OnDataChanged()
        {
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }


        DateTime start;
        List<double> values;
        List<long> dates;
        private DateTime minDate;
        private DateTime maxDate;

        //the numerical values of the axis that should be mapped
        //to the minDate and maxDate. Could be public properties.
        private double minValue = 0;
        private double maxValue = 1;

        private int labelInterval;
        private DateTimeFormat dateTimeFormat;
        private string customDateTimeFormat;
        private LabelKinds labelKinds;
    }
}