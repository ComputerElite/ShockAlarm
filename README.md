# ShockAlarm
A UI for setting alarms for [OpenShock](https://openshock.app).

## Installation
1. `git clone https://github.com/ComputerElite/ShockAlarm.git`
2. `cd ShockAlarm`
3. `git submodule update --init --recursive`
4. `docker compose up --build`
5. Open `http://localhost:14008` in your browser.

Voila, it's set up!

## Usage
1. Open the app.
2. Create an account by entering a username and password then pressing register
3. Now enter an api token into the token field and press add
4. After that create alarms and you're done

## Features
- [x] Schedule alarms for specific times and toggle weekdays
- [x] Set alarms for own and **share** shockers
- [x] Create alarm tones (patterns of what to do on a shocker)
- [x] Copy components of alarm tone
- [x] Select alarm tone
- [x] Preview alarm behavior on shocker


## Planned features
- [ ] Pretty up UI
- [ ] Update shocker limits when you edit an alarm

## UI as of 2024-12-02
![ShockAlarm UI example](ShockAlarm/images/preview.jpg)