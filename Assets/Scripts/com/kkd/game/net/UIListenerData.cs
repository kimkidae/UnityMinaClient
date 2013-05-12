using com.kkd.game.codes;

namespace com.kkd.game.net
{
    /// <summary>
    /// 프로토콜, Listener 저장 객체
    /// </summary>
    public class UIListenerData
	{
        public Protocol Protocol { get; set; }
        public IUIEventListener Listener { get; set; }

        public UIListenerData(Protocol protocol, IUIEventListener listener) {
            this.Protocol = protocol;
            this.Listener = listener;
        }
	}
}
