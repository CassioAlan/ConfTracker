//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ConfSpider.BLL.ProcessComunicator
//{
//    public class PComunicator
//    {
//        #region Members
//        private Thread m_processThreadWrite;
//        private Thread m_processThreadRead;
//        private Queue<string> m_mensagens;
//        private Process m_process;
//        #endregion

//        #region Constructors
//        #endregion

//        #region Events
//        private void process_Exited(object sender, EventArgs e)
//        {
//            string _erro = m_process.StandardError.ReadToEnd();
//        }

//        private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
//        {
//        }

//        protected virtual void OnReceivedMessage(string message)
//        {
//            if (ReceivedMessage != null)
//                ReceivedMessage(this, new ProcessComunicatorEventArgs(message));
//        }
//        #endregion

//        #region Delegates

//        public event EventHandler<ProcessComunicatorEventArgs> ReceivedMessage;

//        #endregion

//        #region Methods
//        public void ExecCommand(string command, string arguments)
//        {
//            m_mensagens = new Queue<string>();

//            m_process = new Process();
//            m_process.StartInfo.FileName = command;
//            m_process.StartInfo.Arguments = arguments;

//            m_process.StartInfo.UseShellExecute = false;
//            m_process.StartInfo.RedirectStandardOutput = true;
//            m_process.StartInfo.RedirectStandardInput = true;
//            m_process.StartInfo.RedirectStandardError = true;
//            m_process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
//            m_process.StartInfo.CreateNoWindow = false;
//            m_process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
//            m_process.Exited += process_Exited;
//            m_process.Start();

//            m_processThreadWrite = new Thread(RunWrite);

//            m_processThreadRead = new Thread(RunRead);

//            m_processThreadWrite.IsBackground = true;
//            m_processThreadRead.IsBackground = true;

//            m_processThreadWrite.Start();
//            m_processThreadRead.Start();
//        }
        
//        private void RunWrite()
//        {
//            while (true)
//            {
//                Stream _sOut = this.m_process.StandardInput.BaseStream;
//                {
//                    string enviarMensagem = null;
//                    if (m_mensagens.Count > 0)
//                    {
//                        enviarMensagem = m_mensagens.Dequeue();
//                    }
//                    else
//                    {
//                        Thread.Sleep(200);
//                    }

//                    if (!string.IsNullOrEmpty(enviarMensagem))
//                    {
//                        byte[] mensagemBytes = Encoding.UTF8.GetBytes(enviarMensagem);
//                        byte[] mensagemLengthBytes = BitConverter.GetBytes(mensagemBytes.Length);

//                        _sOut.Write(mensagemLengthBytes, 0, mensagemLengthBytes.Length);
//                        _sOut.Write(mensagemBytes, 0, mensagemBytes.Length);
//                        _sOut.Flush();
//                    }
//                }
//            }
//        }

//        private void RunRead()
//        {
//            while (true)
//            {
//                Stream sIn = this.m_process.StandardOutput.BaseStream;
//                {
//                    byte[] inputSizeData = new byte[4];

//                    for (int i = 0; i < 4; i++)
//                    {
//                        inputSizeData[i] = (byte)sIn.ReadByte();
//                    }

//                    int inputSize = BitConverter.ToInt32(inputSizeData, 0);

//                    if (inputSize <= 0)
//                    {
//                        Thread.Sleep(200);
//                        continue;
//                    }

//                    byte[] inputBytes = new byte[inputSize];
//                    sIn.Read(inputBytes, 0, inputSize);

//                    string inputText = Encoding.UTF8.GetString(inputBytes);

//                    if (!string.IsNullOrEmpty(inputText))
//                        OnReceivedMessage(inputText);
//                }
//            }
//        }
//        #endregion
//    }
//}
