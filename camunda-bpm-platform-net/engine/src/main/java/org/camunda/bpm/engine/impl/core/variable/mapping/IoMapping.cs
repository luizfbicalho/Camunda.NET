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
namespace org.camunda.bpm.engine.impl.core.variable.mapping
{

	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;

	/// <summary>
	/// Maps variables in and out of a variable scope.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class IoMapping
	{

	  protected internal IList<InputParameter> inputParameters;

	  protected internal IList<OutputParameter> outputParameters;

	  public virtual void executeInputParameters(AbstractVariableScope variableScope)
	  {
		foreach (InputParameter inputParameter in InputParameters)
		{
		  inputParameter.execute(variableScope);
		}
	  }

	  public virtual void executeOutputParameters(AbstractVariableScope variableScope)
	  {
		foreach (OutputParameter outputParameter in OutputParameters)
		{
		  outputParameter.execute(variableScope);
		}
	  }

	  public virtual void addInputParameter(InputParameter param)
	  {
		if (inputParameters == null)
		{
		  inputParameters = new List<InputParameter>();
		}
		inputParameters.Add(param);
	  }

	  public virtual void addOutputParameter(OutputParameter param)
	  {
		if (outputParameters == null)
		{
		  outputParameters = new List<OutputParameter>();
		}
		outputParameters.Add(param);
	  }

	  public virtual IList<InputParameter> InputParameters
	  {
		  get
		  {
			if (inputParameters == null)
			{
			  return Collections.emptyList();
    
			}
			else
			{
			  return inputParameters;
			}
		  }
		  set
		  {
			this.inputParameters = value;
		  }
	  }


	  public virtual IList<OutputParameter> OutputParameters
	  {
		  get
		  {
			if (outputParameters == null)
			{
			  return Collections.emptyList();
    
			}
			else
			{
			  return outputParameters;
			}
		  }
	  }

	  public virtual IList<OutputParameter> OuputParameters
	  {
		  set
		  {
			this.outputParameters = value;
		  }
	  }

	}

}