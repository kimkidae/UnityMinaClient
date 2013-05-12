using com.kkd.game.net;
using com.kkd.game.codes;

namespace com.kkd.game.protocols {
    /// <summary>
    /// 중복 로그인이 발생될 때
    /// 서버로부터 메시지 수신 프로토콜(수신전용)
    /// </summary>
    public class PDuplicateLogin : ClientEventAbstract {
        public override Protocol GetProtocol() {
            return Protocol.DUPLICATE_LOGIN;
        }

        protected override void ReceivePacket(IoBuffer buffer) {
        }

        protected override void EventProcess() {
        }

        protected override byte[] MakePacket() {
            return null;
        }
    }
}
