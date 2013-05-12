using UnityEngine;
using com.kkd.game.protocols;
using com.kkd.game.net;
using com.kkd.game.codes;
using com.kkd.game.log;
/// <summary>
/// �α��� ȭ��
/// </summary>
public class LoginScene :SceneAbstract {
    private string email = "";//����
    private string password = "";//�н�����
    private string mesage = "���̵�/ �н����带 �Է����ּ���.";//message box

    //�α��� ���� WAIT: ���, SUCESS ����, FAIL ����, DUPLICATE_LOGIN �ߺ��α���
    private enum LoginStatus {
        WAIT, SUCESS, FAIL, DUPLICATE_LOGIN
    }

    //�α��� ���� Field
    private LoginStatus status;

    void Start() {
        Debug.Log("LoginScene Start");

        //init
        status = LoginStatus.WAIT;
        controller = ClientController.Instance;
        addListener();

        Log.game.Debug("Game Started");
    }

    //��� ���� �ڽ�
    private Rect CenterRect(int left, int top, int width, int height) {
        int screenHWidth = Screen.width / 2 - ( width / 2 );
        int screenHHeight = Screen.height / 2 - ( height / 2 );

        return new Rect(screenHWidth + left, screenHHeight + top , width, height);
    }

    void OnGUI() {
        GUI.Box(CenterRect(0, -80, 300, 50), "����ƼŬ��+�ڹټ��� ����");
        GUI.Box(CenterRect(0, -30, 300, 25), mesage);
        email = GUI.TextField(CenterRect(0, 0, 150, 25), email, 30);
        password = GUI.TextField(CenterRect(0, 25, 150, 25), password, 30);

        //�α��� ��ư Ŭ���� ó�� �������� ȣ��
        if( GUI.Button(CenterRect(0, 55, 50, 25), "login") ) {
            PUserLogin login = new PUserLogin();
            login.sendLogin(email, password);
        }

        //�α��� ���� ����
        if( status == LoginStatus.SUCESS ) {
            Application.LoadLevel("LobbyScene");
        }else if( status == LoginStatus.FAIL ) {
            mesage = "�̸��� �Ǵ� �н����尡 ���� �ʽ��ϴ�.";
            status = LoginStatus.WAIT;
        }else if( status == LoginStatus.DUPLICATE_LOGIN ) {
            mesage = "�ߺ� �α������� ���� ����";
            status = LoginStatus.WAIT;
        }
    }

    /// <summary>
    /// �������� UI ������ ���
    /// </summary>
    public override void addListener() {
        ClientController.Instance.AddUIListener(Protocol.USER_LOGIN, this);
        ClientController.Instance.AddUIListener(Protocol.DUPLICATE_LOGIN, this);
    }

    /// <summary>
    /// �������� UI ������ ����
    /// </summary>
    public override void removeListener() {
        ClientController.Instance.RemoveUIListener(Protocol.USER_LOGIN, this);
        ClientController.Instance.RemoveUIListener(Protocol.DUPLICATE_LOGIN, this);
    }

    /// <summary>
    /// �������� ��� ����
    /// ClientIoHandler -> IClientEventListener -> IUIEventListener.perform
    /// </summary>
    /// <param name="gameEvent"></param>
    public override void perform(ClientEventAbstract clientEvent) {
        //�α��� ��������
        if( clientEvent.GetProtocol() == Protocol.USER_LOGIN ) {
            if( clientEvent.GetHResult() == RCode.SUCESS ) {
                status = LoginStatus.SUCESS;
            } else {
                status = LoginStatus.FAIL;
            }
        //�ߺ� �α��� ��������
        } else if( clientEvent.GetProtocol() == Protocol.DUPLICATE_LOGIN ) {
            status = LoginStatus.DUPLICATE_LOGIN;
        }
    }

    public override void SceneDestroy() {
    }

    public override void SceneQuit() {
    }
}
