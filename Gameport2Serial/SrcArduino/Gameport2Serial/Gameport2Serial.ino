
#define BUTTON_0_PIN 2
#define BUTTON_1_PIN 3
#define BUTTON_2_PIN 4
#define BUTTON_3_PIN 5

#define AXE_0_PIN 0
#define AXE_1_PIN 1
#define AXE_2_PIN 2
#define AXE_3_PIN 3


void setup() {
  Serial.begin(9600);
  while(!Serial){
    ;
  }

  pinMode(BUTTON_0_PIN, INPUT_PULLUP);
  pinMode(BUTTON_1_PIN, INPUT_PULLUP);
  pinMode(BUTTON_2_PIN, INPUT_PULLUP);
  pinMode(BUTTON_3_PIN, INPUT_PULLUP);
  
  
}

void loop() {
  //test();
  mainLoop();
}


void test(){
  
  delay(500);
}

void mainLoop(){
  sendDataAxe("a0", analogRead(AXE_0_PIN));
  sendDataAxe("a1", analogRead(AXE_1_PIN));
  sendDataAxe("a2", analogRead(AXE_2_PIN));
  sendDataAxe("a3", analogRead(AXE_3_PIN));

  
  sendDataButton("b0" ,digitalRead(BUTTON_0_PIN) == LOW);
  sendDataButton("b1" ,digitalRead(BUTTON_1_PIN) == LOW);
  sendDataButton("b2" ,digitalRead(BUTTON_2_PIN) == LOW);
  sendDataButton("b3" ,digitalRead(BUTTON_3_PIN) == LOW);
  
}


void sendDataButton(String n, bool v){
  Serial.println(n + ":" + (v ? '1' : '0') + ';');
}
void sendDataAxe(String n, int v){
  Serial.println(n + ":" + v + ';');
}
