using System;
using System.Collections.Generic;
using System.Linq;
using Common.Packages.SceneSingleton;
using UnityEngine;

namespace Common.Packages.Timer
{
    public class TimerManager : SceneSingleton<TimerManager>
    {
        private List<Timer> timers = new List<Timer>();
        
        public Timer make(float maxTime)
        {
            Timer timer = new Timer(this, maxTime);
            timers.Add(timer);
            
            return timer;
        }

        private void FixedUpdate()
        {
            foreach (var timer in timers.Where(timer => timer.status == TimerStatus.Running))
            {
                timer.Update(Time.fixedDeltaTime);
            }
        }

        public void DeleteTimer(Timer timer)
        {
            timers.Remove(timer);
        }
    }
}