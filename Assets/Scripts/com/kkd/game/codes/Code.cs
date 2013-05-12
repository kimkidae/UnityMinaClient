
namespace com.kkd.game.codes
{
    /// <summary>
    /// value와 desc를 가지는 코드 기본
    /// </summary>
	public class Code
	{
        public short Value { get; set; }

        public string Desc { get; set; }

        protected Code(int value, string desc) {
            this.Value = (short)value;
            this.Desc = desc;
        }

        public override string ToString() {
            return "[value:" + Value + ",desc:" + Desc + "]";
        }
	}
}
