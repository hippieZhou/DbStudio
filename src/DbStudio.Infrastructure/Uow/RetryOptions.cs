namespace DbStudio.Infrastructure.Uow
{
    public class RetryOptions
    {
        public int MaxRetries { get; private set; }
        public int WaitMillis { get; private set; }

        public RetryOptions()
        {
            this.MaxRetries = 3;
            this.WaitMillis = 100;
        }


        public RetryOptions(int maxRetries, int waitMillis, bool enabled)
        {
            this.MaxRetries = maxRetries;
            this.WaitMillis = waitMillis;
        }
    }
}