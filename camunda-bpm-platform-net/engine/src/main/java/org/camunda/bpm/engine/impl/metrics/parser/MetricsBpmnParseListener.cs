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
namespace org.camunda.bpm.engine.impl.metrics.parser
{
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using AbstractBpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.AbstractBpmnParseListener;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using Metrics = org.camunda.bpm.engine.management.Metrics;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MetricsBpmnParseListener : AbstractBpmnParseListener
	{

	  public static MetricsExecutionListener ACTIVITY_INSTANCE_START_COUNTER = new MetricsExecutionListener(Metrics.ACTIVTY_INSTANCE_START);
	  public static MetricsExecutionListener ACTIVITY_INSTANCE_END_COUNTER = new MetricsExecutionListener(Metrics.ACTIVTY_INSTANCE_END);

	  protected internal virtual void addListeners(ActivityImpl activity)
	  {
		activity.addBuiltInListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, ACTIVITY_INSTANCE_START_COUNTER);
		activity.addBuiltInListener(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, ACTIVITY_INSTANCE_END_COUNTER);
	  }

	  public override void parseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseParallelGateway(Element parallelGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseManualTask(Element manualTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseEventBasedGateway(Element eventBasedGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement, ActivityImpl activity)
	  {
		addListeners(activity);
	  }

	  public override void parseMultiInstanceLoopCharacteristics(Element activityElement, Element multiInstanceLoopCharacteristicsElement, ActivityImpl activity)
	  {
		addListeners(activity);
	  }
	}

}