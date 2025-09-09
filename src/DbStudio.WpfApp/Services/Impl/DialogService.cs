using HandyControl.Controls;

namespace DbStudio.WpfApp.Services.Impl
{
    public class DialogService : IDialogService
    {
        public void Success(string message, string caption = "系统提示")
        {
            MessageBox.Success(message, caption);
        }

        public void Error(string message, string caption = "系统提示")
        {
            MessageBox.Error(message, caption);
        }
    }
}