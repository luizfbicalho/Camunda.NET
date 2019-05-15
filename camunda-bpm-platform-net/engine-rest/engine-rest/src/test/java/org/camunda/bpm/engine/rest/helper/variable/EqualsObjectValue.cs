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
	  protected internal string serializationFormat_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string objectTypeName_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string serializedValue_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal object value_Renamed;
	  protected internal bool isDeserialized = false;

	  public EqualsObjectValue()
	  {
		this.type_Renamed = ValueType.OBJECT;
	  }

	  public virtual EqualsObjectValue serializationFormat(string serializationFormat)
	  {
		this.serializationFormat_Renamed = serializationFormat;
		return this;
	  }

	  public virtual EqualsObjectValue objectTypeName(string objectTypeName)
	  {
		this.objectTypeName_Renamed = objectTypeName;
		return this;
	  }

	  public virtual EqualsObjectValue value(object value)
	  {
		this.value_Renamed = value;
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
		this.serializedValue_Renamed = serializedValue;
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

		  if (value_Renamed == null)
		  {
			if (objectValue.Value != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!value_Renamed.Equals(objectValue.Value))
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


		  if (string.ReferenceEquals(serializationFormat_Renamed, null))
		  {
			if (objectValue.SerializationDataFormat != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!serializationFormat_Renamed.Equals(objectValue.SerializationDataFormat))
			{
			  return false;
			}
		  }

		  if (string.ReferenceEquals(objectTypeName_Renamed, null))
		  {
			if (objectValue.ObjectTypeName != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!objectTypeName_Renamed.Equals(objectValue.ObjectTypeName))
			{
			  return false;
			}
		  }

		  if (string.ReferenceEquals(serializedValue_Renamed, null))
		  {
			if (objectValue.ValueSerialized != null)
			{
			  return false;
			}
		  }
		  else
		  {
			if (!serializedValue_Renamed.Equals(objectValue.ValueSerialized))
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
		sb.Append(serializedValue_Renamed);
		sb.Append(", objectTypeName=");
		sb.Append(objectTypeName_Renamed);
		sb.Append(", serializationFormat=");
		sb.Append(serializationFormat_Renamed);
		sb.Append(", isDeserialized=false");

		description.appendText(sb.ToString());
	  }

	}

}