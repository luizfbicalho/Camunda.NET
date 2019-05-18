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
namespace org.camunda.bpm.model.bpmn
{

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public sealed class BpmnTestConstants
	{

	  public const string COLLABORATION_ID = "collaboration";
	  public const string PARTICIPANT_ID = "participant";
	  public const string PROCESS_ID = "process";
	  public const string START_EVENT_ID = "startEvent";
	  public const string TASK_ID = "task";
	  public const string USER_TASK_ID = "userTask";
	  public const string SERVICE_TASK_ID = "serviceTask";
	  public const string EXTERNAL_TASK_ID = "externalTask";
	  public const string SEND_TASK_ID = "sendTask";
	  public const string SCRIPT_TASK_ID = "scriptTask";
	  public const string SEQUENCE_FLOW_ID = "sequenceFlow";
	  public const string MESSAGE_FLOW_ID = "messageFlow";
	  public const string DATA_INPUT_ASSOCIATION_ID = "dataInputAssociation";
	  public const string ASSOCIATION_ID = "association";
	  public const string CALL_ACTIVITY_ID = "callActivity";
	  public const string BUSINESS_RULE_TASK = "businessRuleTask";
	  public const string END_EVENT_ID = "endEvent";
	  public const string EXCLUSIVE_GATEWAY = "exclusiveGateway";
	  public const string SUB_PROCESS_ID = "subProcess";
	  public const string TRANSACTION_ID = "transaction";
	  public const string CONDITION_ID = "condition";
	  public const string BOUNDARY_ID = "boundary";
	  public const string CATCH_ID = "catch";

	  public const string TEST_STRING_XML = "test";
	  public const string TEST_STRING_API = "api";
	  public const string TEST_CLASS_XML = "org.camunda.test.Test";
	  public const string TEST_CLASS_API = "org.camunda.test.Api";
	  public static readonly string TEST_EXPRESSION_XML = "${" + TEST_STRING_XML + "}";
	  public static readonly string TEST_EXPRESSION_API = "${" + TEST_STRING_API + "}";
	  public static readonly string TEST_DELEGATE_EXPRESSION_XML = "${" + TEST_CLASS_XML + "}";
	  public static readonly string TEST_DELEGATE_EXPRESSION_API = "${" + TEST_CLASS_API + "}";
	  public const string TEST_GROUPS_XML = "group1, ${group2(a, b)}, group3";
	  public static readonly IList<string> TEST_GROUPS_LIST_XML = Arrays.asList("group1", "${group2(a, b)}", "group3");
	  public const string TEST_GROUPS_API = "#{group1( c,d)}, group5";
	  public static readonly IList<string> TEST_GROUPS_LIST_API = Arrays.asList("#{group1( c,d)}", "group5");
	  public const string TEST_USERS_XML = "user1, ${user2(a, b)}, user3";
	  public static readonly IList<string> TEST_USERS_LIST_XML = Arrays.asList("user1", "${user2(a, b)}", "user3");
	  public const string TEST_USERS_API = "#{user1( c,d)}, user5";
	  public static readonly IList<string> TEST_USERS_LIST_API = Arrays.asList("#{user1( c,d)}", "user5");
	  public const string TEST_DUE_DATE_XML = "2014-02-27";
	  public const string TEST_DUE_DATE_API = "2015-03-28";
	  public const string TEST_FOLLOW_UP_DATE_API = "2015-01-01";
	  public const string TEST_PRIORITY_XML = "12";
	  public const string TEST_PRIORITY_API = "${dateVariable}";
	  public const string TEST_TYPE_XML = "mail";
	  public const string TEST_TYPE_API = "shell";
	  public const string TEST_EXECUTION_EVENT_XML = "start";
	  public const string TEST_EXECUTION_EVENT_API = "end";
	  public const string TEST_TASK_EVENT_XML = "create";
	  public const string TEST_TASK_EVENT_API = "complete";
	  public const string TEST_FLOW_NODE_JOB_PRIORITY = "${test}";
	  public const string TEST_PROCESS_JOB_PRIORITY = "15";
	  public const string TEST_PROCESS_TASK_PRIORITY = "13";
	  public const string TEST_SERVICE_TASK_PRIORITY = "${test}";
	  public const string TEST_EXTERNAL_TASK_TOPIC = "${externalTaskTopic}";
	  public const int? TEST_HISTORY_TIME_TO_LIVE = 5;
	  public const bool? TEST_STARTABLE_IN_TASKLIST = false;
	  public const string TEST_VERSION_TAG = "v1.0.0";

	  public const string TEST_CONDITION = "${true}";
	  public const string TEST_CONDITIONAL_VARIABLE_NAME = "variable";
	  public const string TEST_CONDITIONAL_VARIABLE_EVENTS = "create, update";
	  public static readonly IList<string> TEST_CONDITIONAL_VARIABLE_EVENTS_LIST = Arrays.asList("create", "update");

	}

}