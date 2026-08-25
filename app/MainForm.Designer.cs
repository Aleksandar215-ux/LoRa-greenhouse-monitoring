namespace DateTimeSeries
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.lineChart = new MindFusion.Charting.WinForms.LineChart();
            this.startDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.btnRealTimeMode = new MindFusion.UI.WinForms.Button();
            this.endDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 630);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1500, 55);
            this.label1.TabIndex = 1;
            this.label1.Text = "Here we use a custom DateTimeSeries that supports adding of values dynamically. T" +
    "he series also exposes properties for specifing the intervals of time stamps and" +
    " the update rate.";
            // 
            // lineChart
            // 
            this.lineChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lineChart.LegendTitle = "Legend";
            this.lineChart.Location = new System.Drawing.Point(3, -3);
            this.lineChart.Margin = new System.Windows.Forms.Padding(4);
            this.lineChart.Name = "lineChart";
            this.lineChart.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.lineChart.ShowLegend = true;
            this.lineChart.Size = new System.Drawing.Size(1496, 629);
            this.lineChart.SubtitleFontName = null;
            this.lineChart.SubtitleFontSize = null;
            this.lineChart.SubtitleFontStyle = null;
            this.lineChart.TabIndex = 2;
            this.lineChart.Text = "lineChart";
            this.lineChart.Theme.UniformSeriesFill = new MindFusion.Drawing.SolidBrush("#FF90EE90");
            this.lineChart.Theme.UniformSeriesStroke = new MindFusion.Drawing.SolidBrush("#FF000000");
            this.lineChart.Theme.UniformSeriesStrokeThickness = 2D;
            this.lineChart.TitleFontName = null;
            this.lineChart.TitleFontSize = null;
            this.lineChart.TitleFontStyle = null;
            // 
            // startDateTimePicker
            // 
            this.startDateTimePicker.CustomFormat = "\"dd/MM/yyyy\"";
            this.startDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDateTimePicker.Location = new System.Drawing.Point(977, 69);
            this.startDateTimePicker.Name = "startDateTimePicker";
            this.startDateTimePicker.ShowUpDown = true;
            this.startDateTimePicker.Size = new System.Drawing.Size(200, 22);
            this.startDateTimePicker.TabIndex = 5;
            this.startDateTimePicker.ValueChanged += new System.EventHandler(this.startDateTimePicker_ValueChanged_1);
            // 
            // btnRealTimeMode
            // 
            this.btnRealTimeMode.BackgroundBrush = new MindFusion.Drawing.SolidBrush("#FFF0F0F0");
            this.btnRealTimeMode.BackgroundBrushDisabled = new MindFusion.Drawing.SolidBrush("#FFDEDEDE");
            this.btnRealTimeMode.BackgroundBrushDown = new MindFusion.Drawing.SolidBrush("#FFAEAEAE");
            this.btnRealTimeMode.BackgroundBrushOver = new MindFusion.Drawing.SolidBrush("#FFC5C5C5");
            this.btnRealTimeMode.BorderBrush = new MindFusion.Drawing.SolidBrush("#FFA6A6A6");
            this.btnRealTimeMode.BorderBrushDisabled = new MindFusion.Drawing.SolidBrush("#FFA6A6A6");
            this.btnRealTimeMode.BorderBrushDown = new MindFusion.Drawing.SolidBrush("#FF777777");
            this.btnRealTimeMode.BorderBrushOver = new MindFusion.Drawing.SolidBrush("#FFA6A6A6");
            this.btnRealTimeMode.BorderThickness = 0;
            this.btnRealTimeMode.ForegroundBrush = new MindFusion.Drawing.SolidBrush("#FF000000");
            this.btnRealTimeMode.ForegroundBrushDisabled = new MindFusion.Drawing.SolidBrush("#FF777777");
            this.btnRealTimeMode.ForegroundBrushDown = new MindFusion.Drawing.SolidBrush("#FF000000");
            this.btnRealTimeMode.ForegroundBrushOver = new MindFusion.Drawing.SolidBrush("#FF000000");
            this.btnRealTimeMode.Location = new System.Drawing.Point(226, 12);
            this.btnRealTimeMode.Name = "btnRealTimeMode";
            this.btnRealTimeMode.Size = new System.Drawing.Size(143, 23);
            this.btnRealTimeMode.TabIndex = 4;
            this.btnRealTimeMode.Text = "\"Real-Time Mode\"";
            this.btnRealTimeMode.Click += new System.EventHandler(this.btnRealTimeMode_Click);
            // 
            // endDateTimePicker
            // 
            this.endDateTimePicker.CustomFormat = "\"dd/MM/yyyy\"";
            this.endDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDateTimePicker.Location = new System.Drawing.Point(1213, 69);
            this.endDateTimePicker.Name = "endDateTimePicker";
            this.endDateTimePicker.ShowUpDown = true;
            this.endDateTimePicker.Size = new System.Drawing.Size(200, 22);
            this.endDateTimePicker.TabIndex = 6;
            this.endDateTimePicker.ValueChanged += new System.EventHandler(this.endDateTimePicker_ValueChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(977, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(200, 22);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = "Početni datum";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(1213, 41);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(200, 22);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = "Krajnji datum";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1499, 681);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.endDateTimePicker);
            this.Controls.Add(this.btnRealTimeMode);
            this.Controls.Add(this.startDateTimePicker);
            this.Controls.Add(this.lineChart);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "MindFusion.Charting Sample: Real-time DateTime Series";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private MindFusion.Charting.WinForms.LineChart lineChart;
        private System.Windows.Forms.DateTimePicker startDateTimePicker;
        private MindFusion.UI.WinForms.Button btnRealTimeMode;
        private System.Windows.Forms.DateTimePicker endDateTimePicker;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

