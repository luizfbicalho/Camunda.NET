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
	using ConditionExpression = org.camunda.bpm.model.bpmn.instance.ConditionExpression;
	using FlowNode = org.camunda.bpm.model.bpmn.instance.FlowNode;
	using SequenceFlow = org.camunda.bpm.model.bpmn.instance.SequenceFlow;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractSequenceFlowBuilder<B> : AbstractFlowElementBuilder<B, SequenceFlow> where B : AbstractSequenceFlowBuilder<B>
	{

	  protected internal AbstractSequenceFlowBuilder(BpmnModelInstance modelInstance, SequenceFlow element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the source flow node of this sequence flow.
	  /// </summary>
	  /// <param name="source">  the source of this sequence flow </param>
	  /// <returns> the builder object </returns>
	  public virtual B from(FlowNode source)
	  {
		element.Source = source;
		source.Outgoing.Add(element);
		return myself;
	  }

	  /// <summary>
	  /// Sets the target flow node of this sequence flow.
	  /// </summary>
	  /// <param name="target">  the target of this sequence flow </param>
	  /// <returns> the builder object </returns>
	  public virtual B to(FlowNode target)
	  {
		element.Target = target;
		target.Incoming.Add(element);
		return myself;
	  }

	  /// <summary>
	  /// Sets the condition for this sequence flow.
	  /// </summary>
	  /// <param name="conditionExpression">  the condition expression for this sequence flow </param>
	  /// <returns> the builder object </returns>
	  public virtual B condition(ConditionExpression conditionExpression)
	  {
		element.ConditionExpression = conditionExpression;
		return myself;
	  }

	}

}