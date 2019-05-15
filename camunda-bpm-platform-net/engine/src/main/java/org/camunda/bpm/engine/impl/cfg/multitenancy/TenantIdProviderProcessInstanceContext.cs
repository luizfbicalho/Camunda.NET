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
namespace org.camunda.bpm.engine.impl.cfg.multitenancy
{
	using DelegateCaseExecution = org.camunda.bpm.engine.@delegate.DelegateCaseExecution;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// Provides information about a starting process instance to a <seealso cref="TenantIdProvider"/> implementation.
	/// 
	/// @author Daniel Meyer
	/// @since 7.5
	/// </summary>
	public class TenantIdProviderProcessInstanceContext
	{

	  protected internal ProcessDefinition processDefinition;

	  protected internal VariableMap variables;

	  protected internal DelegateExecution superExecution;

	  protected internal DelegateCaseExecution superCaseExecution;

	  public TenantIdProviderProcessInstanceContext(ProcessDefinition processDefinition, VariableMap variables)
	  {
		this.processDefinition = processDefinition;
		this.variables = variables;
	  }

	  public TenantIdProviderProcessInstanceContext(ProcessDefinition processDefinition, VariableMap variables, DelegateExecution superExecution) : this(processDefinition, variables)
	  {
		this.superExecution = superExecution;
	  }

	  public TenantIdProviderProcessInstanceContext(ProcessDefinition processDefinition, VariableMap variables, DelegateCaseExecution superCaseExecution) : this(processDefinition, variables)
	  {
		this.superCaseExecution = superCaseExecution;
	  }

	  /// <returns> the process definition of the process instance which is being started </returns>
	  public virtual ProcessDefinition ProcessDefinition
	  {
		  get
		  {
			return processDefinition;
		  }
	  }

	  /// <returns> the variables which were passed to the starting process instance </returns>
	  public virtual VariableMap Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  /// <returns> the super execution. Null if the starting process instance is a root process instance and not started using a call activity.
	  /// If the process instance is started using a call activity, this method returns the execution in the super process
	  /// instance executing the call activity. </returns>
	  public virtual DelegateExecution SuperExecution
	  {
		  get
		  {
			return superExecution;
		  }
	  }

	  /// <returns> the super case execution. Null if the starting process instance is not a sub process instance started using a CMMN case task. </returns>
	  public virtual DelegateCaseExecution SuperCaseExecution
	  {
		  get
		  {
			return superCaseExecution;
		  }
	  }

	}

}