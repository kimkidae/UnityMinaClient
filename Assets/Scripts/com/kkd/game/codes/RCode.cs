using System.Collections.Generic;
using com.kkd.game.net;

namespace com.kkd.game.codes
{
    /// <summary>
    /// 결과코드
    /// 서버의 RCode.java와 대응
    /// </summary>
	public class RCode : Code
	{
        private static readonly Dictionary<short, RCode> codes = new Dictionary<short, RCode>();

        public static RCode NULL = new RCode(0, "NULL");//코드없음
        public static RCode SUCESS = new RCode(1, "SUCESS");
        public static RCode FAIL = new RCode(2, "FAIL");

        public static RCode LOGIN_FAIL = new RCode(3, "로그인 실패");
        public static RCode DUPLICATE_LOGIN = new RCode(4, "중복로그인 발생");

        public RCode(int code, string desc)
            : base(code, desc) {
            codes.Add((short)code, this);
        }

        public static RCode GetCode(short code) {
            if( !codes.ContainsKey(code) ) {
                return RCode.NULL;
            }
            return codes[code];
        }

        public static RCode GetCode(IoBuffer buffer) {
            return GetCode(buffer.getShort());
        }
	}
}
