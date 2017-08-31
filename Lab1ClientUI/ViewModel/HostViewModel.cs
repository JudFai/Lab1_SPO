using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Lab1Client;
using Lab1ClientUI.Model;
using Microsoft.Win32;

namespace Lab1ClientUI.ViewModel
{
    public class HostViewModel : ViewModelBase
    {
        #region Fields

        private CommandClient _selectedCommandClient;
        private DateTime? _serverTime;
        private IPEndPoint _serverIPEndPoint;
        private RelayCommand _connectCommand;
        private IClientWorker _clientWorker;
        private LogMessageCollection _logMessageCollection;
        private bool _isConnecting;
        private bool _connected;
        private RelayCommand _disconnectCommand;
        private List<CommandClient> _commandClientCollection;
        private RelayCommand _sendCommand;
        private string _receivedServerMessage;
        private string _sendServerMessage;
        private string _pathToUploadFile;
        private RelayCommand _uploadPathCommand;
        private int _progress;

        #endregion

        #region Properties

        public IPEndPoint ServerIPEndPoint
        {
            get { return _serverIPEndPoint; }
            set
            {
                _serverIPEndPoint = value;
                RaisePropertyChanged(() => ServerIPEndPoint);
            }
        }

        public IClientWorker ClientWorker
        {
            get { return _clientWorker; }
            set
            {
                _clientWorker = value;
                RaisePropertyChanged(() => ClientWorker);
            }
        }

        public LogMessageCollection LogMessageCollection
        {
            get { return _logMessageCollection; }
            set
            {
                _logMessageCollection = value;
                RaisePropertyChanged(() => LogMessageCollection);
            }
        }

        public CommandClient SelectedCommandClient
        {
            get { return _selectedCommandClient; }
            set
            {
                _selectedCommandClient = value;
                RaisePropertyChanged(() => SelectedCommandClient);
            }
        }

        public List<CommandClient> CommandClientCollection { get; set; }

        public DateTime? ServerTime
        {
            get { return _serverTime; }
            set
            {
                _serverTime = value;
                RaisePropertyChanged(() => ServerTime);
            }
        }

        public string ReceivedServerMessage
        {
            get { return _receivedServerMessage; }
            set
            {
                _receivedServerMessage = value;
                RaisePropertyChanged(() => ReceivedServerMessage);
            }
        }

        public string SendServerMessage
        {
            get { return _sendServerMessage; }
            set
            {
                _sendServerMessage = value;
                RaisePropertyChanged(() => SendServerMessage);
            }
        }

        public string PathToUploadFile
        {
            get { return _pathToUploadFile; }
            set
            {
                _pathToUploadFile = value;
                RaisePropertyChanged(() => PathToUploadFile);
            }
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set
            {
                _isConnecting = value;
                DispatcherHelper.CheckBeginInvokeOnUI(() => RaisePropertyChanged(() => IsConnecting));
            }
        }

        public bool Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                DispatcherHelper.CheckBeginInvokeOnUI(() => RaisePropertyChanged(() => Connected));
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        #endregion

        #region Commands

        public RelayCommand ConnectCommand
        {
            get
            {
                return _connectCommand ??
                       (_connectCommand = new RelayCommand(Connect, () => ServerIPEndPoint != null));
            }
        }

        public RelayCommand DisconnectCommand
        {
            get
            {
                return _disconnectCommand ??
                       (_disconnectCommand = new RelayCommand(Disconnect));
            }
        }

        public RelayCommand SendCommand
        {
            get
            {
                return _sendCommand ??
                       (_sendCommand = new RelayCommand(Send));
            }
        }

        public RelayCommand UploadPathCommand
        {
            get
            {
                return _uploadPathCommand ??
                       (_uploadPathCommand = new RelayCommand(UploadPath));
            }
        }

        #endregion

        #region Constructors

        public HostViewModel()
        {
            DispatcherHelper.Initialize();
            _serverIPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.13.205"), 10001);
            _logMessageCollection = new LogMessageCollection();
            CommandClientCollection = new List<CommandClient>
            {
                CommandClient.Donwload,
                CommandClient.Echo,
                CommandClient.Time,
                CommandClient.Upload
            };
        }

        #endregion

        #region Command Methods

        private void Connect()
        {
            Task.Factory.StartNew(() =>
            {
                IsConnecting = true;
                ClientWorker = new ClientWorker();
                ClientWorker.Client.MessageReceived += OnMessageReceived;
                ClientWorker.Client.ProgressChanged += OnProgressChanged;
                Connected = (bool)ClientWorker.Send(CommandClient.Connect, ServerIPEndPoint);
                IsConnecting = false;
            });
        }

        private void Disconnect()
        {
            Task.Factory.StartNew(() =>
            {
                if ((bool) ClientWorker.Send(CommandClient.Close))
                {
                    Connected = false;
                    ClientWorker.Client.MessageReceived -= OnMessageReceived;
                    ClientWorker.Dispose();
                    ClientWorker = null;
                }
            });
        }

        private void Send()
        {
            Task.Factory.StartNew(() =>
            {
                switch (SelectedCommandClient)
                {
                    case CommandClient.Time:
                        ServerTime = (DateTime?)ClientWorker.Send(SelectedCommandClient);
                        break;
                    case CommandClient.Echo:
                        ReceivedServerMessage = (string)ClientWorker.Send(SelectedCommandClient, SendServerMessage);
                        break;
                    case CommandClient.Upload:
                        ClientWorker.Send(SelectedCommandClient, PathToUploadFile);
                        break;
                }
            });
        }

        private void UploadPath()
        {
            var openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = path;
            openFileDialog.Title = "Выбрать файл для записи на сервер";
            //openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog().GetValueOrDefault(false))
                PathToUploadFile = openFileDialog.FileName;
        }

        #endregion

        #region Private Methods

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            LogMessageCollection.Add(new LogMessage(e.Time, e.Message));
            RaisePropertyChanged(() => LogMessageCollection);
        }

        private void OnProgressChanged(object sender, ProgressEventArgs e)
        {
            Progress = e.Progress;
        }

        #endregion

    }
}
