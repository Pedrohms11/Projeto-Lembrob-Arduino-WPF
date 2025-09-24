using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace ControleRoboWPF
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private bool conectado = false;

        public MainWindow()
        {
            InitializeComponent();
            CarregarPortasCOM();
        }

        private void CarregarPortasCOM()
        {
            try
            {
                cmbPortasCOM.Items.Clear();
                string[] portas = SerialPort.GetPortNames();
                foreach (string porta in portas)
                {
                    cmbPortasCOM.Items.Add(porta);
                }
                if (cmbPortasCOM.Items.Count > 0)
                    cmbPortasCOM.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                AdicionarLog($"Erro ao carregar portas COM: {ex.Message}");
            }
        }

        private void BtnConectar_Click(object sender, RoutedEventArgs e)
        {
            if (!conectado)
            {
                Conectar();
            }
            else
            {
                Desconectar();
            }
        }

        private void Conectar()
        {
            if (cmbPortasCOM.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma porta COM válida.");
                return;
            }

            try
            {
                serialPort = new SerialPort(
                    cmbPortasCOM.SelectedItem.ToString(),
                    9600,
                    Parity.None,
                    8,
                    StopBits.One
                );

                serialPort.Open();
                conectado = true;
                
                AtualizarInterface();
                AdicionarLog($"Conectado à porta {serialPort.PortName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar: {ex.Message}");
                AdicionarLog($"Erro de conexão: {ex.Message}");
            }
        }

        private void Desconectar()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                conectado = false;
                AtualizarInterface();
                AdicionarLog("Desconectado do Arduino");
            }
            catch (Exception ex)
            {
                AdicionarLog($"Erro ao desconectar: {ex.Message}");
            }
        }

        private void AtualizarInterface()
        {
            btnConectar.Content = conectado ? "Desconectar" : "Conectar";
            txtStatus.Text = conectado ? "Conectado" : "Desconectado";
            txtStatus.Foreground = conectado ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            
            btnCumprimento.IsEnabled = conectado;
            btnEnviarLCD.IsEnabled = conectado;
            btnLimparLCD.IsEnabled = conectado;
            sldServo1.IsEnabled = conectado;
            sldServo2.IsEnabled = conectado;
        }

        private async void BtnCumprimento_Click(object sender, RoutedEventArgs e)
        {
            if (!conectado) return;

            try
            {
                btnCumprimento.IsEnabled = false;
                AdicionarLog("Executando movimento de cumprimento...");
                
                // Envia comando para o Arduino
                serialPort.WriteLine("MOVE");
                
                // Aguarda o movimento completar
                await Task.Delay(3000);
                
                AdicionarLog("Movimento de cumprimento concluído!");
            }
            catch (Exception ex)
            {
                AdicionarLog($"Erro no movimento: {ex.Message}");
            }
            finally
            {
                btnCumprimento.IsEnabled = true;
            }
        }

        private void BtnEnviarLCD_Click(object sender, RoutedEventArgs e)
        {
            if (!conectado || string.IsNullOrWhiteSpace(txtMensagemLCD.Text))
                return;

            try
            {
                string mensagem = txtMensagemLCD.Text.Trim();
                serialPort.WriteLine($"LCD:{mensagem}");
                AdicionarLog($"Mensagem enviada para LCD: {mensagem}");
            }
            catch (Exception ex)
            {
                AdicionarLog($"Erro ao enviar para LCD: {ex.Message}");
            }
        }

        private void BtnLimparLCD_Click(object sender, RoutedEventArgs e)
        {
            if (!conectado) return;

            try
            {
                serialPort.WriteLine("CLEAR");
                AdicionarLog("LCD limpo");
            }
            catch (Exception ex)
            {
                AdicionarLog($"Erro ao limpar LCD: {ex.Message}");
            }
        }

        private void SldServo1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (conectado && sldServo1.IsEnabled)
            {
                try
                {
                    int valor = (int)sldServo1.Value;
                    serialPort.WriteLine($"SERVO1:{valor}");
                }
                catch (Exception ex)
                {
                    AdicionarLog($"Erro ao mover servo 1: {ex.Message}");
                }
            }
        }

        private void SldServo2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (conectado && sldServo2.IsEnabled)
            {
                try
                {
                    int valor = (int)sldServo2.Value;
                    serialPort.WriteLine($"SERVO2:{valor}");
                }
                catch (Exception ex)
                {
                    AdicionarLog($"Erro ao mover servo 2: {ex.Message}");
                }
            }
        }

        private void AdicionarLog(string mensagem)
        {
            Dispatcher.Invoke(() =>
            {
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss} - {mensagem}\n");
                txtLog.ScrollToEnd();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            if (conectado)
                Desconectar();
            base.OnClosed(e);
        }
    }
}