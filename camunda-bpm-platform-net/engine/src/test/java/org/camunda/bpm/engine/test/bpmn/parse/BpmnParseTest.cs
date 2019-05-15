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
namespace org.camunda.bpm.engine.test.bpmn.parse
{

	using BoundaryEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.BoundaryEventActivityBehavior;
	using CompensationEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.CompensationEventActivityBehavior;
	using EventSubProcessStartEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.EventSubProcessStartEventActivityBehavior;
	using NoneStartEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.NoneStartEventActivityBehavior;
	using ThrowEscalationEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ThrowEscalationEventActivityBehavior;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using BoundaryConditionalEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.BoundaryConditionalEventActivityBehavior;
	using EventSubProcessStartConditionalEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.EventSubProcessStartConditionalEventActivityBehavior;
	using IntermediateConditionalEventBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.IntermediateConditionalEventBehavior;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Test = org.junit.Test;

	/// 
	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class BpmnParseTest : PluggableProcessEngineTestCase
	{

	  public virtual void testInvalidSubProcessWithTimerStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithTimerStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a timer start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("timerEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidSubProcessWithMessageStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithMessageStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process definition could be parsed, although the sub process contains not a blanco start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("messageEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidSubProcessWithConditionalStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithConditionalStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a conditional start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("conditionalEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidSubProcessWithSignalStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithSignalStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a signal start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("signalEventDefintion only allowed on start event if subprocess is an event subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidSubProcessWithErrorStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithErrorStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a error start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("errorEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidSubProcessWithEscalationStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithEscalationStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a escalation start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("escalationEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidSubProcessWithCompensationStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithCompensationStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a compensation start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("compensateEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidTransactionWithMessageStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithMessageStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process definition could be parsed, although the sub process contains not a blanco start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("messageEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidTransactionWithTimerStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithTimerStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a timer start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("timerEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidTransactionWithConditionalStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithConditionalStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a conditional start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("conditionalEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidTransactionWithSignalStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithSignalStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a signal start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("signalEventDefintion only allowed on start event if subprocess is an event subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidTransactionWithErrorStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithErrorStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a error start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("errorEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidTransactionWithEscalationStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithEscalationStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a escalation start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("escalationEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidTransactionWithCompensationStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithCompensationStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process contains a compensation start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("compensateEventDefinition is not allowed on start event within a subprocess", e.Message);
		}
	  }

	  public virtual void testInvalidProcessDefinition()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidProcessDefinition");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("cvc-complex-type.3.2.2:", e.Message);
		  assertTextPresent("invalidAttribute", e.Message);
		  assertTextPresent("process", e.Message);
		}
	  }

	  public virtual void testExpressionParsingErrors()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testExpressionParsingErrors");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could not be parsed, the expression contains an escalation start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Error parsing '${currentUser()': syntax error at position 15, encountered 'null', expected '}'", e.Message);
		}
	  }

	  public virtual void testXmlParsingErrors()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testXMLParsingErrors");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could not be parsed, the XML contains an escalation start event.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("The end-tag for element type \"bpmndi:BPMNLabel\" must end with a '>' delimiter", e.Message);
		}
	  }

	  public virtual void testInvalidSequenceFlowInAndOutEventSubProcess()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidSequenceFlowInAndOutEventSubProcess");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, although the sub process has incoming and outgoing sequence flows");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Invalid incoming sequence flow of event subprocess", e.Message);
		  assertTextPresent("Invalid outgoing sequence flow of event subprocess", e.Message);
		}
	  }

	  /// <summary>
	  /// this test case check if the multiple start event is supported the test case
	  /// doesn't fail in this behavior because the <seealso cref="BpmnParse"/> parse the event
	  /// definitions with if-else, this means only the first event definition is
	  /// taken
	  /// 
	  /// </summary>
	  public virtual void testParseMultipleStartEvent()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseMultipleStartEvent");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  // fail in "regular" subprocess
		  assertTextPresent("timerEventDefinition is not allowed on start event within a subprocess", e.Message);
		  assertTextPresent("messageEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
		  // doesn't fail in event subprocess/process because the bpmn parser parse
		  // only this first event definition
		}
	  }

	  public virtual void testParseWithBpmnNamespacePrefix()
	  {
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/parse/BpmnParseTest.testParseWithBpmnNamespacePrefix.bpmn20.xml").deploy();
		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());

		repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
	  }

	  public virtual void testParseWithMultipleDocumentation()
	  {
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/parse/BpmnParseTest.testParseWithMultipleDocumentation.bpmn20.xml").deploy();
		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());

		repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
	  }

	  public virtual void testParseCollaborationPlane()
	  {
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/parse/BpmnParseTest.testParseCollaborationPlane.bpmn").deploy();
		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());

		repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
	  }

	  public virtual void testInvalidAsyncAfterEventBasedGateway()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testInvalidAsyncAfterEventBasedGateway");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  // fail on asyncAfter
		  assertTextPresent("'asyncAfter' not supported for", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseDiagramInterchangeElements()
	  public virtual void testParseDiagramInterchangeElements()
	  {

		// Graphical information is not yet exposed publicly, so we need to do some
		// plumbing
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		ProcessDefinitionEntity processDefinitionEntity = commandExecutor.execute(new CommandAnonymousInnerClass(this));

		assertNotNull(processDefinitionEntity);
		assertEquals(7, processDefinitionEntity.Activities.Count);

		// Check if diagram has been created based on Diagram Interchange when it's
		// not a headless instance
		IList<string> resourceNames = repositoryService.getDeploymentResourceNames(processDefinitionEntity.DeploymentId);
		if (processEngineConfiguration.CreateDiagramOnDeploy)
		{
		  assertEquals(2, resourceNames.Count);
		}
		else
		{
		  assertEquals(1, resourceNames.Count);
		}

		foreach (ActivityImpl activity in processDefinitionEntity.Activities)
		{

		  if (activity.Id.Equals("theStart"))
		  {
			assertActivityBounds(activity, 70, 255, 30, 30);
		  }
		  else if (activity.Id.Equals("task1"))
		  {
			assertActivityBounds(activity, 176, 230, 100, 80);
		  }
		  else if (activity.Id.Equals("gateway1"))
		  {
			assertActivityBounds(activity, 340, 250, 40, 40);
		  }
		  else if (activity.Id.Equals("task2"))
		  {
			assertActivityBounds(activity, 445, 138, 100, 80);
		  }
		  else if (activity.Id.Equals("gateway2"))
		  {
			assertActivityBounds(activity, 620, 250, 40, 40);
		  }
		  else if (activity.Id.Equals("task3"))
		  {
			assertActivityBounds(activity, 453, 304, 100, 80);
		  }
		  else if (activity.Id.Equals("theEnd"))
		  {
			assertActivityBounds(activity, 713, 256, 28, 28);
		  }

		  foreach (PvmTransition sequenceFlow in activity.OutgoingTransitions)
		  {
			assertTrue(((TransitionImpl) sequenceFlow).Waypoints.Count >= 4);

			TransitionImpl transitionImpl = (TransitionImpl) sequenceFlow;
			if (transitionImpl.Id.Equals("flowStartToTask1"))
			{
			  assertSequenceFlowWayPoints(transitionImpl, 100, 270, 176, 270);
			}
			else if (transitionImpl.Id.Equals("flowTask1ToGateway1"))
			{
			  assertSequenceFlowWayPoints(transitionImpl, 276, 270, 340, 270);
			}
			else if (transitionImpl.Id.Equals("flowGateway1ToTask2"))
			{
			  assertSequenceFlowWayPoints(transitionImpl, 360, 250, 360, 178, 445, 178);
			}
			else if (transitionImpl.Id.Equals("flowGateway1ToTask3"))
			{
			  assertSequenceFlowWayPoints(transitionImpl, 360, 290, 360, 344, 453, 344);
			}
			else if (transitionImpl.Id.Equals("flowTask2ToGateway2"))
			{
			  assertSequenceFlowWayPoints(transitionImpl, 545, 178, 640, 178, 640, 250);
			}
			else if (transitionImpl.Id.Equals("flowTask3ToGateway2"))
			{
			  assertSequenceFlowWayPoints(transitionImpl, 553, 344, 640, 344, 640, 290);
			}
			else if (transitionImpl.Id.Equals("flowGateway2ToEnd"))
			{
			  assertSequenceFlowWayPoints(transitionImpl, 660, 270, 713, 270);
			}

		  }
		}
	  }

	  private class CommandAnonymousInnerClass : Command<ProcessDefinitionEntity>
	  {
		  private readonly BpmnParseTest outerInstance;

		  public CommandAnonymousInnerClass(BpmnParseTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ProcessDefinitionEntity execute(CommandContext commandContext)
		  {
			return Context.ProcessEngineConfiguration.DeploymentCache.findDeployedLatestProcessDefinitionByKey("myProcess");
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseNamespaceInConditionExpressionType()
	  public virtual void testParseNamespaceInConditionExpressionType()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		ProcessDefinitionEntity processDefinitionEntity = commandExecutor.execute(new CommandAnonymousInnerClass2(this));

		// Test that the process definition has been deployed
		assertNotNull(processDefinitionEntity);
		PvmActivity activity = processDefinitionEntity.findActivity("ExclusiveGateway_1");
		assertNotNull(activity);

		// Test that the conditions has been resolved
		foreach (PvmTransition transition in activity.OutgoingTransitions)
		{
		  if (transition.Destination.Id.Equals("Task_2"))
		  {
			assertTrue(transition.getProperty("conditionText").Equals("#{approved}"));
		  }
		  else if (transition.Destination.Id.Equals("Task_3"))
		  {
			assertTrue(transition.getProperty("conditionText").Equals("#{!approved}"));
		  }
		  else
		  {
			fail("Something went wrong");
		  }

		}
	  }

	  private class CommandAnonymousInnerClass2 : Command<ProcessDefinitionEntity>
	  {
		  private readonly BpmnParseTest outerInstance;

		  public CommandAnonymousInnerClass2(BpmnParseTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ProcessDefinitionEntity execute(CommandContext commandContext)
		  {
			return Context.ProcessEngineConfiguration.DeploymentCache.findDeployedLatestProcessDefinitionByKey("resolvableNamespacesProcess");
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseDiagramInterchangeElementsForUnknownModelElements()
	  public virtual void testParseDiagramInterchangeElementsForUnknownModelElements()
	  {
	  }

	  /// <summary>
	  /// We want to make sure that BPMNs created with the namespace http://activiti.org/bpmn still work.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testParseDefinitionWithDeprecatedActivitiNamespace()
	  public virtual void testParseDefinitionWithDeprecatedActivitiNamespace()
	  {

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testParseDefinitionWithCamundaNamespace()
	  public virtual void testParseDefinitionWithCamundaNamespace()
	  {

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseCompensationEndEvent()
	  public virtual void testParseCompensationEndEvent()
	  {
		ActivityImpl endEvent = findActivityInDeployedProcessDefinition("end");

		assertEquals("compensationEndEvent", endEvent.getProperty("type"));
		assertEquals(true, endEvent.getProperty(BpmnParse.PROPERTYNAME_THROWS_COMPENSATION));
		assertEquals(typeof(CompensationEventActivityBehavior), endEvent.ActivityBehavior.GetType());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseCompensationStartEvent()
	  public virtual void testParseCompensationStartEvent()
	  {
		ActivityImpl compensationStartEvent = findActivityInDeployedProcessDefinition("compensationStartEvent");

		assertEquals("compensationStartEvent", compensationStartEvent.getProperty("type"));
		assertEquals(typeof(EventSubProcessStartEventActivityBehavior), compensationStartEvent.ActivityBehavior.GetType());

		ActivityImpl compensationEventSubProcess = (ActivityImpl) compensationStartEvent.FlowScope;
		assertEquals(true, compensationEventSubProcess.getProperty(BpmnParse.PROPERTYNAME_IS_FOR_COMPENSATION));

		ScopeImpl subprocess = compensationEventSubProcess.FlowScope;
		assertEquals(compensationEventSubProcess.ActivityId, subprocess.getProperty(BpmnParse.PROPERTYNAME_COMPENSATION_HANDLER_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseAsyncMultiInstanceBody()
	  public virtual void testParseAsyncMultiInstanceBody()
	  {
		ActivityImpl innerTask = findActivityInDeployedProcessDefinition("miTask");
		ActivityImpl miBody = innerTask.ParentFlowScopeActivity;

		assertTrue(miBody.AsyncBefore);
		assertTrue(miBody.AsyncAfter);

		assertFalse(innerTask.AsyncBefore);
		assertFalse(innerTask.AsyncAfter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseAsyncActivityWrappedInMultiInstanceBody()
	  public virtual void testParseAsyncActivityWrappedInMultiInstanceBody()
	  {
		ActivityImpl innerTask = findActivityInDeployedProcessDefinition("miTask");
		assertTrue(innerTask.AsyncBefore);
		assertTrue(innerTask.AsyncAfter);

		ActivityImpl miBody = innerTask.ParentFlowScopeActivity;
		assertFalse(miBody.AsyncBefore);
		assertFalse(miBody.AsyncAfter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseAsyncActivityWrappedInMultiInstanceBodyWithAsyncMultiInstance()
	  public virtual void testParseAsyncActivityWrappedInMultiInstanceBodyWithAsyncMultiInstance()
	  {
		ActivityImpl innerTask = findActivityInDeployedProcessDefinition("miTask");
		assertEquals(true, innerTask.AsyncBefore);
		assertEquals(false, innerTask.AsyncAfter);

		ActivityImpl miBody = innerTask.ParentFlowScopeActivity;
		assertEquals(false, miBody.AsyncBefore);
		assertEquals(true, miBody.AsyncAfter);
	  }

	  public virtual void testParseSwitchedSourceAndTargetRefsForAssociations()
	  {
		repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/parse/BpmnParseTest.testParseSwitchedSourceAndTargetRefsForAssociations.bpmn20.xml").deploy();

		assertEquals(1, repositoryService.createProcessDefinitionQuery().count());

		repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.compensationMiActivity.bpmn20.xml")]
	  public virtual void testParseCompensationHandlerOfMiActivity()
	  {
		ActivityImpl miActivity = findActivityInDeployedProcessDefinition("undoBookHotel");
		ScopeImpl flowScope = miActivity.FlowScope;

		assertEquals(ActivityTypes.MULTI_INSTANCE_BODY, flowScope.getProperty(BpmnParse.PROPERTYNAME_TYPE));
		assertEquals("bookHotel" + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX, ((ActivityImpl) flowScope).ActivityId);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/compensate/CompensateEventTest.compensationMiSubprocess.bpmn20.xml")]
	  public virtual void testParseCompensationHandlerOfMiSubprocess()
	  {
		ActivityImpl miActivity = findActivityInDeployedProcessDefinition("undoBookHotel");
		ScopeImpl flowScope = miActivity.FlowScope;

		assertEquals(ActivityTypes.MULTI_INSTANCE_BODY, flowScope.getProperty(BpmnParse.PROPERTYNAME_TYPE));
		assertEquals("scope" + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX, ((ActivityImpl) flowScope).ActivityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseSignalStartEvent()
	  public virtual void testParseSignalStartEvent()
	  {
		ActivityImpl signalStartActivity = findActivityInDeployedProcessDefinition("start");

		assertEquals(ActivityTypes.START_EVENT_SIGNAL, signalStartActivity.getProperty("type"));
		assertEquals(typeof(NoneStartEventActivityBehavior), signalStartActivity.ActivityBehavior.GetType());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseEscalationBoundaryEvent()
	  public virtual void testParseEscalationBoundaryEvent()
	  {
		ActivityImpl escalationBoundaryEvent = findActivityInDeployedProcessDefinition("escalationBoundaryEvent");

		assertEquals(ActivityTypes.BOUNDARY_ESCALATION, escalationBoundaryEvent.Properties.get(BpmnProperties.TYPE));
		assertEquals(typeof(BoundaryEventActivityBehavior), escalationBoundaryEvent.ActivityBehavior.GetType());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseEscalationIntermediateThrowingEvent()
	  public virtual void testParseEscalationIntermediateThrowingEvent()
	  {
		ActivityImpl escalationThrowingEvent = findActivityInDeployedProcessDefinition("escalationThrowingEvent");

		assertEquals(ActivityTypes.INTERMEDIATE_EVENT_ESCALATION_THROW, escalationThrowingEvent.Properties.get(BpmnProperties.TYPE));
		assertEquals(typeof(ThrowEscalationEventActivityBehavior), escalationThrowingEvent.ActivityBehavior.GetType());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseEscalationEndEvent()
	  public virtual void testParseEscalationEndEvent()
	  {
		ActivityImpl escalationEndEvent = findActivityInDeployedProcessDefinition("escalationEndEvent");

		assertEquals(ActivityTypes.END_EVENT_ESCALATION, escalationEndEvent.Properties.get(BpmnProperties.TYPE));
		assertEquals(typeof(ThrowEscalationEventActivityBehavior), escalationEndEvent.ActivityBehavior.GetType());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseEscalationStartEvent()
	  public virtual void testParseEscalationStartEvent()
	  {
		ActivityImpl escalationStartEvent = findActivityInDeployedProcessDefinition("escalationStartEvent");

		assertEquals(ActivityTypes.START_EVENT_ESCALATION, escalationStartEvent.Properties.get(BpmnProperties.TYPE));
		assertEquals(typeof(EventSubProcessStartEventActivityBehavior), escalationStartEvent.ActivityBehavior.GetType());
	  }


	  public virtual void parseInvalidConditionalEvent(string processDefinitionResource)
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), processDefinitionResource);
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition could be parsed, conditional event definition contains no condition.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Conditional event must contain an expression for evaluation.", e.Message);
		}
	  }

	  public virtual void testParseInvalidConditionalBoundaryEvent()
	  {
		parseInvalidConditionalEvent("testParseInvalidConditionalBoundaryEvent");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseConditionalBoundaryEvent()
	  public virtual void testParseConditionalBoundaryEvent()
	  {
		ActivityImpl conditionalBoundaryEvent = findActivityInDeployedProcessDefinition("conditionalBoundaryEvent");

		assertEquals(ActivityTypes.BOUNDARY_CONDITIONAL, conditionalBoundaryEvent.Properties.get(BpmnProperties.TYPE));
		assertEquals(typeof(BoundaryConditionalEventActivityBehavior), conditionalBoundaryEvent.ActivityBehavior.GetType());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseAsyncBoundaryEvent()
	  public virtual void testParseAsyncBoundaryEvent()
	  {
		ActivityImpl conditionalBoundaryEvent1 = findActivityInDeployedProcessDefinition("conditionalBoundaryEvent1");
		ActivityImpl conditionalBoundaryEvent2 = findActivityInDeployedProcessDefinition("conditionalBoundaryEvent2");

		assertTrue(conditionalBoundaryEvent1.AsyncAfter);
		assertTrue(conditionalBoundaryEvent1.AsyncBefore);

		assertFalse(conditionalBoundaryEvent2.AsyncAfter);
		assertFalse(conditionalBoundaryEvent2.AsyncBefore);
	  }

	  public virtual void testParseInvalidIntermediateConditionalEvent()
	  {
		parseInvalidConditionalEvent("testParseInvalidIntermediateConditionalEvent");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseIntermediateConditionalEvent()
	  public virtual void testParseIntermediateConditionalEvent()
	  {
		ActivityImpl intermediateConditionalEvent = findActivityInDeployedProcessDefinition("intermediateConditionalEvent");

		assertEquals(ActivityTypes.INTERMEDIATE_EVENT_CONDITIONAL, intermediateConditionalEvent.Properties.get(BpmnProperties.TYPE));
		assertEquals(typeof(IntermediateConditionalEventBehavior), intermediateConditionalEvent.ActivityBehavior.GetType());
	  }

	  public virtual void testParseInvalidEventSubprocessConditionalStartEvent()
	  {
		parseInvalidConditionalEvent("testParseInvalidEventSubprocessConditionalStartEvent");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseEventSubprocessConditionalStartEvent()
	  public virtual void testParseEventSubprocessConditionalStartEvent()
	  {
		ActivityImpl conditionalStartEventSubProcess = findActivityInDeployedProcessDefinition("conditionalStartEventSubProcess");

		assertEquals(ActivityTypes.START_EVENT_CONDITIONAL, conditionalStartEventSubProcess.Properties.get(BpmnProperties.TYPE));
		assertEquals(typeof(EventSubProcessStartConditionalEventActivityBehavior), conditionalStartEventSubProcess.ActivityBehavior.GetType());

	  }

	  protected internal virtual void assertActivityBounds(ActivityImpl activity, int x, int y, int width, int height)
	  {
		assertEquals(x, activity.X);
		assertEquals(y, activity.Y);
		assertEquals(width, activity.Width);
		assertEquals(height, activity.Height);
	  }

	  protected internal virtual void assertSequenceFlowWayPoints(TransitionImpl sequenceFlow, params Integer[] waypoints)
	  {
		assertEquals(waypoints.Length, sequenceFlow.Waypoints.Count);
		for (int i = 0; i < waypoints.Length; i++)
		{
		  assertEquals(waypoints[i], sequenceFlow.Waypoints[i]);
		}
	  }

	  protected internal virtual ActivityImpl findActivityInDeployedProcessDefinition(string activityId)
	  {
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().singleResult();
		assertNotNull(processDefinition);

		ProcessDefinitionEntity cachedProcessDefinition = processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.get(processDefinition.Id);
		return cachedProcessDefinition.findActivity(activityId);
	  }

	  public virtual void testNoCamundaInSourceThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:in extension element should contain source!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Missing parameter 'source' or 'sourceExpression' when passing variables", e.Message);
		}
	  }

	  public virtual void testNoCamundaInSourceShouldWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		  repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
		}
	  }

	  public virtual void testEmptyCamundaInSourceThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:in extension element should contain source!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Empty attribute 'source' when passing variables", e.Message);
		}
	  }

	  public virtual void testEmptyCamundaInSourceWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		  repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
		}
	  }

	  public virtual void testNoCamundaInTargetThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:in extension element should contain target!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
		}
	  }

	  public virtual void testNoCamundaInTargetWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:in extension element should contain target!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		}
	  }

	  public virtual void testEmptyCamundaInTargetThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:in extension element should contain target!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Empty attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
		}
	  }

	  public virtual void testEmptyCamundaInTargetWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		  repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
		}
	  }

	  public virtual void testNoCamundaOutSourceThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:out extension element should contain source!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Missing parameter 'source' or 'sourceExpression' when passing variables", e.Message);
		}
	  }

	  public virtual void testNoCamundaOutSourceWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		  repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
		}
	  }

	  public virtual void testEmptyCamundaOutSourceThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:out extension element should contain source!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Empty attribute 'source' when passing variables", e.Message);
		}
	  }

	  public virtual void testEmptyCamundaOutSourceWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutSourceThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		  repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
		}
	  }

	  public virtual void testNoCamundaOutTargetThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:out extension element should contain target!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
		}
	  }

	  public virtual void testNoCamundaOutTargetWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:out extension element should contain target!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		}
	  }

	  public virtual void testEmptyCamundaOutTargetThrowsError()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Process camunda:out extension element should contain target!");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Empty attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
		}
	  }

	  public virtual void testEmptyCamundaOutTargetWithoutValidation()
	  {
		try
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = true;

		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutTargetThrowsError");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		}
		finally
		{
		  processEngineConfiguration.DisableStrictCallActivityValidation = false;
		  repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseProcessDefinitionTtl()
	  public virtual void testParseProcessDefinitionTtl()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertNotNull(processDefinitions);
		assertEquals(1, processDefinitions.Count);

		int? timeToLive = processDefinitions[0].HistoryTimeToLive;
		assertNotNull(timeToLive);
		assertEquals(5, timeToLive.Value);

		assertTrue(processDefinitions[0].StartableInTasklist);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseProcessDefinitionStringTtl()
	  public virtual void testParseProcessDefinitionStringTtl()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertNotNull(processDefinitions);
		assertEquals(1, processDefinitions.Count);

		int? timeToLive = processDefinitions[0].HistoryTimeToLive;
		assertNotNull(timeToLive);
		assertEquals(5, timeToLive.Value);
	  }

	  public virtual void testParseProcessDefinitionMalformedStringTtl()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionMalformedStringTtl");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot parse historyTimeToLive", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseProcessDefinitionEmptyTtl()
	  public virtual void testParseProcessDefinitionEmptyTtl()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertNotNull(processDefinitions);
		assertEquals(1, processDefinitions.Count);

		int? timeToLive = processDefinitions[0].HistoryTimeToLive;
		assertNull(timeToLive);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseProcessDefinitionWithoutTtl()
	  public virtual void testParseProcessDefinitionWithoutTtl()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertNotNull(processDefinitions);
		assertEquals(1, processDefinitions.Count);

		int? timeToLive = processDefinitions[0].HistoryTimeToLive;
		assertNull(timeToLive);
	  }

	  public virtual void testParseProcessDefinitionWithoutTtlWithConfigDefault()
	  {
		processEngineConfiguration.HistoryTimeToLive = "6";
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionWithoutTtl");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		  assertNotNull(processDefinitions);
		  assertEquals(1, processDefinitions.Count);

		  int? timeToLive = processDefinitions[0].HistoryTimeToLive;
		  assertNotNull(timeToLive);
		  assertEquals(6, timeToLive.Value);
		}
		finally
		{
		  processEngineConfiguration.HistoryTimeToLive = null;
		  repositoryService.deleteDeployment(repositoryService.createDeploymentQuery().singleResult().Id, true);
		}
	  }

	  public virtual void testParseProcessDefinitionWithoutTtlWithMalformedConfigDefault()
	  {
		processEngineConfiguration.HistoryTimeToLive = "PP555DDD";
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionWithoutTtl");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot parse historyTimeToLive", e.Message);
		}
		finally
		{
		  processEngineConfiguration.HistoryTimeToLive = null;
		}
	  }

	  public virtual void testParseProcessDefinitionWithoutTtlWithInvalidConfigDefault()
	  {
		processEngineConfiguration.HistoryTimeToLive = "invalidValue";
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionWithoutTtl");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot parse historyTimeToLive", e.Message);
		}
		finally
		{
		  processEngineConfiguration.HistoryTimeToLive = null;
		}
	  }

	  public virtual void testParseProcessDefinitionWithoutTtlWithNegativeConfigDefault()
	  {
		processEngineConfiguration.HistoryTimeToLive = "-6";
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionWithoutTtl");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot parse historyTimeToLive", e.Message);
		}
		finally
		{
		  processEngineConfiguration.HistoryTimeToLive = null;
		}
	  }

	  public virtual void testParseProcessDefinitionInvalidTtl()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionInvalidTtl");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot parse historyTimeToLive", e.Message);
		}
	  }

	  public virtual void testParseProcessDefinitionNegativTtl()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionNegativeTtl");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		  fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Cannot parse historyTimeToLive", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseProcessDefinitionStartable()
	  public virtual void testParseProcessDefinitionStartable()
	  {
		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().list();
		assertNotNull(processDefinitions);
		assertEquals(1, processDefinitions.Count);

		assertFalse(processDefinitions[0].StartableInTasklist);
	  }

	  public virtual void testXxeProcessing()
	  {
		try
		{
		  string resource = TestHelper.getBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionXXE");
		  repositoryService.createDeployment().name(resource).addClasspathResource(resource).deploy();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("cvc-datatype-valid.1.2.1: ''", e.Message);
		  assertTextPresent("cvc-type.3.1.3: The value ''", e.Message);
		}
	  }
	}

}