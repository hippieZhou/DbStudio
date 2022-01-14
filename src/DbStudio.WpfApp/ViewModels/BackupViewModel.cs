using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DbStudio.Application.Features.DataBase.Commands;

namespace DbStudio.WpfApp.ViewModels
{
    public class BackupViewModel : ViewModelBase
    {
        protected override void OnActivated()
        {
            Messenger.Register<BackupViewModel, CurrentConnChangedMessage, string>(
                this,
                nameof(ShellViewModel),
                (vm, conn) => vm.CurrentConn = conn.Value);
            base.OnActivated();
        }

        private string _physicalDirectory;

        /// <summary>
        /// 物理路径
        /// </summary>
        public string PhysicalDirectory
        {
            get => _physicalDirectory;
            set => SetProperty(ref _physicalDirectory, value);
        }

        private bool _enableDiff;

        /// <summary>
        /// 是否启用差异备份
        /// </summary>
        public bool EnableDiff
        {
            get => _enableDiff;
            set => SetProperty(ref _enableDiff, value);
        }

        private string _physicalFilePath;

        /// <summary>
        /// 物理文件
        /// </summary>
        public string PhysicalFilePath
        {
            get => _physicalFilePath;
            set => SetProperty(ref _physicalFilePath, value);
        }

        private IRelayCommand _selectPhysicalDirectoryCommand;

        public IRelayCommand SelectPhysicalDirectoryCommand =>
            _selectPhysicalDirectoryCommand ??= new RelayCommand(SelectPhysicalDirectory);

        private void SelectPhysicalDirectory()
        {
            //var openFile = new FolderBrowserDialog();
            //if (openFile.ShowDialog() == DialogResult.OK)
            //{
            //    PhysicalDirectory = openFile.SelectedPath;
            //}
        }

        private IAsyncRelayCommand _dataBaseBackupCommand;

        public IAsyncRelayCommand DataBaseBackupCommand =>
            _dataBaseBackupCommand ??= new AsyncRelayCommand(DataBaseBackupAsync);

        private async Task DataBaseBackupAsync(CancellationToken cancellationToken)
        {
            var request = new DataBaseBackupCommand
            {
                DataSource = CurrentConn.DataSource,
                UserId = CurrentConn.UserId,
                Password = CurrentConn.Password,
                InitialCatalog = CurrentConn.InitialCatalog,
                PhysicalDirectory = PhysicalDirectory,
                EnableDiff = EnableDiff
            };

            var response = await ExecuteOnUILoadingAsync(request, cancellationToken);
            if (response != null)
            {
                Message.Success($"【{request.InitialCatalog}】备份成功，备份路径为：{response.Data.FullName}");
            }
        }
    }
}