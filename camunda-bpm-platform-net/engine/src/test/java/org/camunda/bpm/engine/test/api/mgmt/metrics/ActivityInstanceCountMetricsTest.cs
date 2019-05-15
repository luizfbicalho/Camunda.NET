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
namespace org.camunda.bpm.engine.test.api.mgmt.metrics
{

	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using Task = org.camunda.bpm.engine.task.Task;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ActivityInstanceCountMetricsTest : AbstractMetricsTest
	{

	  public virtual void testBpmnActivityInstances()
	  {
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().manualTask().endEvent().done());

		// given
		// that no activity instances have been executed
		assertEquals(0l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// if
		// a process instance is started
		runtimeService.startProcessInstanceByKey("testProcess");

		// then
		// the increased count is immediately visible
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// and force the db metrics reporter to report
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// still 3
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());
	  }

	  public virtual void testStandaloneTask()
	  {

		// given
		// that no activity instances have been executed
		assertEquals(0l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// if
		// I complete a standalone task
		Task task = taskService.newTask();
		taskService.saveTask(task);

		// then
		// the increased count is immediately visible
		assertEquals(1l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// and force the db metrics reporter to report
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// still 1
		assertEquals(1l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		taskService.deleteTask(task.Id);

		// clean up
		HistoricTaskInstance hti = historyService.createHistoricTaskInstanceQuery().singleResult();
		if (hti != null)
		{
		  historyService.deleteHistoricTaskInstance(hti.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCmmnActivitiyInstances()
	  public virtual void testCmmnActivitiyInstances()
	  {
		// given
		// that no activity instances have been executed
		assertEquals(0l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		caseService.createCaseInstanceByKey("case");

		assertEquals(1l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// start PI_HumanTask_1 and PI_Milestone_1
		IList<CaseExecution> list = caseService.createCaseExecutionQuery().enabled().list();
		foreach (CaseExecution caseExecution in list)
		{
		  caseService.withCaseExecution(caseExecution.Id).manualStart();
		}

		assertEquals(2l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// and force the db metrics reporter to report
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// still 2
		assertEquals(2l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

		// trigger the milestone
		CaseExecution taskExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();
		caseService.completeCaseExecution(taskExecution.Id);

		// milestone is counted
		assertEquals(3l, managementService.createMetricsQuery().name(Metrics.ACTIVTY_INSTANCE_START).sum());

	  }

	}

}