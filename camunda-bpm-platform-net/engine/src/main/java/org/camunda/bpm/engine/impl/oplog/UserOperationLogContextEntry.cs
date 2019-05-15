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
namespace org.camunda.bpm.engine.impl.oplog
{

	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	/// <summary>
	/// One op log context entry represents an operation on a set of entities of the same type (see entityType field).
	/// It consist multiple <seealso cref="PropertyChange"/>s that end up as multiple history events.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class UserOperationLogContextEntry
	{

	  protected internal string deploymentId;
	  protected internal string processDefinitionId;
	  protected internal string processDefinitionKey;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string caseDefinitionId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string taskId;
	  protected internal string operationType;
	  protected internal string entityType;
	  protected internal IList<PropertyChange> propertyChanges;
	  protected internal string jobDefinitionId;
	  protected internal string jobId;
	  protected internal string batchId;
	  protected internal string category;
	  protected internal string rootProcessInstanceId;
	  protected internal string externalTaskId;

	  public UserOperationLogContextEntry(string operationType, string entityType)
	  {
		this.operationType = operationType;
		this.entityType = entityType;
	  }

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId;
		  }
		  set
		  {
			this.executionId = value;
		  }
	  }


	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
		  set
		  {
			this.caseDefinitionId = value;
		  }
	  }


	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
		  set
		  {
			this.caseInstanceId = value;
		  }
	  }


	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
		  set
		  {
			this.caseExecutionId = value;
		  }
	  }


	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }


	  public virtual string OperationType
	  {
		  get
		  {
			return operationType;
		  }
		  set
		  {
			this.operationType = value;
		  }
	  }


	  public virtual string EntityType
	  {
		  get
		  {
			return entityType;
		  }
		  set
		  {
			this.entityType = value;
		  }
	  }


	  public virtual IList<PropertyChange> PropertyChanges
	  {
		  get
		  {
			return propertyChanges;
		  }
		  set
		  {
			this.propertyChanges = value;
		  }
	  }


	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey;
		  }
		  set
		  {
			this.processDefinitionKey = value;
		  }
	  }


	  public virtual string JobDefinitionId
	  {
		  get
		  {
			return jobDefinitionId;
		  }
		  set
		  {
			this.jobDefinitionId = value;
		  }
	  }


	  public virtual string JobId
	  {
		  get
		  {
			return jobId;
		  }
		  set
		  {
			this.jobId = value;
		  }
	  }


	  public virtual string BatchId
	  {
		  get
		  {
			return batchId;
		  }
		  set
		  {
			this.batchId = value;
		  }
	  }


	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
		  set
		  {
			this.category = value;
		  }
	  }


	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId;
		  }
		  set
		  {
			this.rootProcessInstanceId = value;
		  }
	  }


	  public virtual string ExternalTaskId
	  {
		  get
		  {
			return externalTaskId;
		  }
		  set
		  {
			this.externalTaskId = value;
		  }
	  }


	}

}