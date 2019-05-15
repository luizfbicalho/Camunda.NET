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
namespace org.camunda.bpm.engine.impl.core.variable.mapping
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;

	/// <summary>
	/// An <seealso cref="IoParameter"/> creates a variable
	/// in a target variable scope.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public abstract class IoParameter
	{

	  /// <summary>
	  /// The name of the parameter. The name of the parameter is the
	  /// variable name in the target <seealso cref="VariableScope"/>.
	  /// </summary>
	  protected internal string name;

	  /// <summary>
	  /// The provider of the parameter value.
	  /// </summary>
	  protected internal ParameterValueProvider valueProvider;

	  public IoParameter(string name, ParameterValueProvider valueProvider)
	  {
		this.name = name;
		this.valueProvider = valueProvider;
	  }

	  /// <summary>
	  /// Execute the parameter in a given variable scope.
	  /// </summary>
	  public virtual void execute(AbstractVariableScope scope)
	  {
		execute(scope, scope.ParentVariableScope);
	  }

	   /// <param name="innerScope"> </param>
	   /// <param name="outerScope"> </param>
	  protected internal abstract void execute(AbstractVariableScope innerScope, AbstractVariableScope outerScope);

	  // getters / setters ///////////////////////////

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual ParameterValueProvider ValueProvider
	  {
		  get
		  {
			return valueProvider;
		  }
		  set
		  {
			this.valueProvider = value;
		  }
	  }


	}

}