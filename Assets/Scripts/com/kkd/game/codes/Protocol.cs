
using System.Collections.Generic;
namespace com.kkd.game.codes
{
    /// <summary>
    /// 프로토콜 코드
    /// 서버의 Protocol.java와 대응
    /// </summary>
	public class Protocol : Code
	{
        private static readonly Dictionary<short, Protocol> codes = new Dictionary<short, Protocol>();

        public static Protocol NULL = new Protocol(0, "프로토콜 없음");
        public static Protocol CHECK = new Protocol(1, "SESSION CHECK");
        public static Protocol USER_LOGIN = new Protocol(2, "유저로그인");
        public static Protocol DUPLICATE_LOGIN = new Protocol(3, "중복로그인");

        public Protocol(int code, string desc)
            : base(code, desc) {
            codes.Add((short)code, this);
        }

        public static Protocol GetCode(short code) {
            if( !codes.ContainsKey(code) ) {
                return Protocol.NULL;
            }
            return codes[code];
        }
	}
}
