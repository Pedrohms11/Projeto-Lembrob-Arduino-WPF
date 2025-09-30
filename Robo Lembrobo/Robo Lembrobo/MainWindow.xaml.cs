using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.ComponentModel;

namespace LembroteApp
{
    public partial class MainWindow : Window
    {
        // DECLARAÇÃO DE VARIÁVEIS DA CLASSE:
        
        private SerialPort serialPort;           // Objeto para comunicação serial com Arduino
        private DispatcherTimer timerVerificacao; // Timer para verificar lembretes periodicamente
        private List<Lembrete> lembretes;        // Lista para armazenar todos os lembretes agendados

        public MainWindow()
        {
            InitializeComponent(); // Inicializa os componentes da interface (XAML)
            InitializeApplication(); // Chama método de inicialização personalizado
        }

        private void InitializeApplication()
        {
            // MÉTODO DE INICIALIZAÇÃO DA APLICAÇÃO:
            // Configura todos os componentes e carrega dados iniciais
            
            lembretes = new List<Lembrete>(); // Inicializa a lista de lembretes vazia
            
            // CARREGAR PORTAS COM DISPONÍVEIS:
            // Obtém todas as portas seriais disponíveis no sistema
            cmbPortas.ItemsSource = SerialPort.GetPortNames();
            if (cmbPortas.Items.Count > 0)
                cmbPortas.SelectedIndex = 0; // Seleciona a primeira porta se existir

            // CARREGAR HORAS E MINUTOS NOS COMBOBOXES:
            // Preenche o combobox de horas (00-23)
            for (int i = 0; i < 24; i++)
                cmbHora.Items.Add(i.ToString("00")); // Formata com 2 dígitos
            
            // Preenche o combobox de minutos (00-59)
            for (int i = 0; i < 60; i++)
                cmbMinuto.Items.Add(i.ToString("00")); // Formata com 2 dígitos

            // DEFINIR HORA E MINUTO ATUAIS COMO PADRÃO:
            cmbHora.SelectedIndex = DateTime.Now.Hour; // Seleciona hora atual
            cmbMinuto.SelectedIndex = DateTime.Now.Minute; // Seleciona minuto atual

            // CONFIGURAR TIMER PARA VERIFICAÇÃO DE LEMBRETES:
            timerVerificacao = new DispatcherTimer();
            timerVerificacao.Interval = TimeSpan.FromSeconds(1); // Verifica a cada 1 segundo
            timerVerificacao.Tick += TimerVerificacao_Tick; // Associa o evento Tick
            timerVerificacao.Start(); // Inicia o timer

            // CONFIGURAR DATA ATUAL NO DATEPICKER:
            dpData.SelectedDate = DateTime.Today; // Define data atual como padrão

            AtualizarListaLembretes(); // Atualiza a exibição da lista de lembretes
            AdicionarLog("Sistema inicializado. Conecte à porta serial."); // Log inicial
        }

        private void TimerVerificacao_Tick(object sender, EventArgs e)
        {
            // EVENTO EXECUTADO A CADA SEGUNDO PELO TIMER:
            // Verifica se há lembretes que precisam ser enviados
            
            var agora = DateTime.Now; // Obtém data/hora atual
            
            // FILTRAR LEMBRETES QUE DEVEM SER ENVIADOS:
            // - DataHora <= agora: Já passou do horário agendado
            // - Status == "Pendente": Ainda não foi enviado
            var lembretesParaEnviar = lembretes.Where(l => 
                l.DataHora <= agora && l.Status == "Pendente").ToList();

            // ENVIAR CADA LEMBRETE FILTRADO:
            foreach (var lembrete in lembretesParaEnviar)
            {
                EnviarLembrete(lembrete); // Chama método para enviar via serial
            }
        }

        private void BtnConectar_Click(object sender, RoutedEventArgs e)
        {
            // EVENTO DO BOTÃO CONECTAR/DESCONECTAR:
            // Gerencia a conexão e desconexão da porta serial
            
            try
            {
                if (serialPort?.IsOpen == true)
                {
                    // CASO JÁ ESTEJA CONECTADO: EXECUTA DESCONEXÃO
                    serialPort.Close(); // Fecha a porta serial
                    serialPort.Dispose(); // Libera recursos
                    txtStatus.Text = "Desconectado"; // Atualiza status
                    txtStatus.Foreground = System.Windows.Media.Brushes.Red; // Cor vermelha
                    btnConectar.Content = "Conectar"; // Muda texto do botão
                    AdicionarLog("Desconectado da porta serial."); // Registra no log
                }
                else
                {
                    // CASO ESTEJA DESCONECTADO: TENTA CONEXÃO
                    
                    // VALIDAÇÃO: VERIFICA SE PORTA FOI SELECIONADA
                    if (cmbPortas.SelectedItem == null)
                    {
                        MessageBox.Show("Selecione uma porta COM válida.");
                        return; // Interrompe se não há porta selecionada
                    }

                    // CRIA NOVA INSTÂNCIA DO SERIALPORT:
                    // - Porta: Selecionada no combobox
                    // - BaudRate: Selecionado no combobox (convertendo de string para int)
                    serialPort = new SerialPort(
                        cmbPortas.SelectedItem.ToString(),
                        int.Parse((cmbBaudRate.SelectedItem as ComboBoxItem).Content.ToString())
                    );

                    serialPort.Open(); // Abre a conexão serial
                    txtStatus.Text = "Conectado"; // Atualiza status
                    txtStatus.Foreground = System.Windows.Media.Brushes.Green; // Cor verde
                    btnConectar.Content = "Desconectar"; // Muda texto do botão
                    AdicionarLog($"Conectado à {serialPort.PortName} com baud rate {serialPort.BaudRate}");
                }
            }
            catch (Exception ex)
            {
                // TRATAMENTO DE ERROS DE CONEXÃO:
                MessageBox.Show($"Erro ao conectar: {ex.Message}"); // Mensagem para usuário
                AdicionarLog($"Erro na conexão: {ex.Message}"); // Registra detalhes no log
            }
        }

        private void BtnAgendar_Click(object sender, RoutedEventArgs e)
        {
            // EVENTO DO BOTÃO AGENDAR LEMBRETE:
            // Valida e adiciona novo lembrete à lista
            
            // VALIDAÇÃO: TEXTO DO LEMBRETE NÃO PODE SER VAZIO
            if (string.IsNullOrWhiteSpace(txtLembrete.Text))
            {
                MessageBox.Show("Digite um lembrete válido.");
                return;
            }

            // VALIDAÇÃO: VERIFICA SE TODOS OS CAMPOS DE DATA/HORA FORAM PREENCHIDOS
            if (dpData.SelectedDate == null || cmbHora.SelectedItem == null || cmbMinuto.SelectedItem == null)
            {
                MessageBox.Show("Selecione data e hora válidas.");
                return;
            }

            // MONTA A DATA/HORA COMPLETA DO LEMBRETE:
            var data = dpData.SelectedDate.Value; // Data do datepicker
            var hora = int.Parse(cmbHora.SelectedItem.ToString()); // Hora do combobox
            var minuto = int.Parse(cmbMinuto.SelectedItem.ToString()); // Minuto do combobox

            var dataHora = new DateTime(data.Year, data.Month, data.Day, hora, minuto, 0);

            // VALIDAÇÃO: DATA/HORA DEVE SER FUTURA
            if (dataHora <= DateTime.Now)
            {
                MessageBox.Show("Selecione uma data/hora futura.");
                return;
            }

            // CRIA NOVO OBJETO LEMBRETE:
            var lembrete = new Lembrete
            {
                DataHora = dataHora,        // Data/hora agendada
                Texto = txtLembrete.Text.Trim(), // Texto do lembrete (sem espaços extras)
                Status = "Pendente"         // Status inicial
            };

            lembretes.Add(lembrete); // Adiciona à lista
            AtualizarListaLembretes(); // Atualiza a exibição

            AdicionarLog($"Lembrete agendado para {dataHora:dd/MM/yyyy HH:mm}: {lembrete.Texto}");

            // LIMPAR CAMPOS PARA NOVO AGENDAMENTO:
            txtLembrete.Clear(); // Limpa a caixa de texto
        }

        private void EnviarLembrete(Lembrete lembrete)
        {
            // MÉTODO PARA ENVIAR LEMBRETE VIA SERIAL PARA ARDUINO:
            // Segue o protocolo especificado: MOVE -> LCD:texto -> MOVE
            
            try
            {
                // VERIFICA SE PORTA SERIAL ESTÁ CONECTADA:
                if (serialPort?.IsOpen != true)
                {
                    AdicionarLog("ERRO: Porta serial não conectada para enviar lembrete.");
                    return; // Interrompe se não há conexão
                }

                // PROTOCOLO DE ENVIO CONFORME ESPECIFICADO:
                
                // 1. ENVIA PRIMEIRO COMANDO "MOVE":
                serialPort.WriteLine("MOVE"); // Envia comando para acionar servos
                System.Threading.Thread.Sleep(100); // Pequena pausa para processamento

                // 2. ENVIA TEXTO DO LEMBRETE FORMATADO COMO "LCD:texto":
                serialPort.WriteLine($"LCD:{lembrete.Texto}"); // Envia texto para display
                System.Threading.Thread.Sleep(10000); // Pequena pausa para processamento
                // 3. ENVIA SEGUNDO COMANDO "MOVE":
                serialPort.WriteLine("MOVE"); // Envia comando para finalizar movimento
                // ATUALIZA STATUS DO LEMBRETE:
                lembrete.Status = "Enviado"; // Marca como enviado
                AtualizarListaLembretes(); // Atualiza exibição

                AdicionarLog($"Lembrete enviado: {lembrete.Texto}"); // Registra no log
            }
            catch (Exception ex)
            {
                // TRATAMENTO DE ERROS NO ENVIO SERIAL:
                AdicionarLog($"ERRO ao enviar lembrete: {ex.Message}"); // Registra erro detalhado
            }
        }

        private void BtnRemover_Click(object sender, RoutedEventArgs e)
        {
            // EVENTO DO BOTÃO REMOVER LEMBRETE:
            // Remove lembrete selecionado da lista
            
            var button = sender as Button; // Converte sender para Button
            var lembrete = button.DataContext as Lembrete; // Obtém o lembrete associado

            if (lembrete != null)
            {
                lembretes.Remove(lembrete); // Remove da lista
                AtualizarListaLembretes(); // Atualiza exibição
                AdicionarLog($"Lembrete removido: {lembrete.Texto}"); // Registra no log
            }
        }

        private void AtualizarListaLembretes()
        {
            // MÉTODO PARA ATUALIZAR A EXIBIÇÃO DA LISTA DE LEMBRETES:
            // Garante que a DataGrid mostre os dados mais recentes
            
            dgLembretes.ItemsSource = null; // Limpa fonte de dados atual
            dgLembretes.ItemsSource = lembretes
                .OrderBy(l => l.DataHora) // Ordena por data/hora (mais antigos primeiro)
                .ToList(); // Converte para lista
        }

        private void AdicionarLog(string mensagem)
        {
            // MÉTODO PARA ADICIONAR MENSAGENS AO LOG:
            // Mantém registro de todas as atividades do sistema
            
            txtLog.AppendText($"{DateTime.Now:HH:mm:ss} - {mensagem}\n"); // Adiciona linha com timestamp
            txtLog.ScrollToEnd(); // Rola automaticamente para a última linha
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // EVENTO EXECUTADO AO FECHAR A APLICAÇÃO:
            // Garante que recursos sejam liberados adequadamente
            
            timerVerificacao?.Stop(); // Para o timer de verificação
            serialPort?.Close(); // Fecha a porta serial se estiver aberta
            serialPort?.Dispose(); // Libera recursos da porta serial
            base.OnClosing(e); // Chama implementação da classe base
        }
    }

    public class Lembrete
    {
        // CLASSE QUE REPRESENTA UM LEMBRETE:
        // Modelo de dados para armazenar informações dos lembretes
        
        public DateTime DataHora { get; set; }  // Data e hora agendada para o lembrete
        public string Texto { get; set; }       // Texto do lembrete (até 32 caracteres)
        public string Status { get; set; }      // Status: "Pendente" ou "Enviado"
    }
}