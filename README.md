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

https://github.com/user-attachments/assets/0bd42e44-b524-473e-b17f-1e81230ff90d

<p align="center"><em>O LembroBô movimentando os dois braços em um ângulo de 180º.</em></p>

## 6. Estrutura de Código

Abaixo está os códigos que forma utilizados para controlar o LembroBô.

## Código do Arduino IDE

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

## Código no Arduino IDE

## Código do C#


## Fotos do projeto e da execução


## Conclusão

## Referências Bibliográficas
