# Beook Solutions

BeookSolutions bietet Nutzern von "[Beook](https://beook.ch/)" die Möglichkeit, Lösungen von Aufgaben digitaler Lehrmittel direkt in Beook einzublenden.
Bei Aktivierung wird der Toolbar in Beook ein neues Element zum Einblenden der Lösungen hinzugefügt:

<h3 alingn="center">
  <img src="https://github.com/user-attachments/assets/f3871362-294f-4bb7-a788-6b5cbbbe7070" alt="Beook Solutions App Preview Solutions Off" width="400" />
  <img src="https://github.com/user-attachments/assets/f8352033-975e-4e78-a9cd-bf807dfd24aa" alt="Beook Solutions App Preview Solutions On" width="400" />
<h3/>
  
## Installation

BeookSolutions kann auf Windows und Linux verwendet werden. 
  
### Windows

BeookSolutions steht als Desktop-App mit Benutzeroberfläche für Windows zur Verfügung.
Die portable Version und der Installer können [hier](https://marekvonrogall.github.io/BeookSolutions/) oder auf der [Release-Seite](https://github.com/marekvonrogall/BeookSolutions/releases) heruntergeladen werden.

Vorschau der Desktop-App für Windows:

<h3 alingn="center">
  <img src="https://github.com/user-attachments/assets/d87ae6c4-4067-4a9d-8dcc-59c6f276db11" alt="Beook Solutions App Preview" width="260" />
  <img src="https://github.com/user-attachments/assets/d3cbe779-6536-4f6e-a4ff-8ba9efc59029" alt="Beook Solutions App Preview" width="260" />
  <img src="https://github.com/user-attachments/assets/d52d4331-c326-4908-8e96-8a7b337e9361" alt="Beook Solutions App Preview" width="260" />
<h3/>

### Linux

Für Linux wird keine Applikation mit grafischer Benutzeroberfläche bereitgestellt.
Anstattdessen kann die Aktivierung der Lösungen mittels einer lokalen Docker-Installation von BeookSolutions erfolgen.

In dieser Anleitung wird davon ausgegangen, dass [wine](https://gitlab.winehq.org/wine/wine) zur Emulation von Beook verwendet wird.

#### Lokale Docker-Installation

1. Laden Sie das Docker-Image von GHCR herunter und starten Sie den Container:

```bash
docker pull ghcr.io/marekvonrogall/tools/beook-solutions:latest
docker run -it -p 5000:5000 ghcr.io/marekvonrogall/tools/beook-solutions:latest
```
Alternativ können Sie auch das Projekt selbst klonen und mit Docker bauen und ausführen:

```bash
git clone https://github.com/marekvonrogall/beooksolutions-cli.git
cd beooksolutions-cli
docker build -t beook-solutions .
docker run -it -p 5000:5000 beook-solutions
```

2. Lokalisieren Sie das AppData Verzeichnis von wine.
3. Navigieren Sie in den Ordner ihres aktuell verwendeten Profils von Beook unter `ionesoft/beook/release/profiles/*`.
4. Erstellen Sie ggf. ein Backup Ihres Profils (`beook_book_v6.sqlite`).
5. Zum **Aktivieren der Lösungen** Anfrage an den `enable`-Endpoint des BeookSolution Containers mit der .sqlite-Datei:
```bash
curl -F "file=@/data/beook_book_v6.sqlite" http://localhost:5000/Solution/enable -o /data/beook_book_v6.sqlite
```

6. Zum **Deaktivieren der Lösungen** Anfrage an den `disable`-Endpoint des BeookSolution Containers mit der .sqlite-Datei:
```bash
curl -F "file=@/data/beook_book_v6.sqlite" http://localhost:5000/Solution/disable -o /data/beook_book_v6.sqlite
```

All-in-one-Befehl, der die Lösungen für alle Profile aktiviert:
```bash
find /home/$USER/.wine/drive_c/users/$(ls /home/$USER/.wine/drive_c/users | head -n 1)/AppData/Roaming/ionesoft/beook/release/profiles/ -type f -path "*/data/beook_book_v6.sqlite" -exec curl -F "file=@{}" http://localhost:5000/Solution/enable -o {} \;
```

All-in-one-Befehl, der die Lösungen für alle Profile deaktiviert:
```bash
find /home/$USER/.wine/drive_c/users/$(ls /home/$USER/.wine/drive_c/users | head -n 1)/AppData/Roaming/ionesoft/beook/release/profiles/ -type f -path "*/data/beook_book_v6.sqlite" -exec curl -F "file=@{}" http://localhost:5000/Solution/disable -o {} \;
```

Eine genauere Dokumentation der API-Endpunkte finden Sie [hier](https://github.com/marekvonrogall/beooksolutions-cli).

## Disclaimer of Affiliation with Ionesoft
This app, "Beook Solutions", is an independent product and is not affiliated with Ionesoft, the publisher and developer of Beook. The use of the word "Beook" does not imply any official approval or partnership with Ionesoft.
