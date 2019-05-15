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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{

	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using VariableDeclaration = org.camunda.bpm.engine.impl.variable.VariableDeclaration;

	/// <summary>
	/// Abstract base class for implementing a <seealso cref="BpmnParseListener"/> without being forced to implement
	/// all methods provided, which makes the implementation more robust to future changes.
	/// 
	/// @author ruecker
	/// </summary>
	public class AbstractBpmnParseListener : BpmnParseListener
	{

	  public virtual void parseProcess(Element processElement, ProcessDefinitionEntity processDefinition)
	  {
	  }

	  public virtual void parseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl startEventActivity)
	  {
	  }

	  public virtual void parseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting, ActivityImpl timerActivity)
	  {
	  }

	  public virtual void parseBoundaryErrorEventDefinition(Element errorEventDefinition, bool interrupting, ActivityImpl activity, ActivityImpl nestedErrorEventActivity)
	  {
	  }

	  public virtual void parseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseProperty(Element propertyElement, VariableDeclaration variableDeclaration, ActivityImpl activity)
	  {
	  }

	  public virtual void parseSequenceFlow(Element sequenceFlowElement, ScopeImpl scopeElement, TransitionImpl transition)
	  {
	  }

	  public virtual void parseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseMultiInstanceLoopCharacteristics(Element activityElement, Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity)
	  {
	  }

	  public virtual void parseIntermediateTimerEventDefinition(Element timerEventDefinition, ActivityImpl timerActivity)
	  {
	  }

	  public virtual void parseRootElement(Element rootElement, IList<ProcessDefinitionEntity> processDefinitions)
	  {
	  }

	  public virtual void parseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseIntermediateSignalCatchEventDefinition(Element signalEventDefinition, ActivityImpl signalActivity)
	  {
	  }

	  public virtual void parseBoundarySignalEventDefinition(Element signalEventDefinition, bool interrupting, ActivityImpl signalActivity)
	  {
	  }

	  public virtual void parseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseCompensateEventDefinition(Element compensateEventDefinition, ActivityImpl compensationActivity)
	  {
	  }

	  public virtual void parseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
	  }

	  public virtual void parseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement, ActivityImpl nestedActivity)
	  {
	  }

	  public virtual void parseIntermediateMessageCatchEventDefinition(Element messageEventDefinition, ActivityImpl nestedActivity)
	  {
	  }

	  public virtual void parseBoundaryMessageEventDefinition(Element element, bool interrupting, ActivityImpl messageActivity)
	  {
	  }

	  public virtual void parseBoundaryEscalationEventDefinition(Element escalationEventDefinition, bool interrupting, ActivityImpl boundaryEventActivity)
	  {
	  }

	  public virtual void parseBoundaryConditionalEventDefinition(Element element, bool interrupting, ActivityImpl conditionalActivity)
	  {
	  }

	  public virtual void parseIntermediateConditionalEventDefinition(Element conditionalEventDefinition, ActivityImpl conditionalActivity)
	  {
	  }

	  public virtual void parseConditionalStartEventForEventSubprocess(Element element, ActivityImpl conditionalActivity, bool interrupting)
	  {
	  }
	}

}