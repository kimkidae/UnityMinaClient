using UnityEngine;
using com.kkd.game.net;
using com.kkd.game.log;

/// <summary>
/// 프로토콜을 주고 받는 Scene의 추상 클래스
/// </summary>
public abstract class SceneAbstract :MonoBehaviour, IUIEventListener {
    protected ClientController controller;//client Controller

    public SceneAbstract() {
        controller = ClientController.Instance;
    }

    /// <summary>
    /// 프로토콜 UI 리스너 등록
    /// </summary>
    public abstract void addListener();

    /// <summary>
    /// 프로토콜 UI 리스너 제외
    /// </summary>
    public abstract void removeListener();

    /// <summary>
    /// 프로토콜 결과 수신
    /// ClientIoHandler -> IClientEventListener -> IUIEventListener.perform
    /// </summary>
    /// <param name="gameEvent"></param>
    public abstract void perform(ClientEventAbstract clientEvent);

    /// <summary>
    /// 어플리케이션 종료시 연결 종료 처리 
    /// </summary>
    public void OnApplicationQuit() {
        controller.getConnector().Disconnect();

        SceneQuit();
    }

    /// <summary>
    /// Scene 전환시 등록된 lister제거
    /// </summary>
    public void OnDestroy() {
        removeListener();

        SceneDestroy();
    }

    /// <summary>
    /// OnApplicationQuit 구현부분
    /// </summary>
    public abstract void SceneQuit();

    /// <summary>
    /// OnDestroy 구현부분
    /// </summary>
    public abstract void SceneDestroy();

}

