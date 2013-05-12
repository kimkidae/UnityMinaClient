using com.kkd.game.net;

namespace com.kkd.game.net {
    /// <summary>
    /// UI(Scene)에서 프로토콜 이용시 이벤트 수신을 위해 상속받아서 사용한다.
    /// </summary>
    public interface IUIEventListener {

        void addListener();

        void removeListener();

        void perform(ClientEventAbstract clientEvent);

    }
}
