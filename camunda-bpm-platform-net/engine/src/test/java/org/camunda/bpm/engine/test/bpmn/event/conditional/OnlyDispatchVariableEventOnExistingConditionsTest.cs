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
namespace org.camunda.bpm.engine.test.bpmn.@event.conditional
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using DelayedVariableEvent = org.camunda.bpm.engine.impl.persistence.entity.DelayedVariableEvent;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessInstanceWithVariablesImpl = org.camunda.bpm.engine.impl.persistence.entity.ProcessInstanceWithVariablesImpl;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
	using static org.camunda.bpm.engine.test.bpmn.@event.conditional.AbstractConditionalEventTestCase;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class OnlyDispatchVariableEventOnExistingConditionsTest
	{

	  public class CheckDelayedVariablesDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  //given conditional event exist

		  //when variable is set
		  execution.setVariable("v", 1);

		  //then variable events should be delayed
		  IList<DelayedVariableEvent> delayedEvents = ((ExecutionEntity) execution).DelayedEvents;
		  assertEquals(1, delayedEvents.Count);
		  assertEquals("v", delayedEvents[0].Event.VariableInstance.Name);
		}
	  }

	  public class CheckNoDelayedVariablesDelegate : JavaDelegate
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  //given no conditional event exist

		  //when variable is set
		  execution.setVariable("v", 1);

		  //then no variable events should be delayed
		  IList<DelayedVariableEvent> delayedEvents = ((ExecutionEntity) execution).DelayedEvents;
		  assertEquals(0, delayedEvents.Count);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessWithIntermediateConditionalEvent()
	  public virtual void testProcessWithIntermediateConditionalEvent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().serviceTask().camundaClass(typeof(CheckDelayedVariablesDelegate).FullName).intermediateCatchEvent().conditionalEventDefinition().condition("${var==1}").conditionalEventDefinitionDone().endEvent().done();

		//when process is deployed and instance created
		rule.manageDeployment(rule.RepositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
		ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl) rule.RuntimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then process definition contains property which indicates that conditional events exists
		object property = processInstance.ExecutionEntity.ProcessDefinition.getProperty(BpmnParse.PROPERTYNAME_HAS_CONDITIONAL_EVENTS);
		assertNotNull(property);
		assertEquals(true, property);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessWithBoundaryConditionalEvent()
	  public virtual void testProcessWithBoundaryConditionalEvent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().serviceTask().camundaClass(typeof(CheckDelayedVariablesDelegate).FullName).userTask(TASK_WITH_CONDITION_ID).endEvent().done();

		modelInstance = modify(modelInstance).userTaskBuilder(TASK_WITH_CONDITION_ID).boundaryEvent().conditionalEventDefinition().condition("${var==1}").conditionalEventDefinitionDone().endEvent().done();

		//when process is deployed and instance created
		rule.manageDeployment(rule.RepositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
		ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl) rule.RuntimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then process definition contains property which indicates that conditional events exists
		object property = processInstance.ExecutionEntity.ProcessDefinition.getProperty(BpmnParse.PROPERTYNAME_HAS_CONDITIONAL_EVENTS);
		assertNotNull(property);
		assertEquals(true, property);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessWithEventSubProcessConditionalEvent()
	  public virtual void testProcessWithEventSubProcessConditionalEvent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().serviceTask().camundaClass(typeof(CheckDelayedVariablesDelegate).FullName).userTask().endEvent().done();

		modelInstance = modify(modelInstance).addSubProcessTo(CONDITIONAL_EVENT_PROCESS_KEY).triggerByEvent().embeddedSubProcess().startEvent().conditionalEventDefinition().condition("${var==1}").conditionalEventDefinitionDone().endEvent().done();

		//when process is deployed and instance created
		rule.manageDeployment(rule.RepositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
		ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl) rule.RuntimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then process definition contains property which indicates that conditional events exists
		object property = processInstance.ExecutionEntity.ProcessDefinition.getProperty(BpmnParse.PROPERTYNAME_HAS_CONDITIONAL_EVENTS);
		assertNotNull(property);
		assertEquals(true, property);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessWithoutConditionalEvent()
	  public virtual void testProcessWithoutConditionalEvent()
	  {
		//given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).startEvent().serviceTask().camundaClass(typeof(CheckNoDelayedVariablesDelegate).FullName).userTask().endEvent().done();

		//when process is deployed and instance created
		rule.manageDeployment(rule.RepositoryService.createDeployment().addModelInstance(CONDITIONAL_MODEL, modelInstance).deploy());
		ProcessInstanceWithVariablesImpl processInstance = (ProcessInstanceWithVariablesImpl) rule.RuntimeService.startProcessInstanceByKey(CONDITIONAL_EVENT_PROCESS_KEY);

		//then process definition contains no property which indicates that conditional events exists
		object property = processInstance.ExecutionEntity.ProcessDefinition.getProperty(BpmnParse.PROPERTYNAME_HAS_CONDITIONAL_EVENTS);
		assertNull(property);
	  }
	}

}