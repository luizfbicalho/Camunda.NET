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
	using BaseElement = org.camunda.bpm.model.bpmn.instance.BaseElement;
	using CamundaFormField = org.camunda.bpm.model.bpmn.instance.camunda.CamundaFormField;

	/// <summary>
	/// @author Kristin Polenz
	/// 
	/// </summary>
	public class AbstractCamundaFormFieldBuilder<P, B> : AbstractBpmnModelElementBuilder<B, CamundaFormField> where B : AbstractCamundaFormFieldBuilder<P, B>
	{

	  protected internal BaseElement parent;

	  protected internal AbstractCamundaFormFieldBuilder(BpmnModelInstance modelInstance, BaseElement parent, CamundaFormField element, Type selfType) : base(modelInstance, element, selfType)
	  {
		this.parent = parent;
	  }


	  /// <summary>
	  /// Sets the form field id.
	  /// </summary>
	  /// <param name="id"> the form field id </param>
	  /// <returns>  the builder object </returns>
	  public virtual B camundaId(string id)
	  {
		element.CamundaId = id;
		return myself;
	  }

	  /// <summary>
	  /// Sets form field label.
	  /// </summary>
	  /// <param name="label"> the form field label </param>
	  /// <returns>  the builder object </returns>
	  public virtual B camundaLabel(string label)
	  {
		element.CamundaLabel = label;
		return myself;
	  }

	  /// <summary>
	  /// Sets the form field type.
	  /// </summary>
	  /// <param name="type"> the form field type </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaType(string type)
	  {
		element.CamundaType = type;
		return myself;
	  }

	  /// <summary>
	  /// Sets the form field default value.
	  /// </summary>
	  /// <param name="defaultValue"> the form field default value </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaDefaultValue(string defaultValue)
	  {
		element.CamundaDefaultValue = defaultValue;
		return myself;
	  }

	  /// <summary>
	  /// Finishes the building of a form field.
	  /// </summary>
	  /// <returns> the parent activity builder </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked" }) public P camundaFormFieldDone()
	  public virtual P camundaFormFieldDone()
	  {
		return (P) parent.builder();
	  }
	}

}