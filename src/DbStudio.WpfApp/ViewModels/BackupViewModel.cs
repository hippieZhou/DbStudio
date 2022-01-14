using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.Input;
using DbStudio.Application.Features.DataBase.Commands;

namespace DbStudio.WpfApp.ViewModels
{
    public partial class BackupViewModel : ViewModelBase
    {
        protected override void OnActivated()
        {
            Messenger.Register<BackupViewModel, CurrentConnChangedMessage, string>(
                this,
                nameof(ShellViewModel),
                (vm, conn) => vm.CurrentConn = conn.Value);
            base.OnActivated();
        }
    }
    /// <summary>
    /// 备份
    /// </summary>
    public partial class BackupViewModel : ViewModelBase
    {
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

        private IRelayCommand _selectPhysicalDirectoryCommand;

        public IRelayCommand SelectPhysicalDirectoryCommand =>
            _selectPhysicalDirectoryCommand ??= new RelayCommand(SelectPhysicalDirectory);

        private void SelectPhysicalDirectory()
        {
            var openFile = new FolderBrowserDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                PhysicalDirectory = openFile.SelectedPath;
            }
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

    /// <summary>
    /// 还原
    /// </summary>
    public partial class BackupViewModel : ViewModelBase
    {
        private string _physicalFilePath;

        /// <summary>
        /// 物理文件
        /// </summary>
        public string PhysicalFilePath
        {
            get => _physicalFilePath;
            set => SetProperty(ref _physicalFilePath, value);
        }

        private string _newPhysicalDirectory;

        /// <summary>
        /// 物理路径
        /// </summary>
        public string NewPhysicalDirectory
        {
            get => _physicalDirectory;
            set => SetProperty(ref _newPhysicalDirectory, value);
        }

        private IRelayCommand _selectPhysicalFileCommand;

        public IRelayCommand SelectPhysicalFileCommand =>
            _selectPhysicalFileCommand ??= new RelayCommand(SelectPhysicalFile);

        private void SelectPhysicalFile()
        {

        }


        private IRelayCommand _selectNewPhysicalDirectoryCommand;
        public IRelayCommand SelectNewPhysicalDirectoryCommand =>
            _selectNewPhysicalDirectoryCommand ??= new RelayCommand(SelectNewPhysicalDirectory);

        private void SelectNewPhysicalDirectory()
        {

        }

        private IAsyncRelayCommand _dataBaseRestoreCommand;

        public IAsyncRelayCommand DataBaseRestoreCommand =>
            _dataBaseRestoreCommand ??= new AsyncRelayCommand(DataBaseRestoreAsync);

        private async Task DataBaseRestoreAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
        }
    }

    /// <summary>
    /// 还原空库
    /// </summary>
    public partial class BackupViewModel : ViewModelBase
    {
        private string _emptyDbPhysicalDirectory;

        /// <summary>
        /// 物理路径
        /// </summary>
        public string EmptyDbPhysicalDirectory
        {
            get => _physicalDirectory;
            set => SetProperty(ref _emptyDbPhysicalDirectory, value);
        }

        private string _emptyDbName;
        public string EmptyDbName
        {
            get => _emptyDbName;
            set => SetProperty(ref _emptyDbName, value);
        }
        private string _emptyLogName;
        public string EmptyLogName
        {
            get => _emptyLogName;
            set => SetProperty(ref _emptyLogName, value);
        }

        private IRelayCommand _selectEmptyDbPhysicalDirectoryCommand;

        public IRelayCommand SelectEmptyDbPhysicalDirectoryCommand =>
            _selectEmptyDbPhysicalDirectoryCommand ??= new RelayCommand(SelectEmptyDbPhysicalDirectory);

        private void SelectEmptyDbPhysicalDirectory()
        {
            var openFile = new FolderBrowserDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                EmptyDbPhysicalDirectory = openFile.SelectedPath;
            }
        }

        private IAsyncRelayCommand _restoreEmptyDbCommand;

        public IAsyncRelayCommand RestoreEmptyDbCommand =>
            _restoreEmptyDbCommand ??= new AsyncRelayCommand(RestoreEmptyDbAsync);

        private async Task RestoreEmptyDbAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
        }
    }
}