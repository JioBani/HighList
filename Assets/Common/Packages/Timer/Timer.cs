using UnityEngine;

namespace Common.Packages.Timer
{
    public class Timer
    {
        private TimerManager timerManager;
        public TimerStatus status { get; private set; }
        private float currentTime = 0;
        public float maxTime  { get; private set; }
        public bool isOver { get; private set; } = false;

        public Timer(TimerManager timerManager, float maxTime)
        {
            this.timerManager = timerManager;
            status = TimerStatus.Stopped;
            this.maxTime = maxTime;
        }

        public Timer Start()
        {
            status = TimerStatus.Running;
            isOver = false;
            
            return this;
        }

        public void Stop()
        {
            currentTime = maxTime;
            isOver = false;
        }

        public void Pause()
        {
            status = TimerStatus.Paused;
        }

        public void Reset()
        {
            Stop();
        }

        // 시간 진행
        public void Update(float deltaTime)
        {
            currentTime -= deltaTime;

            if (currentTime < 0)
            {
                currentTime = 0;
                isOver = true;
            }
        }

        public void Delete()
        {
            timerManager.DeleteTimer(this);
        }
    }
}
