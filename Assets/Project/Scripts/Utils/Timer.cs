using System;
using UnityEngine;

namespace StartledSeal.Utils
{
    public abstract class Timer
    {
        public bool IsRunning { get; private  set; }
        public float Progress => Time / InitTime;
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected float InitTime;
        protected float Time { get; set; }

        protected Timer(float value)
        {
            InitTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            Time = InitTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public void Pause() => IsRunning = false;

        public void Resume() => IsRunning = true;

        public abstract void Tick(float deltaTime);
    }

    public class CooldownTimer : Timer
    {
        public CooldownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
                Time -= deltaTime;
            
            if (IsRunning && Time <= 0)
                Stop();
        }

        public bool IsFinished => Time <= 0;
        public void Reset() => Time = InitTime;

        public void Reset(float newTime)
        {
            InitTime = newTime;
            Reset();
        }
    }

    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning)
                Time += deltaTime;
        }

        public void Reset() => Time = 0;
        
        public float GetTime => Time;
    }
}