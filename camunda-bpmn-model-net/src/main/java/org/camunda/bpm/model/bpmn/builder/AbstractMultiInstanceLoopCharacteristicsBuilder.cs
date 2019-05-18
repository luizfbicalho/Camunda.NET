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
	using Activity = org.camunda.bpm.model.bpmn.instance.Activity;
	using CompletionCondition = org.camunda.bpm.model.bpmn.instance.CompletionCondition;
	using LoopCardinality = org.camunda.bpm.model.bpmn.instance.LoopCardinality;
	using MultiInstanceLoopCharacteristics = org.camunda.bpm.model.bpmn.instance.MultiInstanceLoopCharacteristics;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AbstractMultiInstanceLoopCharacteristicsBuilder<B> : AbstractBaseElementBuilder<B, MultiInstanceLoopCharacteristics> where B : AbstractMultiInstanceLoopCharacteristicsBuilder<B>
	{

	  protected internal AbstractMultiInstanceLoopCharacteristicsBuilder(BpmnModelInstance modelInstance, MultiInstanceLoopCharacteristics element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the multi instance loop characteristics to be sequential.
	  /// </summary>
	  /// <returns>  the builder object </returns>
	  public virtual B sequential()
	  {
		element.Sequential = true;
		return myself;
	  }

	  /// <summary>
	  /// Sets the multi instance loop characteristics to be parallel.
	  /// </summary>
	  /// <returns>  the builder object </returns>
	  public virtual B parallel()
	  {
		element.Sequential = false;
		return myself;
	  }

	  /// <summary>
	  /// Sets the cardinality expression.
	  /// </summary>
	  /// <param name="expression"> the cardinality expression </param>
	  /// <returns> the builder object </returns>
	  public virtual B cardinality(string expression)
	  {
		LoopCardinality cardinality = getCreateSingleChild(typeof(LoopCardinality));
		cardinality.TextContent = expression;

		return myself;
	  }

	  /// <summary>
	  /// Sets the completion condition expression.
	  /// </summary>
	  /// <param name="expression"> the completion condition expression </param>
	  /// <returns> the builder object </returns>
	  public virtual B completionCondition(string expression)
	  {
		CompletionCondition condition = getCreateSingleChild(typeof(CompletionCondition));
		condition.TextContent = expression;

		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda collection expression.
	  /// </summary>
	  /// <param name="expression"> the collection expression </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaCollection(string expression)
	  {
		element.CamundaCollection = expression;

		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda element variable name.
	  /// </summary>
	  /// <param name="variableName"> the name of the element variable </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaElementVariable(string variableName)
	  {
		element.CamundaElementVariable = variableName;

		return myself;
	  }

	  /// <summary>
	  /// Finishes the building of a multi instance loop characteristics.
	  /// </summary>
	  /// <returns> the parent activity builder </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public <T extends AbstractActivityBuilder> T multiInstanceDone()
	  public virtual T multiInstanceDone<T>() where T : AbstractActivityBuilder
	  {
		return (T)((Activity) element.ParentElement).builder();
	  }

	}

}