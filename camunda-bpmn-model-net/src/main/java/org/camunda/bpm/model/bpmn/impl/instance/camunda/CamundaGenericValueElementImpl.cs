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
namespace org.camunda.bpm.model.bpmn.impl.instance.camunda
{

	using BpmnModelElementInstance = org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance;
	using CamundaGenericValueElement = org.camunda.bpm.model.bpmn.instance.camunda.CamundaGenericValueElement;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelUtil = org.camunda.bpm.model.xml.impl.util.ModelUtil;
	using DomElement = org.camunda.bpm.model.xml.instance.DomElement;

	/// <summary>
	/// A helper interface for camunda extension elements which
	/// hold a generic child element like camunda:inputParameter,
	/// camunda:outputParameter and camunda:entry.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class CamundaGenericValueElementImpl : BpmnModelElementInstanceImpl, CamundaGenericValueElement
	{

	  public CamundaGenericValueElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance> T getValue()
	  public virtual T getValue<T>() where T : org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance
	  {
		IList<DomElement> childElements = DomElement.ChildElements;
		if (childElements.Count == 0)
		{
		  return null;
		}
		else
		{
		  return (T) ModelUtil.getModelElement(childElements[0], modelInstance);
		}
	  }

	  public virtual void removeValue()
	  {
		DomElement domElement = DomElement;
		IList<DomElement> childElements = domElement.ChildElements;
		foreach (DomElement childElement in childElements)
		{
		  domElement.removeChild(childElement);
		}
	  }

	  public virtual void setValue<T>(T value) where T : org.camunda.bpm.model.bpmn.instance.BpmnModelElementInstance
	  {
		removeValue();
		DomElement.appendChild(value.DomElement);
	  }

	}

}