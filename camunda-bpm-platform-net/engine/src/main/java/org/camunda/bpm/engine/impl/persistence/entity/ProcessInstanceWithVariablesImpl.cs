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
namespace org.camunda.bpm.engine.impl.persistence.entity
{
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class ProcessInstanceWithVariablesImpl : ProcessInstanceWithVariables
	{

	  protected internal readonly ExecutionEntity executionEntity;
	  protected internal readonly VariableMap variables;

	  public ProcessInstanceWithVariablesImpl(ExecutionEntity executionEntity, VariableMap variables)
	  {
		this.executionEntity = executionEntity;
		this.variables = variables;
	  }

	  public virtual ExecutionEntity ExecutionEntity
	  {
		  get
		  {
			return executionEntity;
		  }
	  }

	  public virtual VariableMap Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return executionEntity.ProcessDefinitionId;
		  }
	  }

	  public virtual string BusinessKey
	  {
		  get
		  {
			return executionEntity.BusinessKey;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return executionEntity.CaseInstanceId;
		  }
	  }

	  public virtual bool Suspended
	  {
		  get
		  {
			return executionEntity.Suspended;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return executionEntity.Id;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return executionEntity.RootProcessInstanceId;
		  }
	  }

	  public virtual bool Ended
	  {
		  get
		  {
			return executionEntity.Ended;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return executionEntity.ProcessInstanceId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return executionEntity.TenantId;
		  }
	  }
	}

}