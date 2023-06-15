sc delete "BetonarkaModbusProfinet" 
sc create "BetonarkaModbusProfinet" binPath="%~dp0BetonarkaKnezpolService.exe" start=auto
sc start BetonarkaModbusProfinet
PAUSE