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
	using EventBasedGateway = org.camunda.bpm.model.bpmn.instance.EventBasedGateway;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class AbstractEventBasedGatewayBuilder<B> : AbstractGatewayBuilder<B, EventBasedGateway> where B : AbstractEventBasedGatewayBuilder<B>
	{

	  protected internal AbstractEventBasedGatewayBuilder(BpmnModelInstance modelInstance, EventBasedGateway element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the build event based gateway to be instantiate.
	  /// </summary>
	  /// <returns> the builder object </returns>
	  public virtual B instantiate()
	  {
		element.Instantiate = true;
		return myself;
	  }

	  /// <summary>
	  /// Sets the event gateway type of the build event based gateway.
	  /// </summary>
	  /// <param name="eventGatewayType">  the event gateway type to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B eventGatewayType(EventBasedGatewayType eventGatewayType)
	  {
		element.EventGatewayType = eventGatewayType;
		return myself;
	  }

	  public virtual B camundaAsyncAfter()
	  {
		throw new System.NotSupportedException("'asyncAfter' is not supported for 'Event Based Gateway'");
	  }

	  public virtual B camundaAsyncAfter(bool isCamundaAsyncAfter)
	  {
		throw new System.NotSupportedException("'asyncAfter' is not supported for 'Event Based Gateway'");
	  }

	}

}