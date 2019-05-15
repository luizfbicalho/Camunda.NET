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
namespace org.camunda.bpm.engine.impl.core.variable.scope
{

	using VariablesProvider = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore.VariablesProvider;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariableCollectionProvider<T> : VariablesProvider<T> where T : org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance
	{

	  protected internal ICollection<T> variables;

	  public VariableCollectionProvider(ICollection<T> variables)
	  {
		this.variables = variables;
	  }

	  public virtual ICollection<T> provideVariables()
	  {
		if (variables == null)
		{
		  return new List<T>();
		}
		else
		{
		  return variables;
		}
	  }

	  public virtual ICollection<T> provideVariables(ICollection<string> variablesNames)
	  {
		if (variablesNames == null)
		{
		  return provideVariables();
		}

		IList<T> result = new List<T>();
		if (variables != null)
		{
		  foreach (T variable in variables)
		  {
			if (variablesNames.Contains(variable.Name))
			{
			  result.Add(variable);
			}
		  }
		}
		return result;
	  }

	  public static VariableCollectionProvider<T> emptyVariables<T>() where T : org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance
	  {
		return new VariableCollectionProvider<T>(System.Linq.Enumerable.Empty<T>());
	  }

	}

}