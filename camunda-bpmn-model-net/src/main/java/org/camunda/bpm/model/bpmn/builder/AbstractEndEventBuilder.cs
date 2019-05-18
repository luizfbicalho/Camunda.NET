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
	using EndEvent = org.camunda.bpm.model.bpmn.instance.EndEvent;
	using ErrorEventDefinition = org.camunda.bpm.model.bpmn.instance.ErrorEventDefinition;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractEndEventBuilder<B> : AbstractThrowEventBuilder<B, EndEvent> where B : AbstractEndEventBuilder<B>
	{

	  protected internal AbstractEndEventBuilder(BpmnModelInstance modelInstance, EndEvent element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets an error definition for the given error code. If already an error
	  /// with this code exists it will be used, otherwise a new error is created.
	  /// </summary>
	  /// <param name="errorCode"> the code of the error </param>
	  /// <returns> the builder object </returns>
	  public virtual B error(string errorCode)
	  {
		ErrorEventDefinition errorEventDefinition = createErrorEventDefinition(errorCode);
		element.EventDefinitions.add(errorEventDefinition);

		return myself;
	  }
	}

}