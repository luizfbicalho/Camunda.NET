using System;
using System.Collections.Generic;

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


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class LaunchTask : Task
	{

	  private static readonly string FILESEPARATOR = System.getProperty("file.separator");

	  internal File dir;
	  internal string script;
	  internal string msg;
	  internal string args;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute() throws org.apache.tools.ant.BuildException
	  public virtual void execute()
	  {
		if (dir == null)
		{
		  throw new BuildException("dir attribute is required with the launch task");
		}
		if (string.ReferenceEquals(script, null))
		{
		  throw new BuildException("script attribute is required with the launch task");
		}

		string[] cmd = null;
		string executable = Executable;
		if (!string.ReferenceEquals(args, null))
		{
		  IList<string> pieces = new List<string>();
		  pieces.Add(executable);
		  StringTokenizer tokenizer = new StringTokenizer("args", " ");
		  while (tokenizer.hasMoreTokens())
		  {
			pieces.Add(tokenizer.nextToken());
		  }
		  cmd = pieces.ToArray();

		}
		else
		{
		  cmd = new string[]{executable};
		}

		LaunchThread.launch(this,cmd,dir,msg);
	  }

	  public virtual string Executable
	  {
		  get
		  {
			string os = System.getProperty("os.name").ToLower();
			string dirPath = dir.AbsolutePath;
			string @base = dirPath + FILESEPARATOR + script;
			if (exists(@base))
			{
			  return @base;
			}
    
			if (os.IndexOf("windows", StringComparison.Ordinal) != -1)
			{
			  if (exists(@base + ".exe"))
			  {
				return @base + ".exe";
			  }
			  if (exists(@base + ".bat"))
			  {
				return @base + ".bat";
			  }
			}
    
			if (os.IndexOf("linux", StringComparison.Ordinal) != -1 || os.IndexOf("mac", StringComparison.Ordinal) != -1)
			{
			  if (exists(@base + ".sh"))
			  {
				return @base + ".sh";
			  }
			}
    
			throw new BuildException("couldn't find executable for script " + @base);
		  }
	  }

	  public virtual bool exists(string path)
	  {
		File file = new File(path);
		return (file.exists());
	  }

	  public virtual File Dir
	  {
		  set
		  {
			this.dir = value;
		  }
	  }

	  public virtual string Script
	  {
		  set
		  {
			this.script = value;
		  }
	  }

	  public virtual string Msg
	  {
		  set
		  {
			this.msg = value;
		  }
	  }

	  public virtual string Args
	  {
		  set
		  {
			this.args = value;
		  }
	  }
	}

}