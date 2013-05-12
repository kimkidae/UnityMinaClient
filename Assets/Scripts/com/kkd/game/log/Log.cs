using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace com.kkd.game.log
{
	class Log
	{
        public static ILog game = LogManager.GetLogger(typeof(Log));

	}
}
