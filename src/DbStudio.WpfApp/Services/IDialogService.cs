namespace DbStudio.WpfApp.Services
{
    public interface IDialogService
    {
        void Success(string message, string caption = "系统提示");
        void Error(string message, string caption = "系统提示");
    }
}