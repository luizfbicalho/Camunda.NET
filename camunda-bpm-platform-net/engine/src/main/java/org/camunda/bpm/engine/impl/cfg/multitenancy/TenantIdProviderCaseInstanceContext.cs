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
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// Provides information about a starting case instance to a <seealso cref="TenantIdProvider"/> implementation.
	/// 
	/// @author Kristin Polenz
	/// @since 7.5
	/// </summary>
	public class TenantIdProviderCaseInstanceContext
	{

	  protected internal CaseDefinition caseDefinition;

	  protected internal VariableMap variables;

	  protected internal DelegateExecution superExecution;

	  protected internal DelegateCaseExecution superCaseExecution;

	  public TenantIdProviderCaseInstanceContext(CaseDefinition caseDefinition, VariableMap variables)
	  {
		this.caseDefinition = caseDefinition;
		this.variables = variables;
	  }

	  public TenantIdProviderCaseInstanceContext(CaseDefinition caseDefinition, VariableMap variables, DelegateExecution superExecution) : this(caseDefinition, variables)
	  {
		this.superExecution = superExecution;
	  }

	  public TenantIdProviderCaseInstanceContext(CaseDefinition caseDefinition, VariableMap variables, DelegateCaseExecution superCaseExecution) : this(caseDefinition, variables)
	  {
		this.superCaseExecution = superCaseExecution;
	  }

	  /// <returns> the case definition of the case instance which is being started </returns>
	  public virtual CaseDefinition CaseDefinition
	  {
		  get
		  {
			return caseDefinition;
		  }
	  }

	  /// <returns> the variables which were passed to the starting case instance </returns>
	  public virtual VariableMap Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  /// <returns> the super execution. <code>null</code> if the starting case instance is a root process instance and not started using a call activity.
	  /// If the case instance is started using a call activity, this method returns the execution in the super process
	  /// instance executing the call activity. </returns>
	  public virtual DelegateExecution SuperExecution
	  {
		  get
		  {
			return superExecution;
		  }
	  }

	  /// <returns> the super case execution. <code>null</code> if the starting case instance is not a sub case instance started using a CMMN case task. </returns>
	  public virtual DelegateCaseExecution SuperCaseExecution
	  {
		  get
		  {
			return superCaseExecution;
		  }
	  }

	}

}