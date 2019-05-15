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
namespace org.camunda.bpm.engine.test.standalone.variablescope
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// @author Christian Lipphardt
	/// 
	/// </summary>
	public class VariableScopeTest : PluggableProcessEngineTestCase
	{

	  /// <summary>
	  /// A testcase to produce and fix issue ACT-862.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testVariableNamesScope()
	  public virtual void testVariableNamesScope()
	  {

		// After starting the process, the task in the subprocess should be active
		IDictionary<string, object> varMap = new Dictionary<string, object>();
		varMap["test"] = "test";
		varMap["helloWorld"] = "helloWorld";
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("simpleSubProcess", varMap);
		Task subProcessTask = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();
		runtimeService.setVariableLocal(pi.ProcessInstanceId, "mainProcessLocalVariable", "Hello World");

		assertEquals("Task in subprocess", subProcessTask.Name);

		runtimeService.setVariableLocal(subProcessTask.ExecutionId, "subProcessLocalVariable", "Hello SubProcess");

		// Returns a set of local variablenames of pi
		IList<string> result = processEngineConfiguration.CommandExecutorTxRequired.execute(new GetVariableNamesCommand(this, pi.ProcessInstanceId, true));

		// pi contains local the variablenames "test", "helloWorld" and "mainProcessLocalVariable" but not "subProcessLocalVariable"
		assertTrue(result.Contains("test"));
		assertTrue(result.Contains("helloWorld"));
		assertTrue(result.Contains("mainProcessLocalVariable"));
		assertFalse(result.Contains("subProcessLocalVariable"));

		// Returns a set of global variablenames of pi
		result = processEngineConfiguration.CommandExecutorTxRequired.execute(new GetVariableNamesCommand(this, pi.ProcessInstanceId, false));

		// pi contains global the variablenames "test", "helloWorld" and "mainProcessLocalVariable" but not "subProcessLocalVariable"
		assertTrue(result.Contains("test"));
		assertTrue(result.Contains("mainProcessLocalVariable"));
		assertTrue(result.Contains("helloWorld"));
		assertFalse(result.Contains("subProcessLocalVariable"));

		// Returns a set of local variablenames of subProcessTask execution
		result = processEngineConfiguration.CommandExecutorTxRequired.execute(new GetVariableNamesCommand(this, subProcessTask.ExecutionId, true));

		// subProcessTask execution contains local the variablenames "test", "subProcessLocalVariable" but not "helloWorld" and "mainProcessLocalVariable"
		assertTrue(result.Contains("test")); // the variable "test" was set locally by SetLocalVariableTask
		assertTrue(result.Contains("subProcessLocalVariable"));
		assertFalse(result.Contains("helloWorld"));
		assertFalse(result.Contains("mainProcessLocalVariable"));

		// Returns a set of global variablenames of subProcessTask execution
		result = processEngineConfiguration.CommandExecutorTxRequired.execute(new GetVariableNamesCommand(this, subProcessTask.ExecutionId, false));

		// subProcessTask execution contains global all defined variablenames
		assertTrue(result.Contains("test")); // the variable "test" was set locally by SetLocalVariableTask
		assertTrue(result.Contains("subProcessLocalVariable"));
		assertTrue(result.Contains("helloWorld"));
		assertTrue(result.Contains("mainProcessLocalVariable"));

		taskService.complete(subProcessTask.Id);
	  }

	  /// <summary>
	  /// A command to get the names of the variables
	  /// @author Roman Smirnov
	  /// @author Christian Lipphardt
	  /// </summary>
	  private class GetVariableNamesCommand : Command<IList<string>>
	  {
		  private readonly VariableScopeTest outerInstance;


		internal string executionId;
		internal bool isLocal;


		public GetVariableNamesCommand(VariableScopeTest outerInstance, string executionId, bool isLocal)
		{
			this.outerInstance = outerInstance;
		 this.executionId = executionId;
		 this.isLocal = isLocal;
		}

		public virtual IList<string> execute(CommandContext commandContext)
		{
		  ensureNotNull("executionId", executionId);

		  ExecutionEntity execution = commandContext.ExecutionManager.findExecutionById(executionId);

		  ensureNotNull("execution " + executionId + " doesn't exist", "execution", execution);

		  IList<string> executionVariables;
		  if (isLocal)
		  {
			executionVariables = new List<string>(execution.VariableNamesLocal);
		  }
		  else
		  {
			executionVariables = new List<string>(execution.VariableNames);
		  }

		  return executionVariables;
		}

	  }
	}

}