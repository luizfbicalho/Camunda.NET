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
namespace org.camunda.bpm.engine
{
	/// <summary>
	/// Class contains constants that identifies the activity types, which are used by Camunda.
	/// Events, gateways and activities are summed together as activities.
	/// They typically correspond to the XML tags used in the BPMN 2.0 process definition file.
	/// 
	/// @author Thorben Lindhauer
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public sealed class ActivityTypes
	{

	  public const string MULTI_INSTANCE_BODY = "multiInstanceBody";

	  //gateways //////////////////////////////////////////////

	  public const string GATEWAY_EXCLUSIVE = "exclusiveGateway";
	  public const string GATEWAY_INCLUSIVE = "inclusiveGateway";
	  public const string GATEWAY_PARALLEL = "parallelGateway";
	  public const string GATEWAY_COMPLEX = "complexGateway";
	  public const string GATEWAY_EVENT_BASED = "eventBasedGateway";

	  //tasks //////////////////////////////////////////////
	  public const string TASK = "task";
	  public const string TASK_SCRIPT = "scriptTask";
	  public const string TASK_SERVICE = "serviceTask";
	  public const string TASK_BUSINESS_RULE = "businessRuleTask";
	  public const string TASK_MANUAL_TASK = "manualTask";
	  public const string TASK_USER_TASK = "userTask";
	  public const string TASK_SEND_TASK = "sendTask";
	  public const string TASK_RECEIVE_TASK = "receiveTask";

	  //other ////////////////////////////////////////////////
	  public const string SUB_PROCESS = "subProcess";
	  public const string SUB_PROCESS_AD_HOC = "adHocSubProcess";
	  public const string CALL_ACTIVITY = "callActivity";
	  public const string TRANSACTION = "transaction";

	  //boundary events ////////////////////////////////////////
	  public const string BOUNDARY_TIMER = "boundaryTimer";
	  public const string BOUNDARY_MESSAGE = "boundaryMessage";
	  public const string BOUNDARY_SIGNAL = "boundarySignal";
	  public const string BOUNDARY_COMPENSATION = "compensationBoundaryCatch";
	  public const string BOUNDARY_ERROR = "boundaryError";
	  public const string BOUNDARY_ESCALATION = "boundaryEscalation";
	  public const string BOUNDARY_CANCEL = "cancelBoundaryCatch";
	  public const string BOUNDARY_CONDITIONAL = "boundaryConditional";

	  //start events ////////////////////////////////////////
	  public const string START_EVENT = "startEvent";
	  public const string START_EVENT_TIMER = "startTimerEvent";
	  public const string START_EVENT_MESSAGE = "messageStartEvent";
	  public const string START_EVENT_SIGNAL = "signalStartEvent";
	  public const string START_EVENT_ESCALATION = "escalationStartEvent";
	  public const string START_EVENT_COMPENSATION = "compensationStartEvent";
	  public const string START_EVENT_ERROR = "errorStartEvent";
	  public const string START_EVENT_CONDITIONAL = "conditionalStartEvent";

	  //intermediate catch events ////////////////////////////////////////
	  public const string INTERMEDIATE_EVENT_CATCH = "intermediateCatchEvent";
	  public const string INTERMEDIATE_EVENT_MESSAGE = "intermediateMessageCatch";
	  public const string INTERMEDIATE_EVENT_TIMER = "intermediateTimer";
	  public const string INTERMEDIATE_EVENT_LINK = "intermediateLinkCatch";
	  public const string INTERMEDIATE_EVENT_SIGNAL = "intermediateSignalCatch";
	  public const string INTERMEDIATE_EVENT_CONDITIONAL = "intermediateConditional";

	  //intermediate throw events ////////////////////////////////
	  public const string INTERMEDIATE_EVENT_THROW = "intermediateThrowEvent";
	  public const string INTERMEDIATE_EVENT_SIGNAL_THROW = "intermediateSignalThrow";
	  public const string INTERMEDIATE_EVENT_COMPENSATION_THROW = "intermediateCompensationThrowEvent";
	  public const string INTERMEDIATE_EVENT_MESSAGE_THROW = "intermediateMessageThrowEvent";
	  public const string INTERMEDIATE_EVENT_NONE_THROW = "intermediateNoneThrowEvent";
	  public const string INTERMEDIATE_EVENT_ESCALATION_THROW = "intermediateEscalationThrowEvent";


	  //end events ////////////////////////////////////////
	  public const string END_EVENT_ERROR = "errorEndEvent";
	  public const string END_EVENT_CANCEL = "cancelEndEvent";
	  public const string END_EVENT_TERMINATE = "terminateEndEvent";
	  public const string END_EVENT_MESSAGE = "messageEndEvent";
	  public const string END_EVENT_SIGNAL = "signalEndEvent";
	  public const string END_EVENT_COMPENSATION = "compensationEndEvent";
	  public const string END_EVENT_ESCALATION = "escalationEndEvent";
	  public const string END_EVENT_NONE = "noneEndEvent";

	  /// <summary>
	  /// Should not be instantiated, since it makes no sense.
	  /// </summary>
	  private ActivityTypes()
	  {
	  }
	}
}