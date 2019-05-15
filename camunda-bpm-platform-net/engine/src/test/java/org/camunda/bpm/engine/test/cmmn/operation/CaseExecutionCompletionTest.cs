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
namespace org.camunda.bpm.engine.test.cmmn.operation
{
	using CaseIllegalStateTransitionException = org.camunda.bpm.engine.exception.cmmn.CaseIllegalStateTransitionException;
	using StageActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.StageActivityBehavior;
	using CaseExecutionImpl = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionImpl;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnCaseInstance = org.camunda.bpm.engine.impl.cmmn.execution.CmmnCaseInstance;
	using ItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler;
	using CaseDefinitionBuilder = org.camunda.bpm.engine.impl.cmmn.model.CaseDefinitionBuilder;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseExecutionCompletionTest : PvmTestCase
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
	  public virtual void testCompleteActiveTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// when

		// completing task A
		taskA.complete();

		// then
		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// active --complete(A)--> completed
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(A)--> completed");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// task A is completed ...
		assertTrue(taskA.Completed);
		// ... and the case instance is also completed
		assertTrue(caseInstance.Completed);

		// task A is not part of the case instance anymore
		assertNull(caseInstance.findCaseExecution("A"));
		// the case instance has no children
		assertTrue(((CaseExecutionImpl) caseInstance).CaseExecutions.Count == 0);
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
	  public virtual void testManualCompleteActiveTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// when

		// completing task A
		taskA.manualComplete();

		// then
		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// active --complete(A)--> completed
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(A)--> completed");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// task A is completed ...
		assertTrue(taskA.Completed);
		// ... and the case instance is also completed
		assertTrue(caseInstance.Completed);

		// task A is not part of the case instance anymore
		assertNull(caseInstance.findCaseExecution("A"));
		// the case instance has no children
		assertTrue(((CaseExecutionImpl) caseInstance).CaseExecutions.Count == 0);
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
	  public virtual void testCompleteEnabledTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is enabled
		assertTrue(taskA.Enabled);

		try
		{
		  // when
		  // completing task A
		  taskA.complete();
		  fail("It should not be possible to complete an enabled task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // task A is still enabled
		  assertTrue(taskA.Enabled);
		}

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
	  public virtual void testManualCompleteEnabledTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is enabled
		assertTrue(taskA.Enabled);

		try
		{
		  // when
		  // completing task A
		  taskA.manualComplete();
		  fail("It should not be possible to complete an enabled task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // task A is still enabled
		  assertTrue(taskA.Enabled);
		}

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
	  public virtual void testCompleteAlreadyCompletedTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		taskA.complete();

		// task A is completed
		assertTrue(taskA.Completed);

		try
		{
		  // when
		  // complete A
		  taskA.complete();
		  fail("It should not be possible to complete an already completed task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // task A is still completed
		  assertTrue(taskA.Completed);
		}

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
	  public virtual void testManualCompleteAlreadyCompletedTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		taskA.complete();

		// task A is completed
		assertTrue(taskA.Completed);

		try
		{
		  // when
		  // complete A
		  taskA.manualComplete();
		  fail("It should not be possible to complete an already completed task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // task A is still completed
		  assertTrue(taskA.Completed);
		}

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
	  public virtual void testCompleteTerminatedTask()
	  {
		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		taskA.terminate();

		// task A is completed
		assertTrue(taskA.Terminated);

		try
		{
		  // when
		  // complete A
		  taskA.complete();
		  fail("It should not be possible to complete an already completed task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // task A is still completed
		  assertTrue(taskA.Terminated);
		}
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
	  public virtual void testManualCompleteTerminatedTask()
	  {
		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");
		taskA.terminate();

		// task A is completed
		assertTrue(taskA.Terminated);

		try
		{
		  // when
		  // complete A
		  taskA.manualComplete();
		  fail("It should not be possible to complete an already completed task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // task A is still completed
		  assertTrue(taskA.Terminated);
		}
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
	  public virtual void testDisableTaskShouldCompleteCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("disable", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is enabled
		assertTrue(taskA.Enabled);

		// when
		// complete A
		taskA.disable();

		// then

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// enabled --disable(A)-->      disabled
		// active  --complete(Case1)--> completed
		expectedStateTransitions.Add("enabled --disable(A)--> disabled");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// task A is disabled
		assertTrue(taskA.Disabled);

		// case instance is completed
		assertTrue(caseInstance.Completed);

		assertNull(caseInstance.findCaseExecution("A"));
		assertTrue(((CaseExecutionImpl)caseInstance).CaseExecutions.Count == 0);

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
	  public virtual void testTerminateTaskShouldCompleteCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("terminate", stateTransitionCollector).listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is active
		assertTrue(taskA.Active);

		// when
		// terminate A
		taskA.terminate();

		// then

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// active --terminate(A)-->    terminated
		// active  --complete(Case1)--> completed
		expectedStateTransitions.Add("active --terminate(A)--> terminated");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// task A is disabled
		assertTrue(taskA.Terminated);

		// case instance is completed
		assertTrue(caseInstance.Completed);

		assertNull(caseInstance.findCaseExecution("A"));
		assertTrue(((CaseExecutionImpl)caseInstance).CaseExecutions.Count == 0);

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
	  public virtual void testCompleteActiveCaseInstanceWithEnabledTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is enabled
		assertTrue(taskA.Enabled);

		try
		{
		  // when
		  // complete caseInstance
		  caseInstance.complete();
		}
		catch (Exception)
		{
		  // then
		  // case instance is still active
		  assertTrue(caseInstance.Active);

		  assertNotNull(caseInstance.findCaseExecution("A"));
		}
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
	  public virtual void testManualCompleteActiveCaseInstanceWithEnabledTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is enabled
		assertTrue(taskA.Enabled);

		// when

		// complete caseInstance (manualCompletion == true)
		caseInstance.manualComplete();

		// then

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// the case instance
		assertTrue(caseInstance.Completed);

		// task A is not a child of the case instance anymore
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
	  public virtual void testCompleteActiveCaseInstanceWithActiveTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is active
		assertTrue(taskA.Active);

		try
		{
		  // when
		  caseInstance.complete();
		  fail("It should not be possible to complete a case instance containing an active task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // the case instance is still active
		  assertTrue(caseInstance.Active);
		  assertFalse(caseInstance.Completed);
		}
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
	  public virtual void testManualCompleteActiveCaseInstanceWithActiveTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		try
		{
		  // when
		  caseInstance.manualComplete();
		  fail("It should not be possible to complete a case instance containing an active task.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // the case instance is still active
		  assertTrue(caseInstance.Active);
		  assertFalse(caseInstance.Completed);
		}
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
	  public virtual void testCompleteAlreadyCompletedCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is enabled
		assertTrue(taskA.Enabled);

		// case instance is already completed
		caseInstance.manualComplete();

		try
		{
		  // when
		  caseInstance.complete();
		  fail("It should not be possible to complete an already completed case instance.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  // the case instance is still completed
		  assertTrue(caseInstance.Completed);
		}

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
	  public virtual void testManualCompleteAlreadyCompletedCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// task A as a child of the case instance
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// task A is enabled
		assertTrue(taskA.Enabled);

		// case instance is already completed
		caseInstance.manualComplete();

		try
		{
		  // when
		  caseInstance.manualComplete();
		  fail("It should not be possible to complete an already completed case instance.");
		}
		catch (CaseIllegalStateTransitionException)
		{
		  // then

		  assertThat("the case instance is still completed",caseInstance.Completed,@is(true));
		}

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
	  public virtual void testCompleteOnlyTaskA()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// complete task A
		taskA.complete();

		// then ////////////////////////////////////////////////////////////////

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transitions:
		// active --complete(A)--> completed
		expectedStateTransitions.Add("active --complete(A)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		expectedStateTransitions.Clear();
		stateTransitionCollector.stateTransitions.Clear();

		// task A is completed
		assertTrue(taskA.Completed);

		// task B is still active
		assertTrue(taskB.Active);

		// stage X is still active
		assertTrue(stageX.Active);

		// stage X does not contain task A anymore
		assertNull(caseInstance.findCaseExecution("A"));

		// task B is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("B"));

		// stage X is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("X"));

		// case instance has only one child
		assertEquals(1, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// stage X has two children
		assertEquals(1, ((CaseExecutionImpl) stageX).CaseExecutions.Count);

		// case instance is still active
		assertTrue(caseInstance.Active);

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
	  public virtual void testManualCompleteOnlyTaskA()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// complete task A
		taskA.manualComplete();

		// then ////////////////////////////////////////////////////////////////

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transitions:
		// active --complete(A)--> completed
		expectedStateTransitions.Add("active --complete(A)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		expectedStateTransitions.Clear();
		stateTransitionCollector.stateTransitions.Clear();

		// task A is completed
		assertTrue(taskA.Completed);

		// task B is still active
		assertTrue(taskB.Active);

		// stage X is still active
		assertTrue(stageX.Active);

		// stage X does not contain task A anymore
		assertNull(caseInstance.findCaseExecution("A"));

		// task B is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("B"));

		// stage X is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("X"));

		// case instance has only one child
		assertEquals(1, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// stage X has two children
		assertEquals(1, ((CaseExecutionImpl) stageX).CaseExecutions.Count);

		// case instance is still active
		assertTrue(caseInstance.Active);

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
	  public virtual void testDisableOnlyTaskA()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// disable task A
		taskA.disable();

		// then ////////////////////////////////////////////////////////////////

		assertTrue(stateTransitionCollector.stateTransitions.Count == 0);

		// task A is disabled
		assertTrue(taskA.Disabled);

		// task B is still active
		assertTrue(taskB.Active);

		// stage X is still active
		assertTrue(stageX.Active);

		// task B is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("A"));

		// task B is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("B"));

		// stage X is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("X"));

		// case instance has only one child
		assertEquals(1, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// stage X has only one child
		assertEquals(2, ((CaseExecutionImpl) stageX).CaseExecutions.Count);

		// case instance is still active
		assertTrue(caseInstance.Active);

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
	  public virtual void testTerminateOnlyTaskA()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// complete task A
		taskA.terminate();

		// then ////////////////////////////////////////////////////////////////

		assertTrue(stateTransitionCollector.stateTransitions.Count == 0);

		// task A is terminated
		assertTrue(taskA.Terminated);

		// task B is still active
		assertTrue(taskB.Active);

		// stage X is still active
		assertTrue(stageX.Active);

		// stage X does not contain task A anymore
		assertNull(caseInstance.findCaseExecution("A"));

		// task B is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("B"));

		// stage X is still part of the case instance
		assertNotNull(caseInstance.findCaseExecution("X"));

		// case instance has only one child
		assertEquals(1, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// stage X has only one child
		assertEquals(1, ((CaseExecutionImpl) stageX).CaseExecutions.Count);

		// case instance is still active
		assertTrue(caseInstance.Active);

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
	  public virtual void testCompleteTaskAAndTaskB()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// complete task A
		taskA.complete();
		// complete task B
		taskB.complete();

		// then ////////////////////////////////////////////////////////////////

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transitions:
		// active --complete(A)-->     completed
		// active --complete(B)-->     completed
		// active --complete(X)-->     completed
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(A)--> completed");
		expectedStateTransitions.Add("active --complete(B)--> completed");
		expectedStateTransitions.Add("active --complete(X)--> completed");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		expectedStateTransitions.Clear();
		stateTransitionCollector.stateTransitions.Clear();

		// task A is completed
		assertTrue(taskA.Completed);

		// task B is completed
		assertTrue(taskB.Completed);

		// stage X is completed
		assertTrue(stageX.Completed);

		// stage X does not contain task A anymore
		assertNull(caseInstance.findCaseExecution("A"));
		// stage X does not contain task B anymore
		assertNull(caseInstance.findCaseExecution("B"));
		// stage X does not contain task X anymore
		assertNull(caseInstance.findCaseExecution("X"));

		// stage X has only one child
		assertEquals(0, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// case instance is completed
		assertTrue(caseInstance.Completed);

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
	  public virtual void testManualCompleteTaskAAndTaskB()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// complete task A
		taskA.manualComplete();
		// complete task B
		taskB.manualComplete();

		// then ////////////////////////////////////////////////////////////////

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transitions:
		// active --complete(A)-->     completed
		// active --complete(B)-->     completed
		// active --complete(X)-->     completed
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(A)--> completed");
		expectedStateTransitions.Add("active --complete(B)--> completed");
		expectedStateTransitions.Add("active --complete(X)--> completed");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		expectedStateTransitions.Clear();
		stateTransitionCollector.stateTransitions.Clear();

		// task A is completed
		assertTrue(taskA.Completed);

		// task B is completed
		assertTrue(taskB.Completed);

		// stage X is completed
		assertTrue(stageX.Completed);

		// stage X does not contain task A anymore
		assertNull(caseInstance.findCaseExecution("A"));
		// stage X does not contain task B anymore
		assertNull(caseInstance.findCaseExecution("B"));
		// stage X does not contain task X anymore
		assertNull(caseInstance.findCaseExecution("X"));

		// stage X has only one child
		assertEquals(0, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// case instance is completed
		assertTrue(caseInstance.Completed);

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
	  public virtual void testDisableTaskAAndTaskB()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// disable task A
		taskA.disable();
		// disable task B
		taskB.disable();

		// then ////////////////////////////////////////////////////////////////

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transitions:
		// active --complete(X)-->     completed
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(X)--> completed");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		expectedStateTransitions.Clear();
		stateTransitionCollector.stateTransitions.Clear();

		// task A is disabled
		assertTrue(taskA.Disabled);

		// task B is disabled
		assertTrue(taskB.Disabled);

		// stage X is completed
		assertTrue(stageX.Completed);

		// stage X does not contain task A anymore
		assertNull(caseInstance.findCaseExecution("A"));
		// stage X does not contain task B anymore
		assertNull(caseInstance.findCaseExecution("B"));
		// stage X does not contain task X anymore
		assertNull(caseInstance.findCaseExecution("X"));

		// stage X has only one child
		assertEquals(0, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// case instance is completed
		assertTrue(caseInstance.Completed);

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
	  public virtual void testTerminateTaskAAndTaskB()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given ///////////////////////////////////////////////////////////////

		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).behavior(new StageActivityBehavior()).createActivity("A").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("complete", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		// an active case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// a case execution associated with Stage X
		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// a case execution associated with Task A
		CmmnActivityExecution taskA = caseInstance.findCaseExecution("A");

		// a case execution associated with Task B
		CmmnActivityExecution taskB = caseInstance.findCaseExecution("B");

		// when ////////////////////////////////////////////////////////////////

		// terminate task A
		taskA.terminate();
		// terminate task B
		taskB.terminate();

		// then ////////////////////////////////////////////////////////////////

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transitions:
		// active --complete(X)-->     completed
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(X)--> completed");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		expectedStateTransitions.Clear();
		stateTransitionCollector.stateTransitions.Clear();

		// task A is terminated
		assertTrue(taskA.Terminated);

		// task B is terminated
		assertTrue(taskB.Terminated);

		// stage X is completed
		assertTrue(stageX.Completed);

		// stage X does not contain task A anymore
		assertNull(caseInstance.findCaseExecution("A"));
		// stage X does not contain task B anymore
		assertNull(caseInstance.findCaseExecution("B"));
		// stage X does not contain task X anymore
		assertNull(caseInstance.findCaseExecution("X"));

		// stage X has only one child
		assertEquals(0, ((CaseExecutionImpl) caseInstance).CaseExecutions.Count);

		// case instance is completed
		assertTrue(caseInstance.Completed);

	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+---+
	  ///   |                       |
	  ///   |                       |
	  ///   |                       |
	  ///   |                       |
	  ///   |                       |
	  ///   +-----------------------+
	  /// 
	  /// </summary>
	  public virtual void testAutoCompletionCaseInstanceWithoutChildren()
	  {
		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).buildCaseDefinition();

		// when

		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// then

		assertTrue(caseInstance.Completed);

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);
	  }

	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+-----------------+
	  ///   |                                     |
	  ///   |     +------------------------+      |
	  ///   |    / X                        \     |
	  ///   |   +                            +    |
	  ///   |   |                            |    |
	  ///   |   +                            +    |
	  ///   |    \                          /     |
	  ///   |     +------------------------+      |
	  ///   |                                     |
	  ///   +-------------------------------------+
	  /// 
	  /// </summary>
	  public virtual void testAutoCompletionStageWithoutChildren()
	  {
		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		// given
		// a case definition
		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("complete", stateTransitionCollector).createActivity("X").listener("complete", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new StageActivityBehavior()).endActivity().buildCaseDefinition();

		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();


		CmmnActivityExecution stageX = caseInstance.findCaseExecution("X");

		// when
		stageX.manualStart();

		// then

		assertTrue(caseInstance.Completed);
		assertTrue(stageX.Completed);

		IList<string> expectedStateTransitions = new List<string>();

		// expected state transition:
		// active --complete(X)-->     completed
		// active --complete(Case1)--> completed
		expectedStateTransitions.Add("active --complete(X)--> completed");
		expectedStateTransitions.Add("active --complete(Case1)--> completed");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);
	  }

	}

}