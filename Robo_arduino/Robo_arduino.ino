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
