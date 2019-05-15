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
namespace org.camunda.bpm.engine.impl.form.type
{

	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using DefaultFormHandler = org.camunda.bpm.engine.impl.form.handler.DefaultFormHandler;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class FormTypes
	{

	  protected internal IDictionary<string, AbstractFormFieldType> formTypes = new Dictionary<string, AbstractFormFieldType>();

	  public virtual void addFormType(AbstractFormFieldType formType)
	  {
		formTypes[formType.Name] = formType;
	  }

	  public virtual AbstractFormFieldType parseFormPropertyType(Element formFieldElement, BpmnParse bpmnParse)
	  {
		AbstractFormFieldType formType = null;

		string typeText = formFieldElement.attribute("type");
		string datePatternText = formFieldElement.attribute("datePattern");

		if (string.ReferenceEquals(typeText, null) && DefaultFormHandler.FORM_FIELD_ELEMENT.Equals(formFieldElement.TagName))
		{
		  bpmnParse.addError("form field must have a 'type' attribute", formFieldElement);
		}

		if ("date".Equals(typeText) && !string.ReferenceEquals(datePatternText, null))
		{
		  formType = new DateFormType(datePatternText);

		}
		else if ("enum".Equals(typeText))
		{
		  // ACT-1023: Using linked hashmap to preserve the order in which the entries are defined
		  IDictionary<string, string> values = new LinkedHashMap<string, string>();
		  foreach (Element valueElement in formFieldElement.elementsNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS,"value"))
		  {
			string valueId = valueElement.attribute("id");
			string valueName = valueElement.attribute("name");
			values[valueId] = valueName;
		  }
		  formType = new EnumFormType(values);

		}
		else if (!string.ReferenceEquals(typeText, null))
		{
		  formType = formTypes[typeText];
		  if (formType == null)
		  {
			bpmnParse.addError("unknown type '" + typeText + "'", formFieldElement);
		  }
		}
		return formType;
	  }

	  public virtual AbstractFormFieldType getFormType(string name)
	  {
		return formTypes[name];
	  }
	}

}