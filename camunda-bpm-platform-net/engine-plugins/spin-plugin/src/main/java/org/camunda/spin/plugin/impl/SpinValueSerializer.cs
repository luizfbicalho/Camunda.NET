using System.IO;

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
namespace org.camunda.spin.plugin.impl
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using AbstractSerializableValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractSerializableValueSerializer;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using SpinValue = org.camunda.spin.plugin.variable.value.SpinValue;
	using SpinValueImpl = org.camunda.spin.plugin.variable.value.impl.SpinValueImpl;
	using DataFormat = org.camunda.spin.spi.DataFormat;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class SpinValueSerializer : AbstractSerializableValueSerializer<SpinValue>
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.spin.spi.DataFormat<?> dataFormat;
	  protected internal DataFormat<object> dataFormat;
	  protected internal string name;

	  public SpinValueSerializer<T1>(SerializableValueType type, DataFormat<T1> dataFormat, string name) : base(type, dataFormat.Name)
	  {
		this.dataFormat = dataFormat;
		this.name = name;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  protected internal virtual void writeToValueFields(SpinValue value, ValueFields valueFields, sbyte[] serializedValue)
	  {
		valueFields.ByteArrayValue = serializedValue;
	  }

	  protected internal virtual void updateTypedValue(SpinValue value, string serializedStringValue)
	  {
		SpinValueImpl spinValue = (SpinValueImpl) value;
		spinValue.ValueSerialized = serializedStringValue;
		spinValue.SerializationDataFormat = serializationDataFormat;
	  }

	  protected internal override bool canSerializeValue(object value)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: if (value instanceof org.camunda.spin.Spin<?>)
		if (value is Spin<object>)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.spin.Spin<?> wrapper = (org.camunda.spin.Spin<?>) value;
		  Spin<object> wrapper = (Spin<object>) value;
		  return wrapper.DataFormatName.Equals(serializationDataFormat);
		}

		return false;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected byte[] serializeToByteArray(Object deserializedObject) throws Exception
	  protected internal override sbyte[] serializeToByteArray(object deserializedObject)
	  {
		MemoryStream @out = new MemoryStream();
		StreamWriter outWriter = new StreamWriter(@out, Context.ProcessEngineConfiguration.DefaultCharset);
		StreamWriter bufferedWriter = new StreamWriter(outWriter);

		try
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.spin.Spin<?> wrapper = (org.camunda.spin.Spin<?>) deserializedObject;
		  Spin<object> wrapper = (Spin<object>) deserializedObject;
		  wrapper.writeToWriter(bufferedWriter);
		  return @out.toByteArray();
		}
		finally
		{
		  IoUtil.closeSilently(@out);
		  IoUtil.closeSilently(outWriter);
		  IoUtil.closeSilently(bufferedWriter);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object deserializeFromByteArray(byte[] object, org.camunda.bpm.engine.impl.variable.serializer.ValueFields valueFields) throws Exception
	  protected internal override object deserializeFromByteArray(sbyte[] @object, ValueFields valueFields)
	  {
		MemoryStream bais = new MemoryStream(@object);
		StreamReader inReader = new StreamReader(bais, Context.ProcessEngineConfiguration.DefaultCharset);
		StreamReader bufferedReader = new StreamReader(inReader);

		try
		{
		  object wrapper = dataFormat.Reader.readInput(bufferedReader);
		  return dataFormat.createWrapperInstance(wrapper);
		}
		finally
		{
		  IoUtil.closeSilently(bais);
		  IoUtil.closeSilently(inReader);
		  IoUtil.closeSilently(bufferedReader);
		}

	  }

	  protected internal override bool SerializationTextBased
	  {
		  get
		  {
			return true;
		  }
	  }

	}

}