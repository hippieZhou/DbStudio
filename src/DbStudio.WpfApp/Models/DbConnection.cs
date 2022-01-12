using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DbStudio.WpfApp.Models
{
    public class DbConnection : ObservableObject
    {
        public DbConnection()
        {
            if (!Debugger.IsAttached)
                return;
            DataSource = @"10.20.43.90\SQL2017";
            UserId = "sa";
            Password = "95938";
            InitialCatalog = "master";
        }

        private string _dataSource;

        public string DataSource
        {
            get => _dataSource;
            set => SetProperty(ref _dataSource, value);
        }

        private string _userId;

        public string UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _initialCatalog;

        public string InitialCatalog
        {
            get => _initialCatalog;
            set => SetProperty(ref _initialCatalog, value);
        }
    }
}
