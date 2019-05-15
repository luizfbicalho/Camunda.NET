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
namespace org.camunda.bpm.engine.test.api.externaltask
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySortingAndCount;

	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.externalTaskByPriority;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Christopher Zell
	/// </summary>
	public class ExternalTaskQueryByPriorityTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")]
	  public virtual void testOrderByPriority()
	  {
		// given five jobs with priorities from 1 to 5
		//each process has two external tasks - one with priority expression and one without priority
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Variables.createVariables().putValue("priority", i)));
		}

		// then querying and ordering by priority works
		verifySortingAndCount(externalTaskService.createExternalTaskQuery().orderByPriority().asc(), 10, externalTaskByPriority());
		verifySortingAndCount(externalTaskService.createExternalTaskQuery().orderByPriority().desc(), 10, inverted(externalTaskByPriority()));
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")]
	  public virtual void testFilterByExternalTaskPriorityLowerThanOrEquals()
	  {
		// given five jobs with priorities from 1 to 5
		//each process has two external tasks - one with priority expression and one without priority
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a external task query and filtering by priority
		// then the correct external tasks are returned
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().priorityLowerThanOrEquals(2).list();
		assertEquals(8, tasks.Count);

		foreach (ExternalTask task in tasks)
		{
		  assertTrue(task.Priority <= 2);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")]
	  public virtual void testFilterByExternalTaskPriorityLowerThanOrEqualsAndHigherThanOrEqual()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a external task query and filtering by disjunctive external task priority
		// then no external task are returned
		assertEquals(0, externalTaskService.createExternalTaskQuery().priorityLowerThanOrEquals(2).priorityHigherThanOrEquals(3).count());

		// when making a external task query and filtering by external task priority >= 2 and <= 3
		// then two external task are returned
		assertEquals(2, externalTaskService.createExternalTaskQuery().priorityHigherThanOrEquals(2).priorityLowerThanOrEquals(3).count());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")]
	  public virtual void testFilterByExternalTaskPriorityHigherThanOrEquals()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a external task query and filtering by external task priority
		// then the correct external task are returned
		IList<ExternalTask> tasks = externalTaskService.createExternalTaskQuery().priorityHigherThanOrEquals(2L).list();
		assertEquals(3, tasks.Count);

		ISet<string> processInstanceIds = new HashSet<string>();
		processInstanceIds.Add(instances[2].Id);
		processInstanceIds.Add(instances[3].Id);
		processInstanceIds.Add(instances[4].Id);

		foreach (ExternalTask task in tasks)
		{
		  assertTrue(task.Priority >= 2);
		  assertTrue(processInstanceIds.Contains(task.ProcessInstanceId));
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")]
	  public virtual void testFilterByExternalTaskPriorityLowerAndHigher()
	  {
		// given five jobs with priorities from 1 to 5
		IList<ProcessInstance> instances = new List<ProcessInstance>();

		for (int i = 0; i < 5; i++)
		{
		  instances.Add(runtimeService.startProcessInstanceByKey("twoExternalTaskWithPriorityProcess", Variables.createVariables().putValue("priority", i)));
		}

		// when making a external task query and filtering by external task priority
		// then the correct external task is returned
		ExternalTask task = externalTaskService.createExternalTaskQuery().priorityHigherThanOrEquals(2L).priorityLowerThanOrEquals(2L).singleResult();
		assertNotNull(task);
		assertEquals(2, task.Priority);
		assertEquals(instances[2].Id, task.ProcessInstanceId);
	  }
	}

}