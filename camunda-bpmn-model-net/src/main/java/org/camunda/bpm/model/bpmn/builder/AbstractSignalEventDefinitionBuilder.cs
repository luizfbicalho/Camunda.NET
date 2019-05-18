using System;

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
namespace org.camunda.bpm.model.bpmn.builder
{
	using SignalEventDefinition = org.camunda.bpm.model.bpmn.instance.SignalEventDefinition;
	using CamundaIn = org.camunda.bpm.model.bpmn.instance.camunda.CamundaIn;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public abstract class AbstractSignalEventDefinitionBuilder<B> : AbstractRootElementBuilder<B, SignalEventDefinition> where B : AbstractSignalEventDefinitionBuilder<B>
	{

	  protected internal AbstractSignalEventDefinitionBuilder(BpmnModelInstance modelInstance, SignalEventDefinition element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets a "camunda:in" parameter to pass a variable from the signal-throwing
	  /// process instance to the signal-catching process instance
	  /// </summary>
	  /// <param name="source"> the name of the variable in the signal-throwing process instance </param>
	  /// <param name="target"> the name of the variable in the signal-catching process instance </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInSourceTarget(string source, string target)
	  {
		CamundaIn param = modelInstance.newInstance(typeof(CamundaIn));

		param.CamundaSource = source;
		param.CamundaTarget = target;

		addExtensionElement(param);

		return myself;
	  }

	  /// <summary>
	  /// Sets a "camunda:in" parameter to pass an expression from the signal-throwing
	  /// process instance to a variable in the signal-catching process instance
	  /// </summary>
	  /// <param name="sourceExpression"> the expression in the signal-throwing process instance </param>
	  /// <param name="target"> the name of the variable in the signal-catching process instance </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInSourceExpressionTarget(string sourceExpression, string target)
	  {
		CamundaIn param = modelInstance.newInstance(typeof(CamundaIn));

		param.CamundaSourceExpression = sourceExpression;
		param.CamundaTarget = target;

		addExtensionElement(param);

		return myself;
	  }

	  /// <summary>
	  /// Sets a "camunda:in" parameter to pass the business key from the signal-throwing
	  /// process instance to the signal-catching process instance
	  /// </summary>
	  /// <param name="businessKey"> the business key string or expression of the signal-throwing process instance </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInBusinessKey(string businessKey)
	  {
		CamundaIn param = modelInstance.newInstance(typeof(CamundaIn));

		param.CamundaBusinessKey = businessKey;

		addExtensionElement(param);

		return myself;
	  }

	  /// <summary>
	  /// Sets a "camunda:in" parameter to pass all the process variables of the
	  /// signal-throwing process instance to the signal-catching process instance
	  /// </summary>
	  /// <param name="variables"> a String flag to declare that all of the signal-throwing process-instance variables should be passed </param>
	  /// <param name="local"> a Boolean flag to declare that only the local variables should be passed </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInAllVariables(string variables, bool local)
	  {
		CamundaIn param = modelInstance.newInstance(typeof(CamundaIn));

		param.CamundaVariables = variables;

		if (local)
		{
		  param.CamundaLocal = local;
		}

		addExtensionElement(param);

		return myself;
	  }

	  /// <summary>
	  /// Sets a "camunda:in" parameter to pass all the process variables of the
	  /// signal-throwing process instance to the signal-catching process instance
	  /// </summary>
	  /// <param name="variables"> a String flag to declare that all of the signal-throwing process-instance variables should be passed </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaInAllVariables(string variables)
	  {
		return camundaInAllVariables(variables, false);
	  }
	}

}