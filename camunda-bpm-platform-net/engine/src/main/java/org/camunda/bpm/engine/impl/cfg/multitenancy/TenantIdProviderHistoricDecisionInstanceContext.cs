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
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;

	/// <summary>
	/// Provides information about a historic decision instance to a <seealso cref="TenantIdProvider"/> implementation.
	/// 
	/// @author Kristin Polenz
	/// @since 7.5
	/// </summary>
	public class TenantIdProviderHistoricDecisionInstanceContext
	{

	  protected internal DecisionDefinition decisionDefinition;

	  protected internal DelegateExecution execution;

	  protected internal DelegateCaseExecution caseExecution;

	  public TenantIdProviderHistoricDecisionInstanceContext(DecisionDefinition decisionDefinition)
	  {
		this.decisionDefinition = decisionDefinition;
	  }

	  public TenantIdProviderHistoricDecisionInstanceContext(DecisionDefinition decisionDefinition, DelegateExecution execution) : this(decisionDefinition)
	  {
		this.execution = execution;
	  }

	  public TenantIdProviderHistoricDecisionInstanceContext(DecisionDefinition decisionDefinition, DelegateCaseExecution caseExecution) : this(decisionDefinition)
	  {
		this.caseExecution = caseExecution;
	  }

	  /// <returns> the decision definition of the historic decision instance which is being evaluated </returns>
	  public virtual DecisionDefinition DecisionDefinition
	  {
		  get
		  {
			return decisionDefinition;
		  }
	  }

	  /// <returns> the execution. This method returns the execution of the process instance
	  /// which evaluated the decision definition. </returns>
	  public virtual DelegateExecution Execution
	  {
		  get
		  {
			return execution;
		  }
	  }

	  /// <returns> the case execution. This method returns the case execution of the CMMN case task
	  /// which evaluated the decision definition. </returns>
	  public virtual DelegateCaseExecution CaseExecution
	  {
		  get
		  {
			return caseExecution;
		  }
	  }

	}

}