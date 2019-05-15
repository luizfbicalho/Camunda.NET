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
namespace org.camunda.bpm.engine.impl.pvm.runtime
{

	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ExecutionStartContext
	{

	  protected internal bool delayFireHistoricVariableEvents;

	  protected internal InstantiationStack instantiationStack;
	  protected internal IDictionary<string, object> variables;
	  protected internal IDictionary<string, object> variablesLocal;

	  public ExecutionStartContext() : this(true)
	  {
	  }

	  public ExecutionStartContext(bool delayFireHistoricVariableEvents)
	  {
		this.delayFireHistoricVariableEvents = delayFireHistoricVariableEvents;
	  }

	  public virtual void executionStarted(PvmExecutionImpl execution)
	  {
		PvmExecutionImpl parent = execution;
		while (parent != null && parent.ExecutionStartContext != null)
		{
		  if (parent is ExecutionEntity && delayFireHistoricVariableEvents)
		  {
			// with the fix of CAM-9249 we presume that the parent and the child have the same startContext
			ExecutionEntity executionEntity = (ExecutionEntity) parent;
			executionEntity.fireHistoricVariableInstanceCreateEvents();
		  }

		  parent.disposeExecutionStartContext();
		  parent = parent.Parent;
		}
	  }

	  public virtual void applyVariables(CoreExecution execution)
	  {
		execution.Variables = variables;
		execution.VariablesLocal = variablesLocal;
	  }

	  public virtual bool DelayFireHistoricVariableEvents
	  {
		  get
		  {
			return delayFireHistoricVariableEvents;
		  }
	  }

	  public virtual InstantiationStack InstantiationStack
	  {
		  get
		  {
			return instantiationStack;
		  }
		  set
		  {
			this.instantiationStack = value;
		  }
	  }


	  public virtual IDictionary<string, object> Variables
	  {
		  set
		  {
			this.variables = value;
		  }
	  }

	  public virtual IDictionary<string, object> VariablesLocal
	  {
		  set
		  {
			this.variablesLocal = value;
		  }
	  }
	}

}