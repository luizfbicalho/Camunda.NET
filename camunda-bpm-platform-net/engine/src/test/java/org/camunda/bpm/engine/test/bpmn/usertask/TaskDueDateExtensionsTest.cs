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
namespace org.camunda.bpm.engine.test.bpmn.usertask
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Period = org.joda.time.Period;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class TaskDueDateExtensionsTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDueDateExtension() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDueDateExtension()
	  {

		DateTime date = (new SimpleDateFormat("dd-MM-yyyy hh:mm:ss")).parse("06-07-1986 12:10:00");
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = date;

		// Start process-instance, passing date that should be used as dueDate
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("dueDateExtension", variables);

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(task.DueDate);
		assertEquals(date, task.DueDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testDueDateStringExtension() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDueDateStringExtension()
	  {

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = "1986-07-06T12:10:00";

		// Start process-instance, passing date that should be used as dueDate
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("dueDateExtension", variables);

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull(task.DueDate);
		DateTime date = (new SimpleDateFormat("dd-MM-yyyy HH:mm:ss")).parse("06-07-1986 12:10:00");
		assertEquals(date, task.DueDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testRelativeDueDate()
	  public virtual void testRelativeDueDate()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["dateVariable"] = "P2DT2H30M";

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("dueDateExtension", variables);

		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();


		DateTime dueDate = task.DueDate;
		assertNotNull(dueDate);

		Period period = new Period(task.CreateTime.Ticks, dueDate.Ticks);
		assertEquals(period.Days, 2);
		assertEquals(period.Hours, 2);
		assertEquals(period.Minutes, 30);
	  }
	}

}