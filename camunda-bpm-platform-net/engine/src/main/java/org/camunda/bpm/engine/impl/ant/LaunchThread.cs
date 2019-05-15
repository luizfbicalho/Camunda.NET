using System;
using System.Text;
using System.Threading;
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
namespace org.camunda.bpm.engine.impl.ant
{

	using BuildException = org.apache.tools.ant.BuildException;
	using Task = org.apache.tools.ant.Task;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class LaunchThread : Thread
	{

	  internal Task task;
	  internal string[] cmd;
	  internal File dir;
	  internal string msg;

	  public LaunchThread(Task task, string[] cmd, File dir, string msg)
	  {
		this.task = task;
		this.cmd = cmd;
		this.dir = dir;
		this.msg = msg;
	  }

	  public static void launch(Task task, string[] cmd, File dir, string launchCompleteText)
	  {
		if (cmd == null)
		{
		  throw new BuildException("cmd is null");
		}
		try
		{
		  LaunchThread launchThread = new LaunchThread(task, cmd, dir, launchCompleteText);
		  launchThread.Start();
		  launchThread.Join();
		}
		catch (Exception e)
		{
		  throw new BuildException("couldn't launch cmd: " + cmdString(cmd), e);
		}
	  }

	  private static string cmdString(string[] cmd)
	  {
		StringBuilder cmdText = new StringBuilder();
		foreach (string cmdPart in cmd)
		{
		  cmdText.Append(cmdPart);
		  cmdText.Append(" ");
		}
		return cmdText.ToString();
	  }

	  public virtual void run()
	  {
		task.log("launching cmd '" + cmdString(cmd) + "' in dir '" + dir + "'");
		if (!string.ReferenceEquals(msg, null))
		{
		  task.log("waiting for launch completion msg '" + msg + "'...");
		}
		else
		{
		  task.log("not waiting for a launch completion msg.");
		}
		ProcessBuilder processBuilder = (new ProcessBuilder(cmd)).redirectErrorStream(true).directory(dir);

		Stream consoleStream = null;
		try
		{
		  Process process = processBuilder.start();

		  consoleStream = process.InputStream;
		  StreamReader consoleReader = new StreamReader(consoleStream);
		  string consoleLine = "";
		  while ((!string.ReferenceEquals(consoleLine, null)) && (string.ReferenceEquals(msg, null) || consoleLine.IndexOf(msg, StringComparison.Ordinal) == -1))
		  {
			consoleLine = consoleReader.ReadLine();

			if (!string.ReferenceEquals(consoleLine, null))
			{
			  task.log("  " + consoleLine);
			}
			else
			{
			  task.log("launched process completed");
			}
		  }
		}
		catch (Exception e)
		{
		  throw new BuildException("couldn't launch " + cmdString(cmd), e);
		}
		finally
		{
		  IoUtil.closeSilently(consoleStream);
		}
	  }
	}

}