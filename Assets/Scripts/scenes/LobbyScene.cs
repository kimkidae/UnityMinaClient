using UnityEngine;
using com.kkd.game.net;

public class LobbyScene :SceneAbstract {
    void Start () {
        Debug.Log("LobbyScene Start");
    }

    // Update is called once per frame
	void Update () {
        if( Input.GetButtonDown("Jump") ) {
            
        }
	}

    void OnGUI() {
        //접속 종료시 로그인 화면으로
        if( !controller.getConnector().isConnected() ) {
            Application.LoadLevel("LoginScene");
        }
    }

    public override void addListener() {
    }

    public override void removeListener() {
    }

    public override void perform(ClientEventAbstract clientEvent) {
    }

    public override void SceneDestroy() {
    }

    public override void SceneQuit() {
    }

}
