using com.kkd.game.log;
using com.kkd.game.codes;
using System.Collections.Generic;
using System.Threading;

namespace com.kkd.game.net
{
    /// <summary>
    /// 클라이언트 기본 컨트롤러
    /// ,IClientEventListener로써 IoHandler에서 전달받는 이벤트를 처리하기도 한다.
    /// 처리흐름 : ClientIoHandler -> [IClientEventListener] -> ClientEventAbstract -> IUIEventListener
    /// </summary>
    public class ClientController :IClientEventListener
	{
        //Client 컨트롤러(Signleton Patter)
        private static ClientController controller;//컨트롤러
        private static object syncObj = new object();

        //TCP Connnector
        private TCPClientConnector connector = new TCPClientConnector();

        //클라이언트 네트워크 수신 handler
        private ClientIoHandler handler = new ClientIoHandler();

        //클라이언트 UI 이벤트 처리를 위한 객채들
        private object eventLock= new object();
        private List<ClientEventAbstract> clientEvents = new List<ClientEventAbstract>();
        private object listenerLock = new object();
        private List<UIListenerData> uiListeners = new List<UIListenerData>();

        //임시
        private List<ClientEventAbstract> copyClientEvents = new List<ClientEventAbstract>();
        private List<UIListenerData> copyListeners = new List<UIListenerData>();

        //Singleton constructor
        private ClientController(){

            //네트워크 Handler 설정
            connector.SetIoHandler(handler);
            //이벤트 listener 등록 
            handler.SetClientEventListener(this);

            //UI listener 처리용 Thread
            new Thread(DoUiEvent).Start();
            //세션 유지용 Thread
            new Thread(DoSessionCheck).Start();
        }

        public static ClientController Instance{
            get {
                if( controller == null ) {
                    lock( syncObj ) {
                        if( controller == null )
                            controller = new ClientController();
                    }
                }
                return controller;
            }
        }

        /// <summary>
        /// TCPConnector 반환
        /// </summary>
        /// <returns></returns>
        public TCPClientConnector getConnector() {
            return connector;
        }

        /// <summary>
        /// IClieventListener.EventReceive
        /// 처리흐름 : ClientIoHandler -> [IClientEventListener] -> ClientEventAbstract -> IUIEventListener
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="buffer"></param>
        public void EventReceive(Protocol protocol, IoBuffer buffer) {
            Log.game.Debug("controller.EventReceived : " + protocol.Desc);

            //프로토콜에 해당하는 ClientEvent 반환
            ClientEventAbstract clientEvent = ClientEventFactory.getEvent(protocol);
            if( clientEvent == null ) return;
            
            try{
                //이벤트 처리
                clientEvent.Set(buffer);
            }catch{
            }finally{
                //UIEventListener에 전달하기 위해 등록
                AddClientEvent(clientEvent);
            }
        }

        /// <summary>
        /// 세션채크, 5초마다 서버와 연결 확인
        /// 30초이상 지연되면 서버에서 로그아웃 처리된다.
        /// </summary>
        private void DoSessionCheck() {
            //세션 체크용 프로토콜 패킷 생성
            byte[] checkPacket = new Packet().addCode(Protocol.CHECK).getData();

            while( true ) {
                if( connector.isConnected() ) 
                    connector.Write(checkPacket);
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// UIListener Callback을 위해 list에 등록해둔다.
        /// 이후 doUiEvent Thread에서 하나씩 꺼내 처리 한다.
        /// </summary>
        /// <param name="ClientEventAbstract"></param>
        public void AddClientEvent(ClientEventAbstract clientEvent) {
            Log.game.Debug("GameEvent added : " + clientEvent.GetProtocol().Desc);

            lock( eventLock ) {
                if( clientEvents.Contains(clientEvent) ) return;
                clientEvents.Add(clientEvent);
                //이벤트 발생 대기중인 Thread를 해제한다
                Monitor.Pulse(eventLock);
            }
        }

        /// <summary>
        /// 이벤트가 발생하면 UI Listener로 callback 처리 하는 Thread
        /// </summary>
        private void DoUiEvent() {
            while( true ) {
                lock( eventLock ) {
                    //이벤트가 발생할 떄까지 wait 처리
                    if( clientEvents.Count == 0 ) Monitor.Wait(eventLock);
                    copyClientEvents.AddRange(clientEvents);
                    foreach( ClientEventAbstract clientEvent in copyClientEvents ) {
                        CallBack(clientEvent);
                        clientEvents.Remove(clientEvent);
                    }
                    copyClientEvents.Clear();
                }
            }
        }

        /// <summary>
        /// UI Listener callBack
        /// </summary>
        /// <param name="ClientEventAbstract"></param>
        private void CallBack(ClientEventAbstract clientEvent) {
            Log.game.Debug("UIEvent CallBack : " + clientEvent.GetProtocol().Desc);
            if( uiListeners.Count == 0 ) return;
            lock( listenerLock ) {
                if( uiListeners.Count == 0 ) return;
                foreach( UIListenerData data in uiListeners ) {
                    if( data.Protocol == clientEvent.GetProtocol() ) {
                        data.Listener.perform(clientEvent);
                    }
                }
            }
        }

        /// <summary>
        /// UI 리스너 등록
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="listener"></param>
        public void AddUIListener(Protocol protocol, IUIEventListener listener) {
            lock( listenerLock ) {
                uiListeners.Add(new UIListenerData(protocol, listener));
            }
        }

        /// <summary>
        /// UI 리스너 제외
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="listener"></param>
        public void RemoveUIListener(Protocol protocol, IUIEventListener listener) {
            lock( listenerLock ) {
                copyListeners.AddRange(uiListeners);

                foreach( UIListenerData data in copyListeners ) {
                    if( data.Protocol != protocol ) continue;
                    if( data.Listener == listener ) {
                        uiListeners.Remove(data);
                    }
                }
                copyListeners.Clear();
            }
        }
    }
}
