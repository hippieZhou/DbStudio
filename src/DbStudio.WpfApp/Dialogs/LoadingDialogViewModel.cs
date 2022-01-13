using System;
using DbStudio.WpfApp.ViewModels;
using HandyControl.Tools.Extension;

namespace DbStudio.WpfApp.Dialogs
{
    public class LoadingDialogViewModel : ViewModelBase, IDialogResultable<bool>
    {
        public bool Result { get; set; }
        public Action CloseAction { get; set; }
    }
}