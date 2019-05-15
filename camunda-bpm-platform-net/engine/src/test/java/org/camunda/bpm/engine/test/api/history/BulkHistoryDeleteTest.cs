using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.history
{
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricDecisionInputInstance = org.camunda.bpm.engine.history.HistoricDecisionInputInstance;
	using HistoricDecisionInstance = org.camunda.bpm.engine.history.HistoricDecisionInstance;
	using HistoricDecisionOutputInstance = org.camunda.bpm.engine.history.HistoricDecisionOutputInstance;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using HistoricDecisionInputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInputInstanceEntity;
	using HistoricDecisionOutputInstanceEntity = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionOutputInstanceEntity;
	using HistoricExternalTaskLogEntity = org.camunda.bpm.engine.impl.history.@event.HistoricExternalTaskLogEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using HistoricJobLogEventEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogEventEntity;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using TestPojo = org.camunda.bpm.engine.test.dmn.businessruletask.TestPojo;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Svetlana Dorokhova
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class BulkHistoryDeleteTest
	{
		private bool InstanceFieldsInitialized = false;

		public BulkHistoryDeleteTest()
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


	  protected internal const string ONE_TASK_PROCESS = "oneTaskProcess";

	  public const int PROCESS_INSTANCE_COUNT = 5;

	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);
	  public ProcessEngineTestRule testRule;

	  private HistoryService historyService;
	  private TaskService taskService;
	  private RuntimeService runtimeService;
	  private FormService formService;
	  private ExternalTaskService externalTaskService;
	  private CaseService caseService;
	  private IdentityService identityService;

	  public const string USER_ID = "demo";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		taskService = engineRule.TaskService;
		formService = engineRule.FormService;
		externalTaskService = engineRule.ExternalTaskService;
		caseService = engineRule.CaseService;

		identityService = engineRule.IdentityService;
		identityService.AuthenticatedUserId = USER_ID;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		identityService.clearAuthentication();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupHistoryTaskIdentityLink()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupHistoryTaskIdentityLink()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();
		IList<Task> taskList = taskService.createTaskQuery().list();
		taskService.addUserIdentityLink(taskList[0].Id, "someUser", IdentityLinkType.ASSIGNEE);

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, historyService.createHistoricIdentityLinkLogQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupHistoryActivityInstances()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupHistoryActivityInstances()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, historyService.createHistoricActivityInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupTaskAttachmentWithContent()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupTaskAttachmentWithContent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();

		IList<Task> taskList = taskService.createTaskQuery().list();

		string taskWithAttachmentId = taskList[0].Id;
		createTaskAttachmentWithContent(taskWithAttachmentId);
		//remember contentId
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String contentId = findAttachmentContentId(taskService.getTaskAttachments(taskWithAttachmentId));
		string contentId = findAttachmentContentId(taskService.getTaskAttachments(taskWithAttachmentId));

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, taskService.getTaskAttachments(taskWithAttachmentId).Count);
		//check that attachment content was removed
		verifyByteArraysWereRemoved(contentId);
	  }

	  private string findAttachmentContentId(IList<Attachment> attachments)
	  {
		assertEquals(1, attachments.Count);
		return ((AttachmentEntity) attachments[0]).ContentId;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupProcessInstanceAttachmentWithContent()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupProcessInstanceAttachmentWithContent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();

		string processInstanceWithAttachmentId = ids[0];
		createProcessInstanceAttachmentWithContent(processInstanceWithAttachmentId);
		//remember contentId
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String contentId = findAttachmentContentId(taskService.getProcessInstanceAttachments(processInstanceWithAttachmentId));
		string contentId = findAttachmentContentId(taskService.getProcessInstanceAttachments(processInstanceWithAttachmentId));

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, taskService.getProcessInstanceAttachments(processInstanceWithAttachmentId).Count);
		//check that attachment content was removed
		verifyByteArraysWereRemoved(contentId);
	  }

	  private void createProcessInstanceAttachmentWithContent(string processInstanceId)
	  {
		taskService.createAttachment("web page", null, processInstanceId, "weatherforcast", "temperatures and more", new MemoryStream("someContent".GetBytes()));

		IList<Attachment> taskAttachments = taskService.getProcessInstanceAttachments(processInstanceId);
		assertEquals(1, taskAttachments.Count);
		assertNotNull(taskService.getAttachmentContent(taskAttachments[0].Id));
	  }

	  private void createTaskAttachmentWithContent(string taskId)
	  {
		taskService.createAttachment("web page", taskId, null, "weatherforcast", "temperatures and more", new MemoryStream("someContent".GetBytes()));

		IList<Attachment> taskAttachments = taskService.getTaskAttachments(taskId);
		assertEquals(1, taskAttachments.Count);
		assertNotNull(taskService.getAttachmentContent(taskAttachments[0].Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupTaskComment()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupTaskComment()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();

		IList<Task> taskList = taskService.createTaskQuery().list();

		string taskWithCommentId = taskList[2].Id;
		taskService.createComment(taskWithCommentId, null, "Some comment");

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, taskService.getTaskComments(taskWithCommentId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupProcessInstanceComment()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupProcessInstanceComment()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();

		string processInstanceWithCommentId = ids[0];
		taskService.createComment(null, processInstanceWithCommentId, "Some comment");

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, taskService.getProcessInstanceComments(processInstanceWithCommentId).Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupHistoricVariableInstancesAndHistoricDetails()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupHistoricVariableInstancesAndHistoricDetails()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();

		IList<Task> taskList = taskService.createTaskQuery().list();

		taskService.setVariables(taskList[0].Id, Variables);

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, historyService.createHistoricDetailQuery().count());
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupHistoryTaskForm()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupHistoryTaskForm()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();

		IList<Task> taskList = taskService.createTaskQuery().list();

		formService.submitTaskForm(taskList[0].Id, Variables);

		foreach (ProcessInstance processInstance in runtimeService.createProcessInstanceQuery().list())
		{
		  runtimeService.deleteProcessInstance(processInstance.ProcessInstanceId, null);
		}

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, historyService.createHistoricDetailQuery().count());
		assertEquals(0, historyService.createHistoricVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml") public void testCleanupHistoricExternalTaskLog()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml")]
	  public virtual void testCleanupHistoricExternalTaskLog()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses("oneExternalTaskProcess");
		IList<string> ids = prepareHistoricProcesses("oneExternalTaskProcess");

		string workerId = "aWrokerId";
		IList<LockedExternalTask> tasks = externalTaskService.fetchAndLock(1, workerId).topic("externalTaskTopic", 10000L).execute();

		externalTaskService.handleFailure(tasks[0].Id, workerId, "errorMessage", "exceptionStackTrace", 5, 3000L);

		//remember errorDetailsByteArrayId
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String errorDetailsByteArrayId = findErrorDetailsByteArrayId("errorMessage");
		string errorDetailsByteArrayId = findErrorDetailsByteArrayId("errorMessage");

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey(ONE_TASK_PROCESS).count());
		assertEquals(0, historyService.createHistoricExternalTaskLogQuery().count());
		//check that ByteArray was removed
		verifyByteArraysWereRemoved(errorDetailsByteArrayId);
	  }

	  private string findErrorDetailsByteArrayId(string errorMessage)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricExternalTaskLog> historicExternalTaskLogs = historyService.createHistoricExternalTaskLogQuery().errorMessage(errorMessage).list();
		IList<HistoricExternalTaskLog> historicExternalTaskLogs = historyService.createHistoricExternalTaskLogQuery().errorMessage(errorMessage).list();
		assertEquals(1, historicExternalTaskLogs.Count);

		return ((HistoricExternalTaskLogEntity) historicExternalTaskLogs[0]).ErrorDetailsByteArrayId;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn" }) public void testCleanupHistoricIncidents()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn" })]
	  public virtual void testCleanupHistoricIncidents()
	  {
		//given
		IList<string> ids = prepareHistoricProcesses("failingProcess");

		testRule.executeAvailableJobs();

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey("failingProcess").count());
		assertEquals(0, historyService.createHistoricIncidentQuery().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn" }) public void testCleanupHistoricJobLogs()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn" })]
	  public virtual void testCleanupHistoricJobLogs()
	  {
		//given
		IList<string> ids = prepareHistoricProcesses("failingProcess", null, 1);

		testRule.executeAvailableJobs();

		runtimeService.deleteProcessInstances(ids, null, true, true);

		IList<string> byteArrayIds = findExceptionByteArrayIds();

		//when
		historyService.deleteHistoricProcessInstancesBulk(ids);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey("failingProcess").count());
		assertEquals(0, historyService.createHistoricJobLogQuery().count());

		verifyByteArraysWereRemoved(byteArrayIds.ToArray());
	  }

	  private IList<string> findExceptionByteArrayIds()
	  {
		IList<string> exceptionByteArrayIds = new List<string>();
		IList<HistoricJobLog> historicJobLogs = historyService.createHistoricJobLogQuery().list();
		foreach (HistoricJobLog historicJobLog in historicJobLogs)
		{
		  HistoricJobLogEventEntity historicJobLogEventEntity = (HistoricJobLogEventEntity) historicJobLog;
		  if (!string.ReferenceEquals(historicJobLogEventEntity.ExceptionByteArrayId, null))
		  {
			exceptionByteArrayIds.Add(historicJobLogEventEntity.ExceptionByteArrayId);
		  }
		}
		return exceptionByteArrayIds;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void testCleanupHistoryDecisionData()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void testCleanupHistoryDecisionData()
	  {
		//given
		IList<string> ids = prepareHistoricProcesses("testProcess", Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37)));

		runtimeService.deleteProcessInstances(ids, null, true, true);

		//remember input and output ids
		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeInputs().includeOutputs().list();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> inputIds = new java.util.ArrayList<String>();
		IList<string> inputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> inputByteArrayIds = new java.util.ArrayList<String>();
		IList<string> inputByteArrayIds = new List<string>();
		collectHistoricDecisionInputIds(historicDecisionInstances, inputIds, inputByteArrayIds);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputIds = new java.util.ArrayList<String>();
		IList<string> outputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputByteArrayIds = new java.util.ArrayList<String>();
		IList<string> outputByteArrayIds = new List<string>();
		collectHistoricDecisionOutputIds(historicDecisionInstances, outputIds, outputByteArrayIds);

		//when
		historyService.deleteHistoricDecisionInstancesBulk(extractIds(historicDecisionInstances));

		//then
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());

		//check that decision inputs and outputs were removed
		assertDataDeleted(inputIds, inputByteArrayIds, outputIds, outputByteArrayIds);


		IList<UserOperationLogEntry> userOperationLogEntries = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).property("nrOfInstances").list();

		assertEquals(1, userOperationLogEntries.Count);

		UserOperationLogEntry entry = userOperationLogEntries[0];
		assertEquals(historicDecisionInstances.Count.ToString(), entry.NewValue);
		assertEquals(CATEGORY_OPERATOR, entry.Category);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void testCleanupFakeHistoryDecisionData()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/dmn/businessruletask/DmnBusinessRuleTaskTest.testDecisionRef.bpmn20.xml", "org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void testCleanupFakeHistoryDecisionData()
	  {
		//given
		IList<string> ids = Arrays.asList("aFake");

		//when
		historyService.deleteHistoricDecisionInstancesBulk(ids);

		//then expect no exception
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: void assertDataDeleted(final java.util.List<String> inputIds, final java.util.List<String> inputByteArrayIds, final java.util.List<String> outputIds, final java.util.List<String> outputByteArrayIds)
	  internal virtual void assertDataDeleted(IList<string> inputIds, IList<string> inputByteArrayIds, IList<string> outputIds, IList<string> outputByteArrayIds)
	  {
		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, inputIds, inputByteArrayIds, outputIds, outputByteArrayIds));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly BulkHistoryDeleteTest outerInstance;

		  private IList<string> inputIds;
		  private IList<string> inputByteArrayIds;
		  private IList<string> outputIds;
		  private IList<string> outputByteArrayIds;

		  public CommandAnonymousInnerClass(BulkHistoryDeleteTest outerInstance, IList<string> inputIds, IList<string> inputByteArrayIds, IList<string> outputIds, IList<string> outputByteArrayIds)
		  {
			  this.outerInstance = outerInstance;
			  this.inputIds = inputIds;
			  this.inputByteArrayIds = inputByteArrayIds;
			  this.outputIds = outputIds;
			  this.outputByteArrayIds = outputByteArrayIds;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			foreach (string inputId in inputIds)
			{
			  assertNull(commandContext.DbEntityManager.selectById(typeof(HistoricDecisionInputInstanceEntity), inputId));
			}
			foreach (string inputByteArrayId in inputByteArrayIds)
			{
			  assertNull(commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), inputByteArrayId));
			}
			foreach (string outputId in outputIds)
			{
			  assertNull(commandContext.DbEntityManager.selectById(typeof(HistoricDecisionOutputInstanceEntity), outputId));
			}
			foreach (string outputByteArrayId in outputByteArrayIds)
			{
			  assertNull(commandContext.DbEntityManager.selectById(typeof(ByteArrayEntity), outputByteArrayId));
			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" }) public void testCleanupHistoryStandaloneDecisionData()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/history/testDmnWithPojo.dmn11.xml" })]
	  public virtual void testCleanupHistoryStandaloneDecisionData()
	  {
		//given
		for (int i = 0; i < 5; i++)
		{
		  engineRule.DecisionService.evaluateDecisionByKey("testDecision").variables(Variables.createVariables().putValue("pojo", new TestPojo("okay", 13.37))).evaluate();
		}

		//remember input and output ids
		IList<HistoricDecisionInstance> historicDecisionInstances = historyService.createHistoricDecisionInstanceQuery().includeInputs().includeOutputs().list();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> inputIds = new java.util.ArrayList<String>();
		IList<string> inputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> inputByteArrayIds = new java.util.ArrayList<String>();
		IList<string> inputByteArrayIds = new List<string>();
		collectHistoricDecisionInputIds(historicDecisionInstances, inputIds, inputByteArrayIds);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputIds = new java.util.ArrayList<String>();
		IList<string> outputIds = new List<string>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> outputByteArrayIds = new java.util.ArrayList<String>();
		IList<string> outputByteArrayIds = new List<string>();
		collectHistoricDecisionOutputIds(historicDecisionInstances, outputIds, outputByteArrayIds);

		IList<string> decisionInstanceIds = extractIds(historicDecisionInstances);

		//when
		historyService.deleteHistoricDecisionInstancesBulk(decisionInstanceIds);

		//then
		assertEquals(0, historyService.createHistoricProcessInstanceQuery().processDefinitionKey("testProcess").count());
		assertEquals(0, historyService.createHistoricDecisionInstanceQuery().count());

		//check that decision inputs and outputs were removed
		assertDataDeleted(inputIds, inputByteArrayIds, outputIds, outputByteArrayIds);

	  }

	  private IList<string> extractIds(IList<HistoricDecisionInstance> historicDecisionInstances)
	  {
		IList<string> decisionInstanceIds = new List<string>();
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  decisionInstanceIds.Add(historicDecisionInstance.Id);
		}
		return decisionInstanceIds;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupHistoryEmptyProcessIdsException()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupHistoryEmptyProcessIdsException()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();
		runtimeService.deleteProcessInstances(ids, null, true, true);

		try
		{
		  historyService.deleteHistoricProcessInstancesBulk(null);
		  fail("Empty process instance ids exception was expected");
		}
		catch (BadUserRequestException)
		{
		}

		try
		{
		  historyService.deleteHistoricProcessInstancesBulk(new List<string>());
		  fail("Empty process instance ids exception was expected");
		}
		catch (BadUserRequestException)
		{
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) public void testCleanupHistoryProcessesNotFinishedException()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testCleanupHistoryProcessesNotFinishedException()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<String> ids = prepareHistoricProcesses();
		IList<string> ids = prepareHistoricProcesses();
		runtimeService.deleteProcessInstances(ids.subList(1, ids.Count), null, true, true);

		try
		{
		  historyService.deleteHistoricProcessInstancesBulk(ids);
		  fail("Not all processes are finished exception was expected");
		}
		catch (BadUserRequestException)
		{
		}

	  }

	  private void collectHistoricDecisionInputIds(IList<HistoricDecisionInstance> historicDecisionInstances, IList<string> historicDecisionInputIds, IList<string> inputByteArrayIds)
	  {
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  foreach (HistoricDecisionInputInstance inputInstanceEntity in historicDecisionInstance.Inputs)
		  {
			historicDecisionInputIds.Add(inputInstanceEntity.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String byteArrayValueId = ((org.camunda.bpm.engine.impl.history.event.HistoricDecisionInputInstanceEntity) inputInstanceEntity).getByteArrayValueId();
			string byteArrayValueId = ((HistoricDecisionInputInstanceEntity) inputInstanceEntity).ByteArrayValueId;
			if (!string.ReferenceEquals(byteArrayValueId, null))
			{
			  inputByteArrayIds.Add(byteArrayValueId);
			}
		  }
		}
		assertEquals(PROCESS_INSTANCE_COUNT, historicDecisionInputIds.Count);
	  }

	  private void collectHistoricDecisionOutputIds(IList<HistoricDecisionInstance> historicDecisionInstances, IList<string> historicDecisionOutputIds, IList<string> outputByteArrayId)
	  {
		foreach (HistoricDecisionInstance historicDecisionInstance in historicDecisionInstances)
		{
		  foreach (HistoricDecisionOutputInstance outputInstanceEntity in historicDecisionInstance.Outputs)
		  {
			historicDecisionOutputIds.Add(outputInstanceEntity.Id);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String byteArrayValueId = ((org.camunda.bpm.engine.impl.history.event.HistoricDecisionOutputInstanceEntity) outputInstanceEntity).getByteArrayValueId();
			string byteArrayValueId = ((HistoricDecisionOutputInstanceEntity) outputInstanceEntity).ByteArrayValueId;
			if (!string.ReferenceEquals(byteArrayValueId, null))
			{
			  outputByteArrayId.Add(byteArrayValueId);
			}
		  }
		}
		assertEquals(PROCESS_INSTANCE_COUNT, historicDecisionOutputIds.Count);
	  }

	  private IList<string> prepareHistoricProcesses()
	  {
		return prepareHistoricProcesses(ONE_TASK_PROCESS);
	  }

	  private IList<string> prepareHistoricProcesses(string businessKey)
	  {
		return prepareHistoricProcesses(businessKey, null);
	  }

	  private IList<string> prepareHistoricProcesses(string businessKey, VariableMap variables)
	  {
		return prepareHistoricProcesses(businessKey, variables, PROCESS_INSTANCE_COUNT);
	  }

	  private IList<string> prepareHistoricProcesses(string businessKey, VariableMap variables, int? processInstanceCount)
	  {
		IList<string> processInstanceIds = new List<string>();

		for (int i = 0; i < processInstanceCount.Value; i++)
		{
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey(businessKey, variables);
		  processInstanceIds.Add(processInstance.Id);
		}

		return processInstanceIds;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void verifyByteArraysWereRemoved(final String... errorDetailsByteArrayIds)
	  private void verifyByteArraysWereRemoved(params string[] errorDetailsByteArrayIds)
	  {
		engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, errorDetailsByteArrayIds));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly BulkHistoryDeleteTest outerInstance;

		  private string[] errorDetailsByteArrayIds;

		  public CommandAnonymousInnerClass2(BulkHistoryDeleteTest outerInstance, string[] errorDetailsByteArrayIds)
		  {
			  this.outerInstance = outerInstance;
			  this.errorDetailsByteArrayIds = errorDetailsByteArrayIds;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			foreach (string errorDetailsByteArrayId in errorDetailsByteArrayIds)
			{
			  assertNull(commandContext.DbEntityManager.selectOne("selectByteArray", errorDetailsByteArrayId));
			}
			return null;
		  }
	  }

	  private VariableMap Variables
	  {
		  get
		  {
			return Variables.createVariables().putValue("aVariableName", "aVariableValue").putValue("pojoVariableName", new TestPojo("someValue", 111.0));
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstance()
	  {
		// given
		// create case instances
		int instanceCount = 10;
		IList<string> caseInstanceIds = prepareHistoricCaseInstance(instanceCount);

		// assume
		IList<HistoricCaseInstance> caseInstanceList = historyService.createHistoricCaseInstanceQuery().list();
		assertEquals(instanceCount, caseInstanceList.Count);

		// when
		historyService.deleteHistoricCaseInstancesBulk(caseInstanceIds);

		// then
		assertEquals(0, historyService.createHistoricCaseInstanceQuery().count());
		assertEquals(0, historyService.createHistoricTaskInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseActivityInstance()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseActivityInstance()
	  {
		// given
		// create case instance
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;
		terminateAndCloseCaseInstance(caseInstanceId, null);

		// assume
		IList<HistoricCaseActivityInstance> activityInstances = historyService.createHistoricCaseActivityInstanceQuery().list();
		assertEquals(1, activityInstances.Count);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstanceId));

		// then
		activityInstances = historyService.createHistoricCaseActivityInstanceQuery().list();
		assertEquals(0, activityInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceTask()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceTask()
	  {
		// given
		// create case instance
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;
		terminateAndCloseCaseInstance(caseInstanceId, null);

		// assume
		IList<HistoricTaskInstance> taskInstances = historyService.createHistoricTaskInstanceQuery().list();
		assertEquals(1, taskInstances.Count);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstanceId));

		// then
		taskInstances = historyService.createHistoricTaskInstanceQuery().list();
		assertEquals(0, taskInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceTaskComment()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceTaskComment()
	  {
		// given
		// create case instance
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		Task task = taskService.createTaskQuery().singleResult();
		taskService.createComment(task.Id, null, "This is a comment...");

		// assume
		IList<Comment> comments = taskService.getTaskComments(task.Id);
		assertEquals(1, comments.Count);
		terminateAndCloseCaseInstance(caseInstanceId, null);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstanceId));

		// then
		comments = taskService.getTaskComments(task.Id);
		assertEquals(0, comments.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceTaskDetails()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceTaskDetails()
	  {
		// given
		// create case instance
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");

		Task task = taskService.createTaskQuery().singleResult();

		taskService.setVariable(task.Id, "boo", new TestPojo("foo", 123.0));
		taskService.setVariable(task.Id, "goo", 9);
		taskService.setVariable(task.Id, "boo", new TestPojo("foo", 321.0));


		// assume
		IList<HistoricDetail> detailsList = historyService.createHistoricDetailQuery().list();
		assertEquals(3, detailsList.Count);
		terminateAndCloseCaseInstance(caseInstance.Id, taskService.getVariables(task.Id));

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstance.Id));

		// then
		detailsList = historyService.createHistoricDetailQuery().list();
		assertEquals(0, detailsList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceTaskIdentityLink()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceTaskIdentityLink()
	  {
		// given
		// create case instance
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		Task task = taskService.createTaskQuery().singleResult();

		// assume
		taskService.addGroupIdentityLink(task.Id, "accounting", IdentityLinkType.CANDIDATE);
		int identityLinksForTask = taskService.getIdentityLinksForTask(task.Id).Count;
		assertEquals(1, identityLinksForTask);
		terminateAndCloseCaseInstance(caseInstanceId, null);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstanceId));

		// then
		IList<HistoricIdentityLinkLog> historicIdentityLinkLog = historyService.createHistoricIdentityLinkLogQuery().list();
		assertEquals(0, historicIdentityLinkLog.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceTaskAttachmentByteArray()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceTaskAttachmentByteArray()
	  {
		// given
		// create case instance
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");

		Task task = taskService.createTaskQuery().singleResult();
		string taskId = task.Id;
		taskService.createAttachment("foo", taskId, null, "something", null, new MemoryStream("someContent".GetBytes()));

		// assume
		IList<Attachment> attachments = taskService.getTaskAttachments(taskId);
		assertEquals(1, attachments.Count);
		string contentId = findAttachmentContentId(attachments);
		terminateAndCloseCaseInstance(caseInstance.Id, null);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstance.Id));

		// then
		attachments = taskService.getTaskAttachments(taskId);
		assertEquals(0, attachments.Count);
		verifyByteArraysWereRemoved(contentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceTaskAttachmentUrl()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceTaskAttachmentUrl()
	  {
		// given
		// create case instance
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		Task task = taskService.createTaskQuery().singleResult();
		taskService.createAttachment("foo", task.Id, null, "something", null, "http://camunda.org");

		// assume
		IList<Attachment> attachments = taskService.getTaskAttachments(task.Id);
		assertEquals(1, attachments.Count);
		terminateAndCloseCaseInstance(caseInstanceId, null);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstanceId));

		// then
		attachments = taskService.getTaskAttachments(task.Id);
		assertEquals(0, attachments.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceVariables()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceVariables()
	  {
		// given
		// create case instances
		IList<string> caseInstanceIds = new List<string>();
		int instanceCount = 10;
		for (int i = 0; i < instanceCount; i++)
		{
		  VariableMap variables = Variables.createVariables();
		  CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase", variables.putValue("name" + i, "theValue"));
		  caseInstanceIds.Add(caseInstance.Id);
		  terminateAndCloseCaseInstance(caseInstance.Id, variables);
		}
		// assume
		IList<HistoricVariableInstance> variablesInstances = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(instanceCount, variablesInstances.Count);

		// when
		historyService.deleteHistoricCaseInstancesBulk(caseInstanceIds);

		// then
		variablesInstances = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(0, variablesInstances.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceComplexVariable()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceComplexVariable()
	  {
		// given
		// create case instances
		VariableMap variables = Variables.createVariables();
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase", variables.putValue("pojo", new TestPojo("okay", 13.37)));

		caseService.setVariable(caseInstance.Id, "pojo", "theValue");

		// assume
		IList<HistoricVariableInstance> variablesInstances = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(1, variablesInstances.Count);
		IList<HistoricDetail> detailsList = historyService.createHistoricDetailQuery().list();
		assertEquals(2, detailsList.Count);
		terminateAndCloseCaseInstance(caseInstance.Id, variables);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstance.Id));

		// then
		variablesInstances = historyService.createHistoricVariableInstanceQuery().list();
		assertEquals(0, variablesInstances.Count);
		detailsList = historyService.createHistoricDetailQuery().list();
		assertEquals(0, detailsList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceDetails()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceDetails()
	  {
		// given
		// create case instances
		string variableNameCase1 = "varName1";
		CaseInstance caseInstance1 = caseService.createCaseInstanceByKey("oneTaskCase", Variables.createVariables().putValue(variableNameCase1, "value1"));
		CaseInstance caseInstance2 = caseService.createCaseInstanceByKey("oneTaskCase", Variables.createVariables().putValue("varName2", "value2"));

		caseService.setVariable(caseInstance1.Id, variableNameCase1, "theValue");

		// assume
		IList<HistoricDetail> detailsList = historyService.createHistoricDetailQuery().list();
		assertEquals(3, detailsList.Count);
		caseService.terminateCaseExecution(caseInstance1.Id, caseService.getVariables(caseInstance1.Id));
		caseService.terminateCaseExecution(caseInstance2.Id, caseService.getVariables(caseInstance2.Id));
		caseService.closeCaseInstance(caseInstance1.Id);
		caseService.closeCaseInstance(caseInstance2.Id);

		// when
		historyService.deleteHistoricCaseInstancesBulk(Arrays.asList(caseInstance1.Id, caseInstance2.Id));

		// then
		detailsList = historyService.createHistoricDetailQuery().list();
		assertEquals(0, detailsList.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) public void testCleanupHistoryCaseInstanceOperationLog()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCleanupHistoryCaseInstanceOperationLog()
	  {
		// given
		// create case instances
		int instanceCount = 10;
		IList<string> caseInstanceIds = prepareHistoricCaseInstance(instanceCount);

		// assume
		IList<HistoricCaseInstance> caseInstanceList = historyService.createHistoricCaseInstanceQuery().list();
		assertEquals(instanceCount, caseInstanceList.Count);

		// when
		identityService.AuthenticatedUserId = USER_ID;
		historyService.deleteHistoricCaseInstancesBulk(caseInstanceIds);
		identityService.clearAuthentication();

		// then
		assertEquals(1, historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).count());
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().operationType(OPERATION_TYPE_DELETE_HISTORY).singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, entry.Category);
		assertEquals(EntityTypes.CASE_INSTANCE, entry.EntityType);
		assertEquals(OPERATION_TYPE_DELETE_HISTORY, entry.OperationType);
		assertNull(entry.CaseInstanceId);
		assertEquals("nrOfInstances", entry.Property);
		assertNull(entry.OrgValue);
		assertEquals(instanceCount.ToString(), entry.NewValue);
	  }

	  private IList<string> prepareHistoricCaseInstance(int instanceCount)
	  {
		IList<string> caseInstanceIds = new List<string>();
		for (int i = 0; i < instanceCount; i++)
		{
		  CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");
		  string caseInstanceId = caseInstance.Id;
		  caseInstanceIds.Add(caseInstanceId);
		  terminateAndCloseCaseInstance(caseInstanceId, null);
		}
		return caseInstanceIds;
	  }

	  private void terminateAndCloseCaseInstance(string caseInstanceId, IDictionary<string, object> variables)
	  {
		if (variables == null)
		{
		  caseService.terminateCaseExecution(caseInstanceId, variables);
		}
		else
		{
		  caseService.terminateCaseExecution(caseInstanceId);
		}
		caseService.closeCaseInstance(caseInstanceId);
	  }
	}

}