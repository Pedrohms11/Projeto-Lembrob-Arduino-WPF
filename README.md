# Projeto-Lembrobô-Arduino-WPF

**Desenvolvido pelo grupo:** Alice Andrade, Dhulie Alves, Luna Beatriz e Pedro Moura

<img width="600" height="800" alt="Image" src="https://github.com/user-attachments/assets/d7ce3ea9-bef3-49a5-88a5-5db6ebf615c7" />

O nosso projeto chamado LembroBô é um robô desenvolvido com Arduino Uno como uma atividade prática do nosso curso de Desenvolvimento de Sistemas, programado na Arduino IDE e integrado com C#. Sendo semelhante a uma agenda digital, notificando o usuário sobre tarefas do cotidiano, servindo como ferramenta didática para o aprendizado de programação e integração de hardware com software, podendo estar presente no nosso dia a dia. Neste repositório, contém toda a documentação, esquemas de circuitos, códigos-fonte e imagens da criação do nosso robô inteligente, permitindo um fácil entendimento de como foi a criação desse nosso protótipo, servindo também para as pessoas que estejam lendo e tiverem o interesse de reproduzir ou expandir o projeto. Esperamos que ele inspire tanto no aprendizado técnico quanto na criatividade para aplicações práticas do conhecimento adquirido pelo nosso grupo durante a criação. 

## . Objetivo do nosso projeto

O nosso principal objetivo na criação do projeto seria que o Lembrobô funcionasse como um lembrete inteligente, semelhante a uma agenda digital, capaz de notificar o usuário sobre tarefas e compromissos por meio de alertas visuais. Dessa forma, auxiliando na organização pessoal e no gerenciamento de atividades do dia a dia, tornando-se um aliado prático e eficiente.

Além da função prática, buscamos proporcionar uma experiência de aprendizado aplicada, permitindo que os usuários compreendam como sistemas embarcados podem interagir com softwares para realizar tarefas. Essa abordagem estimula o desenvolvimento de habilidades em programação, eletrônica e integração de hardware e software.

Por fim, visamos incentivar a criatividade e a inovação, mostrando que é possível desenvolver ferramentas personalizadas que atendam a necessidades específicas do dia a dia, tornando a tecnologia uma aliada na gestão de tempo e tarefas.

## . Materiais e Componentes

<img width="600" height="800" alt="Image" src="https://github.com/user-attachments/assets/6cdd6400-8b46-4f76-b0f6-c9787168a674" />

<table>
  <tr>
    <td width="50%" valign="top">
      <h4>Componentes Eletrônicos</h4>
      <ul>
        <li>Arduino Uno</li>
        <li>Servo Motor SG90</li>
        <li>Protoboard</li>
        <li>Jumpers</li>
        <li>Cabo USB</li>
        <li>Display Tela LCD 1602</li>
      </ul>
    </td>
    <td width="50%" valign="top">
      <h4>Materiais para Montagem</h4>
      <ul>
        <li>Tinta</li>
        <li>Pistola de cola quente</li>
        <li>Palitos de picolé</li>
        <li>Papelão</li>
      </ul>
    </td>
  </tr>
</table>



## 4. Estrutura do Projeto

<img width="600" height="800" alt="Image" src="https://github.com/user-attachments/assets/b45934e1-003f-46b6-b5bf-18581be96173" />

Possuímos uma estrutura organizada em três camadas principais:

- **Hardware:** composta pelo Arduino Uno, sensores e atuadores responsáveis pelos alertas;
- **Software embarcado:** desenvolvido na Arduino IDE, gerencia a lógica de funcionamento do robô;
- **Interface de integração:** consiste na integração com C#, que permite a comunicação entre o robô e o computador.

A estrutura do projeto inclui documentação completa, com esquemas de circuito, códigos-fonte e instruções detalhadas de uso.

## 5. Instalação e Configuração

Após garantir que todas as ferramentas necessárias estavam instaladas, conectamos o Arduino Uno ao computador através do cabo USB e carregamos o código-fonte para o Arduino utilizando a Arduino IDE.

<img width="600" height="800" alt="Image" src="https://github.com/user-attachments/assets/b14696eb-77ba-483a-8064-322ea506adcd" />

Abrimos a aplicação em C# e configuramos a porta correspondente ao Arduino, garantindo que a comunicação entre o software e o hardware seja estabelecida corretamente. Com todas as configurações concluídas, o *Lembrobô* ficou pronto para uso. Conseguimos cadastrar, editar e monitorar tarefas diretamente na interface, observando o robô responder aos alertas de acordo com os horários programados.

https://github.com/user-attachments/assets/3d4eeb3f-3c40-4389-9ad1-267a7a713384

<p align="center"><em>O LembroBô movimentando os dois braços em um ângulo de 180º.</em></p>

## 6. Estrutura de Código

Abaixo está os códigos que forma utilizados para controlar o LembroBô.

### Código do Arduino IDE

```cpp
#include <Servo.h>
#include <LiquidCrystal.h>

// Configuração do LCD (ajuste os pinos conforme sua montagem)
LiquidCrystal lcd(12, 11, 5, 4, 3, 2);

// Servos
Servo servo1;
Servo servo2;

// Pinos dos servos
int servo1Pin = 8;
int servo2Pin = 10;

// Estado atual dos servos
int servo1Pos = 0;
int servo2Pos = 0;

void setup() {
  Serial.begin(9600);
  
  // Inicializa LCD
  lcd.begin(16, 2);
  lcd.print("Sistema Pronto!");
  
  // Anexa servos aos pinos
  servo1.attach(servo1Pin);
  servo2.attach(servo2Pin);
  
  // Posição inicial
  servo1.write(0);
  servo2.write(0);
  delay(1000);
}

void loop() {
  if (Serial.available() > 0) {
    String command = Serial.readStringUntil('\n');
    command.trim();
    
    // Comando para movimento dos servos
    if (command.startsWith("MOVE")) {
      executarCumprimento();
    }
    // Comando para exibir texto no LCD
    else if (command.startsWith("LCD:")) {
      String texto = command.substring(4);
      exibirNoLCD(texto);
    }
    // Comando para limpar LCD
    else if (command == "CLEAR") {
      lcd.clear();
    }
  }
}

void executarCumprimento() {
  lcd.clear();
  lcd.print("Cumprimentando!");
  
  // Movimento suave de 0 a 90 graus
  for (int pos = 0; pos <= 90; pos += 1) {
    servo1.write(pos);
    servo2.write(pos);
    delay(15);
  }
  
  delay(1000);
  
  // Retorna à posição inicial
  for (int pos = 90; pos >= 0; pos -= 1) {
    servo1.write(pos);
    servo2.write(pos);
    delay(15);
  }
  
  lcd.clear();
  lcd.print("Movimento OK!");
}

void exibirNoLCD(String texto) {
  lcd.clear();
  
  if (texto.length() <= 16) {
    // Texto cabe em uma linha
    lcd.print(texto);
  } else {
    // Divide em duas linhas
    String linha1 = texto.substring(0, 16);
    String linha2 = texto.substring(16, 32);
    
    lcd.print(linha1);
    lcd.setCursor(0, 1);
    lcd.print(linha2);
  }
}
 ```

### Código C# - Back-end (WPF)

```cpp
﻿using System;
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
 ```

### Código C# - Front-end (WPF)

```cpp
﻿<Window x:Class="ControleRoboWPF.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Controle do Robô Arduino" Height="450" Width="600"
        WindowStartupLocation="CenterScreen">
    
    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto"/>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>

        <!-- Conexão Serial -->
        <TextBlock Grid.Row="0" Grid.Column="0" Text="Porta COM:" 
                   Margin="0,0,10,10" VerticalAlignment="Center"/>
        
        <ComboBox Grid.Row="0" Grid.Column="1" x:Name="cmbPortasCOM" 
                  Margin="0,0,10,10" Height="25"/>
        
        <Button Grid.Row="0" Grid.Column="2" x:Name="btnConectar" 
                Content="Conectar" Click="BtnConectar_Click" 
                Margin="0,0,0,10" Width="80" Height="25"/>

        <!-- Controle dos Servos -->
        <GroupBox Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="3" 
                  Header="Controle dos Servos" Margin="0,0,0,10">
            <StackPanel Orientation="Horizontal" Margin="10">
                <Button x:Name="btnCumprimento" Content="Cumprimento" 
                        Click="BtnCumprimento_Click" Width="120" Height="30"
                        Margin="0,0,10,0" IsEnabled="False"/>
                
                <TextBlock Text="Servo 1:" VerticalAlignment="Center" Margin="10,0,5,0"/>
                <Slider x:Name="sldServo1" Minimum="0" Maximum="180" Width="100" 
                        ValueChanged="SldServo1_ValueChanged" IsEnabled="False"/>
                
                <TextBlock Text="Servo 2:" VerticalAlignment="Center" Margin="10,0,5,0"/>
                <Slider x:Name="sldServo2" Minimum="0" Maximum="180" Width="100" 
                        ValueChanged="SldServo2_ValueChanged" IsEnabled="False"/>
            </StackPanel>
        </GroupBox>

        <!-- Controle do LCD -->
        <GroupBox Grid.Row="2" Grid.Column="0" Grid.ColumnSpan="3" 
                  Header="Display LCD 16x2" Margin="0,0,0,10">
            <StackPanel Margin="10">
                <TextBlock Text="Mensagem para o LCD:" Margin="0,0,0,5"/>
                <TextBox x:Name="txtMensagemLCD" Height="60" TextWrapping="Wrap"
                         Text="Olá! Bem-vindo!" MaxLength="32"/>
                
                <StackPanel Orientation="Horizontal" Margin="0,10,0,0">
                    <Button x:Name="btnEnviarLCD" Content="Enviar para LCD" 
                            Click="BtnEnviarLCD_Click" Width="120" Height="25"
                            Margin="0,0,10,0" IsEnabled="False"/>
                    
                    <Button x:Name="btnLimparLCD" Content="Limpar LCD" 
                            Click="BtnLimparLCD_Click" Width="80" Height="25"
                            IsEnabled="False"/>
                </StackPanel>
            </StackPanel>
        </GroupBox>

        <!-- Log de Atividades -->
        <GroupBox Grid.Row="4" Grid.Column="0" Grid.ColumnSpan="3" 
                  Header="Log de Atividades" Margin="0,0,0,10">
            <TextBox x:Name="txtLog" IsReadOnly="True" VerticalScrollBarVisibility="Auto"
                     Height="120" TextWrapping="Wrap"/>
        </GroupBox>

        <!-- Status -->
        <TextBlock Grid.Row="5" Grid.Column="0" Grid.ColumnSpan="3" 
                   x:Name="txtStatus" Text="Desconectado" 
                   Foreground="Red" FontWeight="Bold"/>
    </Grid>
</Window>
 ```

## Conclusão

Sendo assim, concluímos que o projeto Lembrobô cumpriu seu objetivo de criar um robô inteligente capaz de auxiliar nas tarefas do dia a dia, combinando hardware e software de forma integrada. Ao desenvolver tanto a lógica de controle no Arduino quanto a interface em WPF, podemos ter aplicados conhecimentos de programação, eletrônica e comunicação, consolidando conceitos aprendidos durante o curso.

<img width="600" height="800" alt="Image" src="https://github.com/user-attachments/assets/090116b7-d0d3-4f92-bfa6-542730867473" />

Além da funcionalidade prática de notificação e interação com o usuário, o projeto também serviu como ferramenta didática, demonstrando como sistemas embarcados podem ser integrados com interfaces de computador de maneira intuitiva.
