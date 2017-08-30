using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab1Client
{
    class Client : IClient
    {
        #region Fields

        public readonly List<string> _patternCollection;

        private Socket _client;

        #endregion

        #region Construtors

        public Client()
        {
            _patternCollection = new List<string>     
            {
                @"^(?<command>\w*)$",
                @"^(?<command>\w*)\s+'(?<param1>.*):(?<param2>\d*)'$",
                @"^(?<command>\w*)\s+'(?<param1>.*)'$"
            };
        }

        #endregion

        #region Private Methods

        private Match GetMatchByPatterns(string inputString)
        {
            foreach (var pattern in _patternCollection)
            {
                var match = Regex.Match(inputString, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match;
                }
            }

            return null;
        }

        #endregion

        #region IClient Members

        public void SendCommand(string cmd)
        {
            var match = GetMatchByPatterns(cmd);

            var command = match != null
                ? match.Groups["command"].Value.ToUpper()
                : "NULL";
            switch (command)
            {
                case "CONNECT":
                    if (_client != null)
                    {
                        _client.Shutdown(SocketShutdown.Both);
                        _client.Close();
                        _client = null;
                    }

                    var group1 = match.Groups["param1"];
                    var group2 = match.Groups["param2"];
                    if (group1.Success && group2.Success)
                    {
                        var addressStr = group1.Value;
                        var portStr = group2.Value;
                        IPAddress ip;
                        var port = 0;
                        if (IPAddress.TryParse(addressStr, out ip) && int.TryParse(portStr, out port))
                        {
                            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            try
                            {
                                Console.WriteLine("Connecting to '{0}:{1}'", addressStr, portStr);
                                _client.Connect(ip, port);
                                Console.WriteLine("Connected to '{0}:{1}'", addressStr, portStr);
                                Console.WriteLine();
                            }
                            catch
                            {
                                _client = null;
                                Console.WriteLine("Connecting to '{0}:{1}' was failed", addressStr, portStr);
                                Console.WriteLine();
                            }
                            //var fileName = @"d:\ttt.txt";
                            //client.SendFile(fileName);
                            //client.Shutdown(SocketShutdown.Both);
                            //client.Close();
                        }
                        else
                            Console.WriteLine("Address or port not valid");
                    }
                    else
                    {
                        Console.WriteLine("Command without parameters");
                        Console.WriteLine();
                    }

                    break;
                case "UPLOAD":
                    var fileNameGroup = match.Groups["param1"];
                    if (fileNameGroup.Success && (_client != null) && File.Exists(fileNameGroup.Value))
                    {
                        var fi = new FileInfo(fileNameGroup.Value);
                        var uploadCmd = string.Format("{0}, '{1}'{2}", cmd, fi.Length, Environment.NewLine);
                        var uploadCommand = Encoding.ASCII.GetBytes(uploadCmd);
                        _client.Send(uploadCommand);
                        var receivedData = new byte[256];
                        var receivedLength = _client.Receive(receivedData);
                        var result = Encoding.ASCII.GetString(receivedData, 0, receivedLength);
                        var recievedTimeout = 50;
                        if (result.Contains("OK"))
                        {
                            var start = 0;
                            var arr = result.Split('_');
                            if (arr.Length > 1)
                                int.TryParse(arr[1], out start);

                            var step = 1048576;
                            var fileArr = File.ReadAllBytes(fileNameGroup.Value);
                            for (var i = start; i < fileArr.Length; i += step)
                            {
                                var recBuffer = new List<byte>();
                                var byteArr = fileArr.Skip(i).Take(step).ToArray();
                                var continueUpload = string.Format("CONTINUE_UPLOAD '{0}'{1}",
                                    string.Join(" ", byteArr.Select(p => p.ToString("X2"))),
                                    Environment.NewLine);
                                _client.Send(Encoding.ASCII.GetBytes(continueUpload));
                                // Вся эта конструкция необходима, чтоб получить сразу весь буфер
                                _client.ReceiveTimeout = recievedTimeout;
                                while (true)
                                {
                                    try
                                    {
                                        receivedLength = _client.Receive(receivedData);
                                        recBuffer.AddRange(receivedData.Take(receivedLength).ToArray());
                                    }
                                    catch
                                    {
                                        if (recBuffer.Count > 0)
                                            break;
                                    }
                                }

                                result = Encoding.ASCII.GetString(recBuffer.ToArray(), 0, recBuffer.Count);
                                if (result.Contains("OK"))
                                {
                                    var percents = Convert.ToInt32(Math.Round((i / (double)fileArr.Length) * 100));
                                    if (percents > 100)
                                        percents = 100;

                                    Console.WriteLine("Percents: {0} %", percents);
                                }
                                else
                                {
                                    Console.WriteLine("Error uploading file");
                                    Console.WriteLine();
                                    return;
                                }

                            }

                            // FINISH UPLOADING
                            var finishUploadingCommand = string.Format("FINISH_UPLOAD{0}", Environment.NewLine);
                            _client.Send(Encoding.ASCII.GetBytes(finishUploadingCommand));
                            receivedData = new byte[256];
                            var buff = new List<byte>();
                            // Вся эта конструкция необходима, чтоб получить сразу весь буфер
                            _client.ReceiveTimeout = recievedTimeout;
                            while (true)
                            {
                                try
                                {
                                    receivedLength = _client.Receive(receivedData);
                                    buff.AddRange(receivedData.Take(receivedLength).ToArray());
                                }
                                catch
                                {
                                    if (buff.Count > 0)
                                        break;
                                }
                            }
                            //receivedLength = _client.Receive(receivedData);
                            result = Encoding.ASCII.GetString(buff.ToArray(), 0, receivedLength);

                            if (result.Contains("OK"))
                            {
                                Console.WriteLine("Percents: 100 %");
                                Console.WriteLine();
                            }

                            _client.ReceiveTimeout = 0;

                            Console.WriteLine(result);
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine(result);
                            Console.WriteLine();
                        }
                        //_client.SendFile(fileNameGroup.Value);
                    }
                    else
                    {
                        Console.WriteLine("No connection to server or file not found");
                        Console.WriteLine();
                    }

                    break;
                case "TIME":
                    if (_client != null)
                    {
                        var uploadCommand = Encoding.ASCII.GetBytes(cmd + Environment.NewLine);
                        _client.Send(uploadCommand);
                        var receivedData = new byte[256];
                        var receivedLength = _client.Receive(receivedData);
                        var result = Encoding.ASCII.GetString(receivedData, 0, receivedLength);
                        Console.WriteLine(result);
                    }
                    else
                    {
                        Console.WriteLine("No connection to server or file not found");
                        Console.WriteLine();
                    }

                    break;
                case "ECHO":
                    if (_client != null)
                    {
                        var uploadCommand = Encoding.ASCII.GetBytes(cmd + Environment.NewLine);
                        _client.Send(uploadCommand);
                        var receivedData = new byte[256];
                        var receivedLength = _client.Receive(receivedData);
                        var result = Encoding.ASCII.GetString(receivedData, 0, receivedLength);
                        Console.WriteLine(result);
                    }
                    else
                    {
                        Console.WriteLine("No connection to server");
                        Console.WriteLine();
                    }

                    break;
                case "CLOSE":
                    if (_client != null)
                    {
                        var uploadCommand = Encoding.ASCII.GetBytes(cmd + Environment.NewLine);
                        _client.Send(uploadCommand);
                        var receivedData = new byte[256];
                        var receivedLength = _client.Receive(receivedData);
                        if (receivedLength == 0)
                        {
                            _client.Dispose();
                            _client = null;
                            Console.WriteLine("Socket close by Server");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No connection to server or file not found");
                        Console.WriteLine();
                    }

                    break;
                case "QUIT":
                    if (_client != null)
                    {
                        _client.Shutdown(SocketShutdown.Both);
                        _client.Close();
                    }
                    break;
                case "DOWNLOAD":
                    if (_client != null)
                    {
                        var stepDownload = 1048576;
                        var receivedTimeout = 50;
                        var downloadFilePath = match.Groups["param1"];
                        var pathToContinueDownload = @"Download/continue.txt";

                        var directory = Path.GetDirectoryName(pathToContinueDownload);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                            File.WriteAllText(pathToContinueDownload, string.Empty);
                        }

                        // Путь до файла, который уже когда-то начинался записываться
                        var pathToContinueFile = string.Empty;
                        if (File.Exists(pathToContinueDownload))
                        {
                            var lines = File.ReadAllLines(pathToContinueDownload);
                            foreach (var line in lines)
                            {
                                if (line.ToLower().Contains(downloadFilePath.Value.ToLower()))
                                {
                                    var pathToFile = line.Split('|')[1];
                                    if (File.Exists(pathToFile))
                                    {
                                        pathToContinueFile = pathToFile;
                                        break;
                                    }
                                    //else
                                    //    File.Delete(pathToContinueDownload);
                                }
                            }
                        }

                        // Если продолжаем скачивание
                        long currentLength = 0;
                        if (!string.IsNullOrEmpty(pathToContinueFile))
                        {
                            var fiContinueFile = new FileInfo(pathToContinueFile);
                            currentLength = fiContinueFile.Length;
                        }
                        // Иначе
                        else
                        {
                            var ext = Path.GetExtension(downloadFilePath.Value);
                            pathToContinueFile = string.Format("Download/{0}{1}", DateTime.Now.Ticks, ext);
                            directory = Path.GetDirectoryName(pathToContinueFile);
                            if (!Directory.Exists(directory))
                                Directory.CreateDirectory(directory);

                            //File.Create(pathToContinueFile);

                            //File.Create(pathToContinueDownload);
                            using (var stream = new StreamWriter(pathToContinueDownload, true))
                            {
                                stream.WriteLine(string.Format("{0}|{1}", downloadFilePath.Value, pathToContinueFile));
                            }
                            //File.WriteAllText(pathToContinueDownload, );
                        }

                        while (true)
                        {
                            var recBuffer = new List<byte>();
                            //var byteArr = fileArr.Skip(i).Take(step).ToArray();
                            var continueUpload = string.Format("DOWNLOAD '{0}','{1}','{2}'{3}",
                                downloadFilePath.Value,
                                currentLength,
                                stepDownload,
                                Environment.NewLine);
                            _client.Send(Encoding.ASCII.GetBytes(continueUpload));
                            // Вся эта конструкция необходима, чтоб получить сразу весь буфер
                            _client.ReceiveTimeout = receivedTimeout;
                            var receivedLength = 0;
                            var receivedData = new byte[stepDownload];
                            while (true)
                            {
                                try
                                {
                                    receivedLength = _client.Receive(receivedData);
                                    recBuffer.AddRange(receivedData.Take(receivedLength).ToArray());
                                }
                                catch
                                {
                                    if (recBuffer.Count > 0)
                                        break;
                                }
                            }

                            var result = Encoding.ASCII.GetString(recBuffer.ToArray(), 0, recBuffer.Count);
                            if (result.Contains("OK"))
                                break;

                            recBuffer = result
                                .Split(' ')
                                .Select(p => byte.Parse(p, NumberStyles.HexNumber))
                                .ToList();
                            //File.WriteAllBytes(pathToContinueFile, );
                            var bytes = recBuffer.ToArray();
                            using (var stream = new FileStream(pathToContinueFile, FileMode.Append))
                            {
                                stream.Write(bytes, 0, bytes.Length);
                            }

                            currentLength += bytes.Length;

                            recBuffer = null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No connection to server or file not found");
                        Console.WriteLine();
                    }

                    break;
                default:
                    Console.WriteLine("Command \"{0}\" is unknow", command);
                    break;
            }
        }

        #endregion
    }
}
