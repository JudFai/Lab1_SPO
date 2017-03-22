using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1
{
    class MessageManager : IMessageManager
    {
        #region Fields

        private static readonly object _instanceLocker = new object();
        private readonly IMessageConverter _messageConverter;

        private static IMessageManager _locker;

        #endregion

        #region Properties

        public static IMessageManager Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    return _locker ??
                           (_locker = new MessageManager());
                }
            }
        }

        #endregion

        #region Constructors

        private MessageManager()
        {
            _messageConverter = MessageConverter.Instance;
        }

        #endregion

        #region IMessageManager Members

        public ServerMessage Interpret(ISocketListener listener, ClientMessage clientMessage)
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
                        listener.Dispose();
                        data = "SUCCESS";
                    }
                    catch
                    {
                        data = "ERROR";
                    }
                    break;
                case ClientCommand.Upload:
                    var pathToFile = clientMessage.CommandParameters.FirstOrDefault();
                    if (pathToFile != null)
                    {
                        var ext = string.Empty;
                        try
                        {
                            ext = Path.GetExtension((string)pathToFile);
                            if (!listener.ReceivingFileMode)
                            {
                                listener.ChangeModeToReceivingFile(ext);
                                data = "SUCCESS";
                            }
                            else
                            {
                                data = "UPLOADING_WAS_ALREADY_ERROR";
                            }
                        }
                        catch
                        {
                            data = "ERROR";
                        }
                    }
                    else
                    {
                        data = "ERROR";
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }

            return _messageConverter.Convert(data, clientMessage.ClientCommand);
        }

        #endregion
    }
}
