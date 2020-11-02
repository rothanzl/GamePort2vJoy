


void setup() {
  Serial.begin(9600);
  while(!Serial){
    ;
  }
}

void loop() {
  fetchData();
  sendData();
}

byte axe1x = 0;
byte axe1y = 10;
byte axe2x = 20;
byte axe2y = 30;

void fetchData(){
  axe1x++;
  axe1y++;
  axe2x++;
  axe2y++;
}


void sendData(){

  bool b0 = true;
  bool b1 = true;
  bool b2 = false;
  bool b3 = false;

  sendDataButton("b0", b0);
  sendDataAxe("a1", axe1x);
}

void sendDataButton(String n, bool v){
  Serial.println(n + ":" + (v ? '1' : '0') + ';');
}
void sendDataAxe(String n, byte v){
  Serial.println(n + ":" + v + ';');
}
