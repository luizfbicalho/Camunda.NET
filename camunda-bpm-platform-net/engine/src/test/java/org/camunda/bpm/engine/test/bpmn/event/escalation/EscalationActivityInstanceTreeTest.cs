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
namespace org.camunda.bpm.engine.test.bpmn.@event.escalation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class EscalationActivityInstanceTreeTest : PluggableProcessEngineTestCase
	{

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testThrowEscalationEventFromEmbeddedSubprocess.bpmn20.xml")]
	  public virtual void testNonInterruptingEscalationBoundaryEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("escalationProcess");
		// an escalation event is thrown from embedded subprocess and caught by non-interrupting boundary event on subprocess

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("taskAfterCatchedEscalation").beginScope("subProcess").activity("taskInSubprocess").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventTest.testInterruptingEscalationBoundaryEvent.bpmn20.xml")]
	  public virtual void testInterruptingEscalationBoundaryEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("escalationProcess");
		// an escalation event is thrown from embedded subprocess and caught by interrupting boundary event on subprocess

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).activity("taskAfterCatchedEscalation").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventSubprocessTest.testCatchEscalationEventInsideSubprocess.bpmn20.xml")]
	  public virtual void testNonInterruptingEscalationEventSubprocessInsideSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("escalationProcess");
		// an escalation event is thrown from embedded subprocess and caught by non-interrupting event subprocess inside the subprocess

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("taskInSubprocess").beginScope("escalationEventSubprocess").activity("taskAfterCatchedEscalation").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventSubprocessTest.testCatchEscalationEventFromEmbeddedSubprocess.bpmn20.xml")]
	  public virtual void testNonInterruptingEscalationEventSubprocessOutsideSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("escalationProcess");
		// an escalation event is thrown from embedded subprocess and caught by non-interrupting event subprocess outside the subprocess

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").activity("taskInSubprocess").endScope().beginScope("escalationEventSubprocess").activity("taskAfterCatchedEscalation").done());
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/event/escalation/EscalationEventSubprocessTest.testInterruptionEscalationEventSubprocess.bpmn20.xml")]
	  public virtual void testInterruptingEscalationEventSubprocessInsideSubprocess()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("escalationProcess");
		// an escalation event is thrown from embedded subprocess and caught by interrupting event subprocess inside the subprocess

		ActivityInstance tree = runtimeService.getActivityInstance(processInstance.Id);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("subProcess").beginScope("escalationEventSubprocess").activity("taskAfterCatchedEscalation").done());
	  }

	}

}