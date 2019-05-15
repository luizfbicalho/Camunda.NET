using System;

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
namespace org.camunda.bpm.engine.test.api.history.removaltime
{
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using GetByteArrayCommand = org.camunda.bpm.engine.test.api.resources.GetByteArrayCommand;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using AfterClass = org.junit.AfterClass;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[RequiredHistoryLevel(HISTORY_FULL)]
	public abstract class AbstractRemovalTimeTest
	{
		private bool InstanceFieldsInitialized = false;

		public AbstractRemovalTimeTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal RuntimeService runtimeService;
	  protected internal FormService formService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;
	  protected internal ManagementService managementService;
	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal DecisionService decisionService;

	  protected internal static ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		formService = engineRule.FormService;
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		managementService = engineRule.ManagementService;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		externalTaskService = engineRule.ExternalTaskService;
		decisionService = engineRule.DecisionService;

		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void tearDownAfterAll()
	  public static void tearDownAfterAll()
	  {
		if (processEngineConfiguration != null)
		{
		  processEngineConfiguration.setHistoryRemovalTimeProvider(null).setHistoryRemovalTimeStrategy(null).initHistoryRemovalTime();

		  processEngineConfiguration.BatchOperationHistoryTimeToLive = null;
		  processEngineConfiguration.BatchOperationsForHistoryCleanup = null;

		  processEngineConfiguration.initHistoryCleanup();
		}

		ClockUtil.reset();
	  }

	  protected internal virtual ByteArrayEntity findByteArrayById(string byteArrayId)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		return commandExecutor.execute(new GetByteArrayCommand(byteArrayId));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearAttachment(final org.camunda.bpm.engine.task.Attachment attachment)
	  protected internal virtual void clearAttachment(Attachment attachment)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this, attachment));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly AbstractRemovalTimeTest outerInstance;

		  private Attachment attachment;

		  public CommandAnonymousInnerClass(AbstractRemovalTimeTest outerInstance, Attachment attachment)
		  {
			  this.outerInstance = outerInstance;
			  this.attachment = attachment;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.AttachmentManager.delete((AttachmentEntity) attachment);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearCommentByTaskId(final String taskId)
	  protected internal virtual void clearCommentByTaskId(string taskId)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass2(this, taskId));
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  private readonly AbstractRemovalTimeTest outerInstance;

		  private string taskId;

		  public CommandAnonymousInnerClass2(AbstractRemovalTimeTest outerInstance, string taskId)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.CommentManager.deleteCommentsByTaskId(taskId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearCommentByProcessInstanceId(final String processInstanceId)
	  protected internal virtual void clearCommentByProcessInstanceId(string processInstanceId)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass3(this, processInstanceId));
	  }

	  private class CommandAnonymousInnerClass3 : Command<object>
	  {
		  private readonly AbstractRemovalTimeTest outerInstance;

		  private string processInstanceId;

		  public CommandAnonymousInnerClass3(AbstractRemovalTimeTest outerInstance, string processInstanceId)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.CommentManager.deleteCommentsByProcessInstanceIds(Collections.singletonList(processInstanceId));
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearHistoricTaskInst(final String taskId)
	  protected internal virtual void clearHistoricTaskInst(string taskId)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass4(this, taskId));
	  }

	  private class CommandAnonymousInnerClass4 : Command<object>
	  {
		  private readonly AbstractRemovalTimeTest outerInstance;

		  private string taskId;

		  public CommandAnonymousInnerClass4(AbstractRemovalTimeTest outerInstance, string taskId)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricTaskInstanceManager.deleteHistoricTaskInstanceById(taskId);
			commandContext.HistoricIdentityLinkManager.deleteHistoricIdentityLinksLogByTaskId(taskId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearJobLog(final String jobId)
	  protected internal virtual void clearJobLog(string jobId)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass5(this, jobId));
	  }

	  private class CommandAnonymousInnerClass5 : Command<object>
	  {
		  private readonly AbstractRemovalTimeTest outerInstance;

		  private string jobId;

		  public CommandAnonymousInnerClass5(AbstractRemovalTimeTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(jobId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void clearHistoricIncident(final org.camunda.bpm.engine.history.HistoricIncident historicIncident)
	  protected internal virtual void clearHistoricIncident(HistoricIncident historicIncident)
	  {
		CommandExecutor commandExecutor = engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass6(this, historicIncident));
	  }

	  private class CommandAnonymousInnerClass6 : Command<object>
	  {
		  private readonly AbstractRemovalTimeTest outerInstance;

		  private HistoricIncident historicIncident;

		  public CommandAnonymousInnerClass6(AbstractRemovalTimeTest outerInstance, HistoricIncident historicIncident)
		  {
			  this.outerInstance = outerInstance;
			  this.historicIncident = historicIncident;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricIncidentManager.delete((HistoricIncidentEntity) historicIncident);
			return null;
		  }
	  }

	  protected internal virtual DateTime addDays(DateTime date, int amount)
	  {
		DateTime c = new DateTime();
		c = new DateTime(date);
		c.AddDays(amount);
		return c;
	  }

	}

}