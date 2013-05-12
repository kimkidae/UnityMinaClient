using System;
using System.Text;
using System.IO;
using com.kkd.game.codes;

namespace com.kkd.game.net {
    /// <summary>
    /// 네트워크로 전송할 패킷을 구성한다.
    /// 서버의 Packet.java와 대응한다.
    /// </summary>
    public class Packet {
        private MemoryStream ms;
        private BinaryWriter data;
        private static bool reverseByteOrder;
        private static Encoding encoding;

        public Packet()
            : this(false, 65001) {
            //isLittleEndian : false -> BigEndian으로 처리
            //code page 65001 -> UTF-8
        }

        /// <summary>
        /// 패킷 생성
        /// isLittleEndian : true[LittleEndian], false[BigEndian]
        /// codepage : 인코딩 codepage
        /// </summary>
        /// <param name="isLittleEndian"></param>
        /// <param name="codePage"></param>
        public Packet(bool isLittleEndian, int codePage) {
            ms = new MemoryStream();
            data = new BinaryWriter(ms);
            reverseByteOrder = ( isLittleEndian != BitConverter.IsLittleEndian );
            encoding = Encoding.GetEncoding(codePage);
        }

        //packet에 code등록(Protocol, RCode)
        public Packet addCode(Code code) {
            addShort(code.Value);
            return this;
        }

        public Packet addByte(byte value) {
            data.Write(value);
            return this;
        }

        public Packet addByte(byte[] value) {
            data.Write(value);
            return this;
        }

        public Packet addBoolean(bool value) {
            return value ? addByte(1) : addByte(0);
        }

        public Packet addShort(short value) {
            addByte(byteOrder(BitConverter.GetBytes(value)));
            return this;
        }

        public Packet addInt(int value) {
            addByte(byteOrder(BitConverter.GetBytes(value)));
            return this;
        }

        public Packet addLong(long value) {
            addByte(byteOrder(BitConverter.GetBytes(value)));
            return this;
        }

        public Packet addFloat(float value) {
            addByte(byteOrder(BitConverter.GetBytes(value)));
            return this;
        }

        public Packet addDouble(double value) {
            addByte(byteOrder(BitConverter.GetBytes(value)));
            return this;
        }

        public Packet addString(string value) {
            byte[] str = encoding.GetBytes(value);
            addInt(str.Length);
            addByte(str);
            return this;
        }

        public byte[] getData() {
            byte[] result = ms.ToArray();
            data.Close();
            ms.Close();

            return result;
        }

        //Endian ByteORder
        private byte[] byteOrder(byte[] value) {
            if( reverseByteOrder ) Array.Reverse(value);
            return value;
        }

        public static bool getBoolean(IoBuffer buffer) {
            byte b = buffer.get();
            return b == 0 ? false : true;
        }

        public static String getString(IoBuffer buffer) {
            int len = buffer.getInt();
            if( len == 0 ) {
                return "";
            }else {
                return encoding.GetString(buffer.get(len));
            }
        }
    }
}
