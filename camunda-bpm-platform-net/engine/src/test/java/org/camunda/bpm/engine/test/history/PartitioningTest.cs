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
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQueryImpl = org.camunda.bpm.engine.impl.HistoricJobLogQueryImpl;
	using Page = org.camunda.bpm.engine.impl.Page;
	using HistoricBatchEntity = org.camunda.bpm.engine.impl.batch.history.HistoricBatchEntity;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using HistoricProcessInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricProcessInstanceEntity;
	using HistoricTaskInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricTaskInstanceEntity;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class PartitioningTest
	{
		private bool InstanceFieldsInitialized = false;

		public PartitioningTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal ManagementService managementService;

	  protected internal CommandExecutor commandExecutor;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;

		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		historyService = engineRule.HistoryService;
		managementService = engineRule.ManagementService;
	  }

	  protected internal readonly BpmnModelInstance PROCESS_WITH_USERTASK = Bpmn.createExecutableProcess("process").startEvent().userTask().endEvent().done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateHistoricProcessInstance()
	  public virtual void shouldUpdateHistoricProcessInstance()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).getId();
		string processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).Id;

		commandExecutor.execute(new CommandAnonymousInnerClass(this));

		// assume
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));

		// when
		runtimeService.deleteProcessInstance(processInstanceId, "aDeleteReason");

		// then
		assertThat(runtimeService.createProcessInstanceQuery().singleResult(), nullValue());

		// cleanup
		cleanUp(processInstanceId);
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly PartitioningTest outerInstance;

		  public CommandAnonymousInnerClass(PartitioningTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			HistoricProcessInstanceEntity historicProcessInstanceEntity = (HistoricProcessInstanceEntity) outerInstance.historyService.createHistoricProcessInstanceQuery().singleResult();

			commandContext.DbEntityManager.delete(historicProcessInstanceEntity);

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateHistoricTaskInstance()
	  public virtual void shouldUpdateHistoricTaskInstance()
	  {
		// given
		deployAndStartProcess(PROCESS_WITH_USERTASK).Id;

		commandExecutor.execute(new CommandAnonymousInnerClass2(this));

		// assume
		assertThat(historyService.createHistoricTaskInstanceQuery().singleResult(), nullValue());

		// when
		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.complete(taskId);

		// then
		assertThat(taskService.createTaskQuery().singleResult(), nullValue());
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly PartitioningTest outerInstance;

		  public CommandAnonymousInnerClass2(PartitioningTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			HistoricTaskInstanceEntity historicTaskInstanceEntity = (HistoricTaskInstanceEntity) outerInstance.historyService.createHistoricTaskInstanceQuery().singleResult();

			commandContext.DbEntityManager.delete(historicTaskInstanceEntity);

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateHistoricActivityInstance()
	  public virtual void shouldUpdateHistoricActivityInstance()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).getId();
		string processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).Id;

		commandExecutor.execute(new CommandAnonymousInnerClass3(this, processInstanceId));

		// assume
		assertThat(historyService.createHistoricActivityInstanceQuery().count(), @is(0L));

		// when
		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.complete(taskId);

		// then
		assertThat(historyService.createHistoricActivityInstanceQuery().count(), @is(1L));
	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly PartitioningTest outerInstance;

		  private string processInstanceId;

		  public CommandAnonymousInnerClass3(PartitioningTest outerInstance, string processInstanceId)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			commandContext.HistoricActivityInstanceManager.deleteHistoricActivityInstancesByProcessInstanceIds(Collections.singletonList(processInstanceId));

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateHistoricIncident()
	  public virtual void shouldUpdateHistoricIncident()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).getId();
		string processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).Id;

		ExecutionEntity execution = (ExecutionEntity) runtimeService.createExecutionQuery().singleResult();

		string incidentId = engineRule.RuntimeService.createIncident("foo", execution.Id, execution.ActivityId, "bar").Id;

		commandExecutor.execute(new CommandAnonymousInnerClass4(this, processInstanceId));

		// assume
		assertThat(historyService.createHistoricIncidentQuery().count(), @is(0L));
		assertThat(runtimeService.createIncidentQuery().count(), @is(1L));

		// when
		runtimeService.resolveIncident(incidentId);

		// then
		assertThat(runtimeService.createIncidentQuery().count(), @is(0L));
		assertThat(historyService.createHistoricIncidentQuery().count(), @is(0L));
	  }

	  private class CommandAnonymousInnerClass4 : Command<Void>
	  {
		  private readonly PartitioningTest outerInstance;

		  private string processInstanceId;

		  public CommandAnonymousInnerClass4(PartitioningTest outerInstance, string processInstanceId)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			commandContext.HistoricIncidentManager.deleteHistoricIncidentsByProcessInstanceIds(Collections.singletonList(processInstanceId));

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUpdateHistoricBatch()
	  public virtual void shouldUpdateHistoricBatch()
	  {
		// given
		string processInstanceId = deployAndStartProcess(PROCESS_WITH_USERTASK).Id;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.batch.Batch batch = runtimeService.deleteProcessInstancesAsync(java.util.Collections.singletonList(processInstanceId), "aDeleteReason");
		Batch batch = runtimeService.deleteProcessInstancesAsync(Collections.singletonList(processInstanceId), "aDeleteReason");

		// assume
		assertThat(historyService.createHistoricBatchQuery().count(), @is(1L));

		commandExecutor.execute(new CommandAnonymousInnerClass5(this));

		// assume
		assertThat(historyService.createHistoricBatchQuery().count(), @is(0L));

		// when
		string seedJobDefinitionId = batch.SeedJobDefinitionId;
		Job seedJob = managementService.createJobQuery().jobDefinitionId(seedJobDefinitionId).singleResult();
		managementService.executeJob(seedJob.Id);

		string batchJobDefinitionId = batch.BatchJobDefinitionId;
		IList<Job> batchJobs = managementService.createJobQuery().jobDefinitionId(batchJobDefinitionId).list();
		foreach (Job batchJob in batchJobs)
		{
		  managementService.executeJob(batchJob.Id);
		}

		IList<Job> monitorJobs = managementService.createJobQuery().jobDefinitionId(batch.MonitorJobDefinitionId).list();
		foreach (Job monitorJob in monitorJobs)
		{
		  managementService.executeJob(monitorJob.Id);
		}

		// then
		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(0L));
		assertThat(managementService.createBatchQuery().count(), @is(0L));

		// cleanup
		cleanUp(processInstanceId);
	  }

	  private class CommandAnonymousInnerClass5 : Command<Void>
	  {
		  private readonly PartitioningTest outerInstance;

		  public CommandAnonymousInnerClass5(PartitioningTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			HistoricBatchEntity historicBatchEntity = (HistoricBatchEntity) outerInstance.historyService.createHistoricBatchQuery().singleResult();

			commandContext.DbEntityManager.delete(historicBatchEntity);

			return null;
		  }
	  }

	  protected internal virtual ProcessInstance deployAndStartProcess(BpmnModelInstance bpmnModelInstance)
	  {
		testHelper.deploy(bpmnModelInstance);

		string processDefinitionKey = bpmnModelInstance.Definitions.RootElements.GetEnumerator().next().Id;
		return runtimeService.startProcessInstanceByKey(processDefinitionKey);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void cleanUp(final String processInstanceId)
	  protected internal virtual void cleanUp(string processInstanceId)
	  {
		commandExecutor.execute(new CommandAnonymousInnerClass6(this, processInstanceId));
	  }

	  private class CommandAnonymousInnerClass6 : Command<Void>
	  {
		  private readonly PartitioningTest outerInstance;

		  private string processInstanceId;

		  public CommandAnonymousInnerClass6(PartitioningTest outerInstance, string processInstanceId)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			commandContext.HistoricActivityInstanceManager.deleteHistoricActivityInstancesByProcessInstanceIds(Collections.singletonList(processInstanceId));

			commandContext.HistoricTaskInstanceManager.deleteHistoricTaskInstancesByProcessInstanceIds(Collections.singletonList(processInstanceId), true);

			IList<HistoricJobLog> historicJobLogs = commandContext.HistoricJobLogManager.findHistoricJobLogsByQueryCriteria(new HistoricJobLogQueryImpl(), new Page(0, 100));

			foreach (HistoricJobLog historicJobLog in historicJobLogs)
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(historicJobLog.JobId);
			}

			return null;
		  }
	  }

	}

}