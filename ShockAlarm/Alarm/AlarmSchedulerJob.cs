using ComputerUtils.Logging;
using Microsoft.EntityFrameworkCore;
using Quartz;
using ShockAlarm.Database;
using ShockAlarm.Server;

namespace ShockAlarm.Alarm;



public class AlarmSchedulerJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Logger.Log("Checking alarms");
        List<Alarm> alarms = AlarmServer.GetAllAlarms();
        DateTime now = DateTime.UtcNow;
        bool changed = false;
        using (AppDbContext d = new ())
        {
            foreach (Alarm alarm in alarms)
            {
                d.Attach(alarm);
                if(alarm.TriggerIfApplicable(now)) changed = true;
            }
            if(changed) d.SaveChanges();
        }
        return Task.CompletedTask;
    }
}