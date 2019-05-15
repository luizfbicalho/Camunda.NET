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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.gson
{
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using QueryEntityRelationCondition = org.camunda.bpm.engine.impl.QueryEntityRelationCondition;
	using QueryOperator = org.camunda.bpm.engine.impl.QueryOperator;
	using QueryOrderingProperty = org.camunda.bpm.engine.impl.QueryOrderingProperty;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using TaskQueryVariableValue = org.camunda.bpm.engine.impl.TaskQueryVariableValue;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.fail;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[ScenarioUnderTest("TaskFilterScenario"), Origin("7.11.0")]
	public class TaskFilterTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
		public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTaskFilter.1") @Test public void testTaskFilter()
	  [ScenarioUnderTest("initTaskFilter.1")]
	  public virtual void testTaskFilter()
	  {
		string testString = "test";
		int? testInteger = 1;
		DelegationState testDelegationState = DelegationState.PENDING;
		DateTime testDate = new DateTime();
		string[] testActivityInstances = new string[] {"a", "b", "c"};
		string[] testKeys = new string[] {"d", "e"};
		IList<string> testCandidateGroups = new List<string>();
		testCandidateGroups.Add("group");
		testCandidateGroups.Add("anotherGroup");

		string[] variableNames = new string[] {"a", "b", "c", "d", "e", "f"};
		object[] variableValues = new object[] {1, 2, "3", "4", 5, 6};

		QueryOperator[] variableOperators = new QueryOperator[] {QueryOperator.EQUALS, QueryOperator.GREATER_THAN_OR_EQUAL, QueryOperator.LESS_THAN, QueryOperator.LIKE, QueryOperator.NOT_EQUALS, QueryOperator.LESS_THAN_OR_EQUAL};
		bool[] isTaskVariable = new bool[] {true, true, false, false, false, false};
		bool[] isProcessVariable = new bool[] {false, false, true, true, false, false};

		TaskQueryImpl expectedOrderingPropertiesQuery = new TaskQueryImpl();
		expectedOrderingPropertiesQuery.orderByExecutionId().desc();
		expectedOrderingPropertiesQuery.orderByDueDate().asc();
		expectedOrderingPropertiesQuery.orderByProcessVariable("var", ValueType.STRING).desc();

		IList<QueryOrderingProperty> expectedOrderingProperties = expectedOrderingPropertiesQuery.OrderingProperties;

		Filter filter = engineRule.FilterService.createTaskFilterQuery().filterName("filter").singleResult();

		TaskQueryImpl query = filter.Query;
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

			foreach (QueryEntityRelationCondition expectedFilteringProperty in expectedRelationConditions)
			{

			  assertEquals(expectedFilteringProperty.Property, expectedFilteringProperty.Property);
			  assertEquals(expectedFilteringProperty.ComparisonProperty, expectedFilteringProperty.ComparisonProperty);
			  assertEquals(expectedFilteringProperty.ScalarValue, expectedFilteringProperty.ScalarValue);
			}
		  }
		  else if (expectedRelationConditions != null || actualRelationConditions != null)
		  {
			fail("Expected filtering properties: " + expectedRelationConditions + ". " + "Actual filtering properties: " + actualRelationConditions);
		  }
		}
	  }

	}
}