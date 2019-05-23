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
	using Event = org.camunda.bpm.model.bpmn.instance.Event;
	using MessageEventDefinition = org.camunda.bpm.model.bpmn.instance.MessageEventDefinition;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>

	public abstract class AbstractMessageEventDefinitionBuilder<B> : AbstractRootElementBuilder<B, MessageEventDefinition> where B : AbstractMessageEventDefinitionBuilder<B>
	{

	  public AbstractMessageEventDefinitionBuilder(BpmnModelInstance modelInstance, MessageEventDefinition element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  public override B id(string identifier)
	  {
		return base.id(identifier);
	  }

	  /// <summary>
	  /// Sets the message attribute.
	  /// </summary>
	  /// <param name="message"> the message for the message event definition </param>
	  /// <returns> the builder object </returns>
	  public virtual B message(string message)
	  {
		element.Message = findMessageForName(message);
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda topic attribute. This is only meaningful when
	  /// the <seealso cref="camundaType(string)"/> attribute has the value <code>external</code>.
	  /// </summary>
	  /// <param name="camundaTopic"> the topic to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTopic(string camundaTopic)
	  {
		element.CamundaTopic = camundaTopic;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda type attribute.
	  /// </summary>
	  /// <param name="camundaType">  the type of the service task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaType(string camundaType)
	  {
		element.CamundaType = camundaType;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda task priority attribute. This is only meaningful when
	  /// the <seealso cref="camundaType(string)"/> attribute has the value <code>external</code>.
	  /// 
	  /// </summary>
	  /// <param name="taskPriority"> the priority for the external task </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaTaskPriority(string taskPriority)
	  {
		element.CamundaTaskPriority = taskPriority;
		return myself;
	  }

	  /// <summary>
	  /// Finishes the building of a message event definition.
	  /// </summary>
	  /// @param <T> </param>
	  /// <returns> the parent event builder </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public <T extends AbstractFlowNodeBuilder> T messageEventDefinitionDone()
	  public virtual T messageEventDefinitionDone<T>() where T : AbstractFlowNodeBuilder
	  {
		return (T)((Event) element.ParentElement).builder();
	  }
	}

}