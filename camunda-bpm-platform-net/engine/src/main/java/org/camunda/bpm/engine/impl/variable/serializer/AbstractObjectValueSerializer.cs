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
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValueImpl = org.camunda.bpm.engine.variable.impl.value.ObjectValueImpl;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;

	/// <summary>
	/// Abstract implementation of a <seealso cref="TypedValueSerializer"/> for <seealso cref="ObjectValue ObjectValues"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractObjectValueSerializer : AbstractSerializableValueSerializer<ObjectValue>
	{

	  public AbstractObjectValueSerializer(string serializationDataFormat) : base(ValueType.OBJECT, serializationDataFormat)
	  {
	  }

	  public virtual ObjectValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		// untyped values are always deserialized
		return Variables.objectValue(untypedValue.Value, untypedValue.Transient).create();
	  }

	  protected internal virtual void writeToValueFields(ObjectValue value, ValueFields valueFields, sbyte[] serializedValue)
	  {
		string objectTypeName = getObjectTypeName(value, valueFields);
		valueFields.ByteArrayValue = serializedValue;
		valueFields.TextValue2 = objectTypeName;
	  }

	  protected internal virtual string getObjectTypeName(ObjectValue value, ValueFields valueFields)
	  {
		string objectTypeName = value.ObjectTypeName;

		if (string.ReferenceEquals(objectTypeName, null) && !value.Deserialized && value.ValueSerialized != null)
		{
		  throw new ProcessEngineException("Cannot write serialized value for variable '" + valueFields.Name + "': no 'objectTypeName' provided for non-null value.");
		}

		// update type name if the object is deserialized
		if (value.Deserialized && value.Value != null)
		{
		  objectTypeName = getTypeNameForDeserialized(value.Value);
		}

		return objectTypeName;
	  }

	  protected internal virtual void updateTypedValue(ObjectValue value, string serializedStringValue)
	  {
		string objectTypeName = getObjectTypeName(value, null);
		ObjectValueImpl objectValue = (ObjectValueImpl) value;
		objectValue.ObjectTypeName = objectTypeName;
		objectValue.SerializedValue = serializedStringValue;
		objectValue.SerializationDataFormat = serializationDataFormat;
	  }

	  protected internal override ObjectValue createDeserializedValue(object deserializedObject, string serializedStringValue, ValueFields valueFields)
	  {
		string objectTypeName = readObjectNameFromFields(valueFields);
		return new ObjectValueImpl(deserializedObject, serializedStringValue, serializationDataFormat, objectTypeName, true);
	  }


	  protected internal override ObjectValue createSerializedValue(string serializedStringValue, ValueFields valueFields)
	  {
		string objectTypeName = readObjectNameFromFields(valueFields);
		return new ObjectValueImpl(null, serializedStringValue, serializationDataFormat, objectTypeName, false);
	  }

	  protected internal virtual string readObjectNameFromFields(ValueFields valueFields)
	  {
		return valueFields.TextValue2;
	  }

	  public virtual bool isMutableValue(ObjectValue typedValue)
	  {
		return typedValue.Deserialized;
	  }

	  // methods to be implemented by subclasses ////////////

	  /// <summary>
	  /// Returns the type name for the deserialized object.
	  /// </summary>
	  /// <param name="deserializedObject."> Guaranteed not to be null </param>
	  /// <returns> the type name fot the object. </returns>
	  protected internal abstract string getTypeNameForDeserialized(object deserializedObject);

	  /// <summary>
	  /// Implementations must return a byte[] representation of the provided object.
	  /// The object is guaranteed not to be null.
	  /// </summary>
	  /// <param name="deserializedObject"> the object to serialize </param>
	  /// <returns> the byte array value of the object </returns>
	  /// <exception cref="exception"> in case the object cannot be serialized </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract byte[] serializeToByteArray(Object deserializedObject) throws Exception;
	  protected internal override abstract sbyte[] serializeToByteArray(object deserializedObject);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object deserializeFromByteArray(byte[] object, ValueFields valueFields) throws Exception
	  protected internal override object deserializeFromByteArray(sbyte[] @object, ValueFields valueFields)
	  {
		string objectTypeName = readObjectNameFromFields(valueFields);
		return deserializeFromByteArray(@object, objectTypeName);
	  }

	  /// <summary>
	  /// Deserialize the object from a byte array.
	  /// </summary>
	  /// <param name="object"> the object to deserialize </param>
	  /// <param name="objectTypeName"> the type name of the object to deserialize </param>
	  /// <returns> the deserialized object </returns>
	  /// <exception cref="exception"> in case the object cannot be deserialized </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract Object deserializeFromByteArray(byte[] object, String objectTypeName) throws Exception;
	  protected internal abstract object deserializeFromByteArray(sbyte[] @object, string objectTypeName);

	  /// <summary>
	  /// Return true if the serialization is text based. Return false otherwise
	  /// 
	  /// </summary>
	  protected internal override abstract bool SerializationTextBased {get;}

	}

}