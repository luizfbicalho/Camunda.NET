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
namespace org.camunda.bpm.engine.test.bpmn.usertask
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Thilo-Alexander Ginkel
	/// </summary>
	public class TaskPriorityExtensionsTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testPriorityExtension() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPriorityExtension()
	  {
		testPriorityExtension(25);
		testPriorityExtension(75);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void testPriorityExtension(int priority) throws Exception
	  private void testPriorityExtension(int priority)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map<String, Object> variables = new java.util.HashMap<String, Object>();
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["taskPriority"] = priority;

		// Start process-instance, passing priority that should be used as task priority
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskPriorityExtension", variables);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskPriorityExtension", variables);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		assertEquals(priority, task.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testPriorityExtensionString() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPriorityExtensionString()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskPriorityExtensionString");
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("taskPriorityExtensionString");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().processInstanceId(processInstance.getId()).singleResult();
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertEquals(42, task.Priority);
	  }
	}

}