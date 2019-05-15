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
namespace org.camunda.bpm.engine.test.api.filter
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using Filter = org.camunda.bpm.engine.filter.Filter;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using Direction = org.camunda.bpm.engine.impl.Direction;
	using QueryEntityRelationCondition = org.camunda.bpm.engine.impl.QueryEntityRelationCondition;
	using QueryOperator = org.camunda.bpm.engine.impl.QueryOperator;
	using QueryOrderingProperty = org.camunda.bpm.engine.impl.QueryOrderingProperty;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using TaskQueryProperty = org.camunda.bpm.engine.impl.TaskQueryProperty;
	using TaskQueryVariableValue = org.camunda.bpm.engine.impl.TaskQueryVariableValue;
	using VariableOrderProperty = org.camunda.bpm.engine.impl.VariableOrderProperty;
	using JsonTaskQueryConverter = org.camunda.bpm.engine.impl.json.JsonTaskQueryConverter;
	using FilterEntity = org.camunda.bpm.engine.impl.persistence.entity.FilterEntity;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using JsonObject = com.google.gson.JsonObject;
	using Query = org.camunda.bpm.engine.query.Query;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Mocks = org.camunda.bpm.engine.test.mock.Mocks;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterTaskQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal Filter filter;

	  protected internal string testString = "test";
	  protected internal int? testInteger = 1;
	  protected internal DelegationState testDelegationState = DelegationState.PENDING;
	  protected internal DateTime testDate = new DateTime();
	  protected internal string[] testActivityInstances = new string[] {"a", "b", "c"};
	  protected internal string[] testKeys = new string[] {"d", "e"};
	  protected internal IList<string> testCandidateGroups = new List<string>();

	  protected internal string[] variableNames = new string[] {"a", "b", "c", "d", "e", "f"};
	  protected internal object[] variableValues = new object[] {1, 2, "3", "4", 5, 6};
	  protected internal QueryOperator[] variableOperators = new QueryOperator[] {QueryOperator.EQUALS, QueryOperator.GREATER_THAN_OR_EQUAL, QueryOperator.LESS_THAN, QueryOperator.LIKE, QueryOperator.NOT_EQUALS, QueryOperator.LESS_THAN_OR_EQUAL};
	  protected internal bool[] isTaskVariable = new bool[] {true, true, false, false, false, false};
	  protected internal bool[] isProcessVariable = new bool[] {false, false, true, true, false, false};
	  protected internal User testUser;
	  protected internal Group testGroup;

	  protected internal JsonTaskQueryConverter queryConverter;

	  public override void setUp()
	  {
		filter = filterService.newTaskFilter("name").setOwner("owner").setQuery(taskService.createTaskQuery()).setProperties(new Dictionary<string, object>());
		testUser = identityService.newUser("user");
		testGroup = identityService.newGroup("group");
		identityService.saveUser(testUser);
		identityService.saveGroup(testGroup);
		identityService.createMembership(testUser.Id, testGroup.Id);

		Group anotherGroup = identityService.newGroup("anotherGroup");
		identityService.saveGroup(anotherGroup);
		testCandidateGroups.Add(testGroup.Id);
		testCandidateGroups.Add(anotherGroup.Id);

		createTasks();

		queryConverter = new JsonTaskQueryConverter();
	  }

	  public override void tearDown()
	  {
		processEngineConfiguration.EnableExpressionsInAdhocQueries = false;

		Mocks.reset();

		foreach (Filter filter in filterService.createTaskFilterQuery().list())
		{
		  filterService.deleteFilter(filter.Id);
		}
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
	  }

	  public virtual void testEmptyQuery()
	  {
		TaskQuery emptyQuery = taskService.createTaskQuery();
		string emptyQueryJson = "{}";

		filter.Query = emptyQuery;

		assertEquals(emptyQueryJson, ((FilterEntity) filter).QueryInternal);
		assertNotNull(filter.Query);
	  }

	  public virtual void testTaskQuery()
	  {
		// create query
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskId(testString);
		query.taskName(testString);
		query.taskNameNotEqual(testString);
		query.taskNameLike(testString);
		query.taskNameNotLike(testString);
		query.taskDescription(testString);
		query.taskDescriptionLike(testString);
		query.taskPriority(testInteger);
		query.taskMinPriority(testInteger);
		query.taskMaxPriority(testInteger);
		query.taskAssignee(testString);
		query.taskAssigneeExpression(testString);
		query.taskAssigneeLike(testString);
		query.taskAssigneeLikeExpression(testString);
		query.taskInvolvedUser(testString);
		query.taskInvolvedUserExpression(testString);
		query.taskOwner(testString);
		query.taskOwnerExpression(testString);
		query.taskUnassigned();
		query.taskAssigned();
		query.taskDelegationState(testDelegationState);
		query.taskCandidateGroupIn(testCandidateGroups);
		query.taskCandidateGroupInExpression(testString);
		query.withCandidateGroups();
		query.withoutCandidateGroups();
		query.withCandidateUsers();
		query.withoutCandidateUsers();
		query.processInstanceId(testString);
		query.executionId(testString);
		query.activityInstanceIdIn(testActivityInstances);
		query.taskCreatedOn(testDate);
		query.taskCreatedOnExpression(testString);
		query.taskCreatedBefore(testDate);
		query.taskCreatedBeforeExpression(testString);
		query.taskCreatedAfter(testDate);
		query.taskCreatedAfterExpression(testString);
		query.taskDefinitionKey(testString);
		query.taskDefinitionKeyIn(testKeys);
		query.taskDefinitionKeyLike(testString);
		query.processDefinitionKey(testString);
		query.processDefinitionKeyIn(testKeys);
		query.processDefinitionId(testString);
		query.processDefinitionName(testString);
		query.processDefinitionNameLike(testString);
		query.processInstanceBusinessKey(testString);
		query.processInstanceBusinessKeyExpression(testString);
		query.processInstanceBusinessKeyIn(testKeys);
		query.processInstanceBusinessKeyLike(testString);
		query.processInstanceBusinessKeyLikeExpression(testString);

		// variables
		query.taskVariableValueEquals(variableNames[0], variableValues[0]);
		query.taskVariableValueGreaterThanOrEquals(variableNames[1], variableValues[1]);
		query.processVariableValueLessThan(variableNames[2], variableValues[2]);
		query.processVariableValueLike(variableNames[3], (string) variableValues[3]);
		query.caseInstanceVariableValueNotEquals(variableNames[4], variableValues[4]);
		query.caseInstanceVariableValueLessThanOrEquals(variableNames[5], variableValues[5]);

		query.dueDate(testDate);
		query.dueDateExpression(testString);
		query.dueBefore(testDate);
		query.dueBeforeExpression(testString);
		query.dueAfter(testDate);
		query.dueAfterExpression(testString);
		query.followUpDate(testDate);
		query.followUpDateExpression(testString);
		query.followUpBefore(testDate);
		query.followUpBeforeExpression(testString);
		query.followUpAfter(testDate);
		query.followUpAfterExpression(testString);
		query.excludeSubtasks();
		query.suspended();
		query.caseDefinitionKey(testString);
		query.caseDefinitionId(testString);
		query.caseDefinitionName(testString);
		query.caseDefinitionNameLike(testString);
		query.caseInstanceId(testString);
		query.caseInstanceBusinessKey(testString);
		query.caseInstanceBusinessKeyLike(testString);
		query.caseExecutionId(testString);

		// ordering
		query.orderByExecutionId().desc();
		query.orderByDueDate().asc();
		query.orderByProcessVariable("var", ValueType.STRING).desc();

		IList<QueryOrderingProperty> expectedOrderingProperties = query.OrderingProperties;

		// save filter
		filter.Query = query;
		filterService.saveFilter(filter);

		// fetch from db
		filter = filterService.createTaskFilterQuery().singleResult();

		// test query
		query = filter.Query;
		assertEquals(testString, query.TaskId);
		assertEquals(testString, query.Name);
		assertEquals(testString, query.NameNotEqual);
		assertEquals(testString, query.NameNotLike);
		assertEquals(testString, query.NameLike);
		assertEquals(testString, query.Description);
		assertEquals(testString, query.DescriptionLike);
		assertEquals(testInteger, query.Priority);
		assertEquals(testInteger, query.MinPriority);
		assertEquals(testInteger, query.MaxPriority);
		assertEquals(testString, query.Assignee);
		assertEquals(testString, query.Expressions["taskAssignee"]);
		assertEquals(testString, query.AssigneeLike);
		assertEquals(testString, query.Expressions["taskAssigneeLike"]);
		assertEquals(testString, query.InvolvedUser);
		assertEquals(testString, query.Expressions["taskInvolvedUser"]);
		assertEquals(testString, query.Owner);
		assertEquals(testString, query.Expressions["taskOwner"]);
		assertTrue(query.Unassigned);
		assertTrue(query.Assigned);
		assertEquals(testDelegationState, query.DelegationState);
		assertEquals(testCandidateGroups, query.CandidateGroups);
		assertTrue(query.WithCandidateGroups);
		assertTrue(query.WithoutCandidateGroups);
		assertTrue(query.WithCandidateUsers);
		assertTrue(query.WithoutCandidateUsers);
		assertEquals(testString, query.Expressions["taskCandidateGroupIn"]);
		assertEquals(testString, query.ProcessInstanceId);
		assertEquals(testString, query.ExecutionId);
		assertEquals(testActivityInstances.Length, query.ActivityInstanceIdIn.Length);
		for (int i = 0; i < query.ActivityInstanceIdIn.Length; i++)
		{
		  assertEquals(testActivityInstances[i], query.ActivityInstanceIdIn[i]);
		}
		assertEquals(testDate, query.CreateTime);
		assertEquals(testString, query.Expressions["taskCreatedOn"]);
		assertEquals(testDate, query.CreateTimeBefore);
		assertEquals(testString, query.Expressions["taskCreatedBefore"]);
		assertEquals(testDate, query.CreateTimeAfter);
		assertEquals(testString, query.Expressions["taskCreatedAfter"]);
		assertEquals(testString, query.Key);
		assertEquals(testKeys.Length, query.Keys.Length);
		for (int i = 0; i < query.Keys.Length; i++)
		{
		  assertEquals(testKeys[i], query.Keys[i]);
		}
		assertEquals(testString, query.KeyLike);
		assertEquals(testString, query.ProcessDefinitionKey);
		for (int i = 0; i < query.ProcessDefinitionKeys.Length; i++)
		{
		  assertEquals(testKeys[i], query.ProcessDefinitionKeys[i]);
		}
		assertEquals(testString, query.ProcessDefinitionId);
		assertEquals(testString, query.ProcessDefinitionName);
		assertEquals(testString, query.ProcessDefinitionNameLike);
		assertEquals(testString, query.ProcessInstanceBusinessKey);
		assertEquals(testString, query.Expressions["processInstanceBusinessKey"]);
		for (int i = 0; i < query.ProcessInstanceBusinessKeys.Length; i++)
		{
		  assertEquals(testKeys[i], query.ProcessInstanceBusinessKeys[i]);
		}
		assertEquals(testString, query.ProcessInstanceBusinessKeyLike);
		assertEquals(testString, query.Expressions["processInstanceBusinessKeyLike"]);

		// variables
		IList<TaskQueryVariableValue> variables = query.Variables;
		for (int i = 0; i < variables.Count; i++)
		{
		  TaskQueryVariableValue variable = variables[i];
		  assertEquals(variableNames[i], variable.Name);
		  assertEquals(variableValues[i], variable.Value);
		  assertEquals(variableOperators[i], variable.Operator);
		  assertEquals(isTaskVariable[i], variable.Local);
		  assertEquals(isProcessVariable[i], variable.ProcessInstanceVariable);
		}

		assertEquals(testDate, query.DueDate);
		assertEquals(testString, query.Expressions["dueDate"]);
		assertEquals(testDate, query.DueBefore);
		assertEquals(testString, query.Expressions["dueBefore"]);
		assertEquals(testDate, query.DueAfter);
		assertEquals(testString, query.Expressions["dueAfter"]);
		assertEquals(testDate, query.FollowUpDate);
		assertEquals(testString, query.Expressions["followUpDate"]);
		assertEquals(testDate, query.FollowUpBefore);
		assertEquals(testString, query.Expressions["followUpBefore"]);
		assertEquals(testDate, query.FollowUpAfter);
		assertEquals(testString, query.Expressions["followUpAfter"]);
		assertTrue(query.ExcludeSubtasks);
		assertEquals(org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED, query.SuspensionState);
		assertEquals(testString, query.CaseDefinitionKey);
		assertEquals(testString, query.CaseDefinitionId);
		assertEquals(testString, query.CaseDefinitionName);
		assertEquals(testString, query.CaseDefinitionNameLike);
		assertEquals(testString, query.CaseInstanceId);
		assertEquals(testString, query.CaseInstanceBusinessKey);
		assertEquals(testString, query.CaseInstanceBusinessKeyLike);
		assertEquals(testString, query.CaseExecutionId);

		// ordering
		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);
	  }

	  protected internal virtual void verifyOrderingProperties(IList<QueryOrderingProperty> expectedProperties, IList<QueryOrderingProperty> actualProperties)
	  {
		assertEquals(expectedProperties.Count, actualProperties.Count);

		for (int i = 0; i < expectedProperties.Count; i++)
		{
		  QueryOrderingProperty expectedProperty = expectedProperties[i];
		  QueryOrderingProperty actualProperty = actualProperties[i];

		  assertEquals(expectedProperty.Relation, actualProperty.Relation);
		  assertEquals(expectedProperty.Direction, actualProperty.Direction);
		  assertEquals(expectedProperty.ContainedProperty, actualProperty.ContainedProperty);
		  assertEquals(expectedProperty.QueryProperty, actualProperty.QueryProperty);

		  IList<QueryEntityRelationCondition> expectedRelationConditions = expectedProperty.RelationConditions;
		  IList<QueryEntityRelationCondition> actualRelationConditions = expectedProperty.RelationConditions;

		  if (expectedRelationConditions != null && actualRelationConditions != null)
		  {
			assertEquals(expectedRelationConditions.Count, actualRelationConditions.Count);

			for (int j = 0; j < expectedRelationConditions.Count; j++)
			{
			  QueryEntityRelationCondition expectedFilteringProperty = expectedRelationConditions[j];
			  QueryEntityRelationCondition actualFilteringProperty = expectedRelationConditions[j];

			  assertEquals(expectedFilteringProperty.Property, actualFilteringProperty.Property);
			  assertEquals(expectedFilteringProperty.ComparisonProperty, actualFilteringProperty.ComparisonProperty);
			  assertEquals(expectedFilteringProperty.ScalarValue, actualFilteringProperty.ScalarValue);
			}
		  }
		  else if ((expectedRelationConditions == null && actualRelationConditions != null) || (expectedRelationConditions != null && actualRelationConditions == null))
		  {
			fail("Expected filtering properties: " + expectedRelationConditions + ". " + "Actual filtering properties: " + actualRelationConditions);
		  }
		}
	  }

	  public virtual void testTaskQueryByBusinessKeyExpression()
	  {
		// given
		string aBusinessKey = "business key";
		Mocks.register("aBusinessKey", aBusinessKey);

		createDeploymentWithBusinessKey(aBusinessKey);

		// when
		TaskQueryImpl extendedQuery = (TaskQueryImpl)taskService.createTaskQuery().processInstanceBusinessKeyExpression("${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");

		Filter filter = filterService.newTaskFilter("aFilterName");
		filter.Query = extendedQuery;
		filterService.saveFilter(filter);

		TaskQueryImpl filterQuery = filterService.getFilter(filter.Id).Query;

		// then
		assertEquals(extendedQuery.Expressions["processInstanceBusinessKey"], filterQuery.Expressions["processInstanceBusinessKey"]);
		assertEquals(1, filterService.list(filter.Id).Count);
	  }

	  public virtual void testTaskQueryByBusinessKeyExpressionInAdhocQuery()
	  {
		// given
		processEngineConfiguration.EnableExpressionsInAdhocQueries = true;

		string aBusinessKey = "business key";
		Mocks.register("aBusinessKey", aBusinessKey);

		createDeploymentWithBusinessKey(aBusinessKey);

		// when
		Filter filter = filterService.newTaskFilter("aFilterName");
		filter.Query = taskService.createTaskQuery();
		filterService.saveFilter(filter);

		TaskQueryImpl extendingQuery = (TaskQueryImpl)taskService.createTaskQuery().processInstanceBusinessKeyExpression("${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");

		// then
		assertEquals(extendingQuery.Expressions["processInstanceBusinessKey"], "${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");
		assertEquals(1, filterService.list(filter.Id, extendingQuery).Count);
	  }

	  public virtual void testTaskQueryByBusinessKeyLikeExpression()
	  {
		// given
		string aBusinessKey = "business key";
		Mocks.register("aBusinessKeyLike", "%" + aBusinessKey.Substring(5));

		createDeploymentWithBusinessKey(aBusinessKey);

		// when
		TaskQueryImpl extendedQuery = (TaskQueryImpl)taskService.createTaskQuery().processInstanceBusinessKeyLikeExpression("${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");

		Filter filter = filterService.newTaskFilter("aFilterName");
		filter.Query = extendedQuery;
		filterService.saveFilter(filter);

		TaskQueryImpl filterQuery = filterService.getFilter(filter.Id).Query;

		// then
		assertEquals(extendedQuery.Expressions["processInstanceBusinessKeyLike"], filterQuery.Expressions["processInstanceBusinessKeyLike"]);
		assertEquals(1, filterService.list(filter.Id).Count);
	  }

	  public virtual void testTaskQueryByBusinessKeyLikeExpressionInAdhocQuery()
	  {
		// given
		processEngineConfiguration.EnableExpressionsInAdhocQueries = true;

		string aBusinessKey = "business key";
		Mocks.register("aBusinessKeyLike", "%" + aBusinessKey.Substring(5));

		createDeploymentWithBusinessKey(aBusinessKey);

		// when
		Filter filter = filterService.newTaskFilter("aFilterName");
		filter.Query = taskService.createTaskQuery();
		filterService.saveFilter(filter);

		TaskQueryImpl extendingQuery = (TaskQueryImpl)taskService.createTaskQuery().processInstanceBusinessKeyLikeExpression("${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");

		// then
		assertEquals(extendingQuery.Expressions["processInstanceBusinessKeyLike"], "${ " + Mocks.Mocks.Keys.ToArray()[0] + " }");
		assertEquals(1, filterService.list(filter.Id, extendingQuery).Count);
	  }

	  protected internal virtual void createDeploymentWithBusinessKey(string aBusinessKey)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess("aProcessDefinition").startEvent().userTask().endEvent().done();

		deployment(modelInstance);

		runtimeService.startProcessInstanceByKey("aProcessDefinition", aBusinessKey);
	  }

	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistent()
	  {
		// create query
		TaskQueryImpl query = new TaskQueryImpl();

		query.followUpBeforeOrNotExistent(testDate);

		// save filter
		filter.Query = query;
		filterService.saveFilter(filter);

		// fetch from db
		filter = filterService.createTaskFilterQuery().singleResult();

		// test query
		query = filter.Query;
		assertTrue(query.FollowUpNullAccepted);
		assertEquals(testDate, query.FollowUpBefore);
	  }

	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistentExtendingQuery()
	  {
		// create query
		TaskQueryImpl query = new TaskQueryImpl();

		query.followUpBeforeOrNotExistent(testDate);

		// save filter without query
		filterService.saveFilter(filter);

		// fetch from db
		filter = filterService.createTaskFilterQuery().singleResult();

		// use query as extending query
		IList<Task> tasks = filterService.list(filter.Id, query);
		assertEquals(3, tasks.Count);

		// set as filter query and save filter
		filter.Query = query;
		filterService.saveFilter(filter);

		// fetch from db
		filter = filterService.createTaskFilterQuery().singleResult();

		tasks = filterService.list(filter.Id);
		assertEquals(3, tasks.Count);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.orderByTaskCreateTime().asc();

		tasks = filterService.list(filter.Id, extendingQuery);
		assertEquals(3, tasks.Count);
	  }

	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistentExpression()
	  {
		// create query
		TaskQueryImpl query = new TaskQueryImpl();

		query.followUpBeforeOrNotExistentExpression(testString);

		// save filter
		filter.Query = query;
		filterService.saveFilter(filter);

		// fetch from db
		filter = filterService.createTaskFilterQuery().singleResult();

		// test query
		query = filter.Query;
		assertTrue(query.FollowUpNullAccepted);
		assertEquals(testString, query.Expressions["followUpBeforeOrNotExistent"]);
	  }

	  public virtual void testTaskQueryByFollowUpBeforeOrNotExistentExpressionExtendingQuery()
	  {
		// create query
		TaskQueryImpl query = new TaskQueryImpl();

		query.followUpBeforeOrNotExistentExpression("${dateTime().withMillis(0)}");

		// save filter without query
		filterService.saveFilter(filter);

		// fetch from db
		filter = filterService.createTaskFilterQuery().singleResult();

		// use query as extending query
		IList<Task> tasks = filterService.list(filter.Id, query);
		assertEquals(3, tasks.Count);

		// set as filter query and save filter
		filter.Query = query;
		filterService.saveFilter(filter);

		// fetch from db
		filter = filterService.createTaskFilterQuery().singleResult();

		tasks = filterService.list(filter.Id);
		assertEquals(3, tasks.Count);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.orderByTaskCreateTime().asc();

		tasks = filterService.list(filter.Id, extendingQuery);
		assertEquals(3, tasks.Count);
	  }

	  public virtual void testTaskQueryCandidateUser()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateUser(testUser.Id);
		query.taskCandidateUserExpression(testUser.Id);

		filter.Query = query;
		query = filter.Query;

		assertEquals(testUser.Id, query.CandidateUser);
		assertEquals(testUser.Id, query.Expressions["taskCandidateUser"]);
	  }

	  public virtual void testTaskQueryCandidateGroup()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateGroup(testGroup.Id);
		query.taskCandidateGroupExpression(testGroup.Id);

		filter.Query = query;
		query = filter.Query;

		assertEquals(testGroup.Id, query.CandidateGroup);
		assertEquals(testGroup.Id, query.Expressions["taskCandidateGroup"]);
	  }

	  public virtual void testTaskQueryCandidateUserIncludeAssignedTasks()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateUser(testUser.Id);
		query.includeAssignedTasks();

		saveQuery(query);
		query = filterService.getFilter(filter.Id).Query;

		assertEquals(testUser.Id, query.CandidateUser);
		assertTrue(query.IncludeAssignedTasks);
	  }

	  public virtual void testTaskQueryCandidateUserExpressionIncludeAssignedTasks()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateUserExpression(testString);
		query.includeAssignedTasks();

		saveQuery(query);
		query = filterService.getFilter(filter.Id).Query;

		assertEquals(testString, query.Expressions["taskCandidateUser"]);
		assertTrue(query.IncludeAssignedTasks);
	  }

	  public virtual void testTaskQueryCandidateGroupIncludeAssignedTasks()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateGroup(testGroup.Id);
		query.includeAssignedTasks();

		saveQuery(query);
		query = filterService.getFilter(filter.Id).Query;

		assertEquals(testGroup.Id, query.CandidateGroup);
		assertTrue(query.IncludeAssignedTasks);
	  }

	  public virtual void testTaskQueryCandidateGroupExpressionIncludeAssignedTasks()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateGroupExpression(testString);
		query.includeAssignedTasks();

		saveQuery(query);
		query = filterService.getFilter(filter.Id).Query;

		assertEquals(testString, query.Expressions["taskCandidateGroup"]);
		assertTrue(query.IncludeAssignedTasks);
	  }

	  public virtual void testTaskQueryCandidateGroupsIncludeAssignedTasks()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateGroupIn(testCandidateGroups);
		query.includeAssignedTasks();

		saveQuery(query);
		query = filterService.getFilter(filter.Id).Query;

		assertEquals(testCandidateGroups, query.CandidateGroupsInternal);
		assertTrue(query.IncludeAssignedTasks);
	  }

	  public virtual void testTaskQueryCandidateGroupsExpressionIncludeAssignedTasks()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateGroupInExpression(testString);
		query.includeAssignedTasks();

		saveQuery(query);
		query = filterService.getFilter(filter.Id).Query;

		assertEquals(testString, query.Expressions["taskCandidateGroupIn"]);
		assertTrue(query.IncludeAssignedTasks);
	  }

	  public virtual void testExecuteTaskQueryList()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskNameLike("Task%");

		saveQuery(query);

		IList<Task> tasks = filterService.list(filter.Id);
		assertEquals(3, tasks.Count);
		foreach (Task task in tasks)
		{
		  assertEquals(testUser.Id, task.Owner);
		}
	  }

	  public virtual void testExtendingTaskQueryList()
	  {
		TaskQuery query = taskService.createTaskQuery();

		saveQuery(query);

		IList<Task> tasks = filterService.list(filter.Id);
		assertEquals(3, tasks.Count);

		tasks = filterService.list(filter.Id, query);
		assertEquals(3, tasks.Count);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.taskDelegationState(DelegationState.RESOLVED);

		tasks = filterService.list(filter.Id, extendingQuery);
		assertEquals(2, tasks.Count);

		foreach (Task task in tasks)
		{
		  assertEquals(DelegationState.RESOLVED, task.DelegationState);
		}
	  }

	  public virtual void testExtendingTaskQueryListWithCandidateGroups()
	  {
		TaskQuery query = taskService.createTaskQuery();

		IList<string> candidateGroups = new List<string>();
		candidateGroups.Add("accounting");
		query.taskCandidateGroupIn(candidateGroups);

		saveQuery(query);

		IList<Task> tasks = filterService.list(filter.Id);
		assertEquals(1, tasks.Count);

		tasks = filterService.list(filter.Id, query);
		assertEquals(1, tasks.Count);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.orderByTaskCreateTime().asc();

		tasks = filterService.list(filter.Id, extendingQuery);
		assertEquals(1, tasks.Count);
	  }

	  public virtual void testExtendingTaskQueryListWithIncludeAssignedTasks()
	  {
		TaskQuery query = taskService.createTaskQuery();

		query.taskCandidateGroup("accounting");

		saveQuery(query);

		IList<Task> tasks = filterService.list(filter.Id);
		assertEquals(1, tasks.Count);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.taskCandidateGroup("accounting").includeAssignedTasks();

		tasks = filterService.list(filter.Id, extendingQuery);
		assertEquals(2, tasks.Count);
	  }

	  public virtual void testExtendTaskQueryWithCandidateUserExpressionAndIncludeAssignedTasks()
	  {
		// create an empty query and save it as a filter
		TaskQuery emptyQuery = taskService.createTaskQuery();
		Filter emptyFilter = filterService.newTaskFilter("empty");
		emptyFilter.Query = emptyQuery;

		// create a query with candidate user expression and include assigned tasks
		// and save it as filter
		TaskQuery query = taskService.createTaskQuery();
		query.taskCandidateUserExpression("${'test'}").includeAssignedTasks();
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// extend empty query by query with candidate user expression and include assigned tasks
		Filter extendedFilter = emptyFilter.extend(query);
		TaskQueryImpl extendedQuery = extendedFilter.Query;
		assertEquals("${'test'}", extendedQuery.Expressions["taskCandidateUser"]);
		assertTrue(extendedQuery.IncludeAssignedTasks);

		// extend query with candidate user expression and include assigned tasks with empty query
		extendedFilter = filter.extend(emptyQuery);
		extendedQuery = extendedFilter.Query;
		assertEquals("${'test'}", extendedQuery.Expressions["taskCandidateUser"]);
		assertTrue(extendedQuery.IncludeAssignedTasks);
	  }

	  public virtual void testExtendTaskQueryWithCandidateGroupExpressionAndIncludeAssignedTasks()
	  {
		// create an empty query and save it as a filter
		TaskQuery emptyQuery = taskService.createTaskQuery();
		Filter emptyFilter = filterService.newTaskFilter("empty");
		emptyFilter.Query = emptyQuery;

		// create a query with candidate group expression and include assigned tasks
		// and save it as filter
		TaskQuery query = taskService.createTaskQuery();
		query.taskCandidateGroupExpression("${'test'}").includeAssignedTasks();
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// extend empty query by query with candidate group expression and include assigned tasks
		Filter extendedFilter = emptyFilter.extend(query);
		TaskQueryImpl extendedQuery = extendedFilter.Query;
		assertEquals("${'test'}", extendedQuery.Expressions["taskCandidateGroup"]);
		assertTrue(extendedQuery.IncludeAssignedTasks);

		// extend query with candidate group expression and include assigned tasks with empty query
		extendedFilter = filter.extend(emptyQuery);
		extendedQuery = extendedFilter.Query;
		assertEquals("${'test'}", extendedQuery.Expressions["taskCandidateGroup"]);
		assertTrue(extendedQuery.IncludeAssignedTasks);
	  }

	  public virtual void testExtendTaskQueryWithCandidateGroupInAndCandidateGroup()
	  {
		// create an query with candidate group in and save it as a filter
		TaskQueryImpl candidateGroupInQuery = (TaskQueryImpl)taskService.createTaskQuery().taskCandidateGroupIn(Arrays.asList("testGroup", "testGroup2"));
		assertEquals(2, candidateGroupInQuery.CandidateGroups.Count);
		assertEquals("testGroup", candidateGroupInQuery.CandidateGroups[0]);
		assertEquals("testGroup2", candidateGroupInQuery.CandidateGroups[1]);
		Filter candidateGroupInFilter = filterService.newTaskFilter("Groups filter");
		candidateGroupInFilter.Query = candidateGroupInQuery;

		// create a query with candidate group
		// and save it as filter
		TaskQuery candidateGroupQuery = taskService.createTaskQuery().taskCandidateGroup("testGroup2");

		// extend candidate group in filter by query with candidate group
		Filter extendedFilter = candidateGroupInFilter.extend(candidateGroupQuery);
		TaskQueryImpl extendedQuery = extendedFilter.Query;
		assertEquals(1, extendedQuery.CandidateGroups.Count);
		assertEquals("testGroup2", extendedQuery.CandidateGroups[0]);
	  }

	  public virtual void testTaskQueryWithCandidateGroupInExpressionAndCandidateGroup()
	  {
		// create an query with candidate group in expression and candidate group at once
		TaskQueryImpl candidateGroupInQuery = (TaskQueryImpl)taskService.createTaskQuery().taskCandidateGroupInExpression("${'test'}").taskCandidateGroup("testGroup");
		assertEquals("${'test'}", candidateGroupInQuery.Expressions["taskCandidateGroupIn"]);
		assertEquals("testGroup", candidateGroupInQuery.CandidateGroup);
	  }

	  public virtual void testTaskQueryWithCandidateGroupInAndCandidateGroupExpression()
	  {
		// create an query with candidate group in and candidate group expression
		TaskQueryImpl candidateGroupInQuery = (TaskQueryImpl)taskService.createTaskQuery().taskCandidateGroupIn(Arrays.asList("testGroup", "testGroup2")).taskCandidateGroupExpression("${'test'}");
		assertEquals("${'test'}", candidateGroupInQuery.Expressions["taskCandidateGroup"]);
		assertEquals(2, candidateGroupInQuery.CandidateGroups.Count);
		assertEquals("testGroup", candidateGroupInQuery.CandidateGroups[0]);
		assertEquals("testGroup2", candidateGroupInQuery.CandidateGroups[1]);
	  }

	  public virtual void testExtendTaskQueryWithCandidateGroupInExpressionAndIncludeAssignedTasks()
	  {
		// create an empty query and save it as a filter
		TaskQuery emptyQuery = taskService.createTaskQuery();
		Filter emptyFilter = filterService.newTaskFilter("empty");
		emptyFilter.Query = emptyQuery;

		// create a query with candidate group in expression and include assigned tasks
		// and save it as filter
		TaskQuery query = taskService.createTaskQuery();
		query.taskCandidateGroupInExpression("${'test'}").includeAssignedTasks();
		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// extend empty query by query with candidate group in expression and include assigned tasks
		Filter extendedFilter = emptyFilter.extend(query);
		TaskQueryImpl extendedQuery = extendedFilter.Query;
		assertEquals("${'test'}", extendedQuery.Expressions["taskCandidateGroupIn"]);
		assertTrue(extendedQuery.IncludeAssignedTasks);

		// extend query with candidate group in expression and include assigned tasks with empty query
		extendedFilter = filter.extend(emptyQuery);
		extendedQuery = extendedFilter.Query;
		assertEquals("${'test'}", extendedQuery.Expressions["taskCandidateGroupIn"]);
		assertTrue(extendedQuery.IncludeAssignedTasks);
	  }

	  public virtual void testExecuteTaskQueryListPage()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskNameLike("Task%");

		saveQuery(query);

		IList<Task> tasks = filterService.listPage(filter.Id, 1, 2);
		assertEquals(2, tasks.Count);
		foreach (Task task in tasks)
		{
		  assertEquals(testUser.Id, task.Owner);
		}
	  }

	  public virtual void testExtendingTaskQueryListPage()
	  {
		TaskQuery query = taskService.createTaskQuery();

		saveQuery(query);

		IList<Task> tasks = filterService.listPage(filter.Id, 1, 2);
		assertEquals(2, tasks.Count);

		tasks = filterService.listPage(filter.Id, query, 1, 2);
		assertEquals(2, tasks.Count);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.taskDelegationState(DelegationState.RESOLVED);

		tasks = filterService.listPage(filter.Id, extendingQuery, 1, 2);
		assertEquals(1, tasks.Count);

		assertEquals(DelegationState.RESOLVED, tasks[0].DelegationState);
	  }

	  public virtual void testExecuteTaskQuerySingleResult()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskDelegationState(DelegationState.PENDING);

		saveQuery(query);

		Task task = filterService.singleResult(filter.Id);
		assertNotNull(task);
		assertEquals("Task 1", task.Name);
	  }

	  public virtual void testFailTaskQuerySingleResult()
	  {
		TaskQuery query = taskService.createTaskQuery();

		saveQuery(query);

		try
		{
		  filterService.singleResult(filter.Id);
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  public virtual void testExtendingTaskQuerySingleResult()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskDelegationState(DelegationState.PENDING);

		saveQuery(query);

		Task task = filterService.singleResult(filter.Id);
		assertNotNull(task);
		assertEquals("Task 1", task.Name);
		assertEquals("task1", task.Id);

		task = filterService.singleResult(filter.Id, query);
		assertNotNull(task);
		assertEquals("Task 1", task.Name);
		assertEquals("task1", task.Id);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.taskId("task1");

		task = filterService.singleResult(filter.Id, extendingQuery);
		assertNotNull(task);
		assertEquals("Task 1", task.Name);
		assertEquals("task1", task.Id);
	  }

	  /// <summary>
	  /// CAM-6363
	  /// 
	  /// Verify that search by name returns case insensitive results
	  /// </summary>
	  public virtual void testTaskQueryLookupByNameCaseInsensitive()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskName("task 1");
		saveQuery(query);

		IList<Task> tasks = filterService.list(filter.Id);
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(1));

		query = taskService.createTaskQuery();
		query.taskName("tASk 2");
		saveQuery(query);

		tasks = filterService.list(filter.Id);
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(1));
	  }

	  /// <summary>
	  /// CAM-6165
	  /// 
	  /// Verify that search by name like returns case insensitive results
	  /// </summary>
	  public virtual void testTaskQueryLookupByNameLikeCaseInsensitive()
	  {
		TaskQuery query = taskService.createTaskQuery();
		query.taskNameLike("%task%");
		saveQuery(query);

		IList<Task> tasks = filterService.list(filter.Id);
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(3));

		query = taskService.createTaskQuery();
		query.taskNameLike("%Task%");
		saveQuery(query);

		tasks = filterService.list(filter.Id);
		assertNotNull(tasks);
		assertThat(tasks.Count,@is(3));
	  }

	  public virtual void testExecuteTaskQueryCount()
	  {
		TaskQuery query = taskService.createTaskQuery();

		saveQuery(query);

		long count = filterService.count(filter.Id).Value;
		assertEquals(3, count);

		query.taskDelegationState(DelegationState.RESOLVED);

		saveQuery(query);

		count = filterService.count(filter.Id).Value;
		assertEquals(2, count);
	  }

	  public virtual void testExtendingTaskQueryCount()
	  {
		TaskQuery query = taskService.createTaskQuery();

		saveQuery(query);

		TaskQuery extendingQuery = taskService.createTaskQuery();

		extendingQuery.taskId("task3");

		long count = filterService.count(filter.Id).Value;

		assertEquals(3, count);

		count = filterService.count(filter.Id, query).Value;

		assertEquals(3, count);

		count = filterService.count(filter.Id, extendingQuery).Value;

		assertEquals(1, count);
	  }

	  public virtual void testSpecialExtendingQuery()
	  {
		TaskQuery query = taskService.createTaskQuery();

		saveQuery(query);

		long count = filterService.count(filter.Id, (Query) null).Value;
		assertEquals(3, count);
	  }

	  public virtual void testExtendingSorting()
	  {
		// create empty query
		TaskQueryImpl query = (TaskQueryImpl) taskService.createTaskQuery();
		saveQuery(query);

		// assert default sorting
		query = filter.Query;
		assertTrue(query.OrderingProperties.Empty);

		// extend query by new task query with sorting
		TaskQueryImpl sortQuery = (TaskQueryImpl) taskService.createTaskQuery().orderByTaskName().asc();
		Filter extendedFilter = filter.extend(sortQuery);
		query = extendedFilter.Query;

		IList<QueryOrderingProperty> expectedOrderingProperties = new List<QueryOrderingProperty>(sortQuery.OrderingProperties);

		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);

		// extend query by new task query with additional sorting
		TaskQueryImpl extendingQuery = (TaskQueryImpl) taskService.createTaskQuery().orderByTaskAssignee().desc();
		extendedFilter = extendedFilter.extend(extendingQuery);
		query = extendedFilter.Query;

		((IList<QueryOrderingProperty>)expectedOrderingProperties).AddRange(extendingQuery.OrderingProperties);

		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);

		// extend query by incomplete sorting query (should add sorting anyway)
		sortQuery = (TaskQueryImpl) taskService.createTaskQuery().orderByCaseExecutionId();
		extendedFilter = extendedFilter.extend(sortQuery);
		query = extendedFilter.Query;

		((IList<QueryOrderingProperty>)expectedOrderingProperties).AddRange(sortQuery.OrderingProperties);

		verifyOrderingProperties(expectedOrderingProperties, query.OrderingProperties);
	  }


	  /// <summary>
	  /// Tests compatibility with serialization format that was used in 7.2
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void testDeprecatedOrderingFormatDeserializationSingleOrdering()
	  public virtual void testDeprecatedOrderingFormatDeserializationSingleOrdering()
	  {
		string sortByNameAsc = "RES." + org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME.Name + " " + Direction.ASCENDING.Name;

		JsonTaskQueryConverter converter = (JsonTaskQueryConverter) FilterEntity.queryConverter[EntityTypes.TASK];
		JsonObject queryJson = converter.toJsonObject(filter.getQuery<TaskQuery>());

		// when I apply a specific ordering by one dimension
		queryJson.addProperty(JsonTaskQueryConverter.ORDER_BY, sortByNameAsc);
		TaskQueryImpl deserializedTaskQuery = (TaskQueryImpl) converter.toObject(queryJson);

		// then the ordering is applied accordingly
		assertEquals(1, deserializedTaskQuery.OrderingProperties.size());

		QueryOrderingProperty orderingProperty = deserializedTaskQuery.OrderingProperties.get(0);
		assertNull(orderingProperty.Relation);
		assertEquals("asc", orderingProperty.Direction.Name);
		assertNull(orderingProperty.RelationConditions);
		assertTrue(orderingProperty.ContainedProperty);
		assertEquals(org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME.Name, orderingProperty.QueryProperty.Name);
		assertNull(orderingProperty.QueryProperty.Function);

	  }

	  /// <summary>
	  /// Tests compatibility with serialization format that was used in 7.2
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void testDeprecatedOrderingFormatDeserializationSecondaryOrdering()
	  public virtual void testDeprecatedOrderingFormatDeserializationSecondaryOrdering()
	  {
		string sortByNameAsc = "RES." + org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME.Name + " " + Direction.ASCENDING.Name;
		string secondaryOrdering = sortByNameAsc + ", RES." + org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.ASSIGNEE.Name + " " + Direction.DESCENDING.Name;

		JsonTaskQueryConverter converter = (JsonTaskQueryConverter) FilterEntity.queryConverter[EntityTypes.TASK];
		JsonObject queryJson = converter.toJsonObject(filter.getQuery<TaskQuery>());

		// when I apply a secondary ordering
		queryJson.addProperty(JsonTaskQueryConverter.ORDER_BY, secondaryOrdering);
		TaskQueryImpl deserializedTaskQuery = (TaskQueryImpl) converter.toObject(queryJson);

		// then the ordering is applied accordingly
		assertEquals(2, deserializedTaskQuery.OrderingProperties.size());

		QueryOrderingProperty orderingProperty1 = deserializedTaskQuery.OrderingProperties.get(0);
		assertNull(orderingProperty1.Relation);
		assertEquals("asc", orderingProperty1.Direction.Name);
		assertNull(orderingProperty1.RelationConditions);
		assertTrue(orderingProperty1.ContainedProperty);
		assertEquals(org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME.Name, orderingProperty1.QueryProperty.Name);
		assertNull(orderingProperty1.QueryProperty.Function);

		QueryOrderingProperty orderingProperty2 = deserializedTaskQuery.OrderingProperties.get(1);
		assertNull(orderingProperty2.Relation);
		assertEquals("desc", orderingProperty2.Direction.Name);
		assertNull(orderingProperty2.RelationConditions);
		assertTrue(orderingProperty2.ContainedProperty);
		assertEquals(org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.ASSIGNEE.Name, orderingProperty2.QueryProperty.Name);
		assertNull(orderingProperty2.QueryProperty.Function);
	  }

	  /// <summary>
	  /// Tests compatibility with serialization format that was used in 7.2
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") public void testDeprecatedOrderingFormatDeserializationFunctionOrdering()
	  public virtual void testDeprecatedOrderingFormatDeserializationFunctionOrdering()
	  {
		string orderingWithFunction = "LOWER(RES." + org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME.Name + ") asc";

		JsonTaskQueryConverter converter = (JsonTaskQueryConverter) FilterEntity.queryConverter[EntityTypes.TASK];
		JsonObject queryJson = converter.toJsonObject(filter.getQuery<TaskQuery>());

		// when I apply an ordering with a function
		queryJson.addProperty(JsonTaskQueryConverter.ORDER_BY, orderingWithFunction);
		TaskQueryImpl deserializedTaskQuery = (TaskQueryImpl) converter.toObject(queryJson);

		assertEquals(1, deserializedTaskQuery.OrderingProperties.size());

		// then the ordering is applied accordingly
		QueryOrderingProperty orderingProperty = deserializedTaskQuery.OrderingProperties.get(0);
		assertNull(orderingProperty.Relation);
		assertEquals("asc", orderingProperty.Direction.Name);
		assertNull(orderingProperty.RelationConditions);
		assertFalse(orderingProperty.ContainedProperty);
		assertEquals(org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME_CASE_INSENSITIVE.Name, orderingProperty.QueryProperty.Name);
		assertEquals(org.camunda.bpm.engine.impl.TaskQueryProperty_Fields.NAME_CASE_INSENSITIVE.Function, orderingProperty.QueryProperty.Function);
	  }


	  [Deployment(resources:{"org/camunda/bpm/engine/test/api/task/oneTaskWithFormKeyProcess.bpmn20.xml"})]
	  public virtual void testInitializeFormKeysEnabled()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("testProcess");

		TaskQuery query = taskService.createTaskQuery().processInstanceId(processInstance.Id);

		saveQuery(query);

		Task task = (Task) filterService.list(filter.Id)[0];

		assertEquals("exampleFormKey", task.FormKey);

		task = filterService.singleResult(filter.Id);

		assertEquals("exampleFormKey", task.FormKey);

		runtimeService.deleteProcessInstance(processInstance.Id, "test");
	  }

	  public virtual void testExtendingVariableQuery()
	  {
		TaskQuery taskQuery = taskService.createTaskQuery().processVariableValueEquals("hello", "world");
		saveQuery(taskQuery);

		// variables won't overridden variables with same name in different scopes
		TaskQuery extendingQuery = taskService.createTaskQuery().taskVariableValueEquals("hello", "world").caseInstanceVariableValueEquals("hello", "world");

		Filter extendedFilter = filter.extend(extendingQuery);
		TaskQueryImpl extendedQuery = extendedFilter.Query;
		IList<TaskQueryVariableValue> variables = extendedQuery.Variables;

		assertEquals(3, variables.Count);

		// assert variables (ordering: extending variables are inserted first)
		assertEquals("hello", variables[0].Name);
		assertEquals("world", variables[0].Value);
		assertEquals(QueryOperator.EQUALS, variables[0].Operator);
		assertFalse(variables[0].ProcessInstanceVariable);
		assertTrue(variables[0].Local);
		assertEquals("hello", variables[1].Name);
		assertEquals("world", variables[1].Value);
		assertEquals(QueryOperator.EQUALS, variables[1].Operator);
		assertFalse(variables[1].ProcessInstanceVariable);
		assertFalse(variables[1].Local);
		assertEquals("hello", variables[2].Name);
		assertEquals("world", variables[2].Value);
		assertEquals(QueryOperator.EQUALS, variables[2].Operator);
		assertTrue(variables[2].ProcessInstanceVariable);
		assertFalse(variables[2].Local);

		// variables will override variables with same name in same scope
		extendingQuery = taskService.createTaskQuery().processVariableValueLessThan("hello", 42).taskVariableValueLessThan("hello", 42).caseInstanceVariableValueLessThan("hello", 42);

		extendedFilter = filter.extend(extendingQuery);
		extendedQuery = extendedFilter.Query;
		variables = extendedQuery.Variables;

		assertEquals(3, variables.Count);

		// assert variables (ordering: extending variables are inserted first)
		assertEquals("hello", variables[0].Name);
		assertEquals(42, variables[0].Value);
		assertEquals(QueryOperator.LESS_THAN, variables[0].Operator);
		assertTrue(variables[0].ProcessInstanceVariable);
		assertFalse(variables[0].Local);
		assertEquals("hello", variables[1].Name);
		assertEquals(42, variables[1].Value);
		assertEquals(QueryOperator.LESS_THAN, variables[1].Operator);
		assertFalse(variables[1].ProcessInstanceVariable);
		assertTrue(variables[1].Local);
		assertEquals("hello", variables[2].Name);
		assertEquals(42, variables[2].Value);
		assertEquals(QueryOperator.LESS_THAN, variables[2].Operator);
		assertFalse(variables[2].ProcessInstanceVariable);
		assertFalse(variables[2].Local);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testExtendTaskQueryByOrderByProcessVariable()
	  {
		ProcessInstance instance500 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", 500));
		ProcessInstance instance1000 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", 1000));
		ProcessInstance instance250 = runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("var", 250));

		TaskQuery query = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess");
		saveQuery(query);

		// asc
		TaskQuery extendingQuery = taskService.createTaskQuery().orderByProcessVariable("var", ValueType.INTEGER).asc();

		IList<Task> tasks = filterService.list(filter.Id, extendingQuery);

		assertEquals(3, tasks.Count);
		assertEquals(instance250.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance500.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance1000.Id, tasks[2].ProcessInstanceId);

		// desc
		extendingQuery = taskService.createTaskQuery().orderByProcessVariable("var", ValueType.INTEGER).desc();

		tasks = filterService.list(filter.Id, extendingQuery);

		assertEquals(3, tasks.Count);
		assertEquals(instance1000.Id, tasks[0].ProcessInstanceId);
		assertEquals(instance500.Id, tasks[1].ProcessInstanceId);
		assertEquals(instance250.Id, tasks[2].ProcessInstanceId);

		runtimeService.deleteProcessInstance(instance250.Id, null);
		runtimeService.deleteProcessInstance(instance500.Id, null);
		runtimeService.deleteProcessInstance(instance1000.Id, null);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testExtendTaskQueryByOrderByTaskVariable()
	  {
		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task task500 = taskService.createTaskQuery().processInstanceId(instance1.Id).singleResult();
		taskService.setVariableLocal(task500.Id, "var", 500);

		Task task250 = taskService.createTaskQuery().processInstanceId(instance2.Id).singleResult();
		taskService.setVariableLocal(task250.Id, "var", 250);

		Task task1000 = taskService.createTaskQuery().processInstanceId(instance3.Id).singleResult();
		taskService.setVariableLocal(task1000.Id, "var", 1000);

		TaskQuery query = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess");
		saveQuery(query);

		// asc
		TaskQuery extendingQuery = taskService.createTaskQuery().orderByProcessVariable("var", ValueType.INTEGER).asc();

		IList<Task> tasks = filterService.list(filter.Id, extendingQuery);

		assertEquals(3, tasks.Count);
		assertEquals(task250.Id, tasks[0].Id);
		assertEquals(task500.Id, tasks[1].Id);
		assertEquals(task1000.Id, tasks[2].Id);

		// desc
		extendingQuery = taskService.createTaskQuery().orderByProcessVariable("var", ValueType.INTEGER).desc();

		tasks = filterService.list(filter.Id, extendingQuery);

		assertEquals(3, tasks.Count);
		assertEquals(task1000.Id, tasks[0].Id);
		assertEquals(task500.Id, tasks[1].Id);
		assertEquals(task250.Id, tasks[2].Id);

		runtimeService.deleteProcessInstance(instance1.Id, null);
		runtimeService.deleteProcessInstance(instance2.Id, null);
		runtimeService.deleteProcessInstance(instance3.Id, null);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testExtendTaskQueryByTaskVariableIgnoreCase()
	  {
		string variableName = "variableName";
		string variableValueCamelCase = "someVariableValue";
		string variableValueLowerCase = variableValueCamelCase.ToLower();

		ProcessInstance instance1 = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance2 = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		ProcessInstance instance3 = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task taskCamelCase = taskService.createTaskQuery().processInstanceId(instance1.Id).singleResult();
		taskService.setVariableLocal(taskCamelCase.Id, variableName, variableValueCamelCase);

		Task taskLowerCase = taskService.createTaskQuery().processInstanceId(instance2.Id).singleResult();
		taskService.setVariableLocal(taskLowerCase.Id, variableName, variableValueLowerCase);

		Task taskWithNoVariable = taskService.createTaskQuery().processInstanceId(instance3.Id).singleResult();

		TaskQuery query = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess");
		saveQuery(query);

		// all tasks
		IList<Task> tasks = filterService.list(filter.Id, query);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertTrue(tasks.Contains(taskWithNoVariable));

		// equals case-sensitive for comparison
		TaskQuery extendingQuery = taskService.createTaskQuery().taskVariableValueEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// equals case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// not equals case-sensitive for comparison
		extendingQuery = taskService.createTaskQuery().taskVariableValueNotEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// not equals case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueNotEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// like case-sensitive for comparison
		extendingQuery = taskService.createTaskQuery().taskVariableValueLike(variableName, "somevariable%");
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// like case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().taskVariableValueLike(variableName, "somevariable%");
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// variable name case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableNamesIgnoreCase().taskVariableValueEquals(variableName.ToLower(), variableValueCamelCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// variable name and variable value case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableNamesIgnoreCase().matchVariableValuesIgnoreCase().taskVariableValueEquals(variableName.ToLower(), variableValueCamelCase.ToLower());
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testExtendTaskQueryByCaseInstanceVariableIgnoreCase()
	  {
		string variableName = "variableName";
		string variableValueCamelCase = "someVariableValue";
		string variableValueLowerCase = variableValueCamelCase.ToLower();
		IDictionary<string, object> variables = new Dictionary<string, object>();

		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		variables[variableName] = variableValueCamelCase;
		CaseInstance instanceCamelCase = caseService.createCaseInstanceById(caseDefinitionId, variables);
		variables[variableName] = variableValueLowerCase;
		CaseInstance instanceLowerCase = caseService.createCaseInstanceById(caseDefinitionId, variables);
		CaseInstance instanceWithoutVariables = caseService.createCaseInstanceById(caseDefinitionId);

		Task taskCamelCase = taskService.createTaskQuery().caseInstanceId(instanceCamelCase.Id).singleResult();
		Task taskLowerCase = taskService.createTaskQuery().caseInstanceId(instanceLowerCase.Id).singleResult();
		Task taskWithNoVariable = taskService.createTaskQuery().caseInstanceId(instanceWithoutVariables.Id).singleResult();

		TaskQuery query = taskService.createTaskQuery().caseDefinitionId(caseDefinitionId);
		saveQuery(query);

		// all tasks
		IList<Task> tasks = filterService.list(filter.Id, query);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertTrue(tasks.Contains(taskWithNoVariable));

		// equals case-sensitive for comparison
		TaskQuery extendingQuery = taskService.createTaskQuery().caseInstanceVariableValueEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// equals case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// not equals case-sensitive for comparison
		extendingQuery = taskService.createTaskQuery().caseInstanceVariableValueNotEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// not equals case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueNotEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// like case-sensitive for comparison
		extendingQuery = taskService.createTaskQuery().caseInstanceVariableValueLike(variableName, "somevariable%");
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// like case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().caseInstanceVariableValueLike(variableName, "somevariable%");
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// variable name case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableNamesIgnoreCase().caseInstanceVariableValueEquals(variableName.ToLower(), variableValueCamelCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		//variable name and variable value case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableNamesIgnoreCase().matchVariableValuesIgnoreCase().caseInstanceVariableValueEquals(variableName.ToLower(), variableValueCamelCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// cleanup
		caseService.terminateCaseExecution(instanceCamelCase.Id);
		caseService.terminateCaseExecution(instanceLowerCase.Id);
		caseService.terminateCaseExecution(instanceWithoutVariables.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testExtendTaskQueryByProcessVariableIgnoreCase()
	  {
		string variableName = "variableName";
		string variableValueCamelCase = "someVariableValue";
		string variableValueLowerCase = variableValueCamelCase.ToLower();
		IDictionary<string, object> variables = new Dictionary<string, object>();

		variables[variableName] = variableValueCamelCase;
		ProcessInstance instanceCamelCase = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		variables[variableName] = variableValueLowerCase;
		ProcessInstance instanceLowerCase = runtimeService.startProcessInstanceByKey("oneTaskProcess", variables);
		ProcessInstance instanceWithoutVariables = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		Task taskCamelCase = taskService.createTaskQuery().processInstanceId(instanceCamelCase.Id).singleResult();
		Task taskLowerCase = taskService.createTaskQuery().processInstanceId(instanceLowerCase.Id).singleResult();
		Task taskWithNoVariable = taskService.createTaskQuery().processInstanceId(instanceWithoutVariables.Id).singleResult();

		TaskQuery query = taskService.createTaskQuery().processDefinitionKey("oneTaskProcess");
		saveQuery(query);

		// all tasks
		IList<Task> tasks = filterService.list(filter.Id, query);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertTrue(tasks.Contains(taskWithNoVariable));

		// equals case-sensitive for comparison
		TaskQuery extendingQuery = taskService.createTaskQuery().processVariableValueEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// equals case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// not equals case-sensitive for comparison
		extendingQuery = taskService.createTaskQuery().processVariableValueNotEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// not equals case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueNotEquals(variableName, variableValueLowerCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// like case-sensitive for comparison
		extendingQuery = taskService.createTaskQuery().processVariableValueLike(variableName, "somevariable%");
		tasks = filterService.list(filter.Id, extendingQuery);
		assertFalse(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// like case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableValuesIgnoreCase().processVariableValueLike(variableName, "somevariable%");
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// variable name case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableNamesIgnoreCase().processVariableValueEquals(variableName.ToLower(), variableValueCamelCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertFalse(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

		// variable name and variable value case-insensitive
		extendingQuery = taskService.createTaskQuery().matchVariableNamesIgnoreCase().matchVariableValuesIgnoreCase().processVariableValueEquals(variableName.ToLower(), variableValueCamelCase);
		tasks = filterService.list(filter.Id, extendingQuery);
		assertTrue(tasks.Contains(taskCamelCase));
		assertTrue(tasks.Contains(taskLowerCase));
		assertFalse(tasks.Contains(taskWithNoVariable));

	  }

	  public virtual void testExtendTaskQuery_ORInExtendingQuery()
	  {
		// given
		createTasksForOrQueries();

		// when
		TaskQuery extendedQuery = taskService.createTaskQuery().taskName("taskForOr");

		Filter extendedFilter = filterService.newTaskFilter("extendedOrFilter");
		extendedFilter.Query = extendedQuery;
		filterService.saveFilter(extendedFilter);

		TaskQuery extendingQuery = taskService.createTaskQuery().or().taskDescription("aTaskDescription").taskOwner("aTaskOwner").endOr().or().taskPriority(3).taskAssignee("aTaskAssignee").endOr();

		// then
		assertEquals(4, extendedQuery.list().size());
		assertEquals(4, filterService.list(extendedFilter.Id).Count);
		assertEquals(6, extendingQuery.list().size());
		assertEquals(3, filterService.list(extendedFilter.Id, extendingQuery).Count);
	  }

	  public virtual void testExtendTaskQuery_ORInExtendedQuery()
	  {
		// given
		createTasksForOrQueries();

		// when
		TaskQuery extendedQuery = taskService.createTaskQuery().or().taskDescription("aTaskDescription").taskOwner("aTaskOwner").endOr().or().taskPriority(3).taskAssignee("aTaskAssignee").endOr();

		Filter extendedFilter = filterService.newTaskFilter("extendedOrFilter");
		extendedFilter.Query = extendedQuery;
		filterService.saveFilter(extendedFilter);

		TaskQuery extendingQuery = taskService.createTaskQuery().taskName("taskForOr");

		// then
		assertEquals(6, extendedQuery.list().size());
		assertEquals(6, filterService.list(extendedFilter.Id).Count);
		assertEquals(4, extendingQuery.list().size());
		assertEquals(3, filterService.list(extendedFilter.Id, extendingQuery).Count);
	  }

	  public virtual void testExtendTaskQuery_ORInBothExtendedAndExtendingQuery()
	  {
		// given
		createTasksForOrQueries();

		// when
		TaskQuery extendedQuery = taskService.createTaskQuery().or().taskName("taskForOr").taskDescription("aTaskDescription").endOr();

		Filter extendedFilter = filterService.newTaskFilter("extendedOrFilter");
		extendedFilter.Query = extendedQuery;
		filterService.saveFilter(extendedFilter);

		TaskQuery extendingQuery = taskService.createTaskQuery().or().tenantIdIn("aTenantId").taskOwner("aTaskOwner").endOr().or().taskPriority(3).taskAssignee("aTaskAssignee").endOr();

		// then
		assertEquals(6, extendedQuery.list().size());
		assertEquals(6, filterService.list(extendedFilter.Id).Count);
		assertEquals(4, extendingQuery.list().size());
		assertEquals(3, filterService.list(extendedFilter.Id, extendingQuery).Count);
	  }

	  public virtual void testOrderByVariables()
	  {
		// given
		TaskQueryImpl query = (TaskQueryImpl) taskService.createTaskQuery().orderByProcessVariable("foo", ValueType.STRING).asc().orderByExecutionVariable("foo", ValueType.STRING).asc().orderByCaseInstanceVariable("foo", ValueType.STRING).asc().orderByCaseExecutionVariable("foo", ValueType.STRING).asc().orderByTaskVariable("foo", ValueType.STRING).asc();

		Filter filter = filterService.newTaskFilter("extendedOrFilter");
		filter.Query = query;
		filterService.saveFilter(filter);

		// when
		filter = filterService.getFilter(filter.Id);

		// then
		IList<QueryOrderingProperty> expectedOrderingProperties = new List<QueryOrderingProperty>(query.OrderingProperties);

		verifyOrderingProperties(expectedOrderingProperties, ((TaskQueryImpl) filter.Query).OrderingProperties);

		foreach (QueryOrderingProperty prop in ((TaskQueryImpl) filter.Query).OrderingProperties)
		{
		  assertTrue(prop is VariableOrderProperty);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testBooleanVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("booleanVariable", true));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("booleanVariable", true);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testIntVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("intVariable", 7));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("intVariable", 7);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testIntOutOfRangeVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("longVariable", int.MaxValue+1L));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("longVariable", int.MaxValue+1L);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDoubleVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("doubleVariable", 88.89D));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("doubleVariable", 88.89D);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testStringVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("stringVariable", "aVariableValue"));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("stringVariable", "aVariableValue");

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testNullVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("nullVariable", null));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("nullVariable", null);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void testDueDate()
	  {
		// given
		DateTime date = DateTime.Now;
		string processInstanceId = runtimeService.startProcessInstanceByKey("oneTaskProcess").Id;

		Task task = taskService.createTaskQuery().processInstanceId(processInstanceId).singleResult();

		task.DueDate = date;

		taskService.saveTask(task);

		TaskQuery query = taskService.createTaskQuery().dueDate(date);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  /// <summary>
	  /// See CAM-9613
	  /// </summary>
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void FAILING_testDateVariable()
	  {
		// given
		DateTime date = DateTime.Now;
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("dateVariable", date));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("dateVariable", date);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  /// <summary>
	  /// See CAM-9613
	  /// </summary>
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void FAILING_testByteArrayVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("bytesVariable", "aByteArray".GetBytes()));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("bytesVariable", "aByteArray".GetBytes());

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  /// <summary>
	  /// See CAM-9613
	  /// </summary>
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void FAILING_testLongVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("longVariable", 7L));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("longVariable", 7L);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  /// <summary>
	  /// See CAM-9613
	  /// </summary>
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml"})]
	  public virtual void FAILING_testShortVariable()
	  {
		// given
		runtimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValue("shortVariable", (short) 7));

		TaskQuery query = taskService.createTaskQuery().processVariableValueEquals("shortVariable", (short) 7);

		Filter filter = filterService.newTaskFilter("filter");
		filter.Query = query;

		// when
		filterService.saveFilter(filter);

		// then
		assertThat(filterService.count(filter.Id), @is(1L));
	  }

	  protected internal virtual void saveQuery(Query query)
	  {
		filter.Query = query;
		filterService.saveFilter(filter);
		filter = filterService.getFilter(filter.Id);
	  }

	  protected internal virtual void createTasks()
	  {
		Task task = taskService.newTask("task1");
		task.Name = "Task 1";
		task.Owner = testUser.Id;
		task.DelegationState = DelegationState.PENDING;
		taskService.saveTask(task);
		taskService.addCandidateGroup(task.Id, "accounting");

		task = taskService.newTask("task2");
		task.Name = "Task 2";
		task.Owner = testUser.Id;
		task.DelegationState = DelegationState.RESOLVED;
		taskService.saveTask(task);
		taskService.setAssignee(task.Id, "kermit");
		taskService.addCandidateGroup(task.Id, "accounting");

		task = taskService.newTask("task3");
		task.Name = "Task 3";
		task.Owner = testUser.Id;
		task.DelegationState = DelegationState.RESOLVED;
		taskService.saveTask(task);
	  }

	  protected internal virtual void createTasksForOrQueries()
	  {
		Task task1 = taskService.newTask();
		task1.Name = "taskForOr";
		task1.Description = "aTaskDescription";
		task1.Priority = 3;
		taskService.saveTask(task1);

		Task task2 = taskService.newTask();
		task2.Name = "taskForOr";
		task2.Description = "aTaskDescription";
		task2.Assignee = "aTaskAssignee";
		task2.TenantId = "aTenantId";
		taskService.saveTask(task2);

		Task task3 = taskService.newTask();
		task3.Name = "taskForOr";
		task3.Owner = "aTaskOwner";
		taskService.saveTask(task3);

		Task task4 = taskService.newTask();
		task4.Name = "taskForOr";
		task4.Owner = "aTaskOwner";
		task4.Priority = 3;
		taskService.saveTask(task4);

		Task task5 = taskService.newTask();
		task5.Description = "aTaskDescription";
		task5.Assignee = "aTaskAssignee";
		taskService.saveTask(task5);

		Task task6 = taskService.newTask();
		task6.Description = "aTaskDescription";
		task6.Assignee = "aTaskAssignee";
		task6.TenantId = "aTenantId";
		taskService.saveTask(task6);

		Task task7 = taskService.newTask();
		task7.TenantId = "aTenantId";
		task7.Owner = "aTaskOwner";
		task7.Priority = 3;
		task7.Assignee = "aTaskAssignee";
		taskService.saveTask(task7);
	  }
	}

}