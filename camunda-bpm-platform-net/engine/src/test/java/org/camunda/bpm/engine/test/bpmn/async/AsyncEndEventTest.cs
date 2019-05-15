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
namespace org.camunda.bpm.engine.test.bpmn.async
{

	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Stefan Hentschel
	/// </summary>
	public class AsyncEndEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncEndEvent()
	  public virtual void testAsyncEndEvent()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("asyncEndEvent");
		long count = runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).active().count();

		Assert.assertEquals(1, runtimeService.createExecutionQuery().activityId("endEvent").count());
		Assert.assertEquals(1, count);

		executeAvailableJobs();
		count = runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).count();

		Assert.assertEquals(0, runtimeService.createExecutionQuery().activityId("endEvent").active().count());
		Assert.assertEquals(0, count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncEndEventListeners()
	  public virtual void testAsyncEndEventListeners()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("asyncEndEvent");
		long count = runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).active().count();

		Assert.assertNull(runtimeService.getVariable(pi.Id, "listener"));
		Assert.assertEquals(1, runtimeService.createExecutionQuery().activityId("endEvent").count());
		Assert.assertEquals(1, count);

		// as we are standing at the end event, we execute it.
		executeAvailableJobs();

		count = runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).active().count();
		Assert.assertEquals(0, count);

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  // after the end event we have a event listener
		  HistoricVariableInstanceQuery name = historyService.createHistoricVariableInstanceQuery().processInstanceId(pi.Id).variableName("listener");
		  Assert.assertNotNull(name);
		  Assert.assertEquals("listener invoked", name.singleResult().Value);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleAsyncEndEvents()
	  public virtual void testMultipleAsyncEndEvents()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("multipleAsyncEndEvent");
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());

		// should stop at both end events
		IList<Job> jobs = managementService.createJobQuery().withRetriesLeft().list();
		assertEquals(2, jobs.Count);

		// execute one of the end events
		managementService.executeJob(jobs[0].Id);
		jobs = managementService.createJobQuery().withRetriesLeft().list();
		assertEquals(1, jobs.Count);

		// execute the second one
		managementService.executeJob(jobs[0].Id);
		// assert that we have finished our instance now
		assertEquals(0, runtimeService.createProcessInstanceQuery().count());

		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  // after the end event we have a event listener
		  HistoricVariableInstanceQuery name = historyService.createHistoricVariableInstanceQuery().processInstanceId(pi.Id).variableName("message");
		  Assert.assertNotNull(name);
		  Assert.assertEquals(true, name.singleResult().Value);

		}
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/AsyncEndEventTest.testCallActivity-super.bpmn20.xml", "org/camunda/bpm/engine/test/bpmn/async/AsyncEndEventTest.testCallActivity-sub.bpmn20.xml" })]
	  public virtual void testCallActivity()
	  {
		runtimeService.startProcessInstanceByKey("super");

		ProcessInstance pi = runtimeService.createProcessInstanceQuery().processDefinitionKey("sub").singleResult();

		assertTrue(pi is ExecutionEntity);

		assertEquals("theSubEnd", ((ExecutionEntity)pi).ActivityId);

	  }

	}

}