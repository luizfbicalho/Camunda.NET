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
namespace org.camunda.bpm.engine.variable.impl.context
{

	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CompositeVariableContext : VariableContext
	{

	  protected internal readonly VariableContext[] delegateContexts;

	  public CompositeVariableContext(VariableContext[] delegateContexts)
	  {
		this.delegateContexts = delegateContexts;
	  }

	  public virtual TypedValue resolve(string variableName)
	  {
		foreach (VariableContext variableContext in delegateContexts)
		{
		  TypedValue resolvedValue = variableContext.resolve(variableName);
		  if (resolvedValue != null)
		  {
			return resolvedValue;
		  }
		}

		return null;
	  }

	  public virtual bool containsVariable(string name)
	  {
		foreach (VariableContext variableContext in delegateContexts)
		{
		  if (variableContext.containsVariable(name))
		  {
			return true;
		  }
		}

		return false;
	  }

	  public virtual ISet<string> keySet()
	  {
		ISet<string> keySet = new HashSet<string>();
		foreach (VariableContext variableContext in delegateContexts)
		{
		  keySet.addAll(variableContext.Keys);
		}
		return keySet;
	  }

	  public static CompositeVariableContext compose(params VariableContext[] variableContexts)
	  {
		return new CompositeVariableContext(variableContexts);
	  }

	}

}