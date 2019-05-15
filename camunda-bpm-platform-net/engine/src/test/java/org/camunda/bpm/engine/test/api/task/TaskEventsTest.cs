using System;
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
namespace org.camunda.bpm.engine.test.api.task
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.task.Event_Fields.ACTION_ADD_ATTACHMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.task.Event_Fields.ACTION_ADD_GROUP_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.task.Event_Fields.ACTION_ADD_USER_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.task.Event_Fields.ACTION_DELETE_ATTACHMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.task.Event_Fields.ACTION_DELETE_GROUP_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.task.Event_Fields.ACTION_DELETE_USER_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.task.IdentityLinkType.CANDIDATE;


	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommentEntity = org.camunda.bpm.engine.impl.persistence.entity.CommentEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Event = org.camunda.bpm.engine.task.Event;
	using Task = org.camunda.bpm.engine.task.Task;
	using AbstractUserOperationLogTest = org.camunda.bpm.engine.test.history.useroperationlog.AbstractUserOperationLogTest;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public class TaskEventsTest extends org.camunda.bpm.engine.test.history.useroperationlog.AbstractUserOperationLogTest
	public class TaskEventsTest : AbstractUserOperationLogTest
	{

	  internal static string JONNY = "jonny";
	  internal static string ACCOUNTING = "accounting";
	  internal static string IMAGE_PNG = "application/png";
	  internal static string IMAGE_NAME = "my-image.png";
	  internal static string IMAGE_DESC = "a super duper image";
	  internal static string IMAGE_URL = "file://some/location/my-image.png";

	  private Task task;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		task = taskService.newTask();
		taskService.saveTask(task);
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		// delete task
		taskService.deleteTask(task.Id, true);
	  }

	  public virtual void testAddUserLinkEvents()
	  {

		// initially there are no task events
		assertTrue(taskService.getTaskEvents(task.Id).Count == 0);

		taskService.addCandidateUser(task.Id, JONNY);

		// now there is a task event created
		IList<Event> events = taskService.getTaskEvents(task.Id);
		assertEquals(1, events.Count);
		Event @event = events[0];
		assertEquals(JONNY, @event.MessageParts[0]);
		assertEquals(CANDIDATE, @event.MessageParts[1]);
		assertEquals(task.Id, @event.TaskId);
		assertEquals(ACTION_ADD_USER_LINK, @event.Action);
		assertEquals(JONNY + CommentEntity.MESSAGE_PARTS_MARKER + CANDIDATE, @event.Message);
		assertEquals(null, @event.ProcessInstanceId);
		assertNotNull(@event.Time.Ticks <= ClockUtil.CurrentTime.Ticks);

		assertNoCommentsForTask();
	  }

	  public virtual void testDeleteUserLinkEvents()
	  {

		// initially there are no task events
		assertTrue(taskService.getTaskEvents(task.Id).Count == 0);

		taskService.addCandidateUser(task.Id, JONNY);

		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + 5000);

		taskService.deleteCandidateUser(task.Id, JONNY);

		// now there is a task event created
		IList<Event> events = taskService.getTaskEvents(task.Id);
		assertEquals(2, events.Count);
		Event @event = events[0];
		assertEquals(JONNY, @event.MessageParts[0]);
		assertEquals(CANDIDATE, @event.MessageParts[1]);
		assertEquals(task.Id, @event.TaskId);
		assertEquals(ACTION_DELETE_USER_LINK, @event.Action);
		assertEquals(JONNY + CommentEntity.MESSAGE_PARTS_MARKER + CANDIDATE, @event.Message);
		assertEquals(null, @event.ProcessInstanceId);
		assertNotNull(@event.Time.Ticks <= ClockUtil.CurrentTime.Ticks);

		assertNoCommentsForTask();
	  }

	  public virtual void testAddGroupLinkEvents()
	  {

		// initially there are no task events
		assertTrue(taskService.getTaskEvents(task.Id).Count == 0);

		taskService.addCandidateGroup(task.Id, ACCOUNTING);

		// now there is a task event created
		IList<Event> events = taskService.getTaskEvents(task.Id);
		assertEquals(1, events.Count);
		Event @event = events[0];
		assertEquals(ACCOUNTING, @event.MessageParts[0]);
		assertEquals(CANDIDATE, @event.MessageParts[1]);
		assertEquals(task.Id, @event.TaskId);
		assertEquals(ACTION_ADD_GROUP_LINK, @event.Action);
		assertEquals(ACCOUNTING + CommentEntity.MESSAGE_PARTS_MARKER + CANDIDATE, @event.Message);
		assertEquals(null, @event.ProcessInstanceId);
		assertNotNull(@event.Time.Ticks <= ClockUtil.CurrentTime.Ticks);

		assertNoCommentsForTask();
	  }

	  public virtual void testDeleteGroupLinkEvents()
	  {

		// initially there are no task events
		assertTrue(taskService.getTaskEvents(task.Id).Count == 0);

		taskService.addCandidateGroup(task.Id, ACCOUNTING);

		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + 5000);

		taskService.deleteCandidateGroup(task.Id, ACCOUNTING);

		// now there is a task event created
		IList<Event> events = taskService.getTaskEvents(task.Id);
		assertEquals(2, events.Count);
		Event @event = events[0];
		assertEquals(ACCOUNTING, @event.MessageParts[0]);
		assertEquals(CANDIDATE, @event.MessageParts[1]);
		assertEquals(task.Id, @event.TaskId);
		assertEquals(ACTION_DELETE_GROUP_LINK, @event.Action);
		assertEquals(ACCOUNTING + CommentEntity.MESSAGE_PARTS_MARKER + CANDIDATE, @event.Message);
		assertEquals(null, @event.ProcessInstanceId);
		assertNotNull(@event.Time.Ticks <= ClockUtil.CurrentTime.Ticks);

		assertNoCommentsForTask();
	  }

	  public virtual void testAddAttachmentEvents()
	  {
		// initially there are no task events
		assertTrue(taskService.getTaskEvents(task.Id).Count == 0);

		identityService.AuthenticatedUserId = JONNY;
		taskService.createAttachment(IMAGE_PNG, task.Id, null, IMAGE_NAME, IMAGE_DESC, IMAGE_URL);

		// now there is a task event created
		IList<Event> events = taskService.getTaskEvents(task.Id);
		assertEquals(1, events.Count);
		Event @event = events[0];
		assertEquals(1, @event.MessageParts.Count);
		assertEquals(IMAGE_NAME, @event.MessageParts[0]);
		assertEquals(task.Id, @event.TaskId);
		assertEquals(ACTION_ADD_ATTACHMENT, @event.Action);
		assertEquals(IMAGE_NAME, @event.Message);
		assertEquals(null, @event.ProcessInstanceId);
		assertNotNull(@event.Time.Ticks <= ClockUtil.CurrentTime.Ticks);

		assertNoCommentsForTask();
	  }

	  public virtual void testDeleteAttachmentEvents()
	  {
		// initially there are no task events
		assertTrue(taskService.getTaskEvents(task.Id).Count == 0);

		identityService.AuthenticatedUserId = JONNY;
		Attachment attachment = taskService.createAttachment(IMAGE_PNG, task.Id, null, IMAGE_NAME, IMAGE_DESC, IMAGE_URL);

		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + 5000);

		taskService.deleteAttachment(attachment.Id);

		// now there is a task event created
		IList<Event> events = taskService.getTaskEvents(task.Id);
		assertEquals(2, events.Count);
		Event @event = events[0];
		assertEquals(1, @event.MessageParts.Count);
		assertEquals(IMAGE_NAME, @event.MessageParts[0]);
		assertEquals(task.Id, @event.TaskId);
		assertEquals(ACTION_DELETE_ATTACHMENT, @event.Action);
		assertEquals(IMAGE_NAME, @event.Message);
		assertEquals(null, @event.ProcessInstanceId);
		assertNotNull(@event.Time.Ticks <= ClockUtil.CurrentTime.Ticks);

		assertNoCommentsForTask();
	  }


	  private void assertNoCommentsForTask()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly TaskEventsTest outerInstance;

		  public CommandAnonymousInnerClass(TaskEventsTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			assertTrue(commandContext.CommentManager.findCommentsByTaskId(outerInstance.task.Id).Count == 0);
			return null;
		  }
	  }

	}

}