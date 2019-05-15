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
namespace org.camunda.bpm.engine.test.standalone.task
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using IdentityLink = org.camunda.bpm.engine.task.IdentityLink;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskDecoratorTest : PluggableProcessEngineTestCase
	{

	  protected internal TaskEntity task;
	  protected internal TaskDefinition taskDefinition;
	  protected internal TaskDecorator taskDecorator;
	  protected internal ExpressionManager expressionManager;

	  public virtual void setUp()
	  {
		task = (TaskEntity) taskService.newTask();
		taskService.saveTask(task);

		expressionManager = processEngineConfiguration.ExpressionManager;

		taskDefinition = new TaskDefinition(null);
		taskDecorator = new TaskDecorator(taskDefinition, expressionManager);
	  }

	  public virtual void tearDown()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new DeleteTaskCommand(this, task));
	  }

	  protected internal virtual void decorate(TaskEntity task, TaskDecorator decorator)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new DecorateTaskCommand(this, task, decorator));
	  }

	  public virtual void testDecorateName()
	  {
		// given
		string aTaskName = "A Task Name";
		Expression nameExpression = expressionManager.createExpression(aTaskName);
		taskDefinition.NameExpression = nameExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(aTaskName, task.Name);
	  }

	  public virtual void testDecorateNameFromVariable()
	  {
		// given
		string aTaskName = "A Task Name";
		taskService.setVariable(task.Id, "taskName", aTaskName);

		Expression nameExpression = expressionManager.createExpression("${taskName}");
		taskDefinition.NameExpression = nameExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(aTaskName, task.Name);
	  }

	  public virtual void testDecorateDescription()
	  {
		// given
		string aDescription = "This is a Task";
		Expression descriptionExpression = expressionManager.createExpression(aDescription);
		taskDefinition.DescriptionExpression = descriptionExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(aDescription, task.Description);
	  }

	  public virtual void testDecorateDescriptionFromVariable()
	  {
		// given
		string aDescription = "This is a Task";
		taskService.setVariable(task.Id, "description", aDescription);

		Expression descriptionExpression = expressionManager.createExpression("${description}");
		taskDefinition.DescriptionExpression = descriptionExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(aDescription, task.Description);
	  }

	  public virtual void testDecorateDueDate()
	  {
		// given
		string aDueDate = "2014-06-01";
		DateTime dueDate = DateTimeUtil.parseDate(aDueDate);

		Expression dueDateExpression = expressionManager.createExpression(aDueDate);
		taskDefinition.DueDateExpression = dueDateExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(dueDate, task.DueDate);
	  }

	  public virtual void testDecorateDueDateFromVariable()
	  {
		// given
		string aDueDate = "2014-06-01";
		DateTime dueDate = DateTimeUtil.parseDate(aDueDate);
		taskService.setVariable(task.Id, "dueDate", dueDate);

		Expression dueDateExpression = expressionManager.createExpression("${dueDate}");
		taskDefinition.DueDateExpression = dueDateExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(dueDate, task.DueDate);
	  }

	  public virtual void testDecorateFollowUpDate()
	  {
		// given
		string aFollowUpDate = "2014-06-01";
		DateTime followUpDate = DateTimeUtil.parseDate(aFollowUpDate);

		Expression followUpDateExpression = expressionManager.createExpression(aFollowUpDate);
		taskDefinition.FollowUpDateExpression = followUpDateExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(followUpDate, task.FollowUpDate);
	  }

	  public virtual void testDecorateFollowUpDateFromVariable()
	  {
		// given
		string aFollowUpDateDate = "2014-06-01";
		DateTime followUpDate = DateTimeUtil.parseDate(aFollowUpDateDate);
		taskService.setVariable(task.Id, "followUpDate", followUpDate);

		Expression followUpDateExpression = expressionManager.createExpression("${followUpDate}");
		taskDefinition.FollowUpDateExpression = followUpDateExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(followUpDate, task.FollowUpDate);
	  }

	  public virtual void testDecoratePriority()
	  {
		// given
		string aPriority = "10";
		Expression priorityExpression = expressionManager.createExpression(aPriority);
		taskDefinition.PriorityExpression = priorityExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(int.Parse(aPriority), task.Priority);
	  }

	  public virtual void testDecoratePriorityFromVariable()
	  {
		// given
		int aPriority = 10;
		taskService.setVariable(task.Id, "priority", aPriority);

		Expression priorityExpression = expressionManager.createExpression("${priority}");
		taskDefinition.PriorityExpression = priorityExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(aPriority, task.Priority);
	  }

	  public virtual void testDecorateAssignee()
	  {
		// given
		string aAssignee = "john";
		Expression assigneeExpression = expressionManager.createExpression(aAssignee);
		taskDefinition.AssigneeExpression = assigneeExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(aAssignee, task.Assignee);
	  }

	  public virtual void testDecorateAssigneeFromVariable()
	  {
		// given
		string aAssignee = "john";
		taskService.setVariable(task.Id, "assignee", aAssignee);

		Expression assigneeExpression = expressionManager.createExpression("${assignee}");
		taskDefinition.AssigneeExpression = assigneeExpression;

		// when
		decorate(task, taskDecorator);

		// then
		assertEquals(aAssignee, task.Assignee);
	  }

	  public virtual void testDecorateCandidateUsers()
	  {
		// given
		IList<string> aCandidateUserList = new List<string>();
		aCandidateUserList.Add("john");
		aCandidateUserList.Add("peter");
		aCandidateUserList.Add("mary");

		foreach (string candidateUser in aCandidateUserList)
		{
		  Expression candidateUserExpression = expressionManager.createExpression(candidateUser);
		  taskDefinition.addCandidateUserIdExpression(candidateUserExpression);
		}

		// when
		decorate(task, taskDecorator);

		// then
		ISet<IdentityLink> candidates = task.Candidates;
		assertEquals(3, candidates.Count);

		foreach (IdentityLink identityLink in candidates)
		{
		  string taskId = identityLink.TaskId;
		  assertEquals(task.Id, taskId);

		  assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		  string userId = identityLink.UserId;
		  if ("john".Equals(userId))
		  {
			assertEquals("john", userId);
		  }
		  else if ("peter".Equals(userId))
		  {
			assertEquals("peter", userId);
		  }
		  else if ("mary".Equals(userId))
		  {
			assertEquals("mary", userId);
		  }
		  else
		  {
			fail("Unexpected user: " + userId);
		  }
		}

	  }

	  public virtual void testDecorateCandidateUsersFromVariable()
	  {
		// given
		taskService.setVariable(task.Id, "john", "john");
		taskService.setVariable(task.Id, "peter", "peter");
		taskService.setVariable(task.Id, "mary", "mary");

		IList<string> aCandidateUserList = new List<string>();
		aCandidateUserList.Add("${john}");
		aCandidateUserList.Add("${peter}");
		aCandidateUserList.Add("${mary}");

		foreach (string candidateUser in aCandidateUserList)
		{
		  Expression candidateUserExpression = expressionManager.createExpression(candidateUser);
		  taskDefinition.addCandidateUserIdExpression(candidateUserExpression);
		}

		// when
		decorate(task, taskDecorator);

		// then
		ISet<IdentityLink> candidates = task.Candidates;
		assertEquals(3, candidates.Count);

		foreach (IdentityLink identityLink in candidates)
		{
		  string taskId = identityLink.TaskId;
		  assertEquals(task.Id, taskId);

		  assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		  string userId = identityLink.UserId;
		  if ("john".Equals(userId))
		  {
			assertEquals("john", userId);
		  }
		  else if ("peter".Equals(userId))
		  {
			assertEquals("peter", userId);
		  }
		  else if ("mary".Equals(userId))
		  {
			assertEquals("mary", userId);
		  }
		  else
		  {
			fail("Unexpected user: " + userId);
		  }
		}
	  }

	  public virtual void testDecorateCandidateGroups()
	  {
		// given
		IList<string> aCandidateGroupList = new List<string>();
		aCandidateGroupList.Add("management");
		aCandidateGroupList.Add("accounting");
		aCandidateGroupList.Add("backoffice");

		foreach (string candidateGroup in aCandidateGroupList)
		{
		  Expression candidateGroupExpression = expressionManager.createExpression(candidateGroup);
		  taskDefinition.addCandidateGroupIdExpression(candidateGroupExpression);
		}

		// when
		decorate(task, taskDecorator);

		// then
		ISet<IdentityLink> candidates = task.Candidates;
		assertEquals(3, candidates.Count);

		foreach (IdentityLink identityLink in candidates)
		{
		  string taskId = identityLink.TaskId;
		  assertEquals(task.Id, taskId);

		  assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		  string groupId = identityLink.GroupId;
		  if ("management".Equals(groupId))
		  {
			assertEquals("management", groupId);
		  }
		  else if ("accounting".Equals(groupId))
		  {
			assertEquals("accounting", groupId);
		  }
		  else if ("backoffice".Equals(groupId))
		  {
			assertEquals("backoffice", groupId);
		  }
		  else
		  {
			fail("Unexpected group: " + groupId);
		  }
		}

	  }

	  public virtual void testDecorateCandidateGroupsFromVariable()
	  {
		// given
		taskService.setVariable(task.Id, "management", "management");
		taskService.setVariable(task.Id, "accounting", "accounting");
		taskService.setVariable(task.Id, "backoffice", "backoffice");

		IList<string> aCandidateGroupList = new List<string>();
		aCandidateGroupList.Add("${management}");
		aCandidateGroupList.Add("${accounting}");
		aCandidateGroupList.Add("${backoffice}");

		foreach (string candidateGroup in aCandidateGroupList)
		{
		  Expression candidateGroupExpression = expressionManager.createExpression(candidateGroup);
		  taskDefinition.addCandidateGroupIdExpression(candidateGroupExpression);
		}

		// when
		decorate(task, taskDecorator);

		// then
		ISet<IdentityLink> candidates = task.Candidates;
		assertEquals(3, candidates.Count);

		foreach (IdentityLink identityLink in candidates)
		{
		  string taskId = identityLink.TaskId;
		  assertEquals(task.Id, taskId);

		  assertEquals(IdentityLinkType.CANDIDATE, identityLink.Type);

		  string groupId = identityLink.GroupId;
		  if ("management".Equals(groupId))
		  {
			assertEquals("management", groupId);
		  }
		  else if ("accounting".Equals(groupId))
		  {
			assertEquals("accounting", groupId);
		  }
		  else if ("backoffice".Equals(groupId))
		  {
			assertEquals("backoffice", groupId);
		  }
		  else
		  {
			fail("Unexpected group: " + groupId);
		  }
		}
	  }

	  protected internal class DecorateTaskCommand : Command<Void>
	  {
		  private readonly TaskDecoratorTest outerInstance;


		protected internal TaskEntity task;
		protected internal TaskDecorator decorator;

		public DecorateTaskCommand(TaskDecoratorTest outerInstance, TaskEntity task, TaskDecorator decorator)
		{
			this.outerInstance = outerInstance;
		 this.task = task;
		 this.decorator = decorator;
		}

		public virtual Void execute(CommandContext commandContext)
		{
		  decorator.decorate(task, task);
		  return null;
		}

	  }

	  protected internal class DeleteTaskCommand : Command<Void>
	  {
		  private readonly TaskDecoratorTest outerInstance;


		protected internal TaskEntity task;

		public DeleteTaskCommand(TaskDecoratorTest outerInstance, TaskEntity task)
		{
			this.outerInstance = outerInstance;
		 this.task = task;
		}

		public virtual Void execute(CommandContext commandContext)
		{
		  commandContext.TaskManager.deleteTask(task, null, true, false);

		  return null;
		}

	  }

	}

}