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
namespace org.camunda.bpm.engine.impl.variable.listener
{
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateCaseVariableInstance = org.camunda.bpm.engine.@delegate.DelegateCaseVariableInstance;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class DelegateCaseVariableInstanceImpl : DelegateCaseVariableInstance
	{

	  protected internal string eventName;
	  protected internal DelegateCaseExecution sourceExecution;
	  protected internal DelegateCaseExecution scopeExecution;

	  // fields copied from variable instance
	  protected internal string variableId;
	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string caseInstanceId;
	  protected internal string caseExecutionId;
	  protected internal string taskId;
	  protected internal string activityInstanceId;
	  protected internal string tenantId;
	  protected internal string errorMessage;
	  protected internal string name;
	  protected internal TypedValue value;

	  public virtual string EventName
	  {
		  get
		  {
			return eventName;
		  }
		  set
		  {
			this.eventName = value;
		  }
	  }


	  public virtual DelegateCaseExecution SourceExecution
	  {
		  get
		  {
			return sourceExecution;
		  }
		  set
		  {
			this.sourceExecution = value;
		  }
	  }


	  /// <summary>
	  /// Currently not part of public interface.
	  /// </summary>
	  public virtual DelegateCaseExecution ScopeExecution
	  {
		  get
		  {
			return scopeExecution;
		  }
		  set
		  {
			this.scopeExecution = value;
		  }
	  }


	  //// methods delegated to wrapped variable ////

	  public virtual string Id
	  {
		  get
		  {
			return variableId;
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

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual string TypeName
	  {
		  get
		  {
			if (value != null)
			{
			  return value.Type.Name;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			if (value != null)
			{
			  return value.Value;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual ProcessEngineServices ProcessEngineServices
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }

	  public virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }

	  public static DelegateCaseVariableInstanceImpl fromVariableInstance(VariableInstance variableInstance)
	  {
		DelegateCaseVariableInstanceImpl delegateInstance = new DelegateCaseVariableInstanceImpl();
		delegateInstance.variableId = variableInstance.Id;
		delegateInstance.processInstanceId = variableInstance.ProcessInstanceId;
		delegateInstance.executionId = variableInstance.ExecutionId;
		delegateInstance.caseExecutionId = variableInstance.CaseExecutionId;
		delegateInstance.caseInstanceId = variableInstance.CaseInstanceId;
		delegateInstance.taskId = variableInstance.TaskId;
		delegateInstance.activityInstanceId = variableInstance.ActivityInstanceId;
		delegateInstance.tenantId = variableInstance.TenantId;
		delegateInstance.errorMessage = variableInstance.ErrorMessage;
		delegateInstance.name = variableInstance.Name;
		delegateInstance.value = variableInstance.TypedValue;

		return delegateInstance;
	  }

	}
}