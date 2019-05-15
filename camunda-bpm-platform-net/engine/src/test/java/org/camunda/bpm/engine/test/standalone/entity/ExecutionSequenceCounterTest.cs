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
namespace org.camunda.bpm.engine.test.standalone.entity
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ActivitySequenceCounterMap = org.camunda.bpm.engine.test.standalone.entity.ExecutionOrderListener.ActivitySequenceCounterMap;
	using Before = org.junit.Before;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ExecutionSequenceCounterTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		ExecutionOrderListener.clearActivityExecutionOrder();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequence()
	  public virtual void testSequence()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService1", "theService2", "theEnd");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkSameSequenceLengthWithoutWaitStates()
	  public virtual void testForkSameSequenceLengthWithoutWaitStates()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService", "fork", "theService1", "theEnd1", "theService2", "theEnd2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkSameSequenceLengthWithAsyncEndEvent()
	  public virtual void testForkSameSequenceLengthWithAsyncEndEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		JobQuery jobQuery = managementService.createJobQuery();

		// when (1)

		// then (1)
		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(5, order.Count);

		long lastSequenceCounter = 0;

		ActivitySequenceCounterMap theStartElement = order[0];
		assertEquals("theStart", theStartElement.ActivityId);
		assertTrue(theStartElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theStartElement.SequenceCounter;

		ActivitySequenceCounterMap theForkElement = order[1];
		assertEquals("theService", theForkElement.ActivityId);
		assertTrue(theForkElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theForkElement.SequenceCounter;

		ActivitySequenceCounterMap theServiceElement = order[2];
		assertEquals("fork", theServiceElement.ActivityId);
		assertTrue(theServiceElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theServiceElement.SequenceCounter;

		ActivitySequenceCounterMap theService1Element = order[3];
		assertEquals("theService1", theService1Element.ActivityId);
		assertTrue(theService1Element.SequenceCounter > lastSequenceCounter);

		ActivitySequenceCounterMap theService2Element = order[4];
		assertEquals("theService2", theService2Element.ActivityId);
		assertTrue(theService2Element.SequenceCounter > lastSequenceCounter);

		// when (2)
		string jobId = jobQuery.activityId("theEnd1").singleResult().Id;
		managementService.executeJob(jobId);

		// then (2)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(6, order.Count);

		ActivitySequenceCounterMap theEnd1Element = order[5];
		assertEquals("theEnd1", theEnd1Element.ActivityId);
		assertTrue(theEnd1Element.SequenceCounter > theService1Element.SequenceCounter);

		// when (3)
		jobId = jobQuery.activityId("theEnd2").singleResult().Id;
		managementService.executeJob(jobId);

		// then (3)
		assertProcessEnded(processInstanceId);

		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(7, order.Count);

		ActivitySequenceCounterMap theEnd2Element = order[6];
		assertEquals("theEnd2", theEnd2Element.ActivityId);
		assertTrue(theEnd2Element.SequenceCounter > theService2Element.SequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkDifferentSequenceLengthWithoutWaitStates()
	  public virtual void testForkDifferentSequenceLengthWithoutWaitStates()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService", "fork", "theService1", "theEnd1", "theService2", "theService3", "theEnd2");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkDifferentSequenceLengthWithAsyncEndEvent()
	  public virtual void testForkDifferentSequenceLengthWithAsyncEndEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		JobQuery jobQuery = managementService.createJobQuery();

		// when (1)

		// then (1)
		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(6, order.Count);

		long lastSequenceCounter = 0;

		ActivitySequenceCounterMap theStartElement = order[0];
		assertEquals("theStart", theStartElement.ActivityId);
		assertTrue(theStartElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theStartElement.SequenceCounter;

		ActivitySequenceCounterMap theForkElement = order[1];
		assertEquals("theService", theForkElement.ActivityId);
		assertTrue(theForkElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theForkElement.SequenceCounter;

		ActivitySequenceCounterMap theServiceElement = order[2];
		assertEquals("fork", theServiceElement.ActivityId);
		assertTrue(theServiceElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theServiceElement.SequenceCounter;

		ActivitySequenceCounterMap theService1Element = order[3];
		assertEquals("theService1", theService1Element.ActivityId);
		assertTrue(theService1Element.SequenceCounter > lastSequenceCounter);

		ActivitySequenceCounterMap theService2Element = order[4];
		assertEquals("theService2", theService2Element.ActivityId);
		assertTrue(theService2Element.SequenceCounter > lastSequenceCounter);

		ActivitySequenceCounterMap theService3Element = order[5];
		assertEquals("theService3", theService3Element.ActivityId);
		assertTrue(theService3Element.SequenceCounter > theService2Element.SequenceCounter);

		// when (2)
		string jobId = jobQuery.activityId("theEnd1").singleResult().Id;
		managementService.executeJob(jobId);

		// then (2)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(7, order.Count);

		ActivitySequenceCounterMap theEnd1Element = order[6];
		assertEquals("theEnd1", theEnd1Element.ActivityId);
		assertTrue(theEnd1Element.SequenceCounter > theService1Element.SequenceCounter);

		// when (3)
		jobId = jobQuery.activityId("theEnd2").singleResult().Id;
		managementService.executeJob(jobId);

		// then (3)
		assertProcessEnded(processInstanceId);

		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(8, order.Count);

		ActivitySequenceCounterMap theEnd2Element = order[7];
		assertEquals("theEnd2", theEnd2Element.ActivityId);
		assertTrue(theEnd2Element.SequenceCounter > theService3Element.SequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkReplaceBy()
	  public virtual void testForkReplaceBy()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		JobQuery jobQuery = managementService.createJobQuery();

		// when (1)

		// then (1)
		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(2, order.Count);

		ActivitySequenceCounterMap theService1Element = order[0];
		assertEquals("theService1", theService1Element.ActivityId);

		ActivitySequenceCounterMap theService3Element = order[1];
		assertEquals("theService3", theService3Element.ActivityId);

		assertTrue(theService1Element.SequenceCounter == theService3Element.SequenceCounter);

		// when (2)
		string jobId = jobQuery.activityId("theService4").singleResult().Id;
		managementService.executeJob(jobId);

		// then (2)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(5, order.Count);

		ActivitySequenceCounterMap theService4Element = order[2];
		assertEquals("theService4", theService4Element.ActivityId);
		assertTrue(theService4Element.SequenceCounter > theService3Element.SequenceCounter);

		ActivitySequenceCounterMap theService5Element = order[3];
		assertEquals("theService5", theService5Element.ActivityId);
		assertTrue(theService5Element.SequenceCounter > theService4Element.SequenceCounter);

		ActivitySequenceCounterMap theEnd2Element = order[4];
		assertEquals("theEnd2", theEnd2Element.ActivityId);
		assertTrue(theEnd2Element.SequenceCounter > theService5Element.SequenceCounter);

		// when (3)
		jobId = jobQuery.activityId("theService2").singleResult().Id;
		managementService.executeJob(jobId);

		// then (3)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(7, order.Count);

		ActivitySequenceCounterMap theService2Element = order[5];
		assertEquals("theService2", theService2Element.ActivityId);
		assertTrue(theService2Element.SequenceCounter > theService1Element.SequenceCounter);
		assertTrue(theService2Element.SequenceCounter > theEnd2Element.SequenceCounter);

		ActivitySequenceCounterMap theEnd1Element = order[6];
		assertEquals("theEnd1", theEnd1Element.ActivityId);
		assertTrue(theEnd1Element.SequenceCounter > theService2Element.SequenceCounter);

		assertProcessEnded(processInstanceId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/standalone/entity/ExecutionSequenceCounterTest.testForkReplaceBy.bpmn20.xml"})]
	  public virtual void testForkReplaceByAnotherExecutionOrder()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		JobQuery jobQuery = managementService.createJobQuery();

		// when (1)

		// then (1)
		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(2, order.Count);

		ActivitySequenceCounterMap theService1Element = order[0];
		assertEquals("theService1", theService1Element.ActivityId);

		ActivitySequenceCounterMap theService3Element = order[1];
		assertEquals("theService3", theService3Element.ActivityId);

		assertTrue(theService1Element.SequenceCounter == theService3Element.SequenceCounter);

		// when (2)
		string jobId = jobQuery.activityId("theService2").singleResult().Id;
		managementService.executeJob(jobId);

		// then (2)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(4, order.Count);

		ActivitySequenceCounterMap theService2Element = order[2];
		assertEquals("theService2", theService2Element.ActivityId);
		assertTrue(theService2Element.SequenceCounter > theService1Element.SequenceCounter);

		ActivitySequenceCounterMap theEnd1Element = order[3];
		assertEquals("theEnd1", theEnd1Element.ActivityId);
		assertTrue(theEnd1Element.SequenceCounter > theService2Element.SequenceCounter);

		// when (3)
		jobId = jobQuery.activityId("theService4").singleResult().Id;
		managementService.executeJob(jobId);

		// then (3)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(7, order.Count);

		ActivitySequenceCounterMap theService4Element = order[4];
		assertEquals("theService4", theService4Element.ActivityId);
		assertTrue(theService4Element.SequenceCounter > theService3Element.SequenceCounter);
		assertTrue(theService4Element.SequenceCounter > theEnd1Element.SequenceCounter);

		ActivitySequenceCounterMap theService5Element = order[5];
		assertEquals("theService5", theService5Element.ActivityId);
		assertTrue(theService5Element.SequenceCounter > theService4Element.SequenceCounter);

		ActivitySequenceCounterMap theEnd2Element = order[6];
		assertEquals("theEnd2", theEnd2Element.ActivityId);
		assertTrue(theEnd2Element.SequenceCounter > theService5Element.SequenceCounter);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkReplaceByThreeBranches()
	  public virtual void testForkReplaceByThreeBranches()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;
		JobQuery jobQuery = managementService.createJobQuery();

		// when (1)

		// then (1)
		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(3, order.Count);

		ActivitySequenceCounterMap theService1Element = order[0];
		assertEquals("theService1", theService1Element.ActivityId);

		ActivitySequenceCounterMap theService3Element = order[1];
		assertEquals("theService3", theService3Element.ActivityId);

		ActivitySequenceCounterMap theService6Element = order[2];
		assertEquals("theService6", theService6Element.ActivityId);

		assertTrue(theService1Element.SequenceCounter == theService3Element.SequenceCounter);
		assertTrue(theService3Element.SequenceCounter == theService6Element.SequenceCounter);

		// when (2)
		string jobId = jobQuery.activityId("theService2").singleResult().Id;
		managementService.executeJob(jobId);

		// then (2)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(5, order.Count);

		ActivitySequenceCounterMap theService2Element = order[3];
		assertEquals("theService2", theService2Element.ActivityId);
		assertTrue(theService2Element.SequenceCounter > theService1Element.SequenceCounter);

		ActivitySequenceCounterMap theEnd1Element = order[4];
		assertEquals("theEnd1", theEnd1Element.ActivityId);
		assertTrue(theEnd1Element.SequenceCounter > theService2Element.SequenceCounter);

		// when (3)
		jobId = jobQuery.activityId("theService4").singleResult().Id;
		managementService.executeJob(jobId);

		// then (3)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(8, order.Count);

		ActivitySequenceCounterMap theService4Element = order[5];
		assertEquals("theService4", theService4Element.ActivityId);
		assertTrue(theService4Element.SequenceCounter > theService3Element.SequenceCounter);

		ActivitySequenceCounterMap theService5Element = order[6];
		assertEquals("theService5", theService5Element.ActivityId);
		assertTrue(theService5Element.SequenceCounter > theService4Element.SequenceCounter);

		ActivitySequenceCounterMap theEnd2Element = order[7];
		assertEquals("theEnd2", theEnd2Element.ActivityId);
		assertTrue(theEnd2Element.SequenceCounter > theService5Element.SequenceCounter);

		// when (4)
		jobId = jobQuery.activityId("theService7").singleResult().Id;
		managementService.executeJob(jobId);

		// then (4)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(12, order.Count);

		ActivitySequenceCounterMap theService7Element = order[8];
		assertEquals("theService7", theService7Element.ActivityId);
		assertTrue(theService7Element.SequenceCounter > theService6Element.SequenceCounter);
		assertTrue(theService7Element.SequenceCounter > theEnd2Element.SequenceCounter);

		ActivitySequenceCounterMap theService8Element = order[9];
		assertEquals("theService8", theService8Element.ActivityId);
		assertTrue(theService8Element.SequenceCounter > theService7Element.SequenceCounter);

		ActivitySequenceCounterMap theService9Element = order[10];
		assertEquals("theService9", theService9Element.ActivityId);
		assertTrue(theService9Element.SequenceCounter > theService8Element.SequenceCounter);

		ActivitySequenceCounterMap theEnd3Element = order[11];
		assertEquals("theEnd3", theEnd3Element.ActivityId);
		assertTrue(theEnd3Element.SequenceCounter > theService9Element.SequenceCounter);

		assertProcessEnded(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkAndJoinSameSequenceLength()
	  public virtual void testForkAndJoinSameSequenceLength()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(9, order.Count);

		long lastSequenceCounter = 0;

		ActivitySequenceCounterMap theStartElement = order[0];
		assertEquals("theStart", theStartElement.ActivityId);
		assertTrue(theStartElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theStartElement.SequenceCounter;

		ActivitySequenceCounterMap theForkElement = order[1];
		assertEquals("theService", theForkElement.ActivityId);
		assertTrue(theForkElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theForkElement.SequenceCounter;

		ActivitySequenceCounterMap theServiceElement = order[2];
		assertEquals("fork", theServiceElement.ActivityId);
		assertTrue(theServiceElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theServiceElement.SequenceCounter;

		ActivitySequenceCounterMap theService1Element = order[3];
		assertEquals("theService1", theService1Element.ActivityId);
		assertTrue(theService1Element.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theService1Element.SequenceCounter;

		ActivitySequenceCounterMap theJoin1Element = order[4];
		assertEquals("join", theJoin1Element.ActivityId);
		assertTrue(theJoin1Element.SequenceCounter > lastSequenceCounter);

		lastSequenceCounter = theForkElement.SequenceCounter;

		ActivitySequenceCounterMap theService2Element = order[5];
		assertEquals("theService2", theService2Element.ActivityId);
		assertTrue(theService2Element.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theService2Element.SequenceCounter;

		ActivitySequenceCounterMap theJoin2Element = order[6];
		assertEquals("join", theJoin2Element.ActivityId);
		assertTrue(theJoin2Element.SequenceCounter > lastSequenceCounter);

		ActivitySequenceCounterMap theService3Element = order[7];
		assertEquals("theService3", theService3Element.ActivityId);
		assertTrue(theService3Element.SequenceCounter > theJoin1Element.SequenceCounter);
		assertTrue(theService3Element.SequenceCounter > theJoin2Element.SequenceCounter);
		lastSequenceCounter = theService3Element.SequenceCounter;

		ActivitySequenceCounterMap theEndElement = order[8];
		assertEquals("theEnd", theEndElement.ActivityId);
		assertTrue(theEndElement.SequenceCounter > lastSequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkAndJoinDifferentSequenceLength()
	  public virtual void testForkAndJoinDifferentSequenceLength()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(10, order.Count);

		long lastSequenceCounter = 0;

		ActivitySequenceCounterMap theStartElement = order[0];
		assertEquals("theStart", theStartElement.ActivityId);
		assertTrue(theStartElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theStartElement.SequenceCounter;

		ActivitySequenceCounterMap theForkElement = order[1];
		assertEquals("theService", theForkElement.ActivityId);
		assertTrue(theForkElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theForkElement.SequenceCounter;

		ActivitySequenceCounterMap theServiceElement = order[2];
		assertEquals("fork", theServiceElement.ActivityId);
		assertTrue(theServiceElement.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theServiceElement.SequenceCounter;

		ActivitySequenceCounterMap theService1Element = order[3];
		assertEquals("theService1", theService1Element.ActivityId);
		assertTrue(theService1Element.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theService1Element.SequenceCounter;

		ActivitySequenceCounterMap theJoin1Element = order[4];
		assertEquals("join", theJoin1Element.ActivityId);
		assertTrue(theJoin1Element.SequenceCounter > lastSequenceCounter);

		lastSequenceCounter = theForkElement.SequenceCounter;

		ActivitySequenceCounterMap theService2Element = order[5];
		assertEquals("theService2", theService2Element.ActivityId);
		assertTrue(theService2Element.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theService2Element.SequenceCounter;

		ActivitySequenceCounterMap theService3Element = order[6];
		assertEquals("theService3", theService3Element.ActivityId);
		assertTrue(theService3Element.SequenceCounter > lastSequenceCounter);
		lastSequenceCounter = theService3Element.SequenceCounter;

		ActivitySequenceCounterMap theJoin2Element = order[7];
		assertEquals("join", theJoin2Element.ActivityId);
		assertTrue(theJoin2Element.SequenceCounter > lastSequenceCounter);

		assertFalse(theJoin1Element.SequenceCounter == theJoin2Element.SequenceCounter);

		ActivitySequenceCounterMap theService4Element = order[8];
		assertEquals("theService4", theService4Element.ActivityId);
		assertTrue(theService4Element.SequenceCounter > theJoin1Element.SequenceCounter);
		assertTrue(theService4Element.SequenceCounter > theJoin2Element.SequenceCounter);
		lastSequenceCounter = theService4Element.SequenceCounter;

		ActivitySequenceCounterMap theEndElement = order[9];
		assertEquals("theEnd", theEndElement.ActivityId);
		assertTrue(theEndElement.SequenceCounter > lastSequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkAndJoinThreeBranchesDifferentSequenceLength()
	  public virtual void testForkAndJoinThreeBranchesDifferentSequenceLength()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(4, order.Count);

		ActivitySequenceCounterMap theJoin1Element = order[0];
		assertEquals("join", theJoin1Element.ActivityId);

		ActivitySequenceCounterMap theJoin2Element = order[1];
		assertEquals("join", theJoin2Element.ActivityId);

		ActivitySequenceCounterMap theJoin3Element = order[2];
		assertEquals("join", theJoin3Element.ActivityId);

		assertFalse(theJoin1Element.SequenceCounter == theJoin2Element.SequenceCounter);
		assertFalse(theJoin2Element.SequenceCounter == theJoin3Element.SequenceCounter);
		assertFalse(theJoin3Element.SequenceCounter == theJoin1Element.SequenceCounter);

		ActivitySequenceCounterMap theService7Element = order[3];
		assertEquals("theService7", theService7Element.ActivityId);
		assertTrue(theService7Element.SequenceCounter > theJoin1Element.SequenceCounter);
		assertTrue(theService7Element.SequenceCounter > theJoin2Element.SequenceCounter);
		assertTrue(theService7Element.SequenceCounter > theJoin3Element.SequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequenceInsideSubProcess()
	  public virtual void testSequenceInsideSubProcess()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService1", "subProcess", "innerStart", "innerService", "innerEnd", "theService2", "theEnd");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkSameSequenceLengthInsideSubProcess()
	  public virtual void testForkSameSequenceLengthInsideSubProcess()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(3, order.Count);

		ActivitySequenceCounterMap innerEnd1Element = order[0];
		assertEquals("innerEnd1", innerEnd1Element.ActivityId);

		ActivitySequenceCounterMap innerEnd2Element = order[1];
		assertEquals("innerEnd2", innerEnd2Element.ActivityId);

		ActivitySequenceCounterMap theService1Element = order[2];
		assertEquals("theService1", theService1Element.ActivityId);

		assertTrue(theService1Element.SequenceCounter > innerEnd1Element.SequenceCounter);
		assertTrue(theService1Element.SequenceCounter > innerEnd2Element.SequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testForkDifferentSequenceLengthInsideSubProcess()
	  public virtual void testForkDifferentSequenceLengthInsideSubProcess()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(3, order.Count);

		ActivitySequenceCounterMap innerEnd1Element = order[0];
		assertEquals("innerEnd1", innerEnd1Element.ActivityId);

		ActivitySequenceCounterMap innerEnd2Element = order[1];
		assertEquals("innerEnd2", innerEnd2Element.ActivityId);

		ActivitySequenceCounterMap theService1Element = order[2];
		assertEquals("theService1", theService1Element.ActivityId);

		assertTrue(theService1Element.SequenceCounter > innerEnd1Element.SequenceCounter);
		assertTrue(theService1Element.SequenceCounter > innerEnd2Element.SequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSequentialMultiInstance()
	  public virtual void testSequentialMultiInstance()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService1", "theService2", "theService2", "theService3", "theEnd");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParallelMultiInstance()
	  public virtual void testParallelMultiInstance()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(6, order.Count);

		ActivitySequenceCounterMap theStartElement = order[0];
		assertEquals("theStart", theStartElement.ActivityId);

		ActivitySequenceCounterMap theService1Element = order[1];
		assertEquals("theService1", theService1Element.ActivityId);
		assertTrue(theService1Element.SequenceCounter > theStartElement.SequenceCounter);

		ActivitySequenceCounterMap theService21Element = order[2];
		assertEquals("theService2", theService21Element.ActivityId);
		assertTrue(theService21Element.SequenceCounter > theService1Element.SequenceCounter);

		ActivitySequenceCounterMap theService22Element = order[3];
		assertEquals("theService2", theService22Element.ActivityId);
		assertTrue(theService22Element.SequenceCounter > theService1Element.SequenceCounter);

		ActivitySequenceCounterMap theService3Element = order[4];
		assertEquals("theService3", theService3Element.ActivityId);
		assertTrue(theService3Element.SequenceCounter > theService21Element.SequenceCounter);
		assertTrue(theService3Element.SequenceCounter > theService22Element.SequenceCounter);

		ActivitySequenceCounterMap theEndElement = order[5];
		assertEquals("theEnd", theEndElement.ActivityId);
		assertTrue(theEndElement.SequenceCounter > theService3Element.SequenceCounter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testLoop()
	  public virtual void testLoop()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when

		// then
		assertProcessEnded(processInstanceId);

		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService1", "join", "theScript", "fork", "join", "theScript", "fork", "theService2", "theEnd");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testInterruptingBoundaryEvent()
	  public virtual void testInterruptingBoundaryEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)

		// then (1)
		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService1", "theTask");

		// when (2)
		runtimeService.correlateMessage("newMessage");

		// then (2)
		assertProcessEnded(processInstanceId);

		order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService1", "theTask", "messageBoundary", "theServiceAfterMessage", "theEnd2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNonInterruptingBoundaryEvent()
	  public virtual void testNonInterruptingBoundaryEvent()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		// when (1)

		// then (1)
		IList<ActivitySequenceCounterMap> order = ExecutionOrderListener.ActivityExecutionOrder;
		verifyOrder(order, "theStart", "theService1", "theTask");

		// when (2)
		runtimeService.correlateMessage("newMessage");

		// then (2)
		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(6, order.Count);

		ActivitySequenceCounterMap theService1Element = order[1];
		assertEquals("theService1", theService1Element.ActivityId);

		ActivitySequenceCounterMap theTaskElement = order[2];
		assertEquals("theTask", theTaskElement.ActivityId);

		ActivitySequenceCounterMap messageBoundaryElement = order[3];
		assertEquals("messageBoundary", messageBoundaryElement.ActivityId);
		assertTrue(messageBoundaryElement.SequenceCounter > theService1Element.SequenceCounter);
		assertFalse(messageBoundaryElement.SequenceCounter > theTaskElement.SequenceCounter);

		ActivitySequenceCounterMap theServiceAfterMessageElement = order[4];
		assertEquals("theServiceAfterMessage", theServiceAfterMessageElement.ActivityId);
		assertTrue(theServiceAfterMessageElement.SequenceCounter > messageBoundaryElement.SequenceCounter);

		ActivitySequenceCounterMap theEnd2Element = order[5];
		assertEquals("theEnd2", theEnd2Element.ActivityId);
		assertTrue(theEnd2Element.SequenceCounter > theServiceAfterMessageElement.SequenceCounter);

		// when (3)
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.complete(taskId);

		// then (3)
		assertProcessEnded(processInstanceId);

		order = ExecutionOrderListener.ActivityExecutionOrder;
		assertEquals(7, order.Count);

		ActivitySequenceCounterMap theEnd1Element = order[6];
		assertEquals("theEnd1", theEnd1Element.ActivityId);
		assertTrue(theEnd1Element.SequenceCounter > theEnd2Element.SequenceCounter);
	  }

	  protected internal virtual void verifyOrder(IList<ActivitySequenceCounterMap> actualOrder, params string[] expectedOrder)
	  {
		assertEquals(expectedOrder.Length, actualOrder.Count);

		long lastActualSequenceCounter = 0;
		for (int i = 0; i < expectedOrder.Length; i++)
		{
		  ActivitySequenceCounterMap actual = actualOrder[i];

		  string actualActivityId = actual.ActivityId;
		  string expectedActivityId = expectedOrder[i];
		  assertEquals(actualActivityId, expectedActivityId);

		  long actualSequenceCounter = actual.SequenceCounter;
		  assertTrue(actualSequenceCounter > lastActualSequenceCounter);

		  lastActualSequenceCounter = actualSequenceCounter;
		}
	  }

	}

}