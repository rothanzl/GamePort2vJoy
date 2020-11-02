


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


  byte buttonsSend = 0;
  if(b0) buttonsSend = buttonsSend | 0x1;
  if(b1) buttonsSend = buttonsSend | (0x1 << 1);
  if(b2) buttonsSend = buttonsSend | (0x1 << 2);
  if(b3) buttonsSend = buttonsSend | (0x1 << 3);


  byte message[ (4*3)+2+1 ] ;
  
  message[0] = 'b';
  message[1] = buttonsSend ;
  message[2] = 'a';
  message[3] = 1;
  message[4] = axe1x;
  message[5] = 'a';
  message[6] = 2;
  message[7] = axe1y;
  message[8] = 'a';
  message[9] = 3;
  message[10] = axe2x;
  message[11] = 'a';
  message[12] = 4;
  message[13] = axe2y;
  
  message[14] = 0xA;

  Serial.write(message, sizeof(message));


  
  
}
