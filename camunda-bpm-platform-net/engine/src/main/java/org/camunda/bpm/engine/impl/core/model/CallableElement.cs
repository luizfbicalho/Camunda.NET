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
namespace org.camunda.bpm.engine.impl.core.model
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CallableElement : BaseCallableElement
	{

	  protected internal ParameterValueProvider businessKeyValueProvider;
	  protected internal IList<CallableElementParameter> inputs;
	  protected internal IList<CallableElementParameter> outputs;
	  protected internal IList<CallableElementParameter> outputsLocal;

	  public CallableElement()
	  {
		this.inputs = new List<CallableElementParameter>();
		this.outputs = new List<CallableElementParameter>();
		this.outputsLocal = new List<CallableElementParameter>();
	  }

	  // definitionKey ////////////////////////////////////////////////////////////////

	  // binding /////////////////////////////////////////////////////////////////////

	  // version //////////////////////////////////////////////////////////////////////

	  // businessKey /////////////////////////////////////////////////////////////////

	  public virtual string getBusinessKey(VariableScope variableScope)
	  {
		if (businessKeyValueProvider == null)
		{
		  return null;
		}

		object result = businessKeyValueProvider.getValue(variableScope);

		if (result != null && !(result is string))
		{
		  throw new System.InvalidCastException("Cannot cast '" + result + "' to String");
		}

		return (string) result;
	  }

	  public virtual ParameterValueProvider BusinessKeyValueProvider
	  {
		  get
		  {
			return businessKeyValueProvider;
		  }
		  set
		  {
			this.businessKeyValueProvider = value;
		  }
	  }


	  // inputs //////////////////////////////////////////////////////////////////////

	  public virtual IList<CallableElementParameter> Inputs
	  {
		  get
		  {
			return inputs;
		  }
	  }

	  public virtual void addInput(CallableElementParameter input)
	  {
		inputs.Add(input);
	  }

	  public virtual void addInputs(IList<CallableElementParameter> inputs)
	  {
		((IList<CallableElementParameter>)this.inputs).AddRange(inputs);
	  }

	  public virtual VariableMap getInputVariables(VariableScope variableScope)
	  {
		IList<CallableElementParameter> inputs = Inputs;
		return getVariables(inputs, variableScope);
	  }

	  // outputs /////////////////////////////////////////////////////////////////////

	  public virtual IList<CallableElementParameter> Outputs
	  {
		  get
		  {
			return outputs;
		  }
	  }

	  public virtual IList<CallableElementParameter> OutputsLocal
	  {
		  get
		  {
			return outputsLocal;
		  }
	  }

	  public virtual void addOutput(CallableElementParameter output)
	  {
		outputs.Add(output);
	  }

	  public virtual void addOutputLocal(CallableElementParameter output)
	  {
		outputsLocal.Add(output);
	  }

	  public virtual void addOutputs(IList<CallableElementParameter> outputs)
	  {
		((IList<CallableElementParameter>)this.outputs).AddRange(outputs);
	  }

	  public virtual VariableMap getOutputVariables(VariableScope calledElementScope)
	  {
		IList<CallableElementParameter> outputs = Outputs;
		return getVariables(outputs, calledElementScope);
	  }

	  public virtual VariableMap getOutputVariablesLocal(VariableScope calledElementScope)
	  {
		IList<CallableElementParameter> outputs = OutputsLocal;
		return getVariables(outputs, calledElementScope);
	  }

	  // variables //////////////////////////////////////////////////////////////////

	  protected internal virtual VariableMap getVariables(IList<CallableElementParameter> @params, VariableScope variableScope)
	  {
		VariableMap result = Variables.createVariables();

		foreach (CallableElementParameter param in @params)
		{
		  param.applyTo(variableScope, result);
		}

		return result;
	  }

	  // deployment id //////////////////////////////////////////////////////////////

	}

}