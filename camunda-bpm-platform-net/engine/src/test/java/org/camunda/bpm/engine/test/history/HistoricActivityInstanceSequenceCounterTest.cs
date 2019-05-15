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
namespace org.camunda.bpm.engine.test.history
{

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricActivityInstanceSequenceCounterTest : PluggableProcessEngineTestCase
	{
		[Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testSequence.bpmn20.xml"})]
		public virtual void testSequence()
		{
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		runtimeService.startProcessInstanceByKey("process");

		// then
		verifyOrder(query, "theStart", "theService1", "theService2", "theEnd");
		}

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testForkSameSequenceLengthWithoutWaitStates.bpmn20.xml"})]
	  public virtual void testFork()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when

		// then
		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService", "fork", "theService2", "theEnd2");

		string firstExecutionId = historyService.createHistoricActivityInstanceQuery().activityId("theService1").singleResult().ExecutionId;
		query.executionId(firstExecutionId);
		verifyOrder(query, "theService1", "theEnd1");

		query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();
		verifyOrder(query, "theStart", "theService", "fork", "theService1", "theEnd1", "theService2", "theEnd2");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testForkAndJoinDifferentSequenceLength.bpmn20.xml"})]
	  public virtual void testForkAndJoin()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// then
		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService", "fork", "join", "theService4", "theEnd");

		string firstExecutionId = historyService.createHistoricActivityInstanceQuery().activityId("theService1").singleResult().ExecutionId;

		string secondExecutionId = historyService.createHistoricActivityInstanceQuery().activityId("theService2").singleResult().ExecutionId;

		query.executionId(firstExecutionId);
		verifyOrder(query, "theService1", "join");

		query.executionId(secondExecutionId);
		verifyOrder(query, "theService2", "theService3");

		query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc().orderByActivityId().asc();
		verifyOrder(query, "theStart", "theService", "fork", "theService1", "theService2", "join", "theService3", "join", "theService4", "theEnd");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testSequenceInsideSubProcess.bpmn20.xml"})]
	  public virtual void testSequenceInsideSubProcess()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// then
		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService1", "theService2", "theEnd");

		string subProcessExecutionId = historyService.createHistoricActivityInstanceQuery().activityId("subProcess").singleResult().ExecutionId;

		query.executionId(subProcessExecutionId);
		verifyOrder(query, "subProcess", "innerStart", "innerService", "innerEnd");

		query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();
		verifyOrder(query, "theStart", "theService1", "subProcess", "innerStart", "innerService", "innerEnd", "theService2", "theEnd");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testSequentialMultiInstance.bpmn20.xml"})]
	  public virtual void testSequentialMultiInstance()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// then
		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService1", "theService3", "theEnd");

		string taskExecutionId = historyService.createHistoricActivityInstanceQuery().activityId("theService2").list().get(0).ExecutionId;

		query.executionId(taskExecutionId);
		verifyOrder(query, "theService2#multiInstanceBody", "theService2", "theService2");

		query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();
		verifyOrder(query, "theStart", "theService1", "theService2#multiInstanceBody", "theService2", "theService2", "theService3", "theEnd");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testParallelMultiInstance.bpmn20.xml"})]
	  public virtual void testParallelMultiInstance()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// then
		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService1", "theService3", "theEnd");

		IList<HistoricActivityInstance> taskActivityInstances = historyService.createHistoricActivityInstanceQuery().activityId("theService2").list();
		foreach (HistoricActivityInstance activityInstance in taskActivityInstances)
		{
		  query.executionId(activityInstance.ExecutionId);
		  verifyOrder(query, "theService2");
		}

		query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();
		verifyOrder(query, "theStart", "theService1", "theService2#multiInstanceBody", "theService2", "theService2", "theService3", "theEnd");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testLoop.bpmn20.xml"})]
	  public virtual void testLoop()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// then
		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService1", "join", "theScript", "fork", "join", "theScript", "fork", "theService2", "theEnd");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testInterruptingBoundaryEvent.bpmn20.xml"})]
	  public virtual void testInterruptingBoundaryEvent()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		runtimeService.correlateMessage("newMessage");

		// then
		verifyOrder(query, "theStart", "theService1", "theTask", "messageBoundary", "theServiceAfterMessage", "theEnd2");

		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService1", "messageBoundary", "theServiceAfterMessage", "theEnd2");

		string taskExecutionId = historyService.createHistoricActivityInstanceQuery().activityId("theTask").singleResult().ExecutionId;

		query.executionId(taskExecutionId);
		verifyOrder(query, "theTask");
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testNonInterruptingBoundaryEvent.bpmn20.xml"})]
	  public virtual void testNonInterruptingBoundaryEvent()
	  {
		// given
		HistoricActivityInstanceQuery query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc();

		// when
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		runtimeService.correlateMessage("newMessage");
		runtimeService.correlateMessage("newMessage");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		// then
		query.executionId(processInstanceId);
		verifyOrder(query, "theStart", "theService1", "theEnd1");

		string taskExecutionId = historyService.createHistoricActivityInstanceQuery().activityId("theTask").singleResult().ExecutionId;

		query.executionId(taskExecutionId);
		verifyOrder(query, "theTask");

		IList<HistoricActivityInstance> activityInstances = historyService.createHistoricActivityInstanceQuery().activityId("messageBoundary").list();
		foreach (HistoricActivityInstance historicActivityInstance in activityInstances)
		{
		  query.executionId(historicActivityInstance.ExecutionId);
		  verifyOrder(query, "messageBoundary", "theServiceAfterMessage", "theEnd2");
		}

		query = historyService.createHistoricActivityInstanceQuery().orderPartiallyByOccurrence().asc().orderByActivityId().asc();

		verifyOrder(query, "theStart", "theService1", "messageBoundary", "theTask", "theServiceAfterMessage", "theEnd2", "messageBoundary", "theServiceAfterMessage", "theEnd2", "theEnd1");
	  }

	  protected internal virtual void verifyOrder(HistoricActivityInstanceQuery query, params string[] expectedOrder)
	  {
		assertEquals(expectedOrder.Length, query.count());

		IList<HistoricActivityInstance> activityInstances = query.list();

		for (int i = 0; i < expectedOrder.Length; i++)
		{
		  HistoricActivityInstance activityInstance = activityInstances[i];
		  string currentActivityId = activityInstance.ActivityId;
		  string expectedActivityId = expectedOrder[i];
		  assertEquals(expectedActivityId, currentActivityId);
		}

	  }

	}

}