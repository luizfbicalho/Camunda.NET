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
namespace org.camunda.bpm.qa.upgrade.gson
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class TaskFilterScenario
	{

	  [DescribesScenario("initTaskFilter")]
	  public static ScenarioSetup initTaskFilter()
	  {
		return new ScenarioSetupAnonymousInnerClass();
	  }

	  private class ScenarioSetupAnonymousInnerClass : ScenarioSetup
	  {
		  public void execute(ProcessEngine engine, string scenarioName)
		  {
			string testString = "test";
			int? testInteger = 1;
			DelegationState testDelegationState = DelegationState.PENDING;
			DateTime testDate = new DateTime();
			string[] testActivityInstances = new string[] {"a", "b", "c"};
			string[] testKeys = new string[] {"d", "e"};
			IList<string> testCandidateGroups = new List<string>();

			string[] variableNames = new string[] {"a", "b", "c", "d", "e", "f"};
			object[] variableValues = new object[] {1, 2, "3", "4", 5, 6};

			testCandidateGroups.Add("group");
			testCandidateGroups.Add("anotherGroup");

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

			query.orderByExecutionId().desc();
			query.orderByDueDate().asc();
			query.orderByProcessVariable("var", ValueType.STRING).desc();

			// save filter
			Filter filter = engine.FilterService.newTaskFilter("filter");
			filter.Query = query;

			engine.FilterService.saveFilter(filter);
		  }
	  }
	}

}