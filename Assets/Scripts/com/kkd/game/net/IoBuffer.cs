using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace com.kkd.game.net
{
    /// <summary>
    /// 수신받은 Packet의 데이터를 기본형식으로 변환
    /// 
    /// 지원되는 형식은 byte, byte[], char, short, int, long, float, double
    /// </summary>
	public class IoBuffer
	{
        private MemoryStream stream;
        private BinaryReader reader;

        public IoBuffer(byte[] readBuffer) {
            stream = new MemoryStream(readBuffer);
            reader = new BinaryReader(stream);
        }
        
        public byte get(){
            return reader.ReadByte();
        }

        public byte[] get(int count) {
            return reader.ReadBytes(count);
        }

        public char getChar() {
            return reader.ReadChar();
        }

        public short getShort() {
            return reader.ReadInt16();
        }

        public int getInt() {
            return reader.ReadInt32();
        }

        public long getLong() {
            return reader.ReadInt64();
        }

        public float getFloat() {
            return reader.ReadSingle();
        }

        public double getDouble() {
            return reader.ReadDouble();
        }

        public void dispose() {
            try {
                reader.Close();
                stream.Dispose();
            }catch { }
        }
    }
}
