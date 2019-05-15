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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

	public class ShellActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal new static readonly BpmnBehaviorLogger LOG = ProcessEngineLogger.BPMN_BEHAVIOR_LOGGER;

	  protected internal Expression command;
	  protected internal Expression wait;
	  protected internal Expression arg1;
	  protected internal Expression arg2;
	  protected internal Expression arg3;
	  protected internal Expression arg4;
	  protected internal Expression arg5;
	  protected internal Expression outputVariable;
	  protected internal Expression errorCodeVariable;
	  protected internal Expression redirectError;
	  protected internal Expression cleanEnv;
	  protected internal Expression directory;

	  internal string commandStr;
	  internal string arg1Str;
	  internal string arg2Str;
	  internal string arg3Str;
	  internal string arg4Str;
	  internal string arg5Str;
	  internal string waitStr;
	  internal string resultVariableStr;
	  internal string errorCodeVariableStr;
	  internal bool? waitFlag;
	  internal bool? redirectErrorFlag;
	  internal bool? cleanEnvBoolan;
	  internal string directoryStr;

	  private void readFields(ActivityExecution execution)
	  {
		commandStr = getStringFromField(command, execution);
		arg1Str = getStringFromField(arg1, execution);
		arg2Str = getStringFromField(arg2, execution);
		arg3Str = getStringFromField(arg3, execution);
		arg4Str = getStringFromField(arg4, execution);
		arg5Str = getStringFromField(arg5, execution);
		waitStr = getStringFromField(wait, execution);
		resultVariableStr = getStringFromField(outputVariable, execution);
		errorCodeVariableStr = getStringFromField(errorCodeVariable, execution);

		string redirectErrorStr = getStringFromField(redirectError, execution);
		string cleanEnvStr = getStringFromField(cleanEnv, execution);

		waitFlag = string.ReferenceEquals(waitStr, null) || waitStr.Equals("true");
		redirectErrorFlag = !string.ReferenceEquals(redirectErrorStr, null) && redirectErrorStr.Equals("true");
		cleanEnvBoolan = !string.ReferenceEquals(cleanEnvStr, null) && cleanEnvStr.Equals("true");
		directoryStr = getStringFromField(directory, execution);

	  }

	  public override void execute(ActivityExecution execution)
	  {

		readFields(execution);

		IList<string> argList = new List<string>();
		argList.Add(commandStr);

		if (!string.ReferenceEquals(arg1Str, null))
		{
		  argList.Add(arg1Str);
		}
		if (!string.ReferenceEquals(arg2Str, null))
		{
		  argList.Add(arg2Str);
		}
		if (!string.ReferenceEquals(arg3Str, null))
		{
		  argList.Add(arg3Str);
		}
		if (!string.ReferenceEquals(arg4Str, null))
		{
		  argList.Add(arg4Str);
		}
		if (!string.ReferenceEquals(arg5Str, null))
		{
		  argList.Add(arg5Str);
		}

		ProcessBuilder processBuilder = new ProcessBuilder(argList);

		try
		{
		  processBuilder.redirectErrorStream(redirectErrorFlag);
		  if (cleanEnvBoolan.Value)
		  {
			IDictionary<string, string> env = processBuilder.environment();
			env.Clear();
		  }
		  if (!string.ReferenceEquals(directoryStr, null) && directoryStr.Length > 0)
		  {
			processBuilder.directory(new File(directoryStr));
		  }

		  Process process = processBuilder.start();

		  if (waitFlag.Value)
		  {
			int errorCode = process.waitFor();

			if (!string.ReferenceEquals(resultVariableStr, null))
			{
			  string result = convertStreamToStr(process.InputStream);
			  execution.setVariable(resultVariableStr, result);
			}

			if (!string.ReferenceEquals(errorCodeVariableStr, null))
			{
			  execution.setVariable(errorCodeVariableStr, Convert.ToString(errorCode));

			}

		  }
		}
		catch (Exception e)
		{
		  throw LOG.shellExecutionException(e);
		}

		leave(execution);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String convertStreamToStr(java.io.InputStream is) throws java.io.IOException
	  public static string convertStreamToStr(Stream @is)
	  {

		if (@is != null)
		{
		  Writer writer = new StringWriter();

		  char[] buffer = new char[1024];
		  try
		  {
			Reader reader = new StreamReader(@is, Encoding.UTF8);
			int n;
			while ((n = reader.read(buffer)) != -1)
			{
			  writer.write(buffer, 0, n);
			}
		  }
		  finally
		  {
			@is.Close();
		  }
		  return writer.ToString();
		}
		else
		{
		  return "";
		}
	  }

	  protected internal virtual string getStringFromField(Expression expression, DelegateExecution execution)
	  {
		if (expression != null)
		{
		  object value = expression.getValue(execution);
		  if (value != null)
		  {
			return value.ToString();
		  }
		}
		return null;
	  }

	}

}