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

	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class EnumFormType : SimpleFormFieldType
	{

	  public const string TYPE_NAME = "enum";

	  protected internal IDictionary<string, string> values;

	  public EnumFormType(IDictionary<string, string> values)
	  {
		this.values = values;
	  }

	  public override string Name
	  {
		  get
		  {
			return TYPE_NAME;
		  }
	  }

	  public override object getInformation(string key)
	  {
		if (key.Equals("values"))
		{
		  return values;
		}
		return null;
	  }

	  public override TypedValue convertValue(TypedValue propertyValue)
	  {
		object value = propertyValue.Value;
		if (value == null || typeof(string).IsInstanceOfType(value))
		{
		  validateValue(value);
		  return Variables.stringValue((string) value, propertyValue.Transient);
		}
		else
		{
		  throw new ProcessEngineException("Value '" + value + "' is not of type String.");
		}
	  }

	  protected internal virtual void validateValue(object value)
	  {
		if (value != null)
		{
		  if (values != null && !values.ContainsKey(value))
		  {
			throw new ProcessEngineException("Invalid value for enum form property: " + value);
		  }
		}
	  }

	  public virtual IDictionary<string, string> Values
	  {
		  get
		  {
			return values;
		  }
	  }

	  //////////////////// deprecated ////////////////////////////////////////

	  public override object convertFormValueToModelValue(object propertyValue)
	  {
		validateValue(propertyValue);
		return propertyValue;
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {
		if (modelValue != null)
		{
		  if (!(modelValue is string))
		  {
			throw new ProcessEngineException("Model value should be a String");
		  }
		  validateValue(modelValue);
		}
		return (string) modelValue;
	  }

	}

}