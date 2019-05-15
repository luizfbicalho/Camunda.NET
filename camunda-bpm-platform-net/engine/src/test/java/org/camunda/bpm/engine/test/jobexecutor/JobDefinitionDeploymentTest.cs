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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using MessageJobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.MessageJobDeclaration;
	using TimerCatchIntermediateEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// These testcases verify that job definitions are created upon deployment of the process definition.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JobDefinitionDeploymentTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerStartEvent()
	  public virtual void testTimerStartEvent()
	  {

		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess").singleResult();

		// then assert
		assertNotNull(jobDefinition);
		assertEquals(TimerStartEventJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("theStart", jobDefinition.ActivityId);
		assertEquals("DATE: 2036-11-14T11:12:22", jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);

		// there exists a job with the correct job definition id:
		Job timerStartJob = managementService.createJobQuery().singleResult();
		assertEquals(jobDefinition.Id, timerStartJob.JobDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerBoundaryEvent()
	  public virtual void testTimerBoundaryEvent()
	  {

		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess").singleResult();

		// then assert
		assertNotNull(jobDefinition);
		assertEquals(TimerExecuteNestedActivityJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("theBoundaryEvent", jobDefinition.ActivityId);
		assertEquals("DATE: 2036-11-14T11:12:22", jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMultipleTimerBoundaryEvents()
	  public virtual void testMultipleTimerBoundaryEvents()
	  {

		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess");

		// then assert
		assertEquals(2, jobDefinitionQuery.count());

		JobDefinition jobDefinition = jobDefinitionQuery.activityIdIn("theBoundaryEvent1").singleResult();
		assertNotNull(jobDefinition);
		assertEquals(TimerExecuteNestedActivityJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("theBoundaryEvent1", jobDefinition.ActivityId);
		assertEquals("DATE: 2036-11-14T11:12:22", jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);

		jobDefinition = jobDefinitionQuery.activityIdIn("theBoundaryEvent2").singleResult();
		assertNotNull(jobDefinition);
		assertEquals(TimerExecuteNestedActivityJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("theBoundaryEvent2", jobDefinition.ActivityId);
		assertEquals("DURATION: PT5M", jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testEventBasedGateway()
	  public virtual void testEventBasedGateway()
	  {

		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinitionQuery jobDefinitionQuery = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess");

		// then assert
		assertEquals(2, jobDefinitionQuery.count());

		JobDefinition jobDefinition = jobDefinitionQuery.activityIdIn("timer1").singleResult();
		assertNotNull(jobDefinition);
		assertEquals(TimerCatchIntermediateEventJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("timer1", jobDefinition.ActivityId);
		assertEquals("DURATION: PT5M", jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);

		jobDefinition = jobDefinitionQuery.activityIdIn("timer2").singleResult();
		assertNotNull(jobDefinition);
		assertEquals(TimerCatchIntermediateEventJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("timer2", jobDefinition.ActivityId);
		assertEquals("DURATION: PT10M", jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTimerIntermediateEvent()
	  public virtual void testTimerIntermediateEvent()
	  {

		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess").singleResult();

		// then assert
		assertNotNull(jobDefinition);
		assertEquals(TimerCatchIntermediateEventJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("timer", jobDefinition.ActivityId);
		assertEquals("DURATION: PT5M", jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncContinuation()
	  public virtual void testAsyncContinuation()
	  {

		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess").singleResult();

		// then assert
		assertNotNull(jobDefinition);
		assertEquals(AsyncContinuationJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("theService", jobDefinition.ActivityId);
		assertEquals(MessageJobDeclaration.ASYNC_BEFORE, jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncContinuationOfMultiInstance()
	  public virtual void testAsyncContinuationOfMultiInstance()
	  {
		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess").singleResult();

		// then assert
		assertNotNull(jobDefinition);
		assertEquals(AsyncContinuationJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("theService" + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX, jobDefinition.ActivityId);
		assertEquals(MessageJobDeclaration.ASYNC_AFTER, jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAsyncContinuationOfActivityWrappedInMultiInstance()
	  public virtual void testAsyncContinuationOfActivityWrappedInMultiInstance()
	  {
		// given
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey("testProcess").singleResult();

		// then assert
		assertNotNull(jobDefinition);
		assertEquals(AsyncContinuationJobHandler.TYPE, jobDefinition.JobType);
		assertEquals("theService", jobDefinition.ActivityId);
		assertEquals(MessageJobDeclaration.ASYNC_AFTER, jobDefinition.JobConfiguration);
		assertEquals(processDefinition.Id, jobDefinition.ProcessDefinitionId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testAsyncContinuation.bpmn20.xml", "org/camunda/bpm/engine/test/jobexecutor/JobDefinitionDeploymentTest.testMultipleProcessesWithinDeployment.bpmn20.xml"})]
	  public virtual void testMultipleProcessDeployment()
	  {
		JobDefinitionQuery query = managementService.createJobDefinitionQuery();
		IList<JobDefinition> jobDefinitions = query.list();
		assertEquals(3, jobDefinitions.Count);

		assertEquals(1, query.processDefinitionKey("testProcess").list().size());
		assertEquals(2, query.processDefinitionKey("anotherTestProcess").list().size());
	  }

	}

}