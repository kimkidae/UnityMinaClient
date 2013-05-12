using System;
using System.Net.Sockets;
using com.kkd.game.log;
using System.IO;

namespace com.kkd.game.net
{
    /// <summary>
    /// TCPClient를 이용한 Tcp통신
    /// </summary>
    public class TCPClientConnector
	{
        const int SEND_BUFFER_SIZE = 256 * 1024;
        const int RECEIVE_BUFFER_SIZE = 256 * 1024;

        private TcpClient client;
        private byte[] readBuffer = new byte[RECEIVE_BUFFER_SIZE];

        //IO처리 핸들러
        private ClientIoHandler handler;

        //ClientStream
        private Stream stream;

        public void SetIoHandler(ClientIoHandler handler) {
            this.handler = handler;
        }

        public void Connect(string sNetIP, int iPORT_NUM) {
            try {
                //기존 연결 disconnect
                Disconnect();

                //TcpClient 생성
                client = new TcpClient(sNetIP, iPORT_NUM);

                //send, receive Buffer size 지정
                client.SendBufferSize = SEND_BUFFER_SIZE;
                client.ReceiveBufferSize = RECEIVE_BUFFER_SIZE;

                //비동기 read 시작
                client.GetStream().BeginRead(readBuffer, 0, RECEIVE_BUFFER_SIZE, new AsyncCallback(MessageReceived), null);

                Log.game.Debug("Connected");
            }
            catch( Exception ex ) {
                if( Log.game.IsDebugEnabled )
                    Log.game.Debug("Connection Fail : " + ex.ToString());
            }
        }

        /// <summary>
        /// TcpClient 연결 종료 
        /// </summary>
        public void Disconnect() {
            if( client != null ) {
                Log.game.Debug("Disconnected");

                if( client.Connected ) {
                    stream.Close();
                    client.Close();
                }
            }
        }

        public bool isConnected() {
            if( client != null ) {
                return client.Connected;
            }
            return false;
        }

        // send message to server.
        public void Write(byte[] tmp) {
            stream = client.GetStream();
            stream.Write(tmp, 0, tmp.Length);
            stream.Flush();

            if( Log.game.IsDebugEnabled )
                Log.game.Debug("send bytes : " + tmp.Length);
        }

        /// <summary>
        /// 메시지 수신
        /// </summary>
        /// <param name="ar"></param>
        public void MessageReceived(IAsyncResult ar) {
            int bytesRead;
            try {
                bytesRead = client.GetStream().EndRead(ar);

                if(Log.game.IsDebugEnabled)
                    Log.game.Debug("receive bytes: " + bytesRead);

                //Server disconnected
                if( bytesRead < 1 ) {
                    Disconnect();
                    return;
                }

                IoBuffer buffer = new IoBuffer(readBuffer);

                handler.MessageReceived(buffer);

                buffer.dispose();

                // start a new asynchronous read
                client.GetStream().BeginRead(readBuffer, 0, RECEIVE_BUFFER_SIZE, new AsyncCallback(MessageReceived), null);
            }
            catch {
                Disconnect();
            }
        }


	}
}
