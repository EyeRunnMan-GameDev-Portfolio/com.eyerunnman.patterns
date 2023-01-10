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


}


