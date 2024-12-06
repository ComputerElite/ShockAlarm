using ComputerUtils.Webserver;
using Quartz;
using Quartz.Impl;
using ShockAlarm.Alarm;

namespace ShockAlarm.Server;

public class Webserver
{
    public HttpServer Server = new ();

    public void SetupRoutesAndStartServer()
    {
        Server.defaultResponseHeaders.Add("Access-Control-Allow-Credentials", "true");
        Server.autoServeOptions = true;
        Server.MaxWebsocketMessageSize = 1024 * 1024 * 5;
        UserManagementServer.AddUsermanagementEndpoints(Server);
        FrontendServer.AddFrontendRoutes(Server);
        AlarmServer.AddRoutes(Server);
        IScheduler s = StdSchedulerFactory.GetDefaultScheduler().Result;
        s.Start();
        IJobDetail job = JobBuilder.Create<AlarmSchedulerJob>().Build();
        ITrigger trigger = TriggerBuilder.Create().WithIdentity("UpdateAlarmStatus").StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever()).Build();
        s.ScheduleJob(job, trigger);
        Server.StartServer(Config.Instance.port);
    }
}