using com.kkd.game.protocols;
using com.kkd.game.codes;

namespace com.kkd.game.net
{
	public class ClientEventFactory
	{
        /// <summary>
        /// 프로토콜에 해당하는 ClientEvent 반환
        /// </summary>
        public static ClientEventAbstract getEvent(Protocol protocol) {
            if( protocol == Protocol.USER_LOGIN ) return new PUserLogin();
            if( protocol == Protocol.DUPLICATE_LOGIN ) return new PDuplicateLogin();

            return null;
        }
	}
}
