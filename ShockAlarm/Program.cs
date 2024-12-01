using ComputerUtils.Logging;
using ShockAlarm;
using ShockAlarm.Server;

Config.LoadConfig();
Config.SaveConfig();
Logger.displayLogInConsole = true;
Webserver s = new();
s.SetupRoutesAndStartServer();