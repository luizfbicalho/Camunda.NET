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
namespace org.camunda.bpm.engine.impl.variable.serializer
{
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class AbstractSerializableValueSerializer<T> : AbstractTypedValueSerializer<T> where T : org.camunda.bpm.engine.variable.value.SerializableValue
	{

	  protected internal string serializationDataFormat;

	  public AbstractSerializableValueSerializer(SerializableValueType type, string serializationDataFormat) : base(type)
	  {
		this.serializationDataFormat = serializationDataFormat;
	  }

	  public override string SerializationDataformat
	  {
		  get
		  {
			return serializationDataFormat;
		  }
	  }

	  public override void writeValue(T value, ValueFields valueFields)
	  {

		string serializedStringValue = value.ValueSerialized;
		sbyte[] serializedByteValue = null;

		if (value.Deserialized)
		{
		  object objectToSerialize = value.Value;
		  if (objectToSerialize != null)
		  {
			// serialize to byte array
			try
			{
			  serializedByteValue = serializeToByteArray(objectToSerialize);
			  serializedStringValue = getSerializedStringValue(serializedByteValue);
			}
			catch (Exception e)
			{
			  throw new ProcessEngineException("Cannot serialize object in variable '" + valueFields.Name + "': " + e.Message, e);
			}
		  }
		}
		else
		{
		  if (!string.ReferenceEquals(serializedStringValue, null))
		  {
			serializedByteValue = getSerializedBytesValue(serializedStringValue);
		  }
		}

		// write value and type to fields.
		writeToValueFields(value, valueFields, serializedByteValue);

		// update the ObjectValue to keep it consistent with value fields.
		updateTypedValue(value, serializedStringValue);
	  }

	  public override T readValue(ValueFields valueFields, bool deserializeObjectValue)
	  {

		sbyte[] serializedByteValue = readSerializedValueFromFields(valueFields);
		string serializedStringValue = getSerializedStringValue(serializedByteValue);

		if (deserializeObjectValue)
		{
		  object deserializedObject = null;
		  if (serializedByteValue != null)
		  {
			try
			{
			  deserializedObject = deserializeFromByteArray(serializedByteValue, valueFields);
			}
			catch (Exception e)
			{
			  throw new ProcessEngineException("Cannot deserialize object in variable '" + valueFields.Name + "': " + e.Message, e);
			}
		  }
		  T value = createDeserializedValue(deserializedObject, serializedStringValue, valueFields);
		  return value;
		}
		else
		{
		  return createSerializedValue(serializedStringValue, valueFields);
		}
	  }

	  protected internal abstract T createDeserializedValue(object deserializedObject, string serializedStringValue, ValueFields valueFields);

	  protected internal abstract T createSerializedValue(string serializedStringValue, ValueFields valueFields);

	  protected internal abstract void writeToValueFields(T value, ValueFields valueFields, sbyte[] serializedValue);

	  protected internal abstract void updateTypedValue(T value, string serializedStringValue);

	  protected internal virtual sbyte[] readSerializedValueFromFields(ValueFields valueFields)
	  {
		return valueFields.ByteArrayValue;
	  }

	  protected internal virtual string getSerializedStringValue(sbyte[] serializedByteValue)
	  {
		if (serializedByteValue != null)
		{
		  if (!SerializationTextBased)
		  {
			serializedByteValue = Base64.encodeBase64(serializedByteValue);
		  }
		  return StringUtil.fromBytes(serializedByteValue);
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual sbyte[] getSerializedBytesValue(string serializedStringValue)
	  {
		if (!string.ReferenceEquals(serializedStringValue, null))
		{
		  sbyte[] serializedByteValue = StringUtil.toByteArray(serializedStringValue);
		  if (!SerializationTextBased)
		  {
			serializedByteValue = Base64.decodeBase64(serializedByteValue);
		  }
		  return serializedByteValue;
		}
		else
		{
		  return null;
		}
	  }

	  protected internal override bool canWriteValue(TypedValue typedValue)
	  {

		if (!(typedValue is SerializableValue) && !(typedValue is UntypedValueImpl))
		{
		  return false;
		}

		if (typedValue is SerializableValue)
		{
		  SerializableValue serializableValue = (SerializableValue) typedValue;
		  string requestedDataFormat = serializableValue.SerializationDataFormat;
		  if (!serializableValue.Deserialized)
		  {
			// serialized object => dataformat must match
			return serializationDataFormat.Equals(requestedDataFormat);
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean canSerialize = typedValue.getValue() == null || canSerializeValue(typedValue.getValue());
			bool canSerialize = typedValue.Value == null || canSerializeValue(typedValue.Value);
			return canSerialize && (string.ReferenceEquals(requestedDataFormat, null) || serializationDataFormat.Equals(requestedDataFormat));
		  }
		}
		else
		{
		  return typedValue.Value == null || canSerializeValue(typedValue.Value);
		}

	  }


	  /// <summary>
	  /// return true if this serializer is able to serialize the provided object.
	  /// </summary>
	  /// <param name="value"> the object to test (guaranteed to be a non-null value) </param>
	  /// <returns> true if the serializer can handle the object. </returns>
	  protected internal abstract bool canSerializeValue(object value);

	  // methods to be implemented by subclasses ////////////

	  /// <summary>
	  /// Implementations must return a byte[] representation of the provided object.
	  /// The object is guaranteed not to be null.
	  /// </summary>
	  /// <param name="deserializedObject"> the object to serialize </param>
	  /// <returns> the byte array value of the object </returns>
	  /// <exception cref="exception"> in case the object cannot be serialized </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract byte[] serializeToByteArray(Object deserializedObject) throws Exception;
	  protected internal abstract sbyte[] serializeToByteArray(object deserializedObject);

	  /// <summary>
	  /// Deserialize the object from a byte array.
	  /// </summary>
	  /// <param name="object"> the object to deserialize </param>
	  /// <param name="valueFields"> the value fields </param>
	  /// <returns> the deserialized object </returns>
	  /// <exception cref="exception"> in case the object cannot be deserialized </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract Object deserializeFromByteArray(byte[] object, ValueFields valueFields) throws Exception;
	  protected internal abstract object deserializeFromByteArray(sbyte[] @object, ValueFields valueFields);

	  /// <summary>
	  /// Return true if the serialization is text based. Return false otherwise
	  /// 
	  /// </summary>
	  protected internal abstract bool SerializationTextBased {get;}

	}

}