using com.kkd.game.log;
using com.kkd.game.codes;

namespace com.kkd.game.net
{
	public class ClientIoHandler
	{
        private IClientEventListener listener;

        /// <summary>
        /// 이벤트를 전달 할 ClientEventListener 등록
        /// </summary>
        /// <param name="listener"></param>
        public void SetClientEventListener(IClientEventListener listener) {
            this.listener = listener;
        }

        /// <summary>
        /// 네트워크로부터 받는 메시지 처리
        /// 프로토콜을 분석한 후 IClientEventListener 로 전달한다.
        /// 처리흐름 : [ClientIoHandler] -> IClientEventListener -> ClientEventAbstract
        /// </summary>
        /// <param name="buffer"></param>
        public void MessageReceived(IoBuffer buffer) {
            Protocol protocol = Protocol.GetCode(buffer.getShort());

            Log.game.Debug("handler.MessageReceived.protocol : " + protocol.Value);

            //EvnetListener로 넘긴다
            listener.EventReceive(protocol, buffer);
        }
	}
}
