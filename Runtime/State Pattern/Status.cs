using UnityEngine;

namespace com.eyerunnman.patterns
{
    public class Status
    {
        bool status;
        public bool Enabled { get => status; set => status = value; }
        public bool Disabled { get => !status; set => status = !value; }

        public Status()
        {
            status = true;
        }

        public void Enable()
        {
            Enabled = true;
        }
        public void Disable()
        {
            Disabled = true;
        }
    }

    public class FrameData<T> where T : struct
    {
        private T currentFrame;
        private T previousFrame;

        public FrameData(){
            currentFrame = new();
            previousFrame = new();
        }
        public FrameData(T defaultValue)
        {
            currentFrame = defaultValue;
            previousFrame = defaultValue;
        }

        public void UpdatePreviousFrame(T newValue)
        {
            previousFrame = currentFrame;
            currentFrame = newValue;
        }
        public void UpdatePreviousFrame()
        {
            previousFrame = currentFrame;
        }

        public T CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public T PreviousFrame
        {
            get { return previousFrame; }
            set { previousFrame = value; }
        }
    }


}


