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
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Description = org.hamcrest.Description;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class EqualsObjectValue : EqualsTypedValue<EqualsObjectValue>
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string serializationFormat_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string objectTypeName_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string serializedValue_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal object value_Conflict;
	  protected internal bool isDeserialized = false;

	  public EqualsObjectValue()
	  {
		this.type_Conflict = ValueType.OBJECT;
	  }

	  public virtual EqualsObjectValue serializationFormat(string serializationFormat)
	  {
		this.serializationFormat_Conflict = serializationFormat;
		return this;
	  }

	  public virtual EqualsObjectValue objectTypeName(string objectTypeName)
	  {
		this.objectTypeName_Conflict = objectTypeName;
		return this;
	  }

	  public virtual EqualsObjectValue value(object value)
	  {
		this.value_Conflict = value;
		return this;
	  }

	  public virtual EqualsObjectValue Deserialized
	  {
		  get
		  {
			this.isDeserialized = true;
			return this;
		  }
	  }

	  public virtual EqualsObjectValue serializedValue(string serializedValue)
	  {
		this.serializedValue_Conflict = serializedValue;
		return this;
	  }

	  public override bool matches(object argument)
	  {
		if (!base.matches(argument))
		{
		  return false;
		}

		if (!argument.GetType().IsAssignableFrom(typeof(ObjectValue)))
		{
		  return false;
		}

		ObjectValue objectValue = (ObjectValue) argument;

		if (isDeserialized)
		{
		  if (!objectValue.Deserialized)
		  {
			return false;
		  }

		  if (value_Conflict == null)
		  {
			if (objectValue.Value != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!value_Conflict.Equals(objectValue.Value))
			{
			  return false;
			}
		  }

		}
		else
		{
		  if (objectValue.Deserialized)
		  {
			return false;
		  }


		  if (string.ReferenceEquals(serializationFormat_Conflict, null))
		  {
			if (objectValue.SerializationDataFormat != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!serializationFormat_Conflict.Equals(objectValue.SerializationDataFormat))
			{
			  return false;
			}
		  }

		  if (string.ReferenceEquals(objectTypeName_Conflict, null))
		  {
			if (objectValue.ObjectTypeName != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!objectTypeName_Conflict.Equals(objectValue.ObjectTypeName))
			{
			  return false;
			}
		  }

		  if (string.ReferenceEquals(serializedValue_Conflict, null))
		  {
			if (objectValue.ValueSerialized != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!serializedValue_Conflict.Equals(objectValue.ValueSerialized))
			{
			  return false;
			}
		  }
		}


		return true;
	  }

	  public static EqualsObjectValue objectValueMatcher()
	  {
		return new EqualsObjectValue();
	  }

	  public virtual void describeTo(Description description)
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append(this.GetType().Name);
		sb.Append(": ");
		sb.Append("serializedValue=");
		sb.Append(serializedValue_Conflict);
		sb.Append(", objectTypeName=");
		sb.Append(objectTypeName_Conflict);
		sb.Append(", serializationFormat=");
		sb.Append(serializationFormat_Conflict);
		sb.Append(", isDeserialized=false");

		description.appendText(sb.ToString());
	  }

	}

}