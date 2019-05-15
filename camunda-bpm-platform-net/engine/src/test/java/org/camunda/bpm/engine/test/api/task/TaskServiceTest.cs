using System;
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
namespace org.camunda.bpm.engine.test.api.task
{
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using HistoricDetail = org.camunda.bpm.engine.history.HistoricDetail;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricDetailVariableInstanceUpdateEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailVariableInstanceUpdateEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Comment = org.camunda.bpm.engine.task.Comment;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Event = org.camunda.bpm.engine.task.Event;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using SerializationDataFormat = org.camunda.bpm.engine.variable.value.SerializationDataFormat;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	/// <summary>
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// @author Falko Menge
	/// </summary>
	public class TaskServiceTest
	{
		private bool InstanceFieldsInitialized = false;

		public TaskServiceTest()
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
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal const string TWO_TASKS_PROCESS = "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml";

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }
	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  private RuntimeService runtimeService;
	  private TaskService taskService;
	  private RepositoryService repositoryService;
	  private HistoryService historyService;
	  private CaseService caseService;
	  private IdentityService identityService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

	  private static readonly SimpleDateFormat SDF = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss.SSS");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
		taskService = engineRule.TaskService;
		repositoryService = engineRule.RepositoryService;
		historyService = engineRule.HistoryService;
		caseService = engineRule.CaseService;
		identityService = engineRule.IdentityService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		ClockUtil.CurrentTime = DateTime.Now;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveTaskUpdate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSaveTaskUpdate()
	  {

		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss");
		Task task = taskService.newTask();
		task.Description = "description";
		task.Name = "taskname";
		task.Priority = 0;
		task.Assignee = "taskassignee";
		task.Owner = "taskowner";
		DateTime dueDate = sdf.parse("01/02/2003 04:05:06");
		task.DueDate = dueDate;
		task.CaseInstanceId = "taskcaseinstanceid";
		taskService.saveTask(task);

		// Fetch the task again and update
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("description", task.Description);
		assertEquals("taskname", task.Name);
		assertEquals("taskassignee", task.Assignee);
		assertEquals("taskowner", task.Owner);
		assertEquals(dueDate, task.DueDate);
		assertEquals(0, task.Priority);
		assertEquals("taskcaseinstanceid", task.CaseInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult();
		  assertEquals("taskname", historicTaskInstance.Name);
		  assertEquals("description", historicTaskInstance.Description);
		  assertEquals("taskassignee", historicTaskInstance.Assignee);
		  assertEquals("taskowner", historicTaskInstance.Owner);
		  assertEquals(dueDate, historicTaskInstance.DueDate);
		  assertEquals(0, historicTaskInstance.Priority);
		  assertEquals("taskcaseinstanceid", historicTaskInstance.CaseInstanceId);
		}

		task.Name = "updatedtaskname";
		task.Description = "updateddescription";
		task.Priority = 1;
		task.Assignee = "updatedassignee";
		task.Owner = "updatedowner";
		dueDate = sdf.parse("01/02/2003 04:05:06");
		task.DueDate = dueDate;
		task.CaseInstanceId = "updatetaskcaseinstanceid";
		taskService.saveTask(task);

		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("updatedtaskname", task.Name);
		assertEquals("updateddescription", task.Description);
		assertEquals("updatedassignee", task.Assignee);
		assertEquals("updatedowner", task.Owner);
		assertEquals(dueDate, task.DueDate);
		assertEquals(1, task.Priority);
		assertEquals("updatetaskcaseinstanceid", task.CaseInstanceId);

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult();
		  assertEquals("updatedtaskname", historicTaskInstance.Name);
		  assertEquals("updateddescription", historicTaskInstance.Description);
		  assertEquals("updatedassignee", historicTaskInstance.Assignee);
		  assertEquals("updatedowner", historicTaskInstance.Owner);
		  assertEquals(dueDate, historicTaskInstance.DueDate);
		  assertEquals(1, historicTaskInstance.Priority);
		  assertEquals("updatetaskcaseinstanceid", historicTaskInstance.CaseInstanceId);
		}

		// Finally, delete task
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveTaskSetParentTaskId()
	  public virtual void testSaveTaskSetParentTaskId()
	  {
		// given
		Task parent = taskService.newTask("parent");
		taskService.saveTask(parent);

		Task task = taskService.newTask("subTask");

		// when
		task.ParentTaskId = "parent";

		// then
		taskService.saveTask(task);

		// update task
		task = taskService.createTaskQuery().taskId("subTask").singleResult();

		assertEquals(parent.Id, task.ParentTaskId);

		taskService.deleteTask("parent", true);
		taskService.deleteTask("subTask", true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveTaskWithNonExistingParentTask()
	  public virtual void testSaveTaskWithNonExistingParentTask()
	  {
		// given
		Task task = taskService.newTask();

		// when
		task.ParentTaskId = "non-existing";

		// then
		try
		{
		  taskService.saveTask(task);
		  fail("It should not be possible to save a task with a non existing parent task.");
		}
		catch (NotValidException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskOwner()
	  public virtual void testTaskOwner()
	  {
		Task task = taskService.newTask();
		task.Owner = "johndoe";
		taskService.saveTask(task);

		// Fetch the task again and update
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("johndoe", task.Owner);

		task.Owner = "joesmoe";
		taskService.saveTask(task);

		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("joesmoe", task.Owner);

		// Finally, delete task
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskComments()
	  public virtual void testTaskComments()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  Task task = taskService.newTask();
		  task.Owner = "johndoe";
		  taskService.saveTask(task);
		  string taskId = task.Id;

		  identityService.AuthenticatedUserId = "johndoe";
		  // Fetch the task again and update
		  Comment comment = taskService.createComment(taskId, null, "look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd");
		  assertNotNull(comment.Id);
		  assertEquals("johndoe", comment.UserId);
		  assertEquals(taskId, comment.TaskId);
		  assertNull(comment.ProcessInstanceId);
		  assertEquals("look at this isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg ...", ((Event)comment).Message);
		  assertEquals("look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd", comment.FullMessage);
		  assertNotNull(comment.Time);

		  taskService.createComment(taskId, "pid", "one");
		  taskService.createComment(taskId, "pid", "two");

		  ISet<string> expectedComments = new HashSet<string>();
		  expectedComments.Add("one");
		  expectedComments.Add("two");

		  ISet<string> comments = new HashSet<string>();
		  foreach (Comment cmt in taskService.getProcessInstanceComments("pid"))
		  {
			comments.Add(cmt.FullMessage);
		  }

		  assertEquals(expectedComments, comments);

		  // Finally, delete task
		  taskService.deleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTaskCommentNull()
	  public virtual void testAddTaskCommentNull()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  Task task = taskService.newTask("testId");
		  taskService.saveTask(task);
		  try
		  {
			taskService.createComment(task.Id, null, null);
			fail("Expected process engine exception");
		  }
		  catch (ProcessEngineException)
		  {
		  }
		  finally
		  {
			taskService.deleteTask(task.Id, true);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTaskNullComment()
	  public virtual void testAddTaskNullComment()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  try
		  {
			taskService.createComment(null, null, "test");
			fail("Expected process engine exception");
		  }
		  catch (ProcessEngineException)
		  {
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskAttachments()
	  public virtual void testTaskAttachments()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  Task task = taskService.newTask();
		  task.Owner = "johndoe";
		  taskService.saveTask(task);
		  string taskId = task.Id;
		  identityService.AuthenticatedUserId = "johndoe";
		  // Fetch the task again and update
		  taskService.createAttachment("web page", taskId, "someprocessinstanceid", "weatherforcast", "temperatures and more", "http://weather.com");
		  Attachment attachment = taskService.getTaskAttachments(taskId)[0];
		  assertEquals("weatherforcast", attachment.Name);
		  assertEquals("temperatures and more", attachment.Description);
		  assertEquals("web page", attachment.Type);
		  assertEquals(taskId, attachment.TaskId);
		  assertEquals("someprocessinstanceid", attachment.ProcessInstanceId);
		  assertEquals("http://weather.com", attachment.Url);
		  assertNull(taskService.getAttachmentContent(attachment.Id));

		  // Finally, clean up
		  taskService.deleteTask(taskId);

		  assertEquals(0, taskService.getTaskComments(taskId).Count);
		  assertEquals(1, historyService.createHistoricTaskInstanceQuery().taskId(taskId).list().size());

		  taskService.deleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testProcessAttachmentsOneProcessExecution()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testProcessAttachmentsOneProcessExecution()
	  {
		  ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		  // create attachment
		  Attachment attachment = taskService.createAttachment("web page", null, processInstance.Id, "weatherforcast", "temperatures and more", "http://weather.com");

		  assertEquals("weatherforcast", attachment.Name);
		  assertEquals("temperatures and more", attachment.Description);
		  assertEquals("web page", attachment.Type);
		  assertNull(attachment.TaskId);
		  assertEquals(processInstance.Id, attachment.ProcessInstanceId);
		  assertEquals("http://weather.com", attachment.Url);
		  assertNull(taskService.getAttachmentContent(attachment.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/twoParallelTasksProcess.bpmn20.xml"}) public void testProcessAttachmentsTwoProcessExecutions()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/twoParallelTasksProcess.bpmn20.xml"})]
	  public virtual void testProcessAttachmentsTwoProcessExecutions()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoParallelTasksProcess");

		// create attachment
		Attachment attachment = taskService.createAttachment("web page", null, processInstance.Id, "weatherforcast", "temperatures and more", "http://weather.com");

		assertEquals("weatherforcast", attachment.Name);
		assertEquals("temperatures and more", attachment.Description);
		assertEquals("web page", attachment.Type);
		assertNull(attachment.TaskId);
		assertEquals(processInstance.Id, attachment.ProcessInstanceId);
		assertEquals("http://weather.com", attachment.Url);
		assertNull(taskService.getAttachmentContent(attachment.Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveAttachment()
	  public virtual void testSaveAttachment()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  // given
		  Task task = taskService.newTask();
		  taskService.saveTask(task);

		  string attachmentType = "someAttachment";
		  string processInstanceId = "someProcessInstanceId";
		  string attachmentName = "attachmentName";
		  string attachmentDescription = "attachmentDescription";
		  string url = "http://camunda.org";

		  Attachment attachment = taskService.createAttachment(attachmentType, task.Id, processInstanceId, attachmentName, attachmentDescription, url);

		  // when
		  attachment.Description = "updatedDescription";
		  attachment.Name = "updatedName";
		  taskService.saveAttachment(attachment);

		  // then
		  Attachment fetchedAttachment = taskService.getAttachment(attachment.Id);
		  assertEquals(attachment.Id, fetchedAttachment.Id);
		  assertEquals(attachmentType, fetchedAttachment.Type);
		  assertEquals(task.Id, fetchedAttachment.TaskId);
		  assertEquals(processInstanceId, fetchedAttachment.ProcessInstanceId);
		  assertEquals("updatedName", fetchedAttachment.Name);
		  assertEquals("updatedDescription", fetchedAttachment.Description);
		  assertEquals(url, fetchedAttachment.Url);

		  taskService.deleteTask(task.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDelegation()
	  public virtual void testTaskDelegation()
	  {
		Task task = taskService.newTask();
		task.Owner = "johndoe";
		task.@delegate("joesmoe");
		taskService.saveTask(task);
		string taskId = task.Id;

		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("johndoe", task.Owner);
		assertEquals("joesmoe", task.Assignee);
		assertEquals(DelegationState.PENDING, task.DelegationState);

		taskService.resolveTask(taskId);
		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("johndoe", task.Owner);
		assertEquals("johndoe", task.Assignee);
		assertEquals(DelegationState.RESOLVED, task.DelegationState);

		task.Assignee = null;
		task.DelegationState = null;
		taskService.saveTask(task);
		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("johndoe", task.Owner);
		assertNull(task.Assignee);
		assertNull(task.DelegationState);

		task.Assignee = "jackblack";
		task.DelegationState = DelegationState.RESOLVED;
		taskService.saveTask(task);
		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("johndoe", task.Owner);
		assertEquals("jackblack", task.Assignee);
		assertEquals(DelegationState.RESOLVED, task.DelegationState);

		// Finally, delete task
		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskDelegationThroughServiceCall()
	  public virtual void testTaskDelegationThroughServiceCall()
	  {
		Task task = taskService.newTask();
		task.Owner = "johndoe";
		taskService.saveTask(task);
		string taskId = task.Id;

		// Fetch the task again and update
		task = taskService.createTaskQuery().taskId(taskId).singleResult();

		taskService.delegateTask(taskId, "joesmoe");

		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("johndoe", task.Owner);
		assertEquals("joesmoe", task.Assignee);
		assertEquals(DelegationState.PENDING, task.DelegationState);

		taskService.resolveTask(taskId);

		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("johndoe", task.Owner);
		assertEquals("johndoe", task.Assignee);
		assertEquals(DelegationState.RESOLVED, task.DelegationState);

		// Finally, delete task
		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskAssignee()
	  public virtual void testTaskAssignee()
	  {
		Task task = taskService.newTask();
		task.Assignee = "johndoe";
		taskService.saveTask(task);

		// Fetch the task again and update
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("johndoe", task.Assignee);

		task.Assignee = "joesmoe";
		taskService.saveTask(task);

		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("joesmoe", task.Assignee);

		// Finally, delete task
		taskService.deleteTask(task.Id, true);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSaveTaskNullTask()
	  public virtual void testSaveTaskNullTask()
	  {
		try
		{
		  taskService.saveTask(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("task is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskNullTaskId()
	  public virtual void testDeleteTaskNullTaskId()
	  {
		try
		{
		  taskService.deleteTask(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskUnexistingTaskId()
	  public virtual void testDeleteTaskUnexistingTaskId()
	  {
		// Deleting unexisting task should be silently ignored
		taskService.deleteTask("unexistingtaskid");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTasksNullTaskIds()
	  public virtual void testDeleteTasksNullTaskIds()
	  {
		try
		{
		  taskService.deleteTasks(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTasksTaskIdsUnexistingTaskId()
	  public virtual void testDeleteTasksTaskIdsUnexistingTaskId()
	  {

		Task existingTask = taskService.newTask();
		taskService.saveTask(existingTask);

		// The unexisting taskId's should be silently ignored. Existing task should
		// have been deleted.
		taskService.deleteTasks(Arrays.asList("unexistingtaskid1", existingTask.Id), true);

		existingTask = taskService.createTaskQuery().taskId(existingTask.Id).singleResult();
		assertNull(existingTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClaimNullArguments()
	  public virtual void testClaimNullArguments()
	  {
		try
		{
		  taskService.claim(null, "userid");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClaimUnexistingTaskId()
	  public virtual void testClaimUnexistingTaskId()
	  {
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		try
		{
		  taskService.claim("unexistingtaskid", user.Id);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingtaskid", ae.Message);
		}

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClaimAlreadyClaimedTaskByOtherUser()
	  public virtual void testClaimAlreadyClaimedTaskByOtherUser()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		User user = identityService.newUser("user");
		identityService.saveUser(user);
		User secondUser = identityService.newUser("seconduser");
		identityService.saveUser(secondUser);

		// Claim task the first time
		taskService.claim(task.Id, user.Id);

		try
		{
		  taskService.claim(task.Id, secondUser.Id);
		  fail("ProcessEngineException expected");
		}
		catch (TaskAlreadyClaimedException ae)
		{
		  testRule.assertTextPresent("Task '" + task.Id + "' is already claimed by someone else.", ae.Message);
		}

		taskService.deleteTask(task.Id, true);
		identityService.deleteUser(user.Id);
		identityService.deleteUser(secondUser.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testClaimAlreadyClaimedTaskBySameUser()
	  public virtual void testClaimAlreadyClaimedTaskBySameUser()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		// Claim task the first time
		taskService.claim(task.Id, user.Id);
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();

		// Claim the task again with the same user. No exception should be thrown
		taskService.claim(task.Id, user.Id);

		taskService.deleteTask(task.Id, true);
		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnClaimTask()
	  public virtual void testUnClaimTask()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		// Claim task the first time
		taskService.claim(task.Id, user.Id);
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals(user.Id, task.Assignee);

		// Unclaim the task
		taskService.claim(task.Id, null);

		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertNull(task.Assignee);

		taskService.deleteTask(task.Id, true);
		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskNullTaskId()
	  public virtual void testCompleteTaskNullTaskId()
	  {
		try
		{
		  taskService.complete(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskUnexistingTaskId()
	  public virtual void testCompleteTaskUnexistingTaskId()
	  {
		try
		{
		  taskService.complete("unexistingtask");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingtask", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskWithParametersNullTaskId()
	  public virtual void testCompleteTaskWithParametersNullTaskId()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["myKey"] = "myValue";

		try
		{
		  taskService.complete(null, variables);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskWithParametersUnexistingTaskId()
	  public virtual void testCompleteTaskWithParametersUnexistingTaskId()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["myKey"] = "myValue";

		try
		{
		  taskService.complete("unexistingtask", variables);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingtask", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskWithParametersNullParameters()
	  public virtual void testCompleteTaskWithParametersNullParameters()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);

		string taskId = task.Id;
		taskService.complete(taskId, null);

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  historyService.deleteHistoricTaskInstance(taskId);
		}

		// Fetch the task again
		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertNull(task);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testCompleteTaskWithParametersEmptyParameters()
	  public virtual void testCompleteTaskWithParametersEmptyParameters()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);

		string taskId = task.Id;
		taskService.complete(taskId, Collections.EMPTY_MAP);

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{
		  historyService.deleteHistoricTaskInstance(taskId);
		}

		// Fetch the task again
		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertNull(task);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = TWO_TASKS_PROCESS) @Test public void testCompleteWithParametersTask()
	  [Deployment(resources : TWO_TASKS_PROCESS)]
	  public virtual void testCompleteWithParametersTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoTasksProcess");

		// Fetch first task
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("First task", task.Name);

		// Complete first task
		IDictionary<string, object> taskParams = new Dictionary<string, object>();
		taskParams["myParam"] = "myValue";
		taskService.complete(task.Id, taskParams);

		// Fetch second task
		task = taskService.createTaskQuery().singleResult();
		assertEquals("Second task", task.Name);

		// Verify task parameters set on execution
		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(1, variables.Count);
		assertEquals("myValue", variables["myParam"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/task/TaskServiceTest.testCompleteTaskWithVariablesInReturn.bpmn20.xml" }) @Test public void testCompleteTaskWithVariablesInReturn()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/task/TaskServiceTest.testCompleteTaskWithVariablesInReturn.bpmn20.xml" })]
	  public virtual void testCompleteTaskWithVariablesInReturn()
	  {
		string processVarName = "processVar";
		string processVarValue = "processVarValue";

		string taskVarName = "taskVar";
		string taskVarValue = "taskVarValue";

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[processVarName] = processVarValue;

		runtimeService.startProcessInstanceByKey("TaskServiceTest.testCompleteTaskWithVariablesInReturn", variables);

		Task firstUserTask = taskService.createTaskQuery().taskName("First User Task").singleResult();
		taskService.setVariable(firstUserTask.Id, "x", 1);
		// local variables should not be returned
		taskService.setVariableLocal(firstUserTask.Id, "localVar", "localVarValue");

		IDictionary<string, object> additionalVariables = new Dictionary<string, object>();
		additionalVariables[taskVarName] = taskVarValue;

		// After completion of firstUserTask a script Task sets 'x' = 5
		VariableMap vars = taskService.completeWithVariablesInReturn(firstUserTask.Id, additionalVariables, true);

		assertEquals(3, vars.size());
		assertEquals(5, vars.get("x"));
		assertEquals(ValueType.INTEGER, vars.getValueTyped("x").Type);
		assertEquals(processVarValue, vars.get(processVarName));
		assertEquals(taskVarValue, vars.get(taskVarName));
		assertEquals(ValueType.STRING, vars.getValueTyped(taskVarName).Type);

		additionalVariables = new Dictionary<>();
		additionalVariables["x"] = 7;
		Task secondUserTask = taskService.createTaskQuery().taskName("Second User Task").singleResult();

		vars = taskService.completeWithVariablesInReturn(secondUserTask.Id, additionalVariables, true);
		assertEquals(3, vars.size());
		assertEquals(7, vars.get("x"));
		assertEquals(processVarValue, vars.get(processVarName));
		assertEquals(taskVarValue, vars.get(taskVarName));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testCompleteStandaloneTaskWithVariablesInReturn()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testCompleteStandaloneTaskWithVariablesInReturn()
	  {
		string taskVarName = "taskVar";
		string taskVarValue = "taskVarValue";

		string taskId = "myTask";
		Task standaloneTask = taskService.newTask(taskId);
		taskService.saveTask(standaloneTask);

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[taskVarName] = taskVarValue;

		IDictionary<string, object> returnedVariables = taskService.completeWithVariablesInReturn(taskId, variables, true);
		// expect empty Map for standalone tasks
		assertEquals(0, returnedVariables.Count);

		historyService.deleteHistoricTaskInstance(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/twoParallelTasksProcess.bpmn20.xml" }) @Test public void testCompleteTaskWithVariablesInReturnParallel()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/twoParallelTasksProcess.bpmn20.xml" })]
	  public virtual void testCompleteTaskWithVariablesInReturnParallel()
	  {
		string processVarName = "processVar";
		string processVarValue = "processVarValue";

		string task1VarName = "taskVar1";
		string task2VarName = "taskVar2";
		string task1VarValue = "taskVarValue1";
		string task2VarValue = "taskVarValue2";

		string additionalVar = "additionalVar";
		string additionalVarValue = "additionalVarValue";

		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables[processVarName] = processVarValue;
		runtimeService.startProcessInstanceByKey("twoParallelTasksProcess", variables);

		Task firstTask = taskService.createTaskQuery().taskName("First Task").singleResult();
		taskService.setVariable(firstTask.Id, task1VarName, task1VarValue);
		Task secondTask = taskService.createTaskQuery().taskName("Second Task").singleResult();
		taskService.setVariable(secondTask.Id, task2VarName, task2VarValue);

		IDictionary<string, object> vars = taskService.completeWithVariablesInReturn(firstTask.Id, null, true);

		assertEquals(3, vars.Count);
		assertEquals(processVarValue, vars[processVarName]);
		assertEquals(task1VarValue, vars[task1VarName]);
		assertEquals(task2VarValue, vars[task2VarName]);

		IDictionary<string, object> additionalVariables = new Dictionary<string, object>();
		additionalVariables[additionalVar] = additionalVarValue;

		vars = taskService.completeWithVariablesInReturn(secondTask.Id, additionalVariables, true);
		assertEquals(4, vars.Count);
		assertEquals(processVarValue, vars[processVarName]);
		assertEquals(task1VarValue, vars[task1VarName]);
		assertEquals(task2VarValue, vars[task2VarName]);
		assertEquals(additionalVarValue, vars[additionalVar]);
	  }

	  /// <summary>
	  /// Tests that the variablesInReturn logic is not applied
	  /// when we call the regular complete API. This is a performance optimization.
	  /// Loading all variables may be expensive.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteTaskAndDoNotDeserializeVariables()
	  public virtual void testCompleteTaskAndDoNotDeserializeVariables()
	  {
		// given
		BpmnModelInstance process = Bpmn.createExecutableProcess("process").startEvent().subProcess().embeddedSubProcess().startEvent().userTask("task1").userTask("task2").endEvent().subProcessDone().endEvent().done();

		testRule.deploy(process);

		runtimeService.startProcessInstanceByKey("process", Variables.putValue("var", "val"));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.task.Task task = taskService.createTaskQuery().singleResult();
		Task task = taskService.createTaskQuery().singleResult();

		// when
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hasLoadedAnyVariables = processEngineConfiguration.getCommandExecutorTxRequired().execute(new org.camunda.bpm.engine.impl.interceptor.Command<bool>()
		bool hasLoadedAnyVariables = processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, task));

		// then
		assertThat(hasLoadedAnyVariables).False;
	  }

	  private class CommandAnonymousInnerClass : Command<bool>
	  {
		  private readonly TaskServiceTest outerInstance;

		  private Task task;

		  public CommandAnonymousInnerClass(TaskServiceTest outerInstance, Task task)
		  {
			  this.outerInstance = outerInstance;
			  this.task = task;
		  }


		  public bool? execute(CommandContext commandContext)
		  {
			outerInstance.taskService.complete(task.Id);
			return commandContext.DbEntityManager.getCachedEntitiesByType(typeof(VariableInstanceEntity)).Count > 0;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml") public void testCompleteTaskWithVariablesInReturnShouldDeserializeObjectValue()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml")]
	  public virtual void testCompleteTaskWithVariablesInReturnShouldDeserializeObjectValue()
	  {
		// given
		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		runtimeService.startProcessInstanceByKey("twoTasksProcess", variables);

		Task task = taskService.createTaskQuery().singleResult();

		// when
		VariableMap result = taskService.completeWithVariablesInReturn(task.Id, null, true);

		// then
		ObjectValue returnedValue = result.getValueTyped("var");
		assertThat(returnedValue.Deserialized).True;
		assertThat(returnedValue.Value).isEqualTo("value");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml") public void testCompleteTaskWithVariablesInReturnShouldNotDeserializeObjectValue()
	  [Deployment(resources : "org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml")]
	  public virtual void testCompleteTaskWithVariablesInReturnShouldNotDeserializeObjectValue()
	  {
		// given
		ObjectValue value = Variables.objectValue("value").create();
		VariableMap variables = Variables.createVariables().putValue("var", value);

		ProcessInstance instance = runtimeService.startProcessInstanceByKey("twoTasksProcess", variables);
		string serializedValue = ((ObjectValue) runtimeService.getVariableTyped(instance.Id, "var")).ValueSerialized;

		Task task = taskService.createTaskQuery().singleResult();

		// when
		VariableMap result = taskService.completeWithVariablesInReturn(task.Id, null, false);

		// then
		ObjectValue returnedValue = result.getValueTyped("var");
		assertThat(returnedValue.Deserialized).False;
		assertThat(returnedValue.ValueSerialized).isEqualTo(serializedValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" }) @Test public void testCompleteTaskWithVariablesInReturnCMMN()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn" })]
	  public virtual void testCompleteTaskWithVariablesInReturnCMMN()
	  {
		string taskVariableName = "taskVar";
		string taskVariableValue = "taskVal";

		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;
		caseService.withCaseDefinition(caseDefinitionId).create();

		Task task1 = taskService.createTaskQuery().singleResult();
		assertNotNull(task1);

		taskService.setVariable(task1.Id, taskVariableName, taskVariableValue);
		IDictionary<string, object> vars = taskService.completeWithVariablesInReturn(task1.Id, null, true);
		assertNull(vars);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) @Test public void testCompleteTaskShouldCompleteCaseExecution()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testCompleteTaskShouldCompleteCaseExecution()
	  {
		// given
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// when
		taskService.complete(task.Id);

		// then

		task = taskService.createTaskQuery().singleResult();

		assertNull(task);

		CaseExecution caseExecution = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		assertNull(caseExecution);

		CaseInstance caseInstance = caseService.createCaseInstanceQuery().singleResult();

		assertNotNull(caseInstance);
		assertTrue(caseInstance.Completed);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskNullTaskId()
	  public virtual void testResolveTaskNullTaskId()
	  {
		try
		{
		  taskService.resolveTask(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskUnexistingTaskId()
	  public virtual void testResolveTaskUnexistingTaskId()
	  {
		try
		{
		  taskService.resolveTask("unexistingtask");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingtask", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolveTaskWithParametersNullParameters()
	  public virtual void testResolveTaskWithParametersNullParameters()
	  {
		Task task = taskService.newTask();
		task.DelegationState = DelegationState.PENDING;
		taskService.saveTask(task);

		string taskId = task.Id;
		taskService.resolveTask(taskId, null);

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  historyService.deleteHistoricTaskInstance(taskId);
		}

		// Fetch the task again
		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals(DelegationState.RESOLVED, task.DelegationState);

		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testResolveTaskWithParametersEmptyParameters()
	  public virtual void testResolveTaskWithParametersEmptyParameters()
	  {
		Task task = taskService.newTask();
		task.DelegationState = DelegationState.PENDING;
		taskService.saveTask(task);

		string taskId = task.Id;
		taskService.resolveTask(taskId, Collections.EMPTY_MAP);

		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_AUDIT)
		{
		  historyService.deleteHistoricTaskInstance(taskId);
		}

		// Fetch the task again
		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals(DelegationState.RESOLVED, task.DelegationState);

		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = TWO_TASKS_PROCESS) @Test public void testResolveWithParametersTask()
	  [Deployment(resources : TWO_TASKS_PROCESS)]
	  public virtual void testResolveWithParametersTask()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("twoTasksProcess");

		// Fetch first task
		Task task = taskService.createTaskQuery().singleResult();
		assertEquals("First task", task.Name);

		task.@delegate("johndoe");

		// Resolve first task
		IDictionary<string, object> taskParams = new Dictionary<string, object>();
		taskParams["myParam"] = "myValue";
		taskService.resolveTask(task.Id, taskParams);

		// Verify that task is resolved
		task = taskService.createTaskQuery().taskDelegationState(DelegationState.RESOLVED).singleResult();
		assertEquals("First task", task.Name);

		// Verify task parameters set on execution
		IDictionary<string, object> variables = runtimeService.getVariables(processInstance.Id);
		assertEquals(1, variables.Count);
		assertEquals("myValue", variables["myParam"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAssignee()
	  public virtual void testSetAssignee()
	  {
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		Task task = taskService.newTask();
		assertNull(task.Assignee);
		taskService.saveTask(task);

		// Set assignee
		taskService.setAssignee(task.Id, user.Id);

		// Fetch task again
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals(user.Id, task.Assignee);

		identityService.deleteUser(user.Id);
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAssigneeNullTaskId()
	  public virtual void testSetAssigneeNullTaskId()
	  {
		try
		{
		  taskService.setAssignee(null, "userId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetAssigneeUnexistingTask()
	  public virtual void testSetAssigneeUnexistingTask()
	  {
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		try
		{
		  taskService.setAssignee("unexistingTaskId", user.Id);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
		}

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCandidateUserDuplicate()
	  public virtual void testAddCandidateUserDuplicate()
	  {
		// Check behavior when adding the same user twice as candidate
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		Task task = taskService.newTask();
		taskService.saveTask(task);

		taskService.addCandidateUser(task.Id, user.Id);

		// Add as candidate the second time
		taskService.addCandidateUser(task.Id, user.Id);

		identityService.deleteUser(user.Id);
		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCandidateUserNullTaskId()
	  public virtual void testAddCandidateUserNullTaskId()
	  {
		try
		{
		  taskService.addCandidateUser(null, "userId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCandidateUserNullUserId()
	  public virtual void testAddCandidateUserNullUserId()
	  {
		try
		{
		  taskService.addCandidateUser("taskId", null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("userId and groupId cannot both be null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCandidateUserUnexistingTask()
	  public virtual void testAddCandidateUserUnexistingTask()
	  {
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		try
		{
		  taskService.addCandidateUser("unexistingTaskId", user.Id);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
		}

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCandidateGroupNullTaskId()
	  public virtual void testAddCandidateGroupNullTaskId()
	  {
		try
		{
		  taskService.addCandidateGroup(null, "groupId");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCandidateGroupNullGroupId()
	  public virtual void testAddCandidateGroupNullGroupId()
	  {
		try
		{
		  taskService.addCandidateGroup("taskId", null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("userId and groupId cannot both be null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddCandidateGroupUnexistingTask()
	  public virtual void testAddCandidateGroupUnexistingTask()
	  {
		Group group = identityService.newGroup("group");
		identityService.saveGroup(group);
		try
		{
		  taskService.addCandidateGroup("unexistingTaskId", group.Id);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
		}
		identityService.deleteGroup(group.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGroupIdentityLinkNullTaskId()
	  public virtual void testAddGroupIdentityLinkNullTaskId()
	  {
		try
		{
		  taskService.addGroupIdentityLink(null, "groupId", IdentityLinkType.CANDIDATE);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGroupIdentityLinkNullUserId()
	  public virtual void testAddGroupIdentityLinkNullUserId()
	  {
		try
		{
		  taskService.addGroupIdentityLink("taskId", null, IdentityLinkType.CANDIDATE);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("userId and groupId cannot both be null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGroupIdentityLinkUnexistingTask()
	  public virtual void testAddGroupIdentityLinkUnexistingTask()
	  {
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		try
		{
		  taskService.addGroupIdentityLink("unexistingTaskId", user.Id, IdentityLinkType.CANDIDATE);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
		}

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddUserIdentityLinkNullTaskId()
	  public virtual void testAddUserIdentityLinkNullTaskId()
	  {
		try
		{
		  taskService.addUserIdentityLink(null, "userId", IdentityLinkType.CANDIDATE);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddUserIdentityLinkNullUserId()
	  public virtual void testAddUserIdentityLinkNullUserId()
	  {
		try
		{
		  taskService.addUserIdentityLink("taskId", null, IdentityLinkType.CANDIDATE);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("userId and groupId cannot both be null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddUserIdentityLinkUnexistingTask()
	  public virtual void testAddUserIdentityLinkUnexistingTask()
	  {
		User user = identityService.newUser("user");
		identityService.saveUser(user);

		try
		{
		  taskService.addUserIdentityLink("unexistingTaskId", user.Id, IdentityLinkType.CANDIDATE);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
		}

		identityService.deleteUser(user.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksWithCandidateUser()
	  public virtual void testGetIdentityLinksWithCandidateUser()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		identityService.saveUser(identityService.newUser("kermit"));

		taskService.addCandidateUser(taskId, "kermit");
		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		assertEquals(1, identityLinks.Count);
		assertEquals("kermit", identityLinks[0].UserId);
		assertNull(identityLinks[0].GroupId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLinks[0].Type);

		//cleanup
		taskService.deleteTask(taskId, true);
		identityService.deleteUser("kermit");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksWithCandidateGroup()
	  public virtual void testGetIdentityLinksWithCandidateGroup()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		identityService.saveGroup(identityService.newGroup("muppets"));

		taskService.addCandidateGroup(taskId, "muppets");
		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		assertEquals(1, identityLinks.Count);
		assertEquals("muppets", identityLinks[0].GroupId);
		assertNull(identityLinks[0].UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLinks[0].Type);

		//cleanup
		taskService.deleteTask(taskId, true);
		identityService.deleteGroup("muppets");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksWithAssignee()
	  public virtual void testGetIdentityLinksWithAssignee()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		identityService.saveUser(identityService.newUser("kermit"));

		taskService.claim(taskId, "kermit");
		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		assertEquals(1, identityLinks.Count);
		assertEquals("kermit", identityLinks[0].UserId);
		assertNull(identityLinks[0].GroupId);
		assertEquals(IdentityLinkType.ASSIGNEE, identityLinks[0].Type);

		//cleanup
		taskService.deleteTask(taskId, true);
		identityService.deleteUser("kermit");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksWithNonExistingAssignee()
	  public virtual void testGetIdentityLinksWithNonExistingAssignee()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		taskService.claim(taskId, "nonExistingAssignee");
		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		assertEquals(1, identityLinks.Count);
		assertEquals("nonExistingAssignee", identityLinks[0].UserId);
		assertNull(identityLinks[0].GroupId);
		assertEquals(IdentityLinkType.ASSIGNEE, identityLinks[0].Type);

		//cleanup
		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksWithOwner()
	  public virtual void testGetIdentityLinksWithOwner()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		identityService.saveUser(identityService.newUser("kermit"));
		identityService.saveUser(identityService.newUser("fozzie"));

		taskService.claim(taskId, "kermit");
		taskService.delegateTask(taskId, "fozzie");

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		assertEquals(2, identityLinks.Count);

		IdentityLink assignee = identityLinks[0];
		assertEquals("fozzie", assignee.UserId);
		assertNull(assignee.GroupId);
		assertEquals(IdentityLinkType.ASSIGNEE, assignee.Type);

		IdentityLink owner = identityLinks[1];
		assertEquals("kermit", owner.UserId);
		assertNull(owner.GroupId);
		assertEquals(IdentityLinkType.OWNER, owner.Type);

		//cleanup
		taskService.deleteTask(taskId, true);
		identityService.deleteUser("kermit");
		identityService.deleteUser("fozzie");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetIdentityLinksWithNonExistingOwner()
	  public virtual void testGetIdentityLinksWithNonExistingOwner()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);
		string taskId = task.Id;

		taskService.claim(taskId, "nonExistingOwner");
		taskService.delegateTask(taskId, "nonExistingAssignee");
		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		assertEquals(2, identityLinks.Count);

		IdentityLink assignee = identityLinks[0];
		assertEquals("nonExistingAssignee", assignee.UserId);
		assertNull(assignee.GroupId);
		assertEquals(IdentityLinkType.ASSIGNEE, assignee.Type);

		IdentityLink owner = identityLinks[1];
		assertEquals("nonExistingOwner", owner.UserId);
		assertNull(owner.GroupId);
		assertEquals(IdentityLinkType.OWNER, owner.Type);

		//cleanup
		taskService.deleteTask(taskId, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriority()
	  public virtual void testSetPriority()
	  {
		Task task = taskService.newTask();
		taskService.saveTask(task);

		taskService.setPriority(task.Id, 12345);

		// Fetch task again to check if the priority is set
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals(12345, task.Priority);

		taskService.deleteTask(task.Id, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityUnexistingTaskId()
	  public virtual void testSetPriorityUnexistingTaskId()
	  {
		try
		{
		  taskService.setPriority("unexistingtask", 12345);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("Cannot find task with id unexistingtask", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetPriorityNullTaskId()
	  public virtual void testSetPriorityNullTaskId()
	  {
		try
		{
		  taskService.setPriority(null, 12345);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

	  /// <seealso cref= http://jira.codehaus.org/browse/ACT-1059 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetDelegationState()
	  public virtual void testSetDelegationState()
	  {
		Task task = taskService.newTask();
		task.Owner = "wuzh";
		task.@delegate("other");
		taskService.saveTask(task);
		string taskId = task.Id;

		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("wuzh", task.Owner);
		assertEquals("other", task.Assignee);
		assertEquals(DelegationState.PENDING, task.DelegationState);

		task.DelegationState = DelegationState.RESOLVED;
		taskService.saveTask(task);

		task = taskService.createTaskQuery().taskId(taskId).singleResult();
		assertEquals("wuzh", task.Owner);
		assertEquals("other", task.Assignee);
		assertEquals(DelegationState.RESOLVED, task.DelegationState);

		taskService.deleteTask(taskId, true);
	  }

	  private void checkHistoricVariableUpdateEntity(string variableName, string processInstanceId)
	  {
		if (processEngineConfiguration.HistoryLevel.Id == ProcessEngineConfigurationImpl.HISTORYLEVEL_FULL)
		{
		  bool deletedVariableUpdateFound = false;

		  IList<HistoricDetail> resultSet = historyService.createHistoricDetailQuery().processInstanceId(processInstanceId).list();
		  foreach (HistoricDetail currentHistoricDetail in resultSet)
		  {
			assertTrue(currentHistoricDetail is HistoricDetailVariableInstanceUpdateEntity);
			HistoricDetailVariableInstanceUpdateEntity historicVariableUpdate = (HistoricDetailVariableInstanceUpdateEntity) currentHistoricDetail;

			if (historicVariableUpdate.Name.Equals(variableName))
			{
			  if (historicVariableUpdate.Value == null)
			  {
				if (deletedVariableUpdateFound)
				{
				  fail("Mismatch: A HistoricVariableUpdateEntity with a null value already found");
				}
				else
				{
				  deletedVariableUpdateFound = true;
				}
			  }
			}
		  }

		  assertTrue(deletedVariableUpdateFound);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testRemoveVariable()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testRemoveVariable()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task currentTask = taskService.createTaskQuery().singleResult();

		taskService.setVariable(currentTask.Id, "variable1", "value1");
		assertEquals("value1", taskService.getVariable(currentTask.Id, "variable1"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable1"));

		taskService.removeVariable(currentTask.Id, "variable1");

		assertNull(taskService.getVariable(currentTask.Id, "variable1"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveVariableNullTaskId()
	  public virtual void testRemoveVariableNullTaskId()
	  {
		try
		{
		  taskService.removeVariable(null, "variable");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testRemoveVariables()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testRemoveVariables()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task currentTask = taskService.createTaskQuery().singleResult();

		IDictionary<string, object> varsToDelete = new Dictionary<string, object>();
		varsToDelete["variable1"] = "value1";
		varsToDelete["variable2"] = "value2";
		taskService.setVariables(currentTask.Id, varsToDelete);
		taskService.setVariable(currentTask.Id, "variable3", "value3");

		assertEquals("value1", taskService.getVariable(currentTask.Id, "variable1"));
		assertEquals("value2", taskService.getVariable(currentTask.Id, "variable2"));
		assertEquals("value3", taskService.getVariable(currentTask.Id, "variable3"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable1"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable2"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable3"));

		taskService.removeVariables(currentTask.Id, varsToDelete.Keys);

		assertNull(taskService.getVariable(currentTask.Id, "variable1"));
		assertNull(taskService.getVariable(currentTask.Id, "variable2"));
		assertEquals("value3", taskService.getVariable(currentTask.Id, "variable3"));

		assertNull(taskService.getVariableLocal(currentTask.Id, "variable1"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable2"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable3"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
		checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testRemoveVariablesNullTaskId()
	  public virtual void testRemoveVariablesNullTaskId()
	  {
		try
		{
		  taskService.removeVariables(null, Collections.EMPTY_LIST);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testRemoveVariableLocal()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testRemoveVariableLocal()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task currentTask = taskService.createTaskQuery().singleResult();

		taskService.setVariableLocal(currentTask.Id, "variable1", "value1");
		assertEquals("value1", taskService.getVariable(currentTask.Id, "variable1"));
		assertEquals("value1", taskService.getVariableLocal(currentTask.Id, "variable1"));

		taskService.removeVariableLocal(currentTask.Id, "variable1");

		assertNull(taskService.getVariable(currentTask.Id, "variable1"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable1"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveVariableLocalNullTaskId()
	  public virtual void testRemoveVariableLocalNullTaskId()
	  {
		try
		{
		  taskService.removeVariableLocal(null, "variable");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testRemoveVariablesLocal()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testRemoveVariablesLocal()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task currentTask = taskService.createTaskQuery().singleResult();

		IDictionary<string, object> varsToDelete = new Dictionary<string, object>();
		varsToDelete["variable1"] = "value1";
		varsToDelete["variable2"] = "value2";
		taskService.setVariablesLocal(currentTask.Id, varsToDelete);
		taskService.setVariableLocal(currentTask.Id, "variable3", "value3");

		assertEquals("value1", taskService.getVariable(currentTask.Id, "variable1"));
		assertEquals("value2", taskService.getVariable(currentTask.Id, "variable2"));
		assertEquals("value3", taskService.getVariable(currentTask.Id, "variable3"));
		assertEquals("value1", taskService.getVariableLocal(currentTask.Id, "variable1"));
		assertEquals("value2", taskService.getVariableLocal(currentTask.Id, "variable2"));
		assertEquals("value3", taskService.getVariableLocal(currentTask.Id, "variable3"));

		taskService.removeVariables(currentTask.Id, varsToDelete.Keys);

		assertNull(taskService.getVariable(currentTask.Id, "variable1"));
		assertNull(taskService.getVariable(currentTask.Id, "variable2"));
		assertEquals("value3", taskService.getVariable(currentTask.Id, "variable3"));

		assertNull(taskService.getVariableLocal(currentTask.Id, "variable1"));
		assertNull(taskService.getVariableLocal(currentTask.Id, "variable2"));
		assertEquals("value3", taskService.getVariableLocal(currentTask.Id, "variable3"));

		checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
		checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Test public void testRemoveVariablesLocalNullTaskId()
	  public virtual void testRemoveVariablesLocalNullTaskId()
	  {
		try
		{
		  taskService.removeVariablesLocal(null, Collections.EMPTY_LIST);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  testRule.assertTextPresent("taskId is null", ae.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testUserTaskOptimisticLocking()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testUserTaskOptimisticLocking()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task1 = taskService.createTaskQuery().singleResult();
		Task task2 = taskService.createTaskQuery().singleResult();

		task1.Description = "test description one";
		taskService.saveTask(task1);

		try
		{
		  task2.Description = "test description two";
		  taskService.saveTask(task2);

		  fail("Expecting exception");
		}
		catch (OptimisticLockingException)
		{
		  // Expected exception
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskWithDeleteReason()
	  public virtual void testDeleteTaskWithDeleteReason()
	  {
		// ACT-900: deleteReason can be manually specified - can only be validated when historyLevel > ACTIVITY
		if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_ACTIVITY)
		{

		  Task task = taskService.newTask();
		  task.Name = "test task";
		  taskService.saveTask(task);

		  assertNotNull(task.Id);

		  taskService.deleteTask(task.Id, "deleted for testing purposes");

		  HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().taskId(task.Id).singleResult();

		  assertNotNull(historicTaskInstance);
		  assertEquals("deleted for testing purposes", historicTaskInstance.DeleteReason);

		  // Delete historic task that is left behind, will not be cleaned up because this is not part of a process
		  taskService.deleteTask(task.Id, true);

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources = { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" }) @Test public void testDeleteTaskPartOfProcess()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml" })]
	  public virtual void testDeleteTaskPartOfProcess()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		try
		{
		  taskService.deleteTask(task.Id);
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running process", ae.Message);
		}

		try
		{
		  taskService.deleteTask(task.Id, true);
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running process", ae.Message);
		}

		try
		{
		  taskService.deleteTask(task.Id, "test");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running process", ae.Message);
		}

		try
		{
		  taskService.deleteTasks(Arrays.asList(task.Id));
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running process", ae.Message);
		}

		try
		{
		  taskService.deleteTasks(Arrays.asList(task.Id), true);
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running process", ae.Message);
		}

		try
		{
		  taskService.deleteTasks(Arrays.asList(task.Id), "test");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running process", ae.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) @Test public void testDeleteTaskPartOfCaseInstance()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testDeleteTaskPartOfCaseInstance()
	  {
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		try
		{
		  taskService.deleteTask(task.Id);
		  fail("Should not be possible to delete task");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running case instance", ae.Message);
		}

		try
		{
		  taskService.deleteTask(task.Id, true);
		  fail("Should not be possible to delete task");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running case instance", ae.Message);
		}

		try
		{
		  taskService.deleteTask(task.Id, "test");
		  fail("Should not be possible to delete task");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running case instance", ae.Message);
		}

		try
		{
		  taskService.deleteTasks(Arrays.asList(task.Id));
		  fail("Should not be possible to delete task");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running case instance", ae.Message);
		}

		try
		{
		  taskService.deleteTasks(Arrays.asList(task.Id), true);
		  fail("Should not be possible to delete task");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running case instance", ae.Message);
		}

		try
		{
		  taskService.deleteTasks(Arrays.asList(task.Id), "test");
		  fail("Should not be possible to delete task");
		}
		catch (ProcessEngineException ae)
		{
		  assertEquals("The task cannot be deleted because is part of a running case instance", ae.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskCommentByTaskIdAndCommentId()
	  public virtual void testGetTaskCommentByTaskIdAndCommentId()
	  {
		if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  // create and save new task
		  Task task = taskService.newTask();
		  taskService.saveTask(task);

		  string taskId = task.Id;

		  // add comment to task
		  Comment comment = taskService.createComment(taskId, null, "look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd");

		  // select task comment for task id and comment id
		  comment = taskService.getTaskComment(taskId, comment.Id);
		  // check returned comment
		  assertNotNull(comment.Id);
		  assertEquals(taskId, comment.TaskId);
		  assertNull(comment.ProcessInstanceId);
		  assertEquals("look at this isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg ...", ((Event)comment).Message);
		  assertEquals("look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd", comment.FullMessage);
		  assertNotNull(comment.Time);

		  // delete task
		  taskService.deleteTask(task.Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskAttachmentByTaskIdAndAttachmentId() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testTaskAttachmentByTaskIdAndAttachmentId()
	  {
		DateTime fixedDate = SDF.parse("01/01/2001 01:01:01.000");
		ClockUtil.CurrentTime = fixedDate;

		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  // create and save task
		  Task task = taskService.newTask();
		  taskService.saveTask(task);
		  string taskId = task.Id;

		  // Fetch the task again and update
		  // add attachment
		  Attachment attachment = taskService.createAttachment("web page", taskId, "someprocessinstanceid", "weatherforcast", "temperatures and more", "http://weather.com");
		  string attachmentId = attachment.Id;

		  // get attachment for taskId and attachmentId
		  attachment = taskService.getTaskAttachment(taskId, attachmentId);
		  assertEquals("weatherforcast", attachment.Name);
		  assertEquals("temperatures and more", attachment.Description);
		  assertEquals("web page", attachment.Type);
		  assertEquals(taskId, attachment.TaskId);
		  assertEquals("someprocessinstanceid", attachment.ProcessInstanceId);
		  assertEquals("http://weather.com", attachment.Url);
		  assertNull(taskService.getAttachmentContent(attachment.Id));
		  assertThat(attachment.CreateTime).isEqualTo(fixedDate);

		  // delete attachment for taskId and attachmentId
		  taskService.deleteTaskAttachment(taskId, attachmentId);

		  // check if attachment deleted
		  assertNull(taskService.getTaskAttachment(taskId, attachmentId));

		  taskService.deleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentContentByTaskIdAndAttachmentId()
	  public virtual void testGetTaskAttachmentContentByTaskIdAndAttachmentId()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  // create and save task
		  Task task = taskService.newTask();
		  taskService.saveTask(task);
		  string taskId = task.Id;

		  // Fetch the task again and update
		  // add attachment
		  Attachment attachment = taskService.createAttachment("web page", taskId, "someprocessinstanceid", "weatherforcast", "temperatures and more", new MemoryStream("someContent".GetBytes()));
		  string attachmentId = attachment.Id;

		  // get attachment for taskId and attachmentId
		  Stream taskAttachmentContent = taskService.getTaskAttachmentContent(taskId, attachmentId);
		  assertNotNull(taskAttachmentContent);

		  sbyte[] byteContent = IoUtil.readInputStream(taskAttachmentContent, "weatherforcast");
		  assertEquals("someContent", StringHelper.NewString(byteContent));

		  taskService.deleteTask(taskId, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentWithNullParameters()
	  public virtual void testGetTaskAttachmentWithNullParameters()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  Attachment attachment = taskService.getTaskAttachment(null, null);
		  assertNull(attachment);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentContentWithNullParameters()
	  public virtual void testGetTaskAttachmentContentWithNullParameters()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  Stream content = taskService.getTaskAttachmentContent(null, null);
		  assertNull(content);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) @Test public void testCreateTaskAttachmentWithNullTaskAndProcessInstance()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testCreateTaskAttachmentWithNullTaskAndProcessInstance()
	  {
		try
		{
		  taskService.createAttachment("web page", null, null, "weatherforcast", "temperatures and more", new MemoryStream("someContent".GetBytes()));
		  fail("expected process engine exception");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) @Test public void testCreateTaskAttachmentWithNullTaskId() throws java.text.ParseException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}), RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testCreateTaskAttachmentWithNullTaskId()
	  {
		DateTime fixedDate = SDF.parse("01/01/2001 01:01:01.000");
		ClockUtil.CurrentTime = fixedDate;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Attachment attachment = taskService.createAttachment("web page", null, processInstance.Id, "weatherforcast", "temperatures and more", new MemoryStream("someContent".GetBytes()));
		Attachment fetched = taskService.getAttachment(attachment.Id);
		assertThat(fetched).NotNull;
		assertThat(fetched.TaskId).Null;
		assertThat(fetched.ProcessInstanceId).NotNull;
		assertThat(fetched.CreateTime).isEqualTo(fixedDate);
		taskService.deleteAttachment(attachment.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskAttachmentWithNullParameters()
	  public virtual void testDeleteTaskAttachmentWithNullParameters()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  try
		  {
			taskService.deleteTaskAttachment(null, null);
			fail("expected process engine exception");
		  }
		  catch (ProcessEngineException)
		  {
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteTaskAttachmentWithTaskIdNull()
	  public virtual void testDeleteTaskAttachmentWithTaskIdNull()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  try
		  {
			taskService.deleteTaskAttachment(null, "myAttachmentId");
			fail("expected process engine exception");
		  }
		  catch (ProcessEngineException)
		  {
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskAttachmentsWithTaskIdNull()
	  public virtual void testGetTaskAttachmentsWithTaskIdNull()
	  {
		int historyLevel = processEngineConfiguration.HistoryLevel.Id;
		if (historyLevel > ProcessEngineConfigurationImpl.HISTORYLEVEL_NONE)
		{
		  assertEquals(System.Linq.Enumerable.Empty<Attachment>(), taskService.getTaskAttachments(null));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"}) @Test public void testUpdateVariablesLocal()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testUpdateVariablesLocal()
	  {
		IDictionary<string, object> globalVars = new Dictionary<string, object>();
		globalVars["variable4"] = "value4";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startSimpleSubProcess", globalVars);

		Task currentTask = taskService.createTaskQuery().singleResult();
		IDictionary<string, object> localVars = new Dictionary<string, object>();
		localVars["variable1"] = "value1";
		localVars["variable2"] = "value2";
		localVars["variable3"] = "value3";
		taskService.setVariablesLocal(currentTask.Id, localVars);

		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "anotherValue1";
		modifications["variable2"] = "anotherValue2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable2");
		deletions.Add("variable3");
		deletions.Add("variable4");

		((TaskServiceImpl) taskService).updateVariablesLocal(currentTask.Id, modifications, deletions);

		assertEquals("anotherValue1", taskService.getVariable(currentTask.Id, "variable1"));
		assertNull(taskService.getVariable(currentTask.Id, "variable2"));
		assertNull(taskService.getVariable(currentTask.Id, "variable3"));
		assertEquals("value4", runtimeService.getVariable(processInstance.Id, "variable4"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateVariablesLocalForNonExistingTaskId()
	  public virtual void testUpdateVariablesLocalForNonExistingTaskId()
	  {
		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "anotherValue1";
		modifications["variable2"] = "anotherValue2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable2");
		deletions.Add("variable3");
		deletions.Add("variable4");

		try
		{
		  ((TaskServiceImpl) taskService).updateVariablesLocal("nonExistingId", modifications, deletions);
		  fail("expected process engine exception");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateVariablesLocaForNullTaskId()
	  public virtual void testUpdateVariablesLocaForNullTaskId()
	  {
		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "anotherValue1";
		modifications["variable2"] = "anotherValue2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable2");
		deletions.Add("variable3");
		deletions.Add("variable4");

		try
		{
		  ((TaskServiceImpl) taskService).updateVariablesLocal(null, modifications, deletions);
		  fail("expected process engine exception");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"}) @Test public void testUpdateVariables()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneSubProcess.bpmn20.xml"})]
	  public virtual void testUpdateVariables()
	  {
		IDictionary<string, object> globalVars = new Dictionary<string, object>();
		globalVars["variable4"] = "value4";
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("startSimpleSubProcess", globalVars);

		Task currentTask = taskService.createTaskQuery().singleResult();
		IDictionary<string, object> localVars = new Dictionary<string, object>();
		localVars["variable1"] = "value1";
		localVars["variable2"] = "value2";
		localVars["variable3"] = "value3";
		taskService.setVariablesLocal(currentTask.Id, localVars);

		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "anotherValue1";
		modifications["variable2"] = "anotherValue2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable2");
		deletions.Add("variable3");
		deletions.Add("variable4");

		((TaskServiceImpl) taskService).updateVariables(currentTask.Id, modifications, deletions);

		assertEquals("anotherValue1", taskService.getVariable(currentTask.Id, "variable1"));
		assertNull(taskService.getVariable(currentTask.Id, "variable2"));
		assertNull(taskService.getVariable(currentTask.Id, "variable3"));
		assertNull(runtimeService.getVariable(processInstance.Id, "variable4"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateVariablesForNonExistingTaskId()
	  public virtual void testUpdateVariablesForNonExistingTaskId()
	  {
		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "anotherValue1";
		modifications["variable2"] = "anotherValue2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable2");
		deletions.Add("variable3");
		deletions.Add("variable4");

		try
		{
		  ((TaskServiceImpl) taskService).updateVariables("nonExistingId", modifications, deletions);
		  fail("expected process engine exception");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateVariablesForNullTaskId()
	  public virtual void testUpdateVariablesForNullTaskId()
	  {
		IDictionary<string, object> modifications = new Dictionary<string, object>();
		modifications["variable1"] = "anotherValue1";
		modifications["variable2"] = "anotherValue2";

		IList<string> deletions = new List<string>();
		deletions.Add("variable2");
		deletions.Add("variable3");
		deletions.Add("variable4");

		try
		{
		  ((TaskServiceImpl) taskService).updateVariables(null, modifications, deletions);
		  fail("expected process engine exception");
		}
		catch (ProcessEngineException)
		{
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskCaseInstanceId()
	  public virtual void testTaskCaseInstanceId()
	  {
		Task task = taskService.newTask();
		task.CaseInstanceId = "aCaseInstanceId";
		taskService.saveTask(task);

		// Fetch the task again and update
		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("aCaseInstanceId", task.CaseInstanceId);

		task.CaseInstanceId = "anotherCaseInstanceId";
		taskService.saveTask(task);

		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertEquals("anotherCaseInstanceId", task.CaseInstanceId);

		// Finally, delete task
		taskService.deleteTask(task.Id, true);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesTyped()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesTyped()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		runtimeService.startProcessInstanceByKey("oneTaskProcess", vars);
		string taskId = taskService.createTaskQuery().singleResult().Id;
		VariableMap variablesTyped = taskService.getVariablesTyped(taskId);
		assertEquals(vars, variablesTyped);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesTypedDeserialize()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesTypedDeserialize()
	  {

		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("broken", Variables.serializedObjectValue("broken").serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName("unexisting").create()));
		string taskId = taskService.createTaskQuery().singleResult().Id;

		// this works
		VariableMap variablesTyped = taskService.getVariablesTyped(taskId, false);
		assertNotNull(variablesTyped.getValueTyped("broken"));
		variablesTyped = taskService.getVariablesTyped(taskId, Arrays.asList("broken"), false);
		assertNotNull(variablesTyped.getValueTyped("broken"));

		// this does not
		try
		{
		  taskService.getVariablesTyped(taskId);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}

		// this does not
		try
		{
		  taskService.getVariablesTyped(taskId, Arrays.asList("broken"), true);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesLocalTyped()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalTyped()
	  {
		IDictionary<string, object> vars = new Dictionary<string, object>();
		vars["variable1"] = "value1";
		vars["variable2"] = "value2";

		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setVariablesLocal(taskId, vars);

		VariableMap variablesTyped = taskService.getVariablesLocalTyped(taskId);
		assertEquals(vars, variablesTyped);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) @Test public void testGetVariablesLocalTypedDeserialize()
	  [Deployment(resources:{ "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalTypedDeserialize()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string taskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setVariablesLocal(taskId, Variables.createVariables().putValue("broken", Variables.serializedObjectValue("broken").serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName("unexisting").create()));

		// this works
		VariableMap variablesTyped = taskService.getVariablesLocalTyped(taskId, false);
		assertNotNull(variablesTyped.getValueTyped("broken"));
		variablesTyped = taskService.getVariablesLocalTyped(taskId, Arrays.asList("broken"), false);
		assertNotNull(variablesTyped.getValueTyped("broken"));

		// this does not
		try
		{
		  taskService.getVariablesLocalTyped(taskId);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}

		// this does not
		try
		{
		  taskService.getVariablesLocalTyped(taskId, Arrays.asList("broken"), true);
		}
		catch (ProcessEngineException e)
		{
		  testRule.assertTextPresent("Cannot deserialize object", e.Message);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) @Test public void testHumanTaskCompleteWithVariables()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testHumanTaskCompleteWithVariables()
	  {
		// given
		caseService.createCaseInstanceByKey("oneTaskCase");

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string taskId = taskService.createTaskQuery().singleResult().Id;

		string variableName = "aVariable";
		string variableValue = "aValue";

		// when
		taskService.complete(taskId, Variables.createVariables().putValue(variableName, variableValue));

		// then
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertEquals(variable.Name, variableName);
		assertEquals(variable.Value, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) @Test public void testHumanTaskWithLocalVariablesCompleteWithVariable()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testHumanTaskWithLocalVariablesCompleteWithVariable()
	  {
		// given
		caseService.createCaseInstanceByKey("oneTaskCase");

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string variableName = "aVariable";
		string variableValue = "aValue";
		string variableAnotherValue = "anotherValue";

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.setVariableLocal(taskId, variableName, variableValue);

		// when
		taskService.complete(taskId, Variables.createVariables().putValue(variableName, variableAnotherValue));

		// then
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertEquals(variable.Name, variableName);
		assertEquals(variable.Value, variableAnotherValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"}) @Test public void testUserTaskWithLocalVariablesCompleteWithVariable()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/twoTasksProcess.bpmn20.xml"})]
	  public virtual void testUserTaskWithLocalVariablesCompleteWithVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("twoTasksProcess");

		string variableName = "aVariable";
		string variableValue = "aValue";
		string variableAnotherValue = "anotherValue";

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.setVariableLocal(taskId, variableName, variableValue);

		// when
		taskService.complete(taskId, Variables.createVariables().putValue(variableName, variableAnotherValue));

		// then
		VariableInstance variable = runtimeService.createVariableInstanceQuery().singleResult();

		assertEquals(variable.Name, variableName);
		assertEquals(variable.Value, variableAnotherValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment(resources={"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"}) @Test public void testHumanTaskLocalVariables()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testHumanTaskLocalVariables()
	  {
		// given
		string caseInstanceId = caseService.createCaseInstanceByKey("oneTaskCase").Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		string variableName = "aVariable";
		string variableValue = "aValue";

		string taskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariableLocal(taskId, variableName, variableValue);

		// then
		VariableInstance variableInstance = runtimeService.createVariableInstanceQuery().taskIdIn(taskId).singleResult();
		assertNotNull(variableInstance);

		assertEquals(caseInstanceId, variableInstance.CaseInstanceId);
		assertEquals(humanTaskId, variableInstance.CaseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;
		string taskId = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult().Id;

		// when
		IDictionary<string, object> variables = taskService.getVariables(taskId, new List<string>());

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesTypedByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesTypedByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;
		string taskId = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult().Id;

		// when
		IDictionary<string, object> variables = taskService.getVariablesTyped(taskId, new List<string>(), false);

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesLocalByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;
		string taskId = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult().Id;

		// when
		IDictionary<string, object> variables = taskService.getVariablesLocal(taskId, new List<string>());

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources={"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"}) public void testGetVariablesLocalTypedByEmptyList()
	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testGetVariablesLocalTypedByEmptyList()
	  {
		// given
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;
		string taskId = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult().Id;

		// when
		IDictionary<string, object> variables = taskService.getVariablesLocalTyped(taskId, new List<string>(), false);

		// then
		assertNotNull(variables);
		assertTrue(variables.Count == 0);
	  }

	}

}