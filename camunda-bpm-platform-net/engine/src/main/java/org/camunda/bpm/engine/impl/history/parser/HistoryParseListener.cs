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
namespace org.camunda.bpm.engine.impl.history.parser
{

	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using UserTaskActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.UserTaskActivityBehavior;
	using BpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseListener;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmEvent = org.camunda.bpm.engine.impl.pvm.PvmEvent;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using VariableDeclaration = org.camunda.bpm.engine.impl.variable.VariableDeclaration;

	/// <summary>
	/// <para>This class is responsible for wiring history as execution listeners into process execution.
	/// 
	/// </para>
	/// <para>NOTE: the role of this class has changed since 7.0: in order to customize history behavior it is
	/// usually not necessary to override this class but rather the <seealso cref="HistoryEventProducer"/> for
	/// customizing data acquisition and <seealso cref="HistoryEventHandler"/> for customizing the persistence behavior
	/// or if you need a history event stream.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Falko Menge
	/// @author Bernd Ruecker (camunda)
	/// @author Christian Lipphardt (camunda)
	/// 
	/// @author Daniel Meyer
	/// </para>
	/// </summary>
	public class HistoryParseListener : BpmnParseListener
	{

	  // Cached listeners
	  // listeners can be reused for a given process engine instance but cannot be cached in static fields since
	  // different process engine instances on the same Classloader may have different HistoryEventProducer
	  // configurations wired
	  protected internal ExecutionListener PROCESS_INSTANCE_START_LISTENER;
	  protected internal ExecutionListener PROCESS_INSTANCE_END_LISTENER;

	  protected internal ExecutionListener ACTIVITY_INSTANCE_START_LISTENER;
	  protected internal ExecutionListener ACTIVITY_INSTANCE_END_LISTENER;

	  protected internal TaskListener USER_TASK_ASSIGNMENT_HANDLER;
	  protected internal TaskListener USER_TASK_ID_HANDLER;

	  // The history level set in the process engine configuration
	  protected internal HistoryLevel historyLevel;

	  public HistoryParseListener(HistoryLevel historyLevel, HistoryEventProducer historyEventProducer)
	  {
		this.historyLevel = historyLevel;
		initExecutionListeners(historyEventProducer, historyLevel);
	  }

	  protected internal virtual void initExecutionListeners(HistoryEventProducer historyEventProducer, HistoryLevel historyLevel)
	  {
		PROCESS_INSTANCE_START_LISTENER = new ProcessInstanceStartListener(historyEventProducer, historyLevel);
		PROCESS_INSTANCE_END_LISTENER = new ProcessInstanceEndListener(historyEventProducer, historyLevel);

		ACTIVITY_INSTANCE_START_LISTENER = new ActivityInstanceStartListener(historyEventProducer, historyLevel);
		ACTIVITY_INSTANCE_END_LISTENER = new ActivityInstanceEndListener(historyEventProducer, historyLevel);

		USER_TASK_ASSIGNMENT_HANDLER = new ActivityInstanceUpdateListener(historyEventProducer, historyLevel);
		USER_TASK_ID_HANDLER = USER_TASK_ASSIGNMENT_HANDLER;
	  }

	  public virtual void parseProcess(Element processElement, ProcessDefinitionEntity processDefinition)
	  {
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.PROCESS_INSTANCE_END, null))
		{
		  processDefinition.addBuiltInListener(PvmEvent.EVENTNAME_END, PROCESS_INSTANCE_END_LISTENER);
		}
	  }

	  public virtual void parseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);

		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.TASK_INSTANCE_CREATE, null))
		{
		  TaskDefinition taskDefinition = ((UserTaskActivityBehavior) activity.ActivityBehavior).TaskDefinition;
		  taskDefinition.addBuiltInTaskListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT, USER_TASK_ASSIGNMENT_HANDLER);
		  taskDefinition.addBuiltInTaskListener(org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE, USER_TASK_ID_HANDLER);
		}
	  }

	  public virtual void parseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting, ActivityImpl timerActivity)
	  {
	  }

	  public virtual void parseBoundaryErrorEventDefinition(Element errorEventDefinition, bool interrupting, ActivityImpl activity, ActivityImpl nestedErrorEventActivity)
	  {
	  }

	  public virtual void parseIntermediateTimerEventDefinition(Element timerEventDefinition, ActivityImpl timerActivity)
	  {
	  }

	  public virtual void parseProperty(Element propertyElement, VariableDeclaration variableDeclaration, ActivityImpl activity)
	  {
	  }

	  public virtual void parseSequenceFlow(Element sequenceFlowElement, ScopeImpl scopeElement, TransitionImpl transition)
	  {
	  }

	  public virtual void parseRootElement(Element rootElement, IList<ProcessDefinitionEntity> processDefinitions)
	  {
	  }

	  public virtual void parseBoundarySignalEventDefinition(Element signalEventDefinition, bool interrupting, ActivityImpl signalActivity)
	  {
	  }

	  public virtual void parseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseMultiInstanceLoopCharacteristics(Element activityElement, Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseIntermediateSignalCatchEventDefinition(Element signalEventDefinition, ActivityImpl signalActivity)
	  {
	  }

	  public virtual void parseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseCompensateEventDefinition(Element compensateEventDefinition, ActivityImpl compensationActivity)
	  {
	  }

	  public virtual void parseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
	  }

	  public virtual void parseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		// do not write history for link events
		if (!activity.getProperty("type").Equals("intermediateLinkCatch"))
		{
		  addActivityHandlers(activity);
		}
	  }

	  public virtual void parseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement, ActivityImpl activity)
	  {
		addActivityHandlers(activity);
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

	  // helper methods ///////////////////////////////////////////////////////////

	  protected internal virtual void addActivityHandlers(ActivityImpl activity)
	  {
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.ACTIVITY_INSTANCE_START, null))
		{
		  activity.addBuiltInListener(PvmEvent.EVENTNAME_START, ACTIVITY_INSTANCE_START_LISTENER, 0);
		}
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.ACTIVITY_INSTANCE_END, null))
		{
		  activity.addBuiltInListener(PvmEvent.EVENTNAME_END, ACTIVITY_INSTANCE_END_LISTENER);
		}
	  }

	}

}