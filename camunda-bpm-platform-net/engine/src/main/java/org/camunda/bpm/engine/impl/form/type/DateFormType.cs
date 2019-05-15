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
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DateFormType : AbstractFormFieldType
	{

	  public const string TYPE_NAME = "date";

	  protected internal string datePattern;
	  protected internal DateFormat dateFormat;

	  public DateFormType(string datePattern)
	  {
		this.datePattern = datePattern;
		this.dateFormat = new SimpleDateFormat(datePattern);
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
		if ("datePattern".Equals(key))
		{
		  return datePattern;
		}
		return null;
	  }

	  public override TypedValue convertToModelValue(TypedValue propertyValue)
	  {
		object value = propertyValue.Value;
		if (value == null)
		{
		  return Variables.dateValue(null, propertyValue.Transient);
		}
		else if (value is DateTime)
		{
		  return Variables.dateValue((DateTime) value, propertyValue.Transient);
		}
		else if (value is string)
		{
		  string strValue = ((string) value).Trim();
		  if (strValue.Length == 0)
		  {
			return Variables.dateValue(null, propertyValue.Transient);
		  }
		  try
		  {
			return Variables.dateValue((DateTime) dateFormat.parseObject(strValue), propertyValue.Transient);
		  }
		  catch (ParseException)
		  {
			throw new ProcessEngineException("Could not parse value '" + value + "' as date using date format '" + datePattern + "'.");
		  }
		}
		else
		{
		  throw new ProcessEngineException("Value '" + value + "' cannot be transformed into a Date.");
		}
	  }

	  public override TypedValue convertToFormValue(TypedValue modelValue)
	  {
		if (modelValue.Value == null)
		{
		  return Variables.stringValue(null, modelValue.Transient);
		}
		else if (modelValue.Type == ValueType.DATE)
		{
		  return Variables.stringValue(dateFormat.format(modelValue.Value), modelValue.Transient);
		}
		else
		{
		  throw new ProcessEngineException("Expected value to be of type '" + ValueType.DATE + "' but got '" + modelValue.Type + "'.");
		}
	  }

	  // deprecated //////////////////////////////////////////////////////////

	  public override object convertFormValueToModelValue(object propertyValue)
	  {
		if (propertyValue == null || "".Equals(propertyValue))
		{
		  return null;
		}
		try
		{
		  return dateFormat.parseObject(propertyValue.ToString());
		}
		catch (ParseException)
		{
		  throw new ProcessEngineException("invalid date value " + propertyValue);
		}
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {
		if (modelValue == null)
		{
		  return null;
		}
		return dateFormat.format(modelValue);
	  }

	}

}