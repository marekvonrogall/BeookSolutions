# Beook Solutions


BeookSolutions bietet Nutzern von "[Beook](https://beook.ch/)" die Möglichkeit, Lösungen von Aufgaben digitaler Lehrmittel direkt in Beook einzublenden.
Bei Aktivierung wird der Toolbar in Beook ein neues Element zum Einblenden der Lösungen hinzugefügt:

<img src="https://github.com/user-attachments/assets/964571fb-3ec6-4215-aac7-8c4b839f7463" alt="Beook Solutions App Preview" width="300" />

## Installation

BeookSolutions kann auf Windows und Linux verwendet werden.

> [!IMPORTANT]
>Bitte beachten Sie, dass BeookSolutions für jedes neu heruntergeladene E-Lehrmittel erneut ausgeführt werden muss.

### Windows

BeookSolutions steht als Desktop-App mit Benutzeroberfläche für Windows zur Verfügung.
Die portable Version und der Installer können [hier](https://marekvonrogall.github.io/BeookSolutions/) oder auf der [Release-Seite](https://github.com/marekvonrogall/BeookSolutions/releases) heruntergeladen werden.

Vorschau der Desktop-App für Windows:

<img src="https://github.com/user-attachments/assets/d2fca192-b860-4bc8-9168-1285943dec6e" alt="Beook Solutions App Preview" width="300" />

### Linux

Für Linux wird keine Applikation mit grafischer Benutzeroberfläche bereitgestellt.
Anstattdessen kann die Aktivierung der Lösungen mittels der BeookSolutions API oder einer lokalen Docker-Installation erfolgen.

In dieser Anleitung wird davon ausgegangen, dass [wine](https://gitlab.winehq.org/wine/wine) zur Emulation von Beook verwendet wird.

#### BeookSolutions API

1. Lokalisieren Sie das AppData Verzeichnis von wine.
2. Navigieren Sie in den Ordner ihres aktuell verwendeten Profils von Beook unter `ionesoft/beook/release/profiles/*`.
3. Erstellen Sie ggf. ein Backup Ihres Profils (`beook_book_v6.sqlite`) und vergewissern Sie sich, [ob die BeookSolutions API erreichbar ist](https://tools.vrmarek.me/beook/ping).
4. Zum **Aktivieren der Lösungen** Anfrage an die BeookSolutions API mit der .sqlite-Datei senden:

```bash
curl -F "file=@/data/beook_book_v6.sqlite" https://tools.vrmarek.me/beook/enable -o /data/beook_book_v6.sqlite
```

5. Zum **Deaktivieren der Lösungen** Anfrage an die BeookSolutions API mit der .sqlite-Datei senden:
```bash
curl -F "file=@/data/beook_book_v6.sqlite" https://tools.vrmarek.me/beook/disable -o /data/beook_book_v6.sqlite
```

All-in-one-Befehl, der die Lösungen für alle Profile aktiviert:

```bash
find /home/$USER/.wine/drive_c/users/$(ls /home/$USER/.wine/drive_c/users | head -n 1)/AppData/Roaming/ionesoft/beook/release/profiles/ -type f -path "*/data/beook_book_v6.sqlite" -exec curl -F "file=@{}" https://tools.vrmarek.me/beook/enable -o {} \;
```

All-in-one-Befehl, der die Lösungen für alle Profile deaktiviert:

```bash
find /home/$USER/.wine/drive_c/users/$(ls /home/$USER/.wine/drive_c/users | head -n 1)/AppData/Roaming/ionesoft/beook/release/profiles/ -type f -path "*/data/beook_book_v6.sqlite" -exec curl -F "file=@{}" https://tools.vrmarek.me/beook/disable -o {} \;
```

#### Lokale Docker-Installation

Die BeookSolutions API kann auch lokal mittels Docker betrieben werden.

Klonen Sie dazu das Docker-Image von GHCR und starten Sie den Container:

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

Nun können dieselben Schritte wie bei der Verwendung der webbasierten "BeookSolutions API" angewandt werden. Die Anfragen sollten nun jedoch nicht mehr an `https://tools.vrmarek.me/beook`, sondern an `http://localhost:5000/Solution` gesendet werden.

Eine genauere Dokumentation der API-Endpunkte finden Sie [hier](https://github.com/marekvonrogall/beooksolutions-cli).

## Disclaimer of Affiliation with Ionesoft
This app, "Beook Solutions", is an independent product and is not affiliated with Ionesoft, the publisher and developer of Beook. The use of the word "Beook" does not imply any official approval or partnership with Ionesoft.
