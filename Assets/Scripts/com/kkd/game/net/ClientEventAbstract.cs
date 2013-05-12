
using com.kkd.game.codes;
namespace com.kkd.game.net
{
    /// <summary>
    /// 네트워크로 부터 받은 Packet을 처리 하는 최상위 추상클래스
    /// 클라이언트 프로토콜 생성시 무조건 상속 받아서 사용해야 한다.
    /// </summary>
	public abstract class ClientEventAbstract
	{
        protected ClientController controller;
        protected RCode hResult = RCode.SUCESS;

        /// <summary>
        /// 결과코드 반환
        /// </summary>
        /// <returns></returns>
        public RCode GetHResult(){
            return hResult;
        }

        public ClientEventAbstract() {
            controller = ClientController.Instance;
        }

        /// <summary>
        /// ClientEvent를 처리하는곳
        /// 처리흐름 : ClientIoHandler -> IClientEventListener -> [ClientEventAbstract] -> ClientEvent
        /// </summary>
        /// <param name="buffer"></param>
        public void Set(IoBuffer buffer) {
            ReceivePacket(buffer);
            EventProcess();
        }

        //server로 protocol 전송 
        protected void Send() {
            controller.getConnector().Write(MakePacket());
        }

        //event의 protocol 설정
        public abstract Protocol GetProtocol();

        //수신된 packet 처리
        protected abstract void ReceivePacket(IoBuffer buffer);

        //수신 프로토콜 처리
        protected abstract void EventProcess();

        //server로 전송할 packet을 반환한다
        protected abstract byte[] MakePacket();
	}
}
