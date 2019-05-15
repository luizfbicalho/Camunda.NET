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
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// 
	/// <summary>
	/// @author Nico Rehwaldt
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricActivityInstanceStateTest : PluggableProcessEngineTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleEndEvent()
		public virtual void testSingleEndEvent()
		{
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "start", 1);
		assertNonCanceledActivityInstance(allInstances, "start");

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleEndActivity()
	  public virtual void testSingleEndActivity()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "start", 1);
		assertNonCanceledActivityInstance(allInstances, "start");

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleEndEventAfterParallelJoin()
	  public virtual void testSingleEndEventAfterParallelJoin()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "parallelJoin", 2);
		assertNonCanceledActivityInstance(allInstances, "parallelJoin");

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEndParallelJoin()
	  public virtual void testEndParallelJoin()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "task1", 1);
		assertNonCanceledActivityInstance(allInstances, "task1");

		assertNonCompletingActivityInstance(allInstances, "task2", 1);
		assertNonCanceledActivityInstance(allInstances, "task2");

		assertIsCompletingActivityInstances(allInstances, "parallelJoinEnd", 2);
		assertNonCanceledActivityInstance(allInstances, "parallelJoinEnd");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoEndEvents()
	  public virtual void testTwoEndEvents()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "parallelSplit", 1);
		assertNonCanceledActivityInstance(allInstances, "parallelSplit", 1);

		assertIsCompletingActivityInstances(allInstances, "end1", 1);
		assertNonCanceledActivityInstance(allInstances, "end1");

		assertIsCompletingActivityInstances(allInstances, "end2", 1);
		assertNonCanceledActivityInstance(allInstances, "end2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTwoEndActivities()
	  public virtual void testTwoEndActivities()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "parallelSplit", 1);
		assertNonCanceledActivityInstance(allInstances, "parallelSplit");

		assertIsCompletingActivityInstances(allInstances, "end1", 1);
		assertNonCanceledActivityInstance(allInstances, "end1");

		assertIsCompletingActivityInstances(allInstances, "end2", 1);
		assertNonCanceledActivityInstance(allInstances, "end2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSingleEndEventAndSingleEndActivity()
	  public virtual void testSingleEndEventAndSingleEndActivity()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "parallelSplit", 1);
		assertNonCanceledActivityInstance(allInstances, "parallelSplit");

		assertIsCompletingActivityInstances(allInstances, "end1");
		assertNonCanceledActivityInstance(allInstances, "end1");

		assertIsCompletingActivityInstances(allInstances, "end2");
		assertNonCanceledActivityInstance(allInstances, "end2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSimpleSubProcess()
	  public virtual void testSimpleSubProcess()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "intermediateSubprocess", 1);
		assertNonCanceledActivityInstance(allInstances, "intermediateSubprocess");

		assertIsCompletingActivityInstances(allInstances, "subprocessEnd", 1);
		assertNonCanceledActivityInstance(allInstances, "subprocessEnd");

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelMultiInstanceSubProcess()
	  public virtual void testParallelMultiInstanceSubProcess()
	  {
		startProcess();

		IList<HistoricActivityInstance> activityInstances = EndActivityInstances;

		assertEquals(7, activityInstances.Count);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCompletingActivityInstances(allInstances, "intermediateSubprocess", 3);
		assertNonCanceledActivityInstance(allInstances, "intermediateSubprocess");

		assertIsCompletingActivityInstances(allInstances, "subprocessEnd", 3);
		assertNonCanceledActivityInstance(allInstances, "subprocessEnd");

		assertNonCompletingActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody", 1);
		assertNonCanceledActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody");

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialMultiInstanceSubProcess()
	  public virtual void testSequentialMultiInstanceSubProcess()
	  {
		startProcess();

		IList<HistoricActivityInstance> activityInstances = EndActivityInstances;

		assertEquals(7, activityInstances.Count);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCompletingActivityInstances(allInstances, "intermediateSubprocess", 3);
		assertNonCanceledActivityInstance(allInstances, "intermediateSubprocess");

		assertIsCompletingActivityInstances(allInstances, "subprocessEnd", 3);
		assertNonCanceledActivityInstance(allInstances, "subprocessEnd");

		assertNonCompletingActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody", 1);
		assertNonCanceledActivityInstance(allInstances, "intermediateSubprocess#multiInstanceBody");

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testIntermediateTask()
	  public virtual void testIntermediateTask()
	  {
		startProcess();

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "intermediateTask", 1);
		assertNonCanceledActivityInstance(allInstances, "intermediateTask");

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundaryErrorCancel()
	  public virtual void testBoundaryErrorCancel()
	  {
		ProcessInstance processInstance = startProcess();
		runtimeService.correlateMessage("continue");
		assertProcessEnded(processInstance.Id);


		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCanceledActivityInstance(allInstances, "start");
		assertNonCompletingActivityInstance(allInstances, "start");

		assertNonCanceledActivityInstance(allInstances, "subprocessStart");
		assertNonCompletingActivityInstance(allInstances, "subprocessStart");

		assertNonCanceledActivityInstance(allInstances, "gtw");
		assertNonCompletingActivityInstance(allInstances, "gtw");

		assertIsCanceledActivityInstances(allInstances, "subprocess", 1);
		assertNonCompletingActivityInstance(allInstances, "subprocess");

		assertIsCanceledActivityInstances(allInstances, "errorSubprocessEnd", 1);
		assertNonCompletingActivityInstance(allInstances, "errorSubprocessEnd");

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertNonCanceledActivityInstance(allInstances, "subprocessBoundary");
		assertNonCompletingActivityInstance(allInstances, "subprocessBoundary");

		assertNonCanceledActivityInstance(allInstances, "endAfterBoundary");
		assertIsCompletingActivityInstances(allInstances, "endAfterBoundary", 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testBoundarySignalCancel()
	  public virtual void testBoundarySignalCancel()
	  {
		ProcessInstance processInstance = startProcess();

		// should wait in user task
		assertFalse(processInstance.Ended);

		// signal sub process
		runtimeService.signalEventReceived("interrupt");

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "subprocess");
		assertIsCanceledActivityInstances(allInstances, "subprocess", 1);

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertNonCanceledActivityInstance(allInstances, "subprocessBoundary");
		assertNonCompletingActivityInstance(allInstances, "subprocessBoundary");

		assertNonCanceledActivityInstance(allInstances, "endAfterBoundary");
		assertIsCompletingActivityInstances(allInstances, "endAfterBoundary", 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEventSubprocessErrorCancel()
	  public virtual void testEventSubprocessErrorCancel()
	  {
		ProcessInstance processInstance = startProcess();
		runtimeService.correlateMessage("continue");
		assertProcessEnded(processInstance.Id);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertIsCanceledActivityInstances(allInstances, "errorEnd", 1);
		assertNonCompletingActivityInstance(allInstances, "errorEnd");

		assertNonCanceledActivityInstance(allInstances, "eventSubprocessStart");
		assertNonCompletingActivityInstance(allInstances, "eventSubprocessStart");

		assertNonCanceledActivityInstance(allInstances, "eventSubprocessEnd");
		assertIsCompletingActivityInstances(allInstances, "eventSubprocessEnd", 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEventSubprocessMessageCancel()
	  public virtual void testEventSubprocessMessageCancel()
	  {
		startProcess();

		runtimeService.correlateMessage("message");

		assertNull(runtimeService.createProcessInstanceQuery().singleResult());

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertNonCanceledActivityInstance(allInstances, "eventSubprocessStart");
		assertNonCompletingActivityInstance(allInstances, "eventSubprocessStart");

		assertNonCanceledActivityInstance(allInstances, "eventSubprocessEnd");
		assertIsCompletingActivityInstances(allInstances, "eventSubprocessEnd", 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEventSubprocessSignalCancel()
	  public virtual void testEventSubprocessSignalCancel()
	  {
		ProcessInstance processInstance = startProcess();
		runtimeService.correlateMessage("continue");
		assertProcessEnded(processInstance.Id);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		// fails due to CAM-4527: end execution listeners are executed twice for the signal end event
		assertIsCanceledActivityInstances(allInstances, "signalEnd", 1);
		assertNonCompletingActivityInstance(allInstances, "signalEnd");

		assertNonCanceledActivityInstance(allInstances, "eventSubprocessStart");
		assertNonCompletingActivityInstance(allInstances, "eventSubprocessStart");

		assertNonCanceledActivityInstance(allInstances, "eventSubprocessEnd");
		assertIsCompletingActivityInstances(allInstances, "eventSubprocessEnd", 1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEndTerminateEventCancel()
	  public virtual void testEndTerminateEventCancel()
	  {
		ProcessInstance processInstance = startProcess();
		runtimeService.correlateMessage("continue");
		assertProcessEnded(processInstance.Id);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertNonCanceledActivityInstance(allInstances, "terminateEnd");
		assertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEndTerminateEventCancelInSubprocess()
	  public virtual void testEndTerminateEventCancelInSubprocess()
	  {
		ProcessInstance processInstance = startProcess();
		runtimeService.correlateMessage("continue");
		assertProcessEnded(processInstance.Id);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertNonCompletingActivityInstance(allInstances, "subprocess");
		assertNonCanceledActivityInstance(allInstances, "subprocess");

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertNonCanceledActivityInstance(allInstances, "terminateEnd");
		assertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);

		assertIsCompletingActivityInstances(allInstances, "end", 1);
		assertNonCanceledActivityInstance(allInstances, "end");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEndTerminateEventCancelWithSubprocess()
	  public virtual void testEndTerminateEventCancelWithSubprocess()
	  {
		ProcessInstance processInstance = startProcess();
		runtimeService.correlateMessage("continue");
		assertProcessEnded(processInstance.Id);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "subprocess", 1);
		assertNonCompletingActivityInstance(allInstances, "subprocess");

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertNonCanceledActivityInstance(allInstances, "terminateEnd");
		assertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);

	  }

	  [Deployment(resources:{ "org/camunda/bpm/engine/test/history/HistoricActivityInstanceStateTest.testCancelProcessInstanceInUserTask.bpmn", "org/camunda/bpm/engine/test/history/HistoricActivityInstanceStateTest.testEndTerminateEventWithCallActivity.bpmn" })]
	  public virtual void testEndTerminateEventCancelWithCallActivity()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process1");
		runtimeService.correlateMessage("continue");
		assertProcessEnded(processInstance.Id);

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "callActivity", 1);
		assertNonCompletingActivityInstance(allInstances, "callActivity");

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertNonCanceledActivityInstance(allInstances, "terminateEnd");
		assertIsCompletingActivityInstances(allInstances, "terminateEnd", 1);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCancelProcessInstanceInUserTask()
	  public virtual void testCancelProcessInstanceInUserTask()
	  {
		ProcessInstance processInstance = startProcess();

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCancelProcessInstanceInSubprocess()
	  public virtual void testCancelProcessInstanceInSubprocess()
	  {
		ProcessInstance processInstance = startProcess();

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "userTask", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask");

		assertIsCanceledActivityInstances(allInstances, "subprocess", 1);
		assertNonCompletingActivityInstance(allInstances, "subprocess");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCancelProcessWithParallelGateway()
	  public virtual void testCancelProcessWithParallelGateway()
	  {
		ProcessInstance processInstance = startProcess();

		runtimeService.deleteProcessInstance(processInstance.Id, "test");

		IList<HistoricActivityInstance> allInstances = AllActivityInstances;

		assertIsCanceledActivityInstances(allInstances, "userTask1", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask1");

		assertIsCanceledActivityInstances(allInstances, "userTask2", 1);
		assertNonCompletingActivityInstance(allInstances, "userTask2");

		assertIsCanceledActivityInstances(allInstances, "subprocess", 1);
		assertNonCompletingActivityInstance(allInstances, "subprocess");

	  }

	  private void assertIsCanceledActivityInstances(IList<HistoricActivityInstance> allInstances, string activityId, int count)
	  {
		assertCorrectCanceledState(allInstances, activityId, count, true);
	  }

	  private void assertNonCanceledActivityInstance(IList<HistoricActivityInstance> instances, string activityId)
	  {
		assertNonCanceledActivityInstance(instances, activityId, -1);
	  }

	  private void assertNonCanceledActivityInstance(IList<HistoricActivityInstance> instances, string activityId, int count)
	  {
		assertCorrectCanceledState(instances, activityId, count, false);
	  }

	  private void assertCorrectCanceledState(IList<HistoricActivityInstance> allInstances, string activityId, int expectedCount, bool canceled)
	  {
		int found = 0;

		foreach (HistoricActivityInstance instance in allInstances)
		{
		  if (instance.ActivityId.Equals(activityId))
		  {
			found++;
			assertEquals(string.Format("expect <{0}> to be {1}canceled", activityId, (canceled ? "" : "non-")), canceled, instance.Canceled);
		  }
		}

		assertTrue("contains entry for activity <" + activityId + ">", found > 0);

		if (expectedCount != -1)
		{
		  assertTrue("contains <" + expectedCount + "> entries for activity <" + activityId + ">", found == expectedCount);
		}
	  }

	  private void assertIsCompletingActivityInstances(IList<HistoricActivityInstance> allInstances, string activityId)
	  {
		assertIsCompletingActivityInstances(allInstances, activityId, -1);
	  }

	  private void assertIsCompletingActivityInstances(IList<HistoricActivityInstance> allInstances, string activityId, int count)
	  {
		assertCorrectCompletingState(allInstances, activityId, count, true);
	  }

	  private void assertNonCompletingActivityInstance(IList<HistoricActivityInstance> instances, string activityId)
	  {
		assertNonCompletingActivityInstance(instances, activityId, -1);
	  }

	  private void assertNonCompletingActivityInstance(IList<HistoricActivityInstance> instances, string activityId, int count)
	  {
		assertCorrectCompletingState(instances, activityId, count, false);
	  }

	  private void assertCorrectCompletingState(IList<HistoricActivityInstance> allInstances, string activityId, int expectedCount, bool completing)
	  {
		int found = 0;

		foreach (HistoricActivityInstance instance in allInstances)
		{
		  if (instance.ActivityId.Equals(activityId))
		  {
			found++;
			assertEquals(string.Format("expect <{0}> to be {1}completing", activityId, (completing ? "" : "non-")), completing, instance.CompleteScope);
		  }
		}

		assertTrue("contains entry for activity <" + activityId + ">", found > 0);

		if (expectedCount != -1)
		{
		  assertTrue("contains <" + expectedCount + "> entries for activity <" + activityId + ">", found == expectedCount);
		}
	  }

	  private IList<HistoricActivityInstance> EndActivityInstances
	  {
		  get
		  {
			return historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceEndTime().asc().completeScope().list();
		  }
	  }

	  private IList<HistoricActivityInstance> AllActivityInstances
	  {
		  get
		  {
			return historyService.createHistoricActivityInstanceQuery().orderByHistoricActivityInstanceStartTime().asc().list();
		  }
	  }

	  private ProcessInstance startProcess()
	  {
		return runtimeService.startProcessInstanceByKey("process");
	  }
	}

}