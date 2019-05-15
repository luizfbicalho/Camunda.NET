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
namespace org.camunda.bpm.engine.impl.form.type
{
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using BooleanValue = org.camunda.bpm.engine.variable.value.BooleanValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class BooleanFormType : SimpleFormFieldType
	{

	  public const string TYPE_NAME = "boolean";

	  public override string Name
	  {
		  get
		  {
			return TYPE_NAME;
		  }
	  }

	  public override TypedValue convertValue(TypedValue propertyValue)
	  {
		if (propertyValue is BooleanValue)
		{
		  return propertyValue;
		}
		else
		{
		  object value = propertyValue.Value;
		  if (value == null)
		  {
			return Variables.booleanValue(null, propertyValue.Transient);
		  }
		  else if ((value is bool?) || (value is string))
		  {
			return Variables.booleanValue(new bool?(value.ToString()), propertyValue.Transient);
		  }
		  else
		  {
			throw new ProcessEngineException("Value '" + value + "' is not of type Boolean.");
		  }
		}
	  }
	  // deprecated /////////////////////////////////////////////////

	  public override object convertFormValueToModelValue(object propertyValue)
	  {
		if (propertyValue == null || "".Equals(propertyValue))
		{
		  return null;
		}
		return Convert.ToBoolean(propertyValue.ToString());
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {

		if (modelValue == null)
		{
		  return null;
		}

		if (modelValue.GetType().IsAssignableFrom(typeof(Boolean)) || modelValue.GetType().IsAssignableFrom(typeof(bool)))
		{
		  return modelValue.ToString();
		}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new ProcessEngineException("Model value is not of type boolean, but of type " + modelValue.GetType().FullName);
	  }

	}

}