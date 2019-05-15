using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.util
{

	using PvmException = org.camunda.bpm.engine.impl.pvm.PvmException;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	/// @deprecated since 7.4, use slf4j 
	[Obsolete("since 7.4, use slf4j")]
	public class LogUtil
	{

	  public enum ThreadLogMode
	  {
		NONE,
		INDENT,
		PRINT_ID
	  }

	  private static readonly string LINE_SEPARATOR = System.getProperty("line.separator");
	  private static IDictionary<int, string> threadIndents = new Dictionary<int, string>();
	  private static ThreadLogMode threadLogMode = ThreadLogMode.NONE;

	  public static ThreadLogMode getThreadLogMode()
	  {
		return threadLogMode;
	  }

	  public static ThreadLogMode setThreadLogMode(ThreadLogMode threadLogMode)
	  {
		ThreadLogMode old = LogUtil.threadLogMode;
		LogUtil.threadLogMode = threadLogMode;
		return old;
	  }

	  public static void readJavaUtilLoggingConfigFromClasspath()
	  {
		Stream inputStream = ReflectUtil.getResourceAsStream("logging.properties");
		try
		{
		  if (inputStream != null)
		  {
			LogManager.LogManager.readConfiguration(inputStream);

			string redirectCommons = LogManager.LogManager.getProperty("redirect.commons.logging");
			if ((!string.ReferenceEquals(redirectCommons, null)) && (!redirectCommons.Equals("false", StringComparison.OrdinalIgnoreCase)))
			{
			  System.setProperty("org.apache.commons.logging.Log", "org.apache.commons.logging.impl.Jdk14Logger");
			}
		  }
		}
		catch (Exception e)
		{
		  throw new PvmException("couldn't initialize logging properly", e);
		}
		finally
		{
		  IoUtil.closeSilently(inputStream);
		}
	  }

	  private static Format dateFormat = new SimpleDateFormat("HH:mm:ss,SSS");

	  public class LogFormatter : Formatter
	  {

		public virtual string format(LogRecord record)
		{
		  StringBuilder line = new StringBuilder();
		  line.Append(dateFormat.format(DateTime.Now));
		  if (Level.FINE.Equals(record.Level))
		  {
			line.Append(" FIN ");
		  }
		  else if (Level.FINEST.Equals(record.Level))
		  {
			line.Append(" FST ");
		  }
		  else if (Level.INFO.Equals(record.Level))
		  {
			line.Append(" INF ");
		  }
		  else if (Level.SEVERE.Equals(record.Level))
		  {
			line.Append(" SEV ");
		  }
		  else if (Level.WARNING.Equals(record.Level))
		  {
			line.Append(" WRN ");
		  }
		  else if (Level.FINER.Equals(record.Level))
		  {
			line.Append(" FNR ");
		  }
		  else if (Level.CONFIG.Equals(record.Level))
		  {
			line.Append(" CFG ");
		  }

		  int threadId = record.ThreadID;
		  string threadIndent = getThreadIndent(threadId);

		  line.Append(threadIndent);
		  line.Append(" | ");
		  line.Append(record.Message);

		  if (record.Thrown != null)
		  {
			line.Append(LINE_SEPARATOR);

			StringWriter stringWriter = new StringWriter();
			PrintWriter printWriter = new PrintWriter(stringWriter);
			record.Thrown.printStackTrace(printWriter);
			line.Append(stringWriter.ToString());
		  }

		  line.Append("  [");
		  line.Append(record.LoggerName);
		  line.Append("]");

		  line.Append(LINE_SEPARATOR);

		  return line.ToString();
		}

		protected internal static string getThreadIndent(int threadId)
		{
		  int? threadIdInteger = Convert.ToInt32(threadId);
		  if (threadLogMode == ThreadLogMode.NONE)
		  {
			return "";
		  }
		  if (threadLogMode == ThreadLogMode.PRINT_ID)
		  {
			return "" + threadId;
		  }
		  string threadIndent = threadIndents[threadIdInteger];
		  if (string.ReferenceEquals(threadIndent, null))
		  {
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < threadIndents.Count; i++)
			{
			  stringBuilder.Append("  ");
			}
			threadIndent = stringBuilder.ToString();
			threadIndents[threadIdInteger] = threadIndent;
		  }
		  return threadIndent;
		}

	  }

	  public static void resetThreadIndents()
	  {
		threadIndents = new Dictionary<>();
	  }
	}

}