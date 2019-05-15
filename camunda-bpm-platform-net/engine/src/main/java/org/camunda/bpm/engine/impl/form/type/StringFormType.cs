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
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class StringFormType : SimpleFormFieldType
	{

	  public const string TYPE_NAME = "string";

	  public override string Name
	  {
		  get
		  {
			return TYPE_NAME;
		  }
	  }

	  public override TypedValue convertValue(TypedValue propertyValue)
	  {
		if (propertyValue is StringValue)
		{
		  return propertyValue;
		}
		else
		{
		  object value = propertyValue.Value;
		  if (value == null)
		  {
			return Variables.stringValue(null, propertyValue.Transient);
		  }
		  else
		  {
			return Variables.stringValue(value.ToString(), propertyValue.Transient);
		  }
		}
	  }

	  // deprecated ////////////////////////////////////////////////////////////

	  public override object convertFormValueToModelValue(object propertyValue)
	  {
		return propertyValue.ToString();
	  }

	  public override string convertModelValueToFormValue(object modelValue)
	  {
		return (string) modelValue;
	  }
	}

}