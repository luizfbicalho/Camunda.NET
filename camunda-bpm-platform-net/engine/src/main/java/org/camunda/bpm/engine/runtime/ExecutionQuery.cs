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
namespace org.camunda.bpm.engine.runtime
{

	using Query = org.camunda.bpm.engine.query.Query;



	/// <summary>
	/// Allows programmatic querying of <seealso cref="Execution"/>s.
	/// 
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	public interface ExecutionQuery : Query<ExecutionQuery, Execution>
	{

	  /// <summary>
	  /// Only select executions which have the given process definition key. * </summary>
	  ExecutionQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select executions which have the given process definition id. * </summary>
	  ExecutionQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select executions which have the given process instance id. * </summary>
	  ExecutionQuery processInstanceId(string processInstanceId);

	  /// <summary>
	  /// Only select executions that belong to a process instance with the given business key </summary>
	  ExecutionQuery processInstanceBusinessKey(string processInstanceBusinessKey);

	  /// <summary>
	  /// Only select executions with the given id. * </summary>
	  ExecutionQuery executionId(string executionId);

	  /// <summary>
	  /// Only select executions which contain an activity with the given id. * </summary>
	  ExecutionQuery activityId(string activityId);

	  /// <summary>
	  /// Only select executions which have a local variable with the given value. The type
	  /// of variable is determined based on the value, using types configured in
	  /// <seealso cref="ProcessEngineConfiguration#getVariableSerializers()"/>.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  ExecutionQuery variableValueEquals(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> name of the variable, cannot be null. </param>
	  ExecutionQuery variableValueNotEquals(string name, object value);


	  /// <summary>
	  /// Only select executions which have a local variable value greater than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueGreaterThan(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value greater than or equal to
	  /// the passed value. Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which
	  /// are not primitive type wrappers) are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueGreaterThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value less than the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueLessThan(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value less than or equal to the passed value.
	  /// Booleans, Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. </param>
	  ExecutionQuery variableValueLessThanOrEqual(string name, object value);

	  /// <summary>
	  /// Only select executions which have a local variable value like the given value.
	  /// This be used on string variables only. </summary>
	  /// <param name="name"> variable name, cannot be null. </param>
	  /// <param name="value"> variable value, cannot be null. The string can include the
	  /// wildcard character '%' to express like-strategy:
	  /// starts with (string%), ends with (%string) or contains (%string%). </param>
	  ExecutionQuery variableValueLike(string name, string value);

	  /// <summary>
	  /// Only select executions which are part of a process that have a variable
	  /// with the given name set to the given value.
	  /// </summary>
	  ExecutionQuery processVariableValueEquals(string variableName, object variableValue);

	  /// <summary>
	  /// Only select executions which are part of a process that have a variable  with the given name, but
	  /// with a different value than the passed value.
	  /// Byte-arrays and <seealso cref="Serializable"/> objects (which are not primitive type wrappers)
	  /// are not supported.
	  /// </summary>
	  ExecutionQuery processVariableValueNotEquals(string variableName, object variableValue);

	  // event subscriptions //////////////////////////////////////////////////

	  /// <seealso cref= #signalEventSubscriptionName(String) </seealso>
	  [Obsolete]
	  ExecutionQuery signalEventSubscription(string signalName);

	  /// <summary>
	  /// Only select executions which have a signal event subscription
	  /// for the given signal name.
	  /// 
	  /// (The signalName is specified using the 'name' attribute of the signal element
	  /// in the BPMN 2.0 XML.)
	  /// </summary>
	  /// <param name="signalName"> the name of the signal the execution has subscribed to </param>
	  ExecutionQuery signalEventSubscriptionName(string signalName);

	  /// <summary>
	  /// Only select executions which have a message event subscription
	  /// for the given messageName.
	  /// 
	  /// (The messageName is specified using the 'name' attribute of the message element
	  /// in the BPMN 2.0 XML.)
	  /// </summary>
	  /// <param name="messageName"> the name of the message the execution has subscribed to </param>
	  ExecutionQuery messageEventSubscriptionName(string messageName);

	  /// <summary>
	  /// Only select executions that have a message event subscription.
	  /// Use <seealso cref="#messageEventSubscriptionName(String)"/> to filter for executions
	  /// with message event subscriptions with a certain name.
	  /// </summary>
	  ExecutionQuery messageEventSubscription();

	  /// <summary>
	  /// Only selects executions which are suspended, because their process instance is suspended.
	  /// </summary>
	  ExecutionQuery suspended();

	  /// <summary>
	  /// Only selects executions which are active (i.e. not suspended).
	  /// </summary>
	  ExecutionQuery active();

	  /// <summary>
	  /// Only selects executions with the given incident type.
	  /// </summary>
	  ExecutionQuery incidentType(string incidentType);

	  /// <summary>
	  /// Only selects executions with the given incident id.
	  /// </summary>
	  ExecutionQuery incidentId(string incidentId);

	  /// <summary>
	  /// Only selects executions with the given incident message.
	  /// </summary>
	  ExecutionQuery incidentMessage(string incidentMessage);

	  /// <summary>
	  /// Only selects executions with an incident message like the given.
	  /// </summary>
	  ExecutionQuery incidentMessageLike(string incidentMessageLike);

	   /// <summary>
	   /// Only selects executions with one of the given tenant ids. </summary>
	  ExecutionQuery tenantIdIn(params string[] tenantIds);

	  /// <summary>
	  /// Only selects executions which have no tenant id. </summary>
	  ExecutionQuery withoutTenantId();

	  //ordering //////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ExecutionQuery orderByProcessInstanceId();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ExecutionQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by process definition id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ExecutionQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>).
	  /// Note that the ordering of executions without tenant id is database-specific. 
	  /// </summary>
	  ExecutionQuery orderByTenantId();

	}

}