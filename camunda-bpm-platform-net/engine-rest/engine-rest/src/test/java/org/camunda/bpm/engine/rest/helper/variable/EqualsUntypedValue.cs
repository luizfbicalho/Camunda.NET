using System.Text;

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
namespace org.camunda.bpm.engine.rest.helper.variable
{
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Description = org.hamcrest.Description;
	using ArgumentMatcher = org.mockito.ArgumentMatcher;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class EqualsUntypedValue : ArgumentMatcher<TypedValue>
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal object value_Conflict;

	  public virtual EqualsUntypedValue value(object value)
	  {
		this.value_Conflict = value;
		return this;
	  }

	  public virtual bool matches(object argument)
	  {
		if (argument == null || !argument.GetType().IsAssignableFrom(typeof(TypedValue)))
		{
		  return false;
		}

		TypedValue typedValue = (TypedValue) argument;

		if (typedValue.Type != null)
		{
		  return false;
		}

		if (value_Conflict == null)
		{
		  if (typedValue.Value != null)
		  {
			return false;
		  }
		}
		else
		{
		  if (!value_Conflict.Equals(typedValue.Value))
		  {
			return false;
		  }
		}

		return true;
	  }

	  public static EqualsUntypedValue matcher()
	  {
		return new EqualsUntypedValue();
	  }

	  public virtual void describeTo(Description description)
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append(this.GetType().Name);
		sb.Append(": ");
		sb.Append("value=");
		sb.Append(value_Conflict);

		description.appendText(sb.ToString());
	  }

	}

}