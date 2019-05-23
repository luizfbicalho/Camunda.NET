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
namespace org.camunda.bpm.engine.externaltask
{

	/// <summary>
	/// Represents an instance of an external task that is created when
	/// a service-task like activity (i.e. service task, send task, ...) with
	/// attribute <code>camunda:type="external"</code> is executed.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public interface ExternalTask
	{

	  /// <returns> the id of the task </returns>
	  string Id {get;}

	  /// <returns> the name of the topic the task belongs to </returns>
	  string TopicName {get;}

	  /// <returns> the id of the worker that has locked the task </returns>
	  string WorkerId {get;}

	  /// <returns> the absolute time at which the lock expires </returns>
	  DateTime LockExpirationTime {get;}

	  /// <returns> the id of the process instance the task exists in </returns>
	  string ProcessInstanceId {get;}

	  /// <returns> the id of the execution that the task is assigned to </returns>
	  string ExecutionId {get;}

	  /// <returns> the id of the activity for which the task is created </returns>
	  string ActivityId {get;}

	  /// <returns> the id of the activity instance in which context the task exists </returns>
	  string ActivityInstanceId {get;}

	  /// <returns> the id of the process definition the task's activity belongs to </returns>
	  string ProcessDefinitionId {get;}

	  /// <returns> the key of the process definition the task's activity belongs to </returns>
	  string ProcessDefinitionKey {get;}

	  /// <returns> the number of retries left. The number of retries is provided by
	  ///   a task client, therefore the initial value is <code>null</code>. </returns>
	  int? Retries {get;}

	  /// <returns> short error message submitted with the latest reported failure executing this task;
	  ///   <code>null</code> if no failure was reported previously or if no error message
	  ///   was submitted
	  /// </returns>
	  /// <seealso cref= ExternalTaskService#handleFailure(String, String,String, String, int, long)
	  /// 
	  /// To get the full error details,
	  /// use <seealso cref="ExternalTaskService.getExternalTaskErrorDetails(string)"/> </seealso>
	  string ErrorMessage {get;}

	  /// <returns> true if the external task is suspended; a suspended external task
	  /// cannot be completed, thereby preventing process continuation </returns>
	  bool Suspended {get;}

	  /// <returns> the id of the tenant the task belongs to. Can be <code>null</code>
	  /// if the task belongs to no single tenant. </returns>
	  string TenantId {get;}

	  /// <summary>
	  /// Returns the priority of the external task.
	  /// </summary>
	  /// <returns> the priority of the external task </returns>
	  long Priority {get;}


	  /// <summary>
	  /// Returns the business key of the process instance the external task belongs to
	  /// </summary>
	  /// <returns> the business key </returns>
	  string BusinessKey {get;}

	}

}