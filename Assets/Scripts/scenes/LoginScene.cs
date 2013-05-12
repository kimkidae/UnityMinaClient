using UnityEngine;
using com.kkd.game.protocols;
using com.kkd.game.net;
using com.kkd.game.codes;
using com.kkd.game.log;
/// <summary>
/// 로그인 화면
/// </summary>
public class LoginScene :SceneAbstract {
    private string email = "";//계정
    private string password = "";//패스워드
    private string mesage = "아이디/ 패스워드를 입력해주세요.";//message box

    //로그인 상태 WAIT: 대기, SUCESS 성공, FAIL 실패, DUPLICATE_LOGIN 중복로그인
    private enum LoginStatus {
        WAIT, SUCESS, FAIL, DUPLICATE_LOGIN
    }

    //로그인 상태 Field
    private LoginStatus status;

    void Start() {
        Debug.Log("LoginScene Start");

        //init
        status = LoginStatus.WAIT;
        controller = ClientController.Instance;
        addListener();

        Log.game.Debug("Game Started");
    }

    //가운데 정렬 박스
    private Rect CenterRect(int left, int top, int width, int height) {
        int screenHWidth = Screen.width / 2 - ( width / 2 );
        int screenHHeight = Screen.height / 2 - ( height / 2 );

        return new Rect(screenHWidth + left, screenHHeight + top , width, height);
    }

    void OnGUI() {
        GUI.Box(CenterRect(0, -80, 300, 50), "유니티클라+자바서버 샘플");
        GUI.Box(CenterRect(0, -30, 300, 25), mesage);
        email = GUI.TextField(CenterRect(0, 0, 150, 25), email, 30);
        password = GUI.TextField(CenterRect(0, 25, 150, 25), password, 30);

        //로그인 버튼 클릭시 처리 프로토콜 호출
        if( GUI.Button(CenterRect(0, 55, 50, 25), "login") ) {
            PUserLogin login = new PUserLogin();
            login.sendLogin(email, password);
        }

        //로그인 상태 변경
        if( status == LoginStatus.SUCESS ) {
            Application.LoadLevel("LobbyScene");
        }else if( status == LoginStatus.FAIL ) {
            mesage = "이메일 또는 패스워드가 맞지 않습니다.";
            status = LoginStatus.WAIT;
        }else if( status == LoginStatus.DUPLICATE_LOGIN ) {
            mesage = "중복 로그인으로 인한 실패";
            status = LoginStatus.WAIT;
        }
    }

    /// <summary>
    /// 프로토콜 UI 리스너 등록
    /// </summary>
    public override void addListener() {
        ClientController.Instance.AddUIListener(Protocol.USER_LOGIN, this);
        ClientController.Instance.AddUIListener(Protocol.DUPLICATE_LOGIN, this);
    }

    /// <summary>
    /// 프로토콜 UI 리스너 제외
    /// </summary>
    public override void removeListener() {
        ClientController.Instance.RemoveUIListener(Protocol.USER_LOGIN, this);
        ClientController.Instance.RemoveUIListener(Protocol.DUPLICATE_LOGIN, this);
    }

    /// <summary>
    /// 프로토콜 결과 수신
    /// ClientIoHandler -> IClientEventListener -> IUIEventListener.perform
    /// </summary>
    /// <param name="gameEvent"></param>
    public override void perform(ClientEventAbstract clientEvent) {
        //로그인 프로토콜
        if( clientEvent.GetProtocol() == Protocol.USER_LOGIN ) {
            if( clientEvent.GetHResult() == RCode.SUCESS ) {
                status = LoginStatus.SUCESS;
            } else {
                status = LoginStatus.FAIL;
            }
        //중복 로그인 프로토콜
        } else if( clientEvent.GetProtocol() == Protocol.DUPLICATE_LOGIN ) {
            status = LoginStatus.DUPLICATE_LOGIN;
        }
    }

    public override void SceneDestroy() {
    }

    public override void SceneQuit() {
    }
}
