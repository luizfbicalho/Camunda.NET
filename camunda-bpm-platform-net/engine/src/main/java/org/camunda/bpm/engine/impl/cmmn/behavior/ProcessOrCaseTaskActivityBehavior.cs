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
namespace org.camunda.bpm.engine.impl.cmmn.behavior
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CallableElement = org.camunda.bpm.engine.impl.core.model.CallableElement;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class ProcessOrCaseTaskActivityBehavior : CallingTaskActivityBehavior, TransferVariablesActivityBehavior
	{

	  protected internal override void performStart(CmmnActivityExecution execution)
	  {
		VariableMap variables = getInputVariables(execution);
		string businessKey = getBusinessKey(execution);
		triggerCallableElement(execution, variables, businessKey);

		if (execution.Active && !isBlocking(execution))
		{
		  execution.complete();
		}
	  }

	  public virtual void transferVariables(VariableScope sourceScope, CmmnActivityExecution caseExecution)
	  {
		VariableMap variables = getOutputVariables(sourceScope);
		caseExecution.Variables = variables;
	  }

	  public override CallableElement CallableElement
	  {
		  get
		  {
			return (CallableElement) callableElement;
		  }
	  }

	  protected internal virtual string getBusinessKey(CmmnActivityExecution execution)
	  {
		return CallableElement.getBusinessKey(execution);
	  }

	  protected internal virtual VariableMap getInputVariables(CmmnActivityExecution execution)
	  {
		return CallableElement.getInputVariables(execution);
	  }

	  protected internal virtual VariableMap getOutputVariables(VariableScope variableScope)
	  {
		return CallableElement.getOutputVariables(variableScope);
	  }

	  protected internal abstract void triggerCallableElement(CmmnActivityExecution execution, IDictionary<string, object> variables, string businessKey);

	}

}