using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lab1.Server
{
    class MessageManager : IMessageManager
    {
        #region Fields

        private static readonly object _instanceLocker = new object();
        private static IMessageManager _instance;
        private readonly IMessageConverter _messageConverter;
        private readonly IDataConverter _dataConverter;
        private readonly IFileManager _fileManager;
        private IUploadingFile _currentUploadingFile;

        #endregion

        #region Constructors

        private MessageManager(IDataConverter dataConverter, IFileManager fileManager)
        {
            _fileManager = fileManager;
            _dataConverter = dataConverter;
            _messageConverter = MessageConverter.Instance;
        }

        #endregion

        #region Public Methods

        public static IMessageManager GetInstance(IDataConverter dataConverter, IFileManager fileManager)
        {
            lock (_instanceLocker)
            {
                return _instance ??
                        (_instance = new MessageManager(dataConverter, fileManager));
            }
        }

        #endregion

        #region IMessageManager Members

        public ServerMessage Interpret(IServer server, IClient client, ClientMessage clientMessage)
        {
            var data = string.Empty;
            switch (clientMessage.ClientCommand)
            {
                case ClientCommand.Time:
                    data = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                    break;
                case ClientCommand.Error:
                    data = "ERROR";
                    break;
                case ClientCommand.Echo:
                    var firstParameter = clientMessage.CommandParameters.FirstOrDefault();
                    if (firstParameter != null)
                        data = (string)firstParameter;
                    else
                        data = "EMPTY";

                    break;
                case ClientCommand.Close:
                    try
                    {
                        client.Dispose();
                        Console.WriteLine("Client '{0}' was disconnected", client.Address);
                        Console.WriteLine();
                        data = "SUCCESS";
                    }
                    catch
                    {
                        data = "ERROR";
                    }
                    break;
                case ClientCommand.BeginUpload:
                    var pathToFile = clientMessage.CommandParameters[0];
                    var fileSize = clientMessage.CommandParameters[1];
                    if ((pathToFile != null) && (fileSize != null) && (_currentUploadingFile == null))
                    {
                        IUploadingFile uploadingFile = new UploadingFile(pathToFile.ToString(), client, Convert.ToInt32(fileSize));
                        //data = server.ChangeReceivingModeFile(uploadingFile) 
                        //    ? "SUCCESS" 
                        //    : "UPLOADING_WAS_ALREADY";
                        _currentUploadingFile = uploadingFile;
                        data = "OK";
                    }
                    else if (_currentUploadingFile != null)
                    {
                        IUploadingFile uploadingFile = new UploadingFile(pathToFile.ToString(), client, Convert.ToInt32(fileSize));
                        if (uploadingFile.Equals(_currentUploadingFile))
                        {
                            data = string.Format("OK_{0}", _currentUploadingFile.CurrentBytes.Count);
                        }
                        else
                        {
                            _currentUploadingFile = uploadingFile;
                            data = "OK";
                        }
                    }
                    else
                        data = "ERROR";

                    break;
                case ClientCommand.ContinueUpload:
                    var fileData = clientMessage.CommandParameters[0];
                    if (_currentUploadingFile == null)
                        data = "UPLOAD_NOT_BEGAN";
                    else if (fileData != null)
                    {
                        var bytes = clientMessage
                            .CommandParameters[0]
                            .ToString().Split(' ')
                            .Select(p => byte.Parse(p, NumberStyles.HexNumber));
                        _currentUploadingFile.CurrentBytes.AddRange(bytes);
                        var percents = _currentUploadingFile.GetLoadingPercentage();
                        if (_currentUploadingFile.CurrentBytes.Count <= _currentUploadingFile.Size)
                        {
                            Console.WriteLine("Uploading percentage: {0} %", percents);
                            Console.WriteLine();
                            data = "OK";
                        }
                        else
                        {
                            _currentUploadingFile = null;
                            data = "INCORRECT_SIZE_OF_FILE";
                        }

                    }
                    else
                        data = "ERROR";

                    break;
                case ClientCommand.FinishUpload:
                    if (_currentUploadingFile.CurrentBytes.Count == _currentUploadingFile.Size)
                    {
                        _fileManager.SaveFile(_currentUploadingFile);
                        data = "OK";
                    }
                    else if (_currentUploadingFile.CurrentBytes.Count > _currentUploadingFile.Size)
                        data = "BUFFER_SIZE_GREATER_THAN_FILE_SIZE";
                    else
                        data = "BUFFER_SIZE_LESS_THAN_FILE_SIZE";

                    _currentUploadingFile = null;

                    break;
                case ClientCommand.Download:
                    var pathToDownloadingFile = clientMessage.CommandParameters[0];
                    var index = clientMessage.CommandParameters[1];
                    var length = clientMessage.CommandParameters[2];
                    if ((pathToDownloadingFile != null) && (index != null))
                    {
                        var path = pathToDownloadingFile.ToString();
                        if (File.Exists(path))
                        {
                            var offset = Convert.ToInt32(index.ToString());
                            var maxBytes = Convert.ToInt32(length.ToString());
                            var bytes = File.ReadAllBytes(path).Skip(offset).Take(maxBytes);
                            if (bytes.Any())
                                data = string.Join(" ", bytes.Select(p => p.ToString("X2")));
                            else
                                data = "OK";
                        }
                        else
                            data = "FILE_NOT_FOUND";
                    }
                    else
                        data = "ERROR";
                    break;
                case ClientCommand.FileSize:
                    var pathToFileForFileSize = clientMessage.CommandParameters[0];
                    if (pathToFileForFileSize != null)
                    {
                        var path = pathToFileForFileSize.ToString();
                        if (File.Exists(path))
                        {
                            var fi = new FileInfo(path);
                            data = path.Length.ToString();
                        }
                        else
                            data = "FILE_NOT_FOUND";
                    }

                    data = "ERROR";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return _messageConverter.Convert(data, clientMessage.ClientCommand);
        }

        #endregion
    }
}
