using System;
using System.IO.Ports;
using System.Globalization;
using System.Windows.Forms;

namespace TARS {
    public partial class Form1 : Form {
        // Declaration
        private SerialPort SerialPort1;
        private volatile bool isReading = false;

        public Form1() {
            InitializeComponent();

            SerialPort1 = new SerialPort();
            SerialPort1.NewLine = "\n";

            SerialPort1.DataReceived += SerialPort1_DataReceived;
            RefreshComPorts();
            comboBox1.DropDown += comboBox1_DropDown;

            if (comboBox1.Items.Count > 0) {
                comboBox1.SelectedIndex = 0;
            }

            comboBox2.Items.AddRange(new object[] {
                "9600", "19200", "38400",
                "57600", "115200"
            });

            comboBox2.SelectedIndex = 0;
        }
        
        // Logic Areas
        private void RefreshComPorts() {
            string selectedPort = comboBox1.Text;

            comboBox1.Items.Clear();

            string[] ports = SerialPort.GetPortNames();

            comboBox1.Items.AddRange(ports);

            if (comboBox1.Items.Contains(selectedPort)) {
                comboBox1.SelectedItem = selectedPort;
            }
            else if (comboBox1.Items.Count > 0) {
                comboBox1.SelectedIndex = 0;
            }
        }
        
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            if (!isReading) {
                return;
            } 
            
            try {
                string dataMasuk = SerialPort1.ReadLine().Trim();

                if (string.IsNullOrWhiteSpace(dataMasuk)) {
                    return;
                }
                
                this.Invoke(new MethodInvoker(delegate {
                    richTextBox1.AppendText(dataMasuk + Environment.NewLine);

                    string[] nilai = dataMasuk.Split(";");

                    if (nilai.Length != 4) {
                        return;
                    }

                    if (!float.TryParse(
                        nilai[0].Trim(), NumberStyles.Float,
                        CultureInfo.InvariantCulture, out float temp)) {
                        return;
                    }
                    
                    if (!float.TryParse(
                        nilai[1].Trim(), NumberStyles.Float,
                        CultureInfo.InvariantCulture, out float hum)) {
                        return;
                    }

                    if (!float.TryParse(
                        nilai[2].Trim(), NumberStyles.Float,
                        CultureInfo.InvariantCulture, out float tempTolerance)) {
                        return;
                    }

                    if (!float.TryParse(
                        nilai[3].Trim(), NumberStyles.Float,
                        CultureInfo.InvariantCulture, out float humTolerance)) {
                        return;
                    }

                    textBox2.Text = temp.ToString();
                    textBox3.Text = hum.ToString();
                    textBox4.Text = tempTolerance.ToString();
                    textBox5.Text = humTolerance.ToString();

                    if (temp >= tempTolerance && hum >= humTolerance) {
                        textBox1.Text = "LED 1 & 2 ON";
                    } else if (temp >= tempTolerance) {
                        textBox1.Text = "LED 1 ON";
                    } else if (hum >= humTolerance) {
                        textBox1.Text = "LED 2 ON";
                    } else {
                        textBox1.Text = "ALL LED OFF";
                    }
                }));
            }
            catch(TimeoutException) {}
            catch(InvalidOperationException) {}
            catch(System.IO.IOException) {}
        }

        private void comboBox1_DropDown(object sender, System.EventArgs e) {
            RefreshComPorts();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (SerialPort1 != null && SerialPort1.IsOpen) {
                SerialPort1.Close();
            }

            base.OnFormClosing(e);
        }

        private void richTextBox1_TextChanged(object sender, System.EventArgs e) {
            richTextBox1.Select(richTextBox1.TextLength, 0);
            richTextBox1.ScrollToCaret();
        }


        // Buttons Areas
        private void button1_Click(object sender, System.EventArgs e) {
            try {
                if (button1.Text.ToUpper() == "CONNECT") {
                    SerialPort1.PortName = comboBox1.Text;
                    SerialPort1.BaudRate = int.Parse(comboBox2.Text);

                    // SerialPort1.DtrEnable = true;
                    // SerialPort1.RtsEnable = true;

                    SerialPort1.Open();
                    button1.Text = "DISCONNECT";
                } else if (button1.Text.ToUpper() == "DISCONNECT") {
                    isReading = false;
                    SerialPort1.Close();
                    button1.Text = "CONNECT";
                }
            } catch (Exception ex) {
                button1.Text = "CONNECT";
                MessageBox.Show(
                    "COM SALAH: " + ex.Message, 
                    "Serial Port Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
            }
        }

        private void Button2_Click(object sender, System.EventArgs e) {
            if (SerialPort1.IsOpen) {
                isReading = true;
                richTextBox1.Text = " ";
            } else {
                MessageBox.Show(
                    "NaN Port Connected! Do press CONNECT button first.",
                    "Informations", MessageBoxButtons.OK, MessageBoxIcon.Information
                );
            }
        }

        private void Button3_Click(object sender, System.EventArgs e) {
            isReading = false;
        }


        // Label
        private void Label2_Click(object sender, System.EventArgs e) {}
        private void Label3_Click(object sender, System.EventArgs e) {}
        private void Label4_Click(object sender, System.EventArgs e) {}
        private void Label5_Click(object sender, System.EventArgs e) {}
    }
}