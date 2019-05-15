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
namespace org.camunda.bpm.engine.test.cmmn.operation
{

	using CaseIllegalStateTransitionException = org.camunda.bpm.engine.exception.cmmn.CaseIllegalStateTransitionException;
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
	public class CaseInstanceCloseTest : PvmTestCase
	{

	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+---+
	  ///   |                       |
	  ///   |     +-------+         |
	  ///   |     |   A   |         |
	  ///   |     +-------+         |
	  ///   |                       |
	  ///   +-----------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCloseCompletedCaseInstance()
	  public virtual void testCloseCompletedCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("close", stateTransitionCollector).createActivity("A").behavior(new TaskWaitState()).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// disable task A -> completes case instance
		taskA.disable();

		assertTrue(caseInstance.Completed);

		// when

		// close case
		caseInstance.close();

		// then
		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// completed --close(Case1)--> closed
		expectedStateTransitions.Add("completed --close(Case1)--> closed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		assertTrue(caseInstance.Closed);
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+---+
	  ///   |                       |
	  ///   |     +-------+         |
	  ///   |     |   A   |         |
	  ///   |     +-------+         |
	  ///   |                       |
	  ///   +-----------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCloseTerminatedCaseInstance()
	  public virtual void testCloseTerminatedCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("close", stateTransitionCollector).createActivity("A").behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		caseInstance.terminate();
		assertTrue(caseInstance.Terminated);

		// when

		// close case
		caseInstance.close();

		// then
		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// terminated --close(Case1)--> closed
		expectedStateTransitions.Add("terminated --close(Case1)--> closed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		assertTrue(caseInstance.Closed);
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+---+
	  ///   |                       |
	  ///   |     +-------+         |
	  ///   |     |   A   |         |
	  ///   |     +-------+         |
	  ///   |                       |
	  ///   +-----------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCloseSuspendedCaseInstance()
	  public virtual void testCloseSuspendedCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("close", stateTransitionCollector).createActivity("A").behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		caseInstance.suspend();
		assertTrue(caseInstance.Suspended);

		// when

		// close case
		caseInstance.close();

		// then
		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// suspended --close(Case1)--> closed
		expectedStateTransitions.Add("suspended --close(Case1)--> closed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		assertTrue(caseInstance.Closed);

		assertNull(caseInstance.findCaseExecution("A"));
	  }


	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+---+
	  ///   |                       |
	  ///   |     +-------+         |
	  ///   |     |   A   |         |
	  ///   |     +-------+         |
	  ///   |                       |
	  ///   +-----------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCloseActiveCaseInstance()
	  public virtual void testCloseActiveCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("close", stateTransitionCollector).createActivity("A").behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		assertTrue(caseInstance.Active);

		try
		{
		  // when
		  caseInstance.close();
		}
		catch (CaseIllegalStateTransitionException)
		{

		}

		// then
		assertTrue(stateTransitionCollector.stateTransitions.Count == 0);

		assertTrue(caseInstance.Active);

		assertNotNull(caseInstance.findCaseExecution("A"));
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+---+
	  ///   |                       |
	  ///   |     +-------+         |
	  ///   |     |   A   |         |
	  ///   |     +-------+         |
	  ///   |                       |
	  ///   +-----------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCloseTask()
	  public virtual void testCloseTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("close", stateTransitionCollector).createActivity("A").behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		try
		{
		  // when
		  taskA.close();
		  fail("It should not be possible to close a task.");
		}
		catch (CaseIllegalStateTransitionException)
		{

		}

		// then
		assertTrue(stateTransitionCollector.stateTransitions.Count == 0);

		assertTrue(caseInstance.Active);
		assertNotNull(caseInstance.findCaseExecution("A"));
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+-----------------+
	  ///   |                                     |
	  ///   |     +------------------------+      |
	  ///   |    / X                        \     |
	  ///   |   +    +-------+  +-------+    +    |
	  ///   |   |    |   A   |  |   B   |    |    |
	  ///   |   +    +-------+  +-------+    +    |
	  ///   |    \                          /     |
	  ///   |     +------------------------+      |
	  ///   |                                     |
	  ///   +-------------------------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCloseStage()
	  public virtual void testCloseStage()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("close", stateTransitionCollector).createActivity("X").behavior(new StageActivityBehavior()).createActivity("A").behavior(new TaskWaitState()).endActivity().createActivity("B").behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		try
		{
		  // when
		  stageX.close();
		  fail("It should not be possible to close a stage.");
		}
		catch (CaseIllegalStateTransitionException)
		{

		}

		// then
		assertTrue(stateTransitionCollector.stateTransitions.Count == 0);

		assertTrue(caseInstance.Active);
		assertNotNull(caseInstance.findCaseExecution("X"));
	  }
	}

}