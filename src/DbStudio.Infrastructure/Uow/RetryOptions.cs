namespace DbStudio.Infrastructure.Uow
{
    public class RetryOptions
    {
        public int MaxRetries { get; private set; }
        public int WaitMillis { get; private set; }
        //public bool Enabled { get; private set; }

        public RetryOptions()
        {
            this.MaxRetries = 3;
            this.WaitMillis = 100;
            //this.Enabled = true;
        }

        //public RetryOptions(bool Enabled)
        //{
        //    this.MaxRetries = 3;
        //    this.WaitMillis = 100;
        //    //this.Enabled = Enabled;
        //}

        public RetryOptions(int maxRetries, int waitMillis, bool enabled)
        {
            this.MaxRetries = maxRetries;
            this.WaitMillis = waitMillis;
            //this.Enabled = Enabled;
        }

    }
}