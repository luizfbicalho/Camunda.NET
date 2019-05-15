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

	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Query = org.camunda.bpm.engine.query.Query;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Mocks = org.camunda.bpm.engine.test.mock.Mocks;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using EndEventBuilder = org.camunda.bpm.model.bpmn.builder.EndEventBuilder;
	using DateTime = org.joda.time.DateTime;
	using After = org.junit.After;
	using Before = org.junit.Before;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class TaskQueryExpressionTest : ResourceProcessEngineTestCase
	{

	  protected internal Task task;
	  protected internal User user;
	  protected internal User anotherUser;
	  protected internal User userWithoutGroups;
	  protected internal Group group1;

	  public TaskQueryExpressionTest() : base("org/camunda/bpm/engine/test/api/task/task-query-expression-test.camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		group1 = createGroup("group1");
		Group group2 = createGroup("group2");
		Group group3 = createGroup("group3");

		user = createUser("user", group1.Id, group2.Id);
		anotherUser = createUser("anotherUser", group3.Id);
		userWithoutGroups = createUser("userWithoutGroups");

		Time = 1427547759000l;
		task = createTestTask("task");
		// shift time to force distinguishable create times
		adjustTime(2 * 60);
		Task anotherTask = createTestTask("anotherTask");
		Task assignedCandidateTask = createTestTask("assignedCandidateTask");

		taskService.setOwner(task.Id, user.Id);
		taskService.setAssignee(task.Id, user.Id);

		taskService.addCandidateUser(anotherTask.Id, user.Id);
		taskService.addCandidateGroup(anotherTask.Id, group1.Id);

		taskService.setAssignee(assignedCandidateTask.Id, user.Id);
		taskService.addCandidateUser(assignedCandidateTask.Id, user.Id);
		taskService.addCandidateGroup(assignedCandidateTask.Id, group1.Id);
	  }

	  public virtual void testQueryByAssigneeExpression()
	  {
		assertCount(taskQuery().taskAssigneeExpression("${'" + user.Id + "'}"), 2);
		assertCount(taskQuery().taskAssigneeExpression("${'" + anotherUser.Id + "'}"), 0);

		CurrentUser = user;
		assertCount(taskQuery().taskAssigneeExpression("${currentUser()}"), 2);

		CurrentUser = anotherUser;
		assertCount(taskQuery().taskAssigneeExpression("${currentUser()}"), 0);
	  }

	  public virtual void testQueryByAssigneeLikeExpression()
	  {
		assertCount(taskQuery().taskAssigneeLikeExpression("${'%" + user.Id.Substring(2) + "'}"), 2);
		assertCount(taskQuery().taskAssigneeLikeExpression("${'%" + anotherUser.Id.Substring(2) + "'}"), 0);

		CurrentUser = user;
		assertCount(taskQuery().taskAssigneeLikeExpression("${'%'.concat(currentUser())}"), 2);

		CurrentUser = anotherUser;
		assertCount(taskQuery().taskAssigneeLikeExpression("${'%'.concat(currentUser())}"), 0);
	  }

	  public virtual void testQueryByOwnerExpression()
	  {
		assertCount(taskQuery().taskOwnerExpression("${'" + user.Id + "'}"), 1);
		assertCount(taskQuery().taskOwnerExpression("${'" + anotherUser.Id + "'}"), 0);

		CurrentUser = user;
		assertCount(taskQuery().taskOwnerExpression("${currentUser()}"), 1);

		CurrentUser = anotherUser;
		assertCount(taskQuery().taskOwnerExpression("${currentUser()}"), 0);
	  }

	  public virtual void testQueryByInvolvedUserExpression()
	  {
		assertCount(taskQuery().taskInvolvedUserExpression("${'" + user.Id + "'}"), 2);
		assertCount(taskQuery().taskInvolvedUserExpression("${'" + anotherUser.Id + "'}"), 0);

		CurrentUser = user;
		assertCount(taskQuery().taskInvolvedUserExpression("${currentUser()}"), 2);

		CurrentUser = anotherUser;
		assertCount(taskQuery().taskInvolvedUserExpression("${currentUser()}"), 0);
	  }

	  public virtual void testQueryByCandidateUserExpression()
	  {
		assertCount(taskQuery().taskCandidateUserExpression("${'" + user.Id + "'}"), 1);
		assertCount(taskQuery().taskCandidateUserExpression("${'" + user.Id + "'}").includeAssignedTasks(), 2);
		assertCount(taskQuery().taskCandidateUserExpression("${'" + anotherUser.Id + "'}"), 0);

		CurrentUser = user;
		assertCount(taskQuery().taskCandidateUserExpression("${currentUser()}"), 1);
		assertCount(taskQuery().taskCandidateUserExpression("${currentUser()}").includeAssignedTasks(), 2);

		CurrentUser = anotherUser;
		assertCount(taskQuery().taskCandidateUserExpression("${currentUser()}"), 0);
	  }

	  public virtual void testQueryByCandidateGroupExpression()
	  {
		assertCount(taskQuery().taskCandidateGroupExpression("${'" + group1.Id + "'}"), 1);
		assertCount(taskQuery().taskCandidateGroupExpression("${'unknown'}"), 0);

		CurrentUser = user;
		assertCount(taskQuery().taskCandidateGroupExpression("${currentUserGroups()[0]}"), 1);
		assertCount(taskQuery().taskCandidateGroupExpression("${currentUserGroups()[0]}").includeAssignedTasks(), 2);

		CurrentUser = anotherUser;
		assertCount(taskQuery().taskCandidateGroupExpression("${currentUserGroups()[0]}"), 0);
	  }

	  public virtual void testQueryByCandidateGroupsExpression()
	  {
		CurrentUser = user;
		assertCount(taskQuery().taskCandidateGroupInExpression("${currentUserGroups()}"), 1);
		assertCount(taskQuery().taskCandidateGroupInExpression("${currentUserGroups()}").includeAssignedTasks(), 2);

		CurrentUser = anotherUser;

		assertCount(taskQuery().taskCandidateGroupInExpression("${currentUserGroups()}"), 0);

		CurrentUser = userWithoutGroups;
		try
		{
		  taskQuery().taskCandidateGroupInExpression("${currentUserGroups()}").count();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected because currentUserGroups will return null
		}
	  }

	  public virtual void testQueryByTaskCreatedBeforeExpression()
	  {
		adjustTime(1);

		assertCount(taskQuery().taskCreatedBeforeExpression("${now()}"), 3);

		adjustTime(-5 * 60);

		assertCount(taskQuery().taskCreatedBeforeExpression("${now()}"), 0);

		Time = task.CreateTime;

		assertCount(taskQuery().taskCreatedBeforeExpression("${dateTime().plusMonths(2)}"), 3);

		assertCount(taskQuery().taskCreatedBeforeExpression("${dateTime().minusYears(1)}"), 0);
	  }

	  public virtual void testQueryByTaskCreatedOnExpression()
	  {
		Time = task.CreateTime;
		assertCount(taskQuery().taskCreatedOnExpression("${now()}"), 1);

		adjustTime(10);

		assertCount(taskQuery().taskCreatedOnExpression("${dateTime().minusSeconds(10)}"), 1);

		assertCount(taskQuery().taskCreatedOnExpression("${now()}"), 0);
	  }

	  public virtual void testQueryByTaskCreatedAfterExpression()
	  {
		adjustTime(1);

		assertCount(taskQuery().taskCreatedAfterExpression("${now()}"), 0);

		adjustTime(-5 * 60);

		assertCount(taskQuery().taskCreatedAfterExpression("${now()}"), 3);

		Time = task.CreateTime;

		assertCount(taskQuery().taskCreatedAfterExpression("${dateTime().plusMonths(2)}"), 0);

		assertCount(taskQuery().taskCreatedAfterExpression("${dateTime().minusYears(1)}"), 3);
	  }

	  public virtual void testQueryByDueBeforeExpression()
	  {
		adjustTime(1);

		assertCount(taskQuery().dueBeforeExpression("${now()}"), 3);

		adjustTime(-5 * 60);

		assertCount(taskQuery().dueBeforeExpression("${now()}"), 0);

		Time = task.CreateTime;

		assertCount(taskQuery().dueBeforeExpression("${dateTime().plusMonths(2)}"), 3);

		assertCount(taskQuery().dueBeforeExpression("${dateTime().minusYears(1)}"), 0);
	  }

	  public virtual void testQueryByDueDateExpression()
	  {
		Time = task.DueDate;
		assertCount(taskQuery().dueDateExpression("${now()}"), 1);

		adjustTime(10);

		assertCount(taskQuery().dueDateExpression("${dateTime().minusSeconds(10)}"), 1);

		assertCount(taskQuery().dueDateExpression("${now()}"), 0);
	  }

	  public virtual void testQueryByDueAfterExpression()
	  {
		adjustTime(1);

		assertCount(taskQuery().dueAfterExpression("${now()}"), 0);

		adjustTime(-5 * 60);

		assertCount(taskQuery().dueAfterExpression("${now()}"), 3);

		Time = task.CreateTime;

		assertCount(taskQuery().dueAfterExpression("${dateTime().plusMonths(2)}"), 0);

		assertCount(taskQuery().dueAfterExpression("${dateTime().minusYears(1)}"), 3);
	  }

	  public virtual void testQueryByFollowUpBeforeExpression()
	  {
		adjustTime(1);

		assertCount(taskQuery().followUpBeforeExpression("${now()}"), 3);

		adjustTime(-5 * 60);

		assertCount(taskQuery().followUpBeforeExpression("${now()}"), 0);

		Time = task.CreateTime;

		assertCount(taskQuery().followUpBeforeExpression("${dateTime().plusMonths(2)}"), 3);

		assertCount(taskQuery().followUpBeforeExpression("${dateTime().minusYears(1)}"), 0);
	  }

	  public virtual void testQueryByFollowUpDateExpression()
	  {
		Time = task.FollowUpDate;
		assertCount(taskQuery().followUpDateExpression("${now()}"), 1);

		adjustTime(10);

		assertCount(taskQuery().followUpDateExpression("${dateTime().minusSeconds(10)}"), 1);

		assertCount(taskQuery().followUpDateExpression("${now()}"), 0);
	  }

	  public virtual void testQueryByFollowUpAfterExpression()
	  {
		adjustTime(1);

		assertCount(taskQuery().followUpAfterExpression("${now()}"), 0);

		adjustTime(-5 * 60);

		assertCount(taskQuery().followUpAfterExpression("${now()}"), 3);

		Time = task.CreateTime;

		assertCount(taskQuery().followUpAfterExpression("${dateTime().plusMonths(2)}"), 0);

		assertCount(taskQuery().followUpAfterExpression("${dateTime().minusYears(1)}"), 3);
	  }

	  public virtual void testQueryByProcessInstanceBusinessKeyExpression()
	  {
		// given
		string aBusinessKey = "business key";
		Mocks.register("aBusinessKey", aBusinessKey);

		createBusinessKeyDeployment(aBusinessKey);

		// when
		TaskQuery taskQuery = taskQuery().processInstanceBusinessKeyExpression("${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");

		// then
		assertCount(taskQuery, 1);
	  }

	  public virtual void testQueryByProcessInstanceBusinessKeyLikeExpression()
	  {
		// given
		string aBusinessKey = "business key";
		Mocks.register("aBusinessKeyLike", "%" + aBusinessKey.Substring(5));

		createBusinessKeyDeployment(aBusinessKey);

		// when
		TaskQuery taskQuery = taskQuery().processInstanceBusinessKeyLikeExpression("${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");

		// then
		assertCount(taskQuery, 1);
	  }

	  protected internal virtual void createBusinessKeyDeployment(string aBusinessKey)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("aProcessDefinition").startEvent().userTask().endEvent().done();

		deployment(modelInstance);

		runtimeService.startProcessInstanceByKey("aProcessDefinition", aBusinessKey);
	  }

	  public virtual void testExpressionOverrideQuery()
	  {
		string queryString = "query";
		string expressionString = "expression";
		string testStringExpression = "${'" + expressionString + "'}";

		DateTime queryDate = (new DateTime(now())).minusYears(1).toDate();
		string testDateExpression = "${now()}";

		TaskQueryImpl taskQuery = (TaskQueryImpl) taskQuery().taskAssignee(queryString).taskAssigneeExpression(testStringExpression).taskAssigneeLike(queryString).taskAssigneeLikeExpression(testStringExpression).taskOwnerExpression(queryString).taskOwnerExpression(expressionString).taskInvolvedUser(queryString).taskInvolvedUserExpression(expressionString).taskCreatedBefore(queryDate).taskCreatedBeforeExpression(testDateExpression).taskCreatedOn(queryDate).taskCreatedOnExpression(testDateExpression).taskCreatedAfter(queryDate).taskCreatedAfterExpression(testDateExpression).dueBefore(queryDate).dueBeforeExpression(testDateExpression).dueDate(queryDate).dueDateExpression(testDateExpression).dueAfter(queryDate).dueAfterExpression(testDateExpression).followUpBefore(queryDate).followUpBeforeExpression(testDateExpression).followUpDate(queryDate).followUpDateExpression(testDateExpression).followUpAfter(queryDate).followUpAfterExpression(testDateExpression);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(expressionString, taskQuery.Assignee);
		assertEquals(expressionString, taskQuery.AssigneeLike);
		assertEquals(expressionString, taskQuery.Owner);
		assertEquals(expressionString, taskQuery.InvolvedUser);
		assertTrue(taskQuery.CreateTimeBefore > queryDate);
		assertTrue(taskQuery.CreateTime > queryDate);
		assertTrue(taskQuery.CreateTimeAfter > queryDate);
		assertTrue(taskQuery.DueBefore > queryDate);
		assertTrue(taskQuery.DueDate > queryDate);
		assertTrue(taskQuery.DueAfter > queryDate);
		assertTrue(taskQuery.FollowUpBefore > queryDate);
		assertTrue(taskQuery.FollowUpDate > queryDate);
		assertTrue(taskQuery.FollowUpAfter > queryDate);

		// candidates has to be tested separately cause they have to be set exclusively

		taskQuery = (TaskQueryImpl) taskQuery().taskCandidateGroup(queryString).taskCandidateGroupExpression(testStringExpression);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(expressionString, taskQuery.CandidateGroup);

		taskQuery = (TaskQueryImpl) taskQuery().taskCandidateUser(queryString).taskCandidateUserExpression(testStringExpression);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(expressionString, taskQuery.CandidateUser);

		CurrentUser = user;
		IList<string> queryList = Arrays.asList("query");
		string testGroupsExpression = "${currentUserGroups()}";

		taskQuery = (TaskQueryImpl) taskQuery().taskCandidateGroupIn(queryList).taskCandidateGroupInExpression(testGroupsExpression);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(2, taskQuery.CandidateGroups.Count);
	  }

	  public virtual void testQueryOverrideExpression()
	  {
		string queryString = "query";
		string expressionString = "expression";
		string testStringExpression = "${'" + expressionString + "'}";

		DateTime queryDate = (new DateTime(now())).minusYears(1).toDate();
		string testDateExpression = "${now()}";

		TaskQueryImpl taskQuery = (TaskQueryImpl) taskQuery().taskAssigneeExpression(testStringExpression).taskAssignee(queryString).taskAssigneeLikeExpression(testStringExpression).taskAssigneeLike(queryString).taskOwnerExpression(expressionString).taskOwner(queryString).taskInvolvedUserExpression(expressionString).taskInvolvedUser(queryString).taskCreatedBeforeExpression(testDateExpression).taskCreatedBefore(queryDate).taskCreatedOnExpression(testDateExpression).taskCreatedOn(queryDate).taskCreatedAfterExpression(testDateExpression).taskCreatedAfter(queryDate).dueBeforeExpression(testDateExpression).dueBefore(queryDate).dueDateExpression(testDateExpression).dueDate(queryDate).dueAfterExpression(testDateExpression).dueAfter(queryDate).followUpBeforeExpression(testDateExpression).followUpBefore(queryDate).followUpDateExpression(testDateExpression).followUpDate(queryDate).followUpAfterExpression(testDateExpression).followUpAfter(queryDate);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(queryString, taskQuery.Assignee);
		assertEquals(queryString, taskQuery.AssigneeLike);
		assertEquals(queryString, taskQuery.Owner);
		assertEquals(queryString, taskQuery.InvolvedUser);
		assertTrue(taskQuery.CreateTimeBefore.Equals(queryDate));
		assertTrue(taskQuery.CreateTime.Equals(queryDate));
		assertTrue(taskQuery.CreateTimeAfter.Equals(queryDate));
		assertTrue(taskQuery.DueBefore.Equals(queryDate));
		assertTrue(taskQuery.DueDate.Equals(queryDate));
		assertTrue(taskQuery.DueAfter.Equals(queryDate));
		assertTrue(taskQuery.FollowUpBefore.Equals(queryDate));
		assertTrue(taskQuery.FollowUpDate.Equals(queryDate));
		assertTrue(taskQuery.FollowUpAfter.Equals(queryDate));

		// candidates has to be tested separately cause they have to be set exclusively

		taskQuery = (TaskQueryImpl) taskQuery().taskCandidateGroupExpression(testStringExpression).taskCandidateGroup(queryString);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(queryString, taskQuery.CandidateGroup);

		taskQuery = (TaskQueryImpl) taskQuery().taskCandidateUserExpression(testStringExpression).taskCandidateUser(queryString);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(queryString, taskQuery.CandidateUser);

		CurrentUser = user;
		IList<string> queryList = Arrays.asList("query");
		string testGroupsExpression = "${currentUserGroups()}";

		taskQuery = (TaskQueryImpl) taskQuery().taskCandidateGroupInExpression(testGroupsExpression).taskCandidateGroupIn(queryList);

		// execute query so expression will be evaluated
		taskQuery.count();

		assertEquals(1, taskQuery.CandidateGroups.Count);
	  }

	  public virtual void testQueryOr()
	  {
		// given
		DateTime date = DateTimeUtil.now().plusDays(2).toDate();

		Task task1 = taskService.newTask();
		task1.FollowUpDate = date;
		task1.Owner = "Luke Optim";
		task1.Name = "taskForOr";
		taskService.saveTask(task1);

		Task task2 = taskService.newTask();
		task2.DueDate = date;
		task2.Name = "taskForOr";
		taskService.saveTask(task2);

		Task task3 = taskService.newTask();
		task3.Assignee = "John Munda";
		task3.DueDate = date;
		task3.Name = "taskForOr";
		taskService.saveTask(task3);

		Task task4 = taskService.newTask();
		task4.Name = "taskForOr";
		taskService.saveTask(task4);

		// when
		IList<Task> tasks = taskService.createTaskQuery().taskName("taskForOr").or().followUpAfterExpression("${ now() }").taskAssigneeLikeExpression("${ 'John%' }").endOr().or().taskOwnerExpression("${ 'Luke Optim' }").dueAfterExpression("${ now() }").endOr().list();

		// then
		assertEquals(2, tasks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		Mocks.reset();

		foreach (Group group in identityService.createGroupQuery().list())
		{
		  identityService.deleteGroup(group.Id);
		}
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Task task in taskService.createTaskQuery().list())
		{
		  if (string.ReferenceEquals(task.ProcessInstanceId, null))
		  {
			taskService.deleteTask(task.Id, true);
		  }
		}

		identityService.clearAuthentication();
	  }


	  protected internal virtual TaskQuery taskQuery()
	  {
		return taskService.createTaskQuery();
	  }

	  protected internal virtual void assertCount(Query query, long count)
	  {
		assertEquals(count, query.count());
	  }

	  protected internal virtual User CurrentUser
	  {
		  set
		  {
			IList<Group> groups = identityService.createGroupQuery().groupMember(value.Id).list();
			IList<string> groupIds = new List<string>();
			foreach (Group group in groups)
			{
			  groupIds.Add(group.Id);
			}
    
			identityService.setAuthentication(value.Id, groupIds);
		  }
	  }

	  protected internal virtual Group createGroup(string groupId)
	  {
		Group group = identityService.newGroup(groupId);
		identityService.saveGroup(group);
		return group;
	  }

	  protected internal virtual User createUser(string userId, params string[] groupIds)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);

		if (groupIds != null)
		{
		  foreach (string groupId in groupIds)
		  {
			identityService.createMembership(userId, groupId);
		  }
		}

		return user;
	  }

	  protected internal virtual Task createTestTask(string taskId)
	  {
		Task task = taskService.newTask(taskId);
		task.DueDate = task.CreateTime;
		task.FollowUpDate = task.CreateTime;
		taskService.saveTask(task);
		return task;
	  }

	  protected internal virtual DateTime now()
	  {
		return ClockUtil.CurrentTime;
	  }

	  protected internal virtual long Time
	  {
		  set
		  {
			Time = new DateTime(value);
		  }
	  }

	  protected internal virtual DateTime Time
	  {
		  set
		  {
			ClockUtil.CurrentTime = value;
		  }
	  }

	  /// <summary>
	  /// Changes the current time about the given amount in seconds.
	  /// </summary>
	  /// <param name="amount"> the amount to adjust the current time </param>
	  protected internal virtual void adjustTime(int amount)
	  {
		long time = now().Ticks + amount * 1000;
		Time = time;
	  }

	}

}