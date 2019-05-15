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

	using IncidentContext = org.camunda.bpm.engine.impl.incident.IncidentContext;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using IncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.IncidentEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using BitMaskUtil = org.camunda.bpm.engine.impl.util.BitMaskUtil;
	using Execution = org.camunda.bpm.engine.runtime.Execution;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ExecutionCachedEntityStateTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessInstanceTasks()
	  public virtual void testProcessInstanceTasks()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.TASKS_STATE_BIT), processInstance.CachedEntityStateRaw);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionTasksScope()
	  public virtual void testExecutionTasksScope()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("userTask").singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.TASKS_STATE_BIT), execution.CachedEntityStateRaw);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionTasksParallel()
	  public virtual void testExecutionTasksParallel()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("userTask").singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.TASKS_STATE_BIT), execution.CachedEntityStateRaw);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionTasksMi()
	  public virtual void testExecutionTasksMi()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

		IList<Execution> executions = runtimeService.createExecutionQuery().activityId("userTask").list();
		foreach (Execution execution in executions)
		{
		  int cachedEntityStateRaw = ((ExecutionEntity) execution).CachedEntityStateRaw;
		  if (!((ExecutionEntity)execution).Scope)
		  {
			assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.TASKS_STATE_BIT) | BitMaskUtil.getMaskForBit(ExecutionEntity.VARIABLES_STATE_BIT), cachedEntityStateRaw);
		  }
		  else
		  {
			assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.VARIABLES_STATE_BIT), cachedEntityStateRaw);
		  }
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessInstanceEventSubscriptions()
	  public virtual void testProcessInstanceEventSubscriptions()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.EVENT_SUBSCRIPTIONS_STATE_BIT), processInstance.CachedEntityStateRaw);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionEventSubscriptionsScope()
	  public virtual void testExecutionEventSubscriptionsScope()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("IntermediateCatchEvent_1").singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.EVENT_SUBSCRIPTIONS_STATE_BIT), execution.CachedEntityStateRaw);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionEventSubscriptionsMi()
	  public virtual void testExecutionEventSubscriptionsMi()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

		IList<Execution> executions = runtimeService.createExecutionQuery().activityId("ReceiveTask_1").list();
		foreach (Execution execution in executions)
		{
		  int cachedEntityStateRaw = ((ExecutionEntity) execution).CachedEntityStateRaw;

		  if (!((ExecutionEntity)execution).Scope)
		  {
			assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.VARIABLES_STATE_BIT), cachedEntityStateRaw);
		  }
		  else
		  {
			assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.EVENT_SUBSCRIPTIONS_STATE_BIT), cachedEntityStateRaw);
		  }
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessInstanceJobs()
	  public virtual void testProcessInstanceJobs()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.JOBS_STATE_BIT), processInstance.CachedEntityStateRaw);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionJobsScope()
	  public virtual void testExecutionJobsScope()
	  {
		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("userTask").singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.JOBS_STATE_BIT), execution.CachedEntityStateRaw);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionJobsParallel()
	  public virtual void testExecutionJobsParallel()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("userTask").singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.JOBS_STATE_BIT), execution.CachedEntityStateRaw);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testProcessInstanceIncident()
	  public virtual void testProcessInstanceIncident()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity execution = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) runtimeService.createExecutionQuery().activityId("task").singleResult();
		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("task").singleResult();
		assertEquals(0, execution.CachedEntityStateRaw);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, execution));

		ExecutionEntity execution2 = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("task").singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.INCIDENT_STATE_BIT), execution2.CachedEntityStateRaw);

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly ExecutionCachedEntityStateTest outerInstance;

		  private ExecutionEntity execution;

		  public CommandAnonymousInnerClass(ExecutionCachedEntityStateTest outerInstance, ExecutionEntity execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IncidentContext incidentContext = new IncidentContext();
			incidentContext.ExecutionId = execution.Id;

			IncidentEntity.createAndInsertIncident("foo", incidentContext, null);

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionIncidentParallel()
	  public virtual void testExecutionIncidentParallel()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		ExecutionEntity processInstance = (ExecutionEntity) runtimeService.createProcessInstanceQuery().singleResult();
		assertEquals(0, processInstance.CachedEntityStateRaw);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity execution = (org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) runtimeService.createExecutionQuery().activityId("task").singleResult();
		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("task").singleResult();
		assertEquals(0, execution.CachedEntityStateRaw);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, execution));

		ExecutionEntity execution2 = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("task").singleResult();
		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.INCIDENT_STATE_BIT), execution2.CachedEntityStateRaw);

	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly ExecutionCachedEntityStateTest outerInstance;

		  private ExecutionEntity execution;

		  public CommandAnonymousInnerClass2(ExecutionCachedEntityStateTest outerInstance, ExecutionEntity execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IncidentContext incidentContext = new IncidentContext();
			incidentContext.ExecutionId = execution.Id;

			IncidentEntity.createAndInsertIncident("foo", incidentContext, null);

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionExternalTask()
	  public virtual void testExecutionExternalTask()
	  {
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().activityId("externalTask").singleResult();

		assertEquals(BitMaskUtil.getMaskForBit(ExecutionEntity.EXTERNAL_TASKS_BIT), execution.CachedEntityStateRaw);

	  }

	}

}