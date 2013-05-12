using System;
using com.kkd.game.net;
using com.kkd.game.log;
using com.kkd.game.codes;

namespace com.kkd.game.protocols
{
    /// <summary>
    /// 유저 로그인 프로토콜
    /// </summary>
    public class PUserLogin :ClientEventAbstract
	{
        private string email;
        private string password;

        public override Protocol GetProtocol() {
            return Protocol.USER_LOGIN;
        }

        /// <summary>
        /// 수신된 패킷 처리
        /// </summary>
        /// <param name="buffer"></param>
        protected override void ReceivePacket(IoBuffer buffer) {
            hResult = RCode.GetCode(buffer);
        }

        /// <summary>
        /// 이벤트 최종 처리
        /// </summary>
        protected override void EventProcess() {
            if( hResult == RCode.SUCESS ) {
                Log.game.Debug("로그인성공");
            }else{
                Log.game.Debug("로그인 실패 " + hResult.Desc);
                controller.getConnector().Disconnect();
            }
        }

        /// <summary>
        /// 서버에 전달할 Packet 반환
        /// </summary>
        /// <returns></returns>
        protected override byte[] MakePacket() {
            Packet p = new Packet();
            p.addCode(GetProtocol());
            p.addString(email);
            p.addString(password);
            return p.getData();
        }

        /// <summary>
        /// 로그인 프로토콜 전송
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void sendLogin(String email, String password) {
            this.email = email;
            this.password = password;

            ConnectServer();

            Send();
        }

        /// <summary>
        /// 서버 연결
        /// </summary>
        public void ConnectServer() {
            try {
                controller.getConnector().Connect("localhost", 9090);
            }
            catch( Exception ) { }
        }
	}
}
