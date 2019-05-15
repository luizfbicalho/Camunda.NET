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
namespace org.camunda.bpm.engine.impl.core
{
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CoreLogger : ProcessEngineLogger
	{

	  public virtual void debugMappingValueFromOuterScopeToInnerScope(object value, AbstractVariableScope outerScope, string name, AbstractVariableScope innerScope)
	  {
		logDebug("001", "Mapping value '{} from outer scope '{}' to variable '{}' in inner scope '{}'.", value, outerScope, name, innerScope);
	  }

	  public virtual void debugMappingValuefromInnerScopeToOuterScope(object value, AbstractVariableScope innerScope, string name, AbstractVariableScope outerScope)
	  {
		logDebug("002", "Mapping value '{}' from inner scope '{}' to variable '{}' in outer scope '{}'.", value, innerScope, name, outerScope);
	  }

	  public virtual void debugPerformingAtomicOperation<T1>(CoreAtomicOperation<T1> atomicOperation, CoreExecution e)
	  {
		logDebug("003", "Performing atomic operation {} on {}", atomicOperation, e);
	  }

	  public virtual ProcessEngineException duplicateVariableInstanceException(CoreVariableInstance variableInstance)
	  {
		return new ProcessEngineException(exceptionMessage("004", "Cannot add variable instance with name {}. Variable already exists", variableInstance.Name));
	  }

	  public virtual ProcessEngineException missingVariableInstanceException(CoreVariableInstance variableInstance)
	  {
		return new ProcessEngineException(exceptionMessage("005", "Cannot update variable instance with name {}. Variable does not exist", variableInstance.Name));
	  }

	  public virtual ProcessEngineException transientVariableException(string variableName)
	  {
		return new ProcessEngineException(exceptionMessage("006", "Cannot set transient variable with name {} to non-transient variable and vice versa.", variableName));
	  }

	  public virtual ProcessEngineException javaSerializationProhibitedException(string variableName)
	  {
		return new ProcessEngineException(exceptionMessage("007", "Cannot set variable with name {}. Java serialization format is prohibited", variableName));
	  }

	}

}