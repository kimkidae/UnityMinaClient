
using com.kkd.game.codes;
namespace com.kkd.game.net
{
    /// <summary>
    /// ClientEventListener
    /// ClientIoHandler로 부터 이벤트를 전달받는다.
    /// ClientController에서 상속받아서 처리한다.
    /// </summary>
	public interface IClientEventListener
	{
        void EventReceive(Protocol protocol, IoBuffer buffer);
	}
}
