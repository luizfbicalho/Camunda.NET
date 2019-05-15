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

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Event = org.camunda.bpm.engine.task.Event;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;
	using Task = org.camunda.bpm.engine.task.Task;

	using AssertionFailedError = junit.framework.AssertionFailedError;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// </summary>
	public class TaskIdentityLinksTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/IdentityLinksProcess.bpmn20.xml")]
	  public virtual void testCandidateUserLink()
	  {
		runtimeService.startProcessInstanceByKey("IdentityLinksProcess");

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.addCandidateUser(taskId, "kermit");

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		IdentityLink identityLink = identityLinks[0];

		assertNull(identityLink.GroupId);
		assertEquals("kermit", identityLink.UserId);
		assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
		assertEquals(taskId, identityLink.TaskId);

		assertEquals(1, identityLinks.Count);

		taskService.deleteCandidateUser(taskId, "kermit");

		assertEquals(0, taskService.getIdentityLinksForTask(taskId).Count);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/IdentityLinksProcess.bpmn20.xml")]
	  public virtual void testCandidateGroupLink()
	  {
		try
		{
		  identityService.AuthenticatedUserId = "demo";

		  runtimeService.startProcessInstanceByKey("IdentityLinksProcess");

		  string taskId = taskService.createTaskQuery().singleResult().Id;

		  taskService.addCandidateGroup(taskId, "muppets");

		  IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		  IdentityLink identityLink = identityLinks[0];

		  assertEquals("muppets", identityLink.GroupId);
		  assertNull("kermit", identityLink.UserId);
		  assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);
		  assertEquals(taskId, identityLink.TaskId);

		  assertEquals(1, identityLinks.Count);

		  if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_FULL)
		  {
			IList<Event> taskEvents = taskService.getTaskEvents(taskId);
			assertEquals(1, taskEvents.Count);
			Event taskEvent = taskEvents[0];
			assertEquals(org.camunda.bpm.engine.task.Event_Fields.ACTION_ADD_GROUP_LINK, taskEvent.Action);
			IList<string> taskEventMessageParts = taskEvent.MessageParts;
			assertEquals("muppets", taskEventMessageParts[0]);
			assertEquals(IdentityLinkType.CANDIDATE, taskEventMessageParts[1]);
			assertEquals(2, taskEventMessageParts.Count);
		  }

		  taskService.deleteCandidateGroup(taskId, "muppets");

		  if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HISTORYLEVEL_FULL)
		  {
			IList<Event> taskEvents = taskService.getTaskEvents(taskId);
			Event taskEvent = findTaskEvent(taskEvents, org.camunda.bpm.engine.task.Event_Fields.ACTION_DELETE_GROUP_LINK);
			assertEquals(org.camunda.bpm.engine.task.Event_Fields.ACTION_DELETE_GROUP_LINK, taskEvent.Action);
			IList<string> taskEventMessageParts = taskEvent.MessageParts;
			assertEquals("muppets", taskEventMessageParts[0]);
			assertEquals(IdentityLinkType.CANDIDATE, taskEventMessageParts[1]);
			assertEquals(2, taskEventMessageParts.Count);
			assertEquals(2, taskEvents.Count);
		  }

		  assertEquals(0, taskService.getIdentityLinksForTask(taskId).Count);
		}
		finally
		{
		  identityService.clearAuthentication();
		}
	  }

	  public virtual void testAssigneeLink()
	  {
		Task task = taskService.newTask("task");
		task.Assignee = "assignee";
		taskService.saveTask(task);

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(task.Id);
		assertNotNull(identityLinks);
		assertEquals(1, identityLinks.Count);

		IdentityLink identityLink = identityLinks[0];
		assertEquals(IdentityLinkType.ASSIGNEE, identityLink.Type);
		assertEquals("assignee", identityLink.UserId);
		assertEquals("task", identityLink.TaskId);

		taskService.deleteTask(task.Id, true);
	  }

	  public virtual void testOwnerLink()
	  {
		Task task = taskService.newTask("task");
		task.Owner = "owner";
		taskService.saveTask(task);

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(task.Id);
		assertNotNull(identityLinks);
		assertEquals(1, identityLinks.Count);

		IdentityLink identityLink = identityLinks[0];
		assertEquals(IdentityLinkType.OWNER, identityLink.Type);
		assertEquals("owner", identityLink.UserId);
		assertEquals("task", identityLink.TaskId);

		taskService.deleteTask(task.Id, true);
	  }

	  private Event findTaskEvent(IList<Event> taskEvents, string action)
	  {
		foreach (Event @event in taskEvents)
		{
		  if (action.Equals(@event.Action))
		  {
			return @event;
		  }
		}
		throw new AssertionFailedError("no task event found with action " + action);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/IdentityLinksProcess.bpmn20.xml")]
	  public virtual void testCustomTypeUserLink()
	  {
		runtimeService.startProcessInstanceByKey("IdentityLinksProcess");

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.addUserIdentityLink(taskId, "kermit", "interestee");

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		IdentityLink identityLink = identityLinks[0];

		assertNull(identityLink.GroupId);
		assertEquals("kermit", identityLink.UserId);
		assertEquals("interestee", identityLink.Type);
		assertEquals(taskId, identityLink.TaskId);

		assertEquals(1, identityLinks.Count);

		taskService.deleteUserIdentityLink(taskId, "kermit", "interestee");

		assertEquals(0, taskService.getIdentityLinksForTask(taskId).Count);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/api/task/IdentityLinksProcess.bpmn20.xml")]
	  public virtual void testCustomLinkGroupLink()
	  {
		runtimeService.startProcessInstanceByKey("IdentityLinksProcess");

		string taskId = taskService.createTaskQuery().singleResult().Id;

		taskService.addGroupIdentityLink(taskId, "muppets", "playing");

		IList<IdentityLink> identityLinks = taskService.getIdentityLinksForTask(taskId);
		IdentityLink identityLink = identityLinks[0];

		assertEquals("muppets", identityLink.GroupId);
		assertNull("kermit", identityLink.UserId);
		assertEquals("playing", identityLink.Type);
		assertEquals(taskId, identityLink.TaskId);

		assertEquals(1, identityLinks.Count);

		taskService.deleteGroupIdentityLink(taskId, "muppets", "playing");

		assertEquals(0, taskService.getIdentityLinksForTask(taskId).Count);
	  }

	  public virtual void testDeleteAssignee()
	  {
		Task task = taskService.newTask();
		task.Assignee = "nonExistingUser";
		taskService.saveTask(task);

		taskService.deleteUserIdentityLink(task.Id, "nonExistingUser", IdentityLinkType.ASSIGNEE);

		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertNull(task.Assignee);
		assertEquals(0, taskService.getIdentityLinksForTask(task.Id).Count);

		// cleanup
		taskService.deleteTask(task.Id, true);
	  }

	  public virtual void testDeleteOwner()
	  {
		Task task = taskService.newTask();
		task.Owner = "nonExistingUser";
		taskService.saveTask(task);

		taskService.deleteUserIdentityLink(task.Id, "nonExistingUser", IdentityLinkType.OWNER);

		task = taskService.createTaskQuery().taskId(task.Id).singleResult();
		assertNull(task.Owner);
		assertEquals(0, taskService.getIdentityLinksForTask(task.Id).Count);

		// cleanup
		taskService.deleteTask(task.Id, true);
	  }

	}

}