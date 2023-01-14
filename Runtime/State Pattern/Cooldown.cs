using UnityEngine;

namespace com.eyerunnman.patterns
{
    public class Cooldown
    {
        private readonly float duration;
        private float elapsedTime;
        private bool isCooldownCompleted;

        public Cooldown(float duration)
        {
            this.duration = duration;
            isCooldownCompleted = true;
            elapsedTime = duration;
        }

        public void OnUpdate()
        {
            if (isCooldownCompleted is false)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= duration)
                {
                    isCooldownCompleted = true;
                    elapsedTime = duration;
                }
            }
        }

        public void OnReset()
        {
            elapsedTime = 0f;
            isCooldownCompleted = false;
        }

        public bool IsCooldownAvailable => isCooldownCompleted;
        public float RemainingTime => (duration - elapsedTime);
    }
}


