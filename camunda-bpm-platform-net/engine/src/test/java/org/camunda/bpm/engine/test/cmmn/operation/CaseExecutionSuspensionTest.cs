﻿/*
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
	using StageActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.StageActivityBehavior;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnCaseInstance = org.camunda.bpm.engine.impl.cmmn.execution.CmmnCaseInstance;
	using ItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler;
	using CaseDefinitionBuilder = org.camunda.bpm.engine.impl.cmmn.model.CaseDefinitionBuilder;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionSuspensionTest : PvmTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendCaseInstance()
	  public virtual void testSuspendCaseInstance()
	  {

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).createActivity("X").behavior(new StageActivityBehavior()).createActivity("A").behavior(new TaskWaitState()).endActivity().createActivity("B").behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when

		caseInstance.suspend();

		// then
		assertTrue(caseInstance.Suspended);
		assertTrue(stageX.Suspended);
		assertTrue(taskA.Suspended);
		assertTrue(taskB.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendStage()
	  public virtual void testSuspendStage()
	  {

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).createActivity("X").behavior(new StageActivityBehavior()).createActivity("A").behavior(new TaskWaitState()).endActivity().createActivity("B").behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when

		stageX.suspend();

		// then
		assertTrue(caseInstance.Active);
		assertTrue(stageX.Suspended);
		assertTrue(taskA.Suspended);
		assertTrue(taskB.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendTask()
	  public virtual void testSuspendTask()
	  {

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).createActivity("X").behavior(new StageActivityBehavior()).createActivity("A").behavior(new TaskWaitState()).endActivity().createActivity("B").behavior(new TaskWaitState()).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when

		taskA.suspend();

		// then
		assertTrue(caseInstance.Active);
		assertTrue(stageX.Active);
		assertTrue(taskA.Suspended);
		assertTrue(taskB.Enabled);
	  }

	}

}