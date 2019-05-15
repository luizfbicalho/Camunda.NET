using System;
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
namespace org.camunda.bpm.engine.impl.externaltask
{

	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class LockedExternalTaskImpl : LockedExternalTask
	{

	  protected internal string id;
	  protected internal string topicName;
	  protected internal string workerId;
	  protected internal DateTime lockExpirationTime;
	  protected internal int? retries;
	  protected internal string errorMessage;
	  protected internal string errorDetails;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string activityId;
	  protected internal string activityInstanceId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string tenantId;
	  protected internal long priority;
	  protected internal VariableMapImpl variables;
	  protected internal string businessKey;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string TopicName
	  {
		  get
		  {
			return topicName;
		  }
	  }

	  public virtual string WorkerId
	  {
		  get
		  {
			return workerId;
		  }
	  }

	  public virtual DateTime LockExpirationTime
	  {
		  get
		  {
			return lockExpirationTime;
		  }
	  }

	  public virtual int? Retries
	  {
		  get
		  {
			return retries;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
	  }

	  public virtual string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual VariableMap Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual string ErrorDetails
	  {
		  get
		  {
			return errorDetails;
		  }
	  }

	  public virtual long Priority
	  {
		  get
		  {
			return priority;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return businessKey;
		  }
	  }

	  /// <summary>
	  /// Construct representation of locked ExternalTask from corresponding entity.
	  /// During mapping variables will be collected,during collection variables will not be deserialized
	  /// and scope will not be set to local.
	  /// </summary>
	  /// <seealso cref= <seealso cref="org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope#collectVariables(VariableMapImpl, Collection, boolean, boolean)"/>
	  /// </seealso>
	  /// <param name="externalTaskEntity"> - source persistent entity to use for fields </param>
	  /// <param name="variablesToFetch"> - list of variable names to fetch, if null then all variables will be fetched </param>
	  /// <param name="isLocal"> - if true only local variables will be collected
	  /// </param>
	  /// <returns> object with all fields copied from the ExternalTaskEntity, error details fetched from the
	  /// database and variables attached </returns>
	  public static LockedExternalTaskImpl fromEntity(ExternalTaskEntity externalTaskEntity, IList<string> variablesToFetch, bool isLocal, bool deserializeVariables)
	  {
		LockedExternalTaskImpl result = new LockedExternalTaskImpl();
		result.id = externalTaskEntity.Id;
		result.topicName = externalTaskEntity.TopicName;
		result.workerId = externalTaskEntity.WorkerId;
		result.lockExpirationTime = externalTaskEntity.LockExpirationTime;
		result.retries = externalTaskEntity.Retries;
		result.errorMessage = externalTaskEntity.ErrorMessage;
		result.errorDetails = externalTaskEntity.ErrorDetails;

		result.processInstanceId = externalTaskEntity.ProcessInstanceId;
		result.executionId = externalTaskEntity.ExecutionId;
		result.activityId = externalTaskEntity.ActivityId;
		result.activityInstanceId = externalTaskEntity.ActivityInstanceId;
		result.processDefinitionId = externalTaskEntity.ProcessDefinitionId;
		result.processDefinitionKey = externalTaskEntity.ProcessDefinitionKey;
		result.tenantId = externalTaskEntity.TenantId;
		result.priority = externalTaskEntity.Priority;
		result.businessKey = externalTaskEntity.BusinessKey;

		ExecutionEntity execution = externalTaskEntity.Execution;
		result.variables = new VariableMapImpl();
		execution.collectVariables(result.variables, variablesToFetch, isLocal, deserializeVariables);

		return result;
	  }
	}

}