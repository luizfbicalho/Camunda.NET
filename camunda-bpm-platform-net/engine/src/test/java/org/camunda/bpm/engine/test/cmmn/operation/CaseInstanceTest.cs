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

	using StageActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.StageActivityBehavior;
	using CaseExecutionImpl = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionImpl;
	using CmmnCaseInstance = org.camunda.bpm.engine.impl.cmmn.execution.CmmnCaseInstance;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using ItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler;
	using CaseDefinitionBuilder = org.camunda.bpm.engine.impl.cmmn.model.CaseDefinitionBuilder;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using PvmTestCase = org.camunda.bpm.engine.impl.test.PvmTestCase;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceTest : PvmTestCase
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
//ORIGINAL LINE: @Test public void testCaseInstanceWithOneTask()
	  public virtual void testCaseInstanceWithOneTask()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("create", stateTransitionCollector).createActivity("A").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().buildCaseDefinition();

		// create a new case instance
		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// expected state transitions after creation of a case instance:
		// ()        --create(Case1)--> active
		// ()        --create(A)-->     available
		// available --enable(A)-->     enabled
		IList<string> expectedStateTransitions = new List<string>();
		expectedStateTransitions.Add("() --create(Case1)--> active");
		expectedStateTransitions.Add("() --create(A)--> available");
		expectedStateTransitions.Add("available --enable(A)--> enabled");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// case instance is active
		assertTrue(caseInstance.Active);

		CaseExecutionImpl instance = (CaseExecutionImpl) caseInstance;

		// case instance has one child plan item
		IList<CaseExecutionImpl> childPlanItems = instance.CaseExecutions;
		assertEquals(1, childPlanItems.Count);

		CaseExecutionImpl planItemA = childPlanItems[0];

		// the child plan item is enabled
		assertTrue(planItemA.Enabled);

		// the parent of the child plan item is the case instance
		assertEquals(caseInstance, planItemA.getParent());

		// manual start of A
		planItemA.manualStart();

		// expected state transition after manual start of A:
		// enabled --enable(A)--> active
		expectedStateTransitions.Add("enabled --manualStart(A)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		assertTrue(planItemA.Active);
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
//ORIGINAL LINE: @Test public void testCaseInstanceWithOneState()
	  public virtual void testCaseInstanceWithOneState()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("create", stateTransitionCollector).createActivity("X").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new StageActivityBehavior()).createActivity("A").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// expected state transitions after the creation of a case instance:
		// ()        --create(Case1)--> active
		// ()        --create(X)-->     available
		// available --enable(X)-->     enabled
		IList<string> expectedStateTransitions = initAndAssertExpectedTransitions(stateTransitionCollector);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		CaseExecutionImpl planItemX = assertCaseXState(caseInstance);
		IList<CaseExecutionImpl> childPlanItems;


		// manual start of x
		planItemX.manualStart();

		// X should be active
		assertTrue(planItemX.Active);

		// expected state transitions after a manual start of X:
		// enabled   --manualStart(X)--> active
		// ()        --create(A)-->      available
		// available --enable(A)-->      enabled
		// ()        --create(B)-->      available
		// available --enable(B)-->      enabled
		expectedStateTransitions.Add("enabled --manualStart(X)--> active");
		expectedStateTransitions.Add("() --create(A)--> available");
		expectedStateTransitions.Add("available --enable(A)--> enabled");
		expectedStateTransitions.Add("() --create(B)--> available");
		expectedStateTransitions.Add("available --enable(B)--> enabled");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// X should have two chil plan items
		childPlanItems = planItemX.CaseExecutions;
		assertEquals(2, childPlanItems.Count);

		foreach (CmmnExecution childPlanItem in childPlanItems)
		{
		  // both children should be enabled
		  assertTrue(childPlanItem.Enabled);

		  // manual start of a child
		  childPlanItem.manualStart();

		  // the child should be active
		  assertTrue(childPlanItem.Active);

		  // X should be the parent of both children
		  assertEquals(planItemX, childPlanItem.Parent);
		}

		// expected state transitions after the manual starts of A and B:
		// enabled   --manualStart(A)--> active
		// enabled   --manualStart(B)--> active
		expectedStateTransitions.Add("enabled --manualStart(A)--> active");
		expectedStateTransitions.Add("enabled --manualStart(B)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

	  }

	  protected internal virtual CaseExecutionImpl assertCaseXState(CmmnCaseInstance caseInstance)
	  {

		// case instance is active
		assertTrue(caseInstance.Active);

		CaseExecutionImpl instance = (CaseExecutionImpl) caseInstance;

		// case instance has one child plan item
		IList<CaseExecutionImpl> childPlanItems = instance.CaseExecutions;
		assertEquals(1, childPlanItems.Count);

		CaseExecutionImpl planItemX = childPlanItems[0];

		// the case instance should be the parent of X
		assertEquals(caseInstance, planItemX.getParent());

		// X should be enabled
		assertTrue(planItemX.Enabled);

		// before activation (ie. manual start) X should not have any children
		assertTrue(planItemX.CaseExecutions.Count == 0);
		return planItemX;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceWithOneStateWithoutManualStartOfChildren()
	  public virtual void testCaseInstanceWithOneStateWithoutManualStartOfChildren()
	  {
		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("create", stateTransitionCollector).createActivity("X").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new StageActivityBehavior()).createActivity("A").listener("create", stateTransitionCollector).listener("start", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().createActivity("B").listener("create", stateTransitionCollector).listener("start", stateTransitionCollector).behavior(new TaskWaitState()).endActivity().endActivity().buildCaseDefinition();

		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();
		IList<string> expectedStateTransitions = initAndAssertExpectedTransitions(stateTransitionCollector);
		emptyCollector(stateTransitionCollector, expectedStateTransitions);


		// clear lists
		CaseExecutionImpl planItemX = assertCaseXState(caseInstance);

		// manual start of x
		planItemX.manualStart();

		// X should be active
		assertTrue(planItemX.Active);

		// expected state transitions after a manual start of X:
		expectedStateTransitions.Add("enabled --manualStart(X)--> active");
		expectedStateTransitions.Add("() --create(A)--> available");
		expectedStateTransitions.Add("available --start(A)--> active");
		expectedStateTransitions.Add("() --create(B)--> available");
		expectedStateTransitions.Add("available --start(B)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// X should have two chil plan items
		IList<CaseExecutionImpl> childPlanItems;
		childPlanItems = planItemX.CaseExecutions;
		assertEquals(2, childPlanItems.Count);

		foreach (CmmnExecution childPlanItem in childPlanItems)
		{
		  // both children should be active
		  assertTrue(childPlanItem.Active);

		  // X should be the parent of both children
		  assertEquals(planItemX, childPlanItem.Parent);
		}
	  }

	  protected internal virtual void emptyCollector(CaseExecutionStateTransitionCollector stateTransitionCollector, IList<string> expectedStateTransitions)
	  {
		// clear lists
		expectedStateTransitions.Clear();
		stateTransitionCollector.stateTransitions.Clear();
	  }

	  protected internal virtual IList<string> initAndAssertExpectedTransitions(CaseExecutionStateTransitionCollector stateTransitionCollector)
	  {
		// expected state transitions after the creation of a case instance:
		// ()        --create(Case1)--> active
		// ()        --create(X)-->     available
		// available --enable(X)-->     enabled
		IList<string> expectedStateTransitions = new List<string>();
		expectedStateTransitions.Add("() --create(Case1)--> active");
		expectedStateTransitions.Add("() --create(X)--> available");
		expectedStateTransitions.Add("available --enable(X)--> enabled");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);
		return expectedStateTransitions;
	  }


	  /// 
	  /// <summary>
	  ///   +-----------------+
	  ///   | Case1            \
	  ///   +-------------------+-------------------+
	  ///   |                                       |
	  ///   |  +-------+                            |
	  ///   |  |  A1   |                            |
	  ///   |  +-------+                            |
	  ///   |                                       |
	  ///   |    +------------------------+         |
	  ///   |   / X1                       \        |
	  ///   |  +    +-------+  +-------+    +       |
	  ///   |  |    |  A2   |  |  B1   |    |       |
	  ///   |  +    +-------+  +-------+    +       |
	  ///   |   \                          /        |
	  ///   |    +------------------------+         |
	  ///   |                                       |
	  ///   |    +-----------------------------+    |
	  ///   |   / Y                             \   |
	  ///   |  +    +-------+                    +  |
	  ///   |  |    |   C   |                    |  |
	  ///   |  |    +-------+                    |  |
	  ///   |  |                                 |  |
	  ///   |  |   +------------------------+    |  |
	  ///   |  |  / X2                       \   |  |
	  ///   |  | +    +-------+  +-------+    +  |  |
	  ///   |  | |    |  A3   |  |  B2   |    |  |  |
	  ///   |  | +    +-------+  +-------+    +  |  |
	  ///   |  |  \                          /   |  |
	  ///   |  +   +------------------------+    +  |
	  ///   |   \                               /   |
	  ///   |    +-----------------------------+    |
	  ///   |                                       |
	  ///   +---------------------------------------+
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartComplexCaseInstance()
	  public virtual void testStartComplexCaseInstance()
	  {

		CaseExecutionStateTransitionCollector stateTransitionCollector = new CaseExecutionStateTransitionCollector();

		CmmnCaseDefinition caseDefinition = (new CaseDefinitionBuilder("Case1")).listener("create", stateTransitionCollector).createActivity("A1").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().createActivity("X1").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new StageActivityBehavior()).createActivity("A2").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().createActivity("B1").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().endActivity().createActivity("Y").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new StageActivityBehavior()).createActivity("C").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().createActivity("X2").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new StageActivityBehavior()).createActivity("A3").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().createActivity("B2").listener("create", stateTransitionCollector).listener("enable", stateTransitionCollector).listener("manualStart", stateTransitionCollector).property(ItemHandler.PROPERTY_MANUAL_ACTIVATION_RULE, defaultManualActivation()).behavior(new TaskWaitState()).endActivity().endActivity().endActivity().buildCaseDefinition();

		CmmnCaseInstance caseInstance = caseDefinition.createCaseInstance();
		caseInstance.create();

		// expected state transitions after the creation of a case instance:
		// ()        --create(Case1)--> active
		// ()        --create(A1)-->    available
		// available --enable(A1)-->    enabled
		// ()        --create(X1)-->    available
		// available --enable(X1)-->    enabled
		// ()        --create(Y)-->     available
		// available --enable(Y)-->     enabled
		IList<string> expectedStateTransitions = new List<string>();
		expectedStateTransitions.Add("() --create(Case1)--> active");
		expectedStateTransitions.Add("() --create(A1)--> available");
		expectedStateTransitions.Add("available --enable(A1)--> enabled");
		expectedStateTransitions.Add("() --create(X1)--> available");
		expectedStateTransitions.Add("available --enable(X1)--> enabled");
		expectedStateTransitions.Add("() --create(Y)--> available");
		expectedStateTransitions.Add("available --enable(Y)--> enabled");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		CaseExecutionImpl instance = (CaseExecutionImpl) caseInstance;

		// the case instance should be active
		assertTrue(instance.Active);

		// the case instance should have three child plan items (A1, X1, Y)
		IList<CaseExecutionImpl> childPlanItems = instance.CaseExecutions;
		assertEquals(3, childPlanItems.Count);

		// handle plan item A1 //////////////////////////////////////////////////

		CaseExecutionImpl planItemA1 = (CaseExecutionImpl) instance.findCaseExecution("A1");

		// case instance should be the parent of A1
		assertEquals(caseInstance, planItemA1.getParent());

		// A1 should be enabled
		assertTrue(planItemA1.Enabled);

		// manual start of A1
		planItemA1.manualStart();

		// A1 should be active
		assertTrue(planItemA1.Active);

		// expected state transitions:
		// enabled --manualStart(A1)--> active
		expectedStateTransitions.Add("enabled --manualStart(A1)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item X1 ///////////////////////////////////////////////////

		CaseExecutionImpl planItemX1 = (CaseExecutionImpl) instance.findCaseExecution("X1");

		// case instance should be the parent of X1
		assertEquals(caseInstance, planItemX1.getParent());

		// X1 should be enabled
		assertTrue(planItemX1.Enabled);

		// manual start of X1
		planItemX1.manualStart();

		// X1 should be active
		assertTrue(planItemX1.Active);

		// X1 should have two children
		childPlanItems = planItemX1.CaseExecutions;
		assertEquals(2, childPlanItems.Count);

		// expected state transitions after manual start of X1:
		// enabled   --manualStart(X1)--> active
		// ()        --create(A2)-->      available
		// available --enable(A2)-->      enabled
		// ()        --create(B1)-->      available
		// available --enable(B1)-->      enabled
		expectedStateTransitions.Add("enabled --manualStart(X1)--> active");
		expectedStateTransitions.Add("() --create(A2)--> available");
		expectedStateTransitions.Add("available --enable(A2)--> enabled");
		expectedStateTransitions.Add("() --create(B1)--> available");
		expectedStateTransitions.Add("available --enable(B1)--> enabled");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item A2 ////////////////////////////////////////////////

		CaseExecutionImpl planItemA2 = (CaseExecutionImpl) instance.findCaseExecution("A2");

		// X1 should be the parent of A2
		assertEquals(planItemX1, planItemA2.getParent());

		// A2 should be enabled
		assertTrue(planItemA2.Enabled);

		// manual start of A2
		planItemA2.manualStart();

		// A2 should be active
		assertTrue(planItemA2.Active);

		// expected state transition after manual start of A2:
		// enabled --manualStart(A2)--> active
		expectedStateTransitions.Add("enabled --manualStart(A2)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item B1 /////////////////////////////////////////////////

		CaseExecutionImpl planItemB1 = (CaseExecutionImpl) instance.findCaseExecution("B1");

		// X1 should be the parent of B1
		assertEquals(planItemX1, planItemB1.getParent());

		// B1 should be enabled
		assertTrue(planItemB1.Enabled);

		// manual start of B1
		planItemB1.manualStart();

		// B1 should be active
		assertTrue(planItemB1.Active);

		// expected state transition after manual start of B1:
		// enabled --manualStart(B1)--> active
		expectedStateTransitions.Add("enabled --manualStart(B1)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item Y ////////////////////////////////////////////////

		CaseExecutionImpl planItemY = (CaseExecutionImpl) instance.findCaseExecution("Y");

		// case instance should be the parent of Y
		assertEquals(caseInstance, planItemY.getParent());

		// Y should be enabled
		assertTrue(planItemY.Enabled);

		// manual start of Y
		planItemY.manualStart();

		// Y should be active
		assertTrue(planItemY.Active);

		// Y should have two children
		childPlanItems = planItemY.CaseExecutions;
		assertEquals(2, childPlanItems.Count);

		// expected state transitions after manual start of Y:
		// enabled   --manualStart(Y)--> active
		// ()        --create(C)-->      available
		// available --enable(C)-->      enabled
		// ()        --create(X2)-->      available
		// available --enable(X2)-->      enabled
		expectedStateTransitions.Add("enabled --manualStart(Y)--> active");
		expectedStateTransitions.Add("() --create(C)--> available");
		expectedStateTransitions.Add("available --enable(C)--> enabled");
		expectedStateTransitions.Add("() --create(X2)--> available");
		expectedStateTransitions.Add("available --enable(X2)--> enabled");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item C //////////////////////////////////////////////////

		CaseExecutionImpl planItemC = (CaseExecutionImpl) instance.findCaseExecution("C");

		// Y should be the parent of C
		assertEquals(planItemY, planItemC.getParent());

		// C should be enabled
		assertTrue(planItemC.Enabled);

		// manual start of C
		planItemC.manualStart();

		// C should be active
		assertTrue(planItemC.Active);

		// expected state transition after manual start of C:
		// enabled --manualStart(C)--> active
		expectedStateTransitions.Add("enabled --manualStart(C)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item X2 ///////////////////////////////////////////

		CaseExecutionImpl planItemX2 = (CaseExecutionImpl) instance.findCaseExecution("X2");

		// Y should be the parent of X2
		assertEquals(planItemY, planItemX2.getParent());

		// X2 should be enabled
		assertTrue(planItemX2.Enabled);

		// manual start of X2
		planItemX2.manualStart();

		// X2 should be active
		assertTrue(planItemX2.Active);

		// X2 should have two children
		childPlanItems = planItemX2.CaseExecutions;
		assertEquals(2, childPlanItems.Count);

		// expected state transitions after manual start of X2:
		// enabled   --manualStart(X2)--> active
		// ()        --create(A3)-->      available
		// available --enable(A3)-->      enabled
		// ()        --create(B2)-->      available
		// available --enable(B2)-->      enabled
		expectedStateTransitions.Add("enabled --manualStart(X2)--> active");
		expectedStateTransitions.Add("() --create(A3)--> available");
		expectedStateTransitions.Add("available --enable(A3)--> enabled");
		expectedStateTransitions.Add("() --create(B2)--> available");
		expectedStateTransitions.Add("available --enable(B2)--> enabled");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item A3 //////////////////////////////////////////////

		CaseExecutionImpl planItemA3 = (CaseExecutionImpl) instance.findCaseExecution("A3");

		// A3 should be the parent of X2
		assertEquals(planItemX2, planItemA3.getParent());

		// A3 should be enabled
		assertTrue(planItemA3.Enabled);

		// manual start of A3
		planItemA3.manualStart();

		// A3 should be active
		assertTrue(planItemA3.Active);

		// expected state transition after manual start of A3:
		// enabled --manualStart(A3)--> active
		expectedStateTransitions.Add("enabled --manualStart(A3)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

		// handle plan item B2 /////////////////////////////////////////////////

		CaseExecutionImpl planItemB2 = (CaseExecutionImpl) instance.findCaseExecution("B2");

		// B2 should be the parent of X2
		assertEquals(planItemX2, planItemB2.getParent());

		// B2 should be enabled
		assertTrue(planItemB2.Enabled);

		// manual start of B2
		planItemB2.manualStart();

		// B2 should be active
		assertTrue(planItemB2.Active);

		// expected state transition after manual start of B2:
		// enabled --manualStart(B2)--> active
		expectedStateTransitions.Add("enabled --manualStart(B2)--> active");

		assertEquals(expectedStateTransitions, stateTransitionCollector.stateTransitions);

		// clear lists
		emptyCollector(stateTransitionCollector, expectedStateTransitions);

	  }

	}

}