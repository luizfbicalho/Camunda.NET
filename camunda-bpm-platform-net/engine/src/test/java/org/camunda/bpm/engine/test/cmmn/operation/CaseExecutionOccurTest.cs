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
namespace org.camunda.bpm.engine.test.cmmn.operation
{
	using MilestoneActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.MilestoneActivityBehavior;
	using CaseExecutionImpl = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionImpl;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnCaseInstance = org.camunda.bpm.engine.impl.cmmn.execution.CmmnCaseInstance;
	using CaseDefinitionBuilder = org.camunda.bpm.engine.impl.cmmn.model.CaseDefinitionBuilder;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnOnPartDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnOnPartDeclaration;
	using CmmnSentryDeclaration = org.camunda.bpm.engine.impl.cmmn.model.CmmnSentryDeclaration;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionOccurTest : PvmTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOccurMilestone()
	  public virtual void testOccurMilestone()
	  {

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).createActivity("A").behavior(new MilestoneActivityBehavior()).endActivity().buildCaseDefinition();

		CmmnActivity activity = caseDefinition.findActivity("A");

		// a pseudo sentry
		CmmnSentryDeclaration sentryDeclaration = new CmmnSentryDeclaration("X");
		caseDefinition.findActivity("Case1").addSentry(sentryDeclaration);
		activity.addEntryCriteria(sentryDeclaration);

		CmmnOnPartDeclaration onPartDeclaration = new CmmnOnPartDeclaration();
		onPartDeclaration.Source = new CmmnActivity("B", caseDefinition);
		onPartDeclaration.StandardEvent = "complete";
		sentryDeclaration.addOnPart(onPartDeclaration);

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution milestoneA = caseInstance.findCaseExecution("A");

		// when

		// completing
		milestoneA.occur();

		// then
		// task A is completed ...
		assertTrue(milestoneA.Completed);
		// ... and the case instance is also completed
		assertTrue(caseInstance.Completed);

		// task A is not part of the case instance anymore
		assertNull(caseInstance.findCaseExecution("A"));
		// the case instance has no children
		assertTrue(((CaseExecutionImpl) caseInstance).CaseExecutions.Count == 0);
	  }

	}

}