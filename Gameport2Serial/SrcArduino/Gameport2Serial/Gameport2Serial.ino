
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
  
  //delay(500);
}


void mainLoop(){
            
  Serial.println(String("a0:") + analogRead(AXE_0_PIN) + ";a1:" + analogRead(AXE_1_PIN) + ";a2:" 
    + analogRead(AXE_2_PIN) + ";a3:" + analogRead(AXE_3_PIN) + ";b0:" + digitalRead(BUTTON_0_PIN) + ";b1:"
    + digitalRead(BUTTON_1_PIN) + ";b2:" +  + digitalRead(BUTTON_2_PIN) + ";b3:" +  + digitalRead(BUTTON_3_PIN) + ";");
  
}
