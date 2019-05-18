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
	using ActivationCondition = org.camunda.bpm.model.bpmn.instance.ActivationCondition;
	using ComplexGateway = org.camunda.bpm.model.bpmn.instance.ComplexGateway;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AbstractComplexGatewayBuilder<B> : AbstractGatewayBuilder<B, ComplexGateway> where B : AbstractComplexGatewayBuilder<B>
	{

	  protected internal AbstractComplexGatewayBuilder(BpmnModelInstance modelInstance, ComplexGateway element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the default sequence flow for the build complex gateway.
	  /// </summary>
	  /// <param name="sequenceFlow">  the default sequence flow to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B defaultFlow(SequenceFlow sequenceFlow)
	  {
		element.Default = sequenceFlow;
		return myself;
	  }

	  /// <summary>
	  /// Sets the activation condition expression for the build complex gateway
	  /// </summary>
	  /// <param name="conditionExpression">  the activation condition expression to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B activationCondition(string conditionExpression)
	  {
		ActivationCondition activationCondition = createInstance(typeof(ActivationCondition));
		activationCondition.TextContent = conditionExpression;
		element.ActivationCondition = activationCondition;
		return myself;
	  }

	}

}