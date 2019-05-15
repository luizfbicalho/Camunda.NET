using System;

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
	using MultiInstanceActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.MultiInstanceActivityBehavior;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using PropertyKey = org.camunda.bpm.engine.impl.core.model.PropertyKey;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using ParseUtil = org.camunda.bpm.engine.impl.util.ParseUtil;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using Namespace = org.camunda.bpm.engine.impl.util.xml.Namespace;

	public class DefaultFailedJobParseListener : AbstractBpmnParseListener
	{

	  protected internal const string TYPE = "type";
	  protected internal const string START_TIMER_EVENT = "startTimerEvent";
	  protected internal const string BOUNDARY_TIMER = "boundaryTimer";
	  protected internal const string INTERMEDIATE_SIGNAL_THROW = "intermediateSignalThrow";
	  protected internal const string INTERMEDIATE_TIMER = "intermediateTimer";
	  protected internal const string SIGNAL_EVENT_DEFINITION = "signalEventDefinition";
	  protected internal const string MULTI_INSTANCE_LOOP_CHARACTERISTICS = "multiInstanceLoopCharacteristics";

	  protected internal const string EXTENSION_ELEMENTS = "extensionElements";
	  protected internal const string FAILED_JOB_RETRY_TIME_CYCLE = "failedJobRetryTimeCycle";

	  /// <summary>
	  /// deprecated since 7.4, use camunda ns.
	  /// </summary>
	  [Obsolete]
	  public static readonly Namespace FOX_ENGINE_NS = new Namespace("http://www.camunda.com/fox");

	  public static readonly PropertyKey<FailedJobRetryConfiguration> FAILED_JOB_CONFIGURATION = new PropertyKey<FailedJobRetryConfiguration>("FAILED_JOB_CONFIGURATION");

	  public override void parseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl startEventActivity)
	  {
		string type = startEventActivity.Properties.get(BpmnProperties.TYPE);
		if (!string.ReferenceEquals(type, null) && type.Equals(START_TIMER_EVENT) || isAsync(startEventActivity))
		{
		  this.setFailedJobRetryTimeCycleValue(startEventElement, startEventActivity);
		}
	  }

	  public override void parseBoundaryEvent(Element boundaryEventElement, ScopeImpl scopeElement, ActivityImpl nestedActivity)
	  {
		string type = nestedActivity.Properties.get(BpmnProperties.TYPE);
		if ((!string.ReferenceEquals(type, null) && type.Equals(BOUNDARY_TIMER)) || isAsync(nestedActivity))
		{
		  setFailedJobRetryTimeCycleValue(boundaryEventElement, nestedActivity);
		}
	  }

	  public override void parseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		string type = activity.Properties.get(BpmnProperties.TYPE);
		if (!string.ReferenceEquals(type, null))
		{
		  this.setFailedJobRetryTimeCycleValue(intermediateEventElement, activity);
		}
	  }

	  public override void parseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		string type = activity.Properties.get(BpmnProperties.TYPE);
		if (!string.ReferenceEquals(type, null) && type.Equals(INTERMEDIATE_TIMER) || isAsync(activity))
		{
		  this.setFailedJobRetryTimeCycleValue(intermediateEventElement, activity);
		}
	  }

	  public override void parseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(endEventElement, activity);
	  }

	  public override void parseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(exclusiveGwElement, activity);
	  }

	  public override void parseInclusiveGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(exclusiveGwElement, activity);
	  }

	  public override void parseEventBasedGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(exclusiveGwElement, activity);
	  }

	  public override void parseParallelGateway(Element exclusiveGwElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(exclusiveGwElement, activity);
	  }

	  public override void parseScriptTask(Element scriptTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(scriptTaskElement, activity);
	  }

	  public override void parseServiceTask(Element serviceTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(serviceTaskElement, activity);
	  }

	  public override void parseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(businessRuleTaskElement, activity);
	  }

	  public override void parseTask(Element taskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(taskElement, activity);
	  }

	  public override void parseUserTask(Element userTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(userTaskElement, activity);
	  }

	  public override void parseCallActivity(Element callActivityElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(callActivityElement, activity);
	  }

	  public override void parseReceiveTask(Element receiveTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(receiveTaskElement, activity);
	  }

	  public override void parseSendTask(Element sendTaskElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(sendTaskElement, activity);
	  }

	  public override void parseSubProcess(Element subProcessElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(subProcessElement, activity);
	  }

	  public override void parseTransaction(Element transactionElement, ScopeImpl scope, ActivityImpl activity)
	  {
		parseActivity(transactionElement, activity);
	  }

	  protected internal virtual bool isAsync(ActivityImpl activity)
	  {
		return activity.AsyncBefore || activity.AsyncAfter;
	  }

	  protected internal virtual void parseActivity(Element element, ActivityImpl activity)
	  {

		if (isMultiInstance(activity))
		{
		  // in case of multi-instance, the extension elements is set according to the async attributes
		  // the extension for multi-instance body is set on the element of the activity
		  ActivityImpl miBody = activity.ParentFlowScopeActivity;
		  if (isAsync(miBody))
		  {
			setFailedJobRetryTimeCycleValue(element, miBody);
		  }
		  // the extension for inner activity is set on the multiInstanceLoopCharacteristics element
		  if (isAsync(activity))
		  {
			Element multiInstanceLoopCharacteristics = element.element(MULTI_INSTANCE_LOOP_CHARACTERISTICS);
			setFailedJobRetryTimeCycleValue(multiInstanceLoopCharacteristics, activity);
		  }

		}
		else if (isAsync(activity))
		{
		  setFailedJobRetryTimeCycleValue(element, activity);
		}
	  }

	  protected internal virtual void setFailedJobRetryTimeCycleValue(Element element, ActivityImpl activity)
	  {
		string failedJobRetryTimeCycleConfiguration = null;

		Element extensionElements = element.element(EXTENSION_ELEMENTS);
		if (extensionElements != null)
		{
		  Element failedJobRetryTimeCycleElement = extensionElements.elementNS(FOX_ENGINE_NS, FAILED_JOB_RETRY_TIME_CYCLE);
		  if (failedJobRetryTimeCycleElement == null)
		  {
			// try to get it from the activiti namespace
			failedJobRetryTimeCycleElement = extensionElements.elementNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, FAILED_JOB_RETRY_TIME_CYCLE);
		  }

		  if (failedJobRetryTimeCycleElement != null)
		  {
			failedJobRetryTimeCycleConfiguration = failedJobRetryTimeCycleElement.Text;
		  }
		}

		if (string.ReferenceEquals(failedJobRetryTimeCycleConfiguration, null) || failedJobRetryTimeCycleConfiguration.Length == 0)
		{
		  failedJobRetryTimeCycleConfiguration = Context.ProcessEngineConfiguration.FailedJobRetryTimeCycle;
		}

		if (!string.ReferenceEquals(failedJobRetryTimeCycleConfiguration, null))
		{
		  FailedJobRetryConfiguration configuration = ParseUtil.parseRetryIntervals(failedJobRetryTimeCycleConfiguration);
		  activity.Properties.set(FAILED_JOB_CONFIGURATION, configuration);
		}
	  }

	  protected internal virtual bool isMultiInstance(ActivityImpl activity)
	  {
		// #isMultiInstance() don't work since the property is not set yet
		ActivityImpl parent = activity.ParentFlowScopeActivity;
		return parent != null && parent.ActivityBehavior is MultiInstanceActivityBehavior;
	  }

	}

}