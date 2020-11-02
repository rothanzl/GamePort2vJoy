
#define BUTTON_0_PIN 2
#define BUTTON_1_PIN 3
#define BUTTON_2_PIN 4
#define BUTTON_3_PIN 5


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
  
  fetchData();
  sendData();
}

byte axe1x = 0;
byte axe1y = 10;
byte axe2x = 20;
byte axe2y = 30;

bool b0 = false;
bool b1 = false;
bool b2 = false;
bool b3 = false;

void fetchData(){
  axe1x++;
  axe1y++;
  axe2x++;
  axe2y++;

  
  sendDataButton("b0" ,digitalRead(BUTTON_0_PIN) == LOW);
  sendDataButton("b1" ,digitalRead(BUTTON_1_PIN) == LOW);
  sendDataButton("b2" ,digitalRead(BUTTON_2_PIN) == LOW);
  sendDataButton("b3" ,digitalRead(BUTTON_3_PIN) == LOW);

  
}


void sendData(){

  //sendDataButton("b0", b0);
  //sendDataButton("b1", b1);
  //sendDataButton("b2", b2);
  //sendDataButton("b3", b3);
  
  sendDataAxe("a0", axe1x);
  sendDataAxe("a1", axe1y);
  sendDataAxe("a2", axe2x);
  sendDataAxe("a3", axe2y);
}

void sendDataButton(String n, bool v){
  Serial.println(n + ":" + (v ? '1' : '0') + ';');
}
void sendDataAxe(String n, byte v){
  Serial.println(n + ":" + v + ';');
}
