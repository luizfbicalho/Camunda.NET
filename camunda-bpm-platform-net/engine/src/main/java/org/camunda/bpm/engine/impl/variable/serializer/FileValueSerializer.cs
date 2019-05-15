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
	using FileValueImpl = org.camunda.bpm.engine.variable.impl.value.FileValueImpl;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using FileValueBuilder = org.camunda.bpm.engine.variable.value.builder.FileValueBuilder;

	/// <summary>
	/// @author Ronny Bräunlich
	/// @since 7.4
	/// </summary>
	public class FileValueSerializer : AbstractTypedValueSerializer<FileValue>
	{

	  /// <summary>
	  /// The numbers values we encoded in textfield two.
	  /// </summary>
	  protected internal const int NR_OF_VALUES_IN_TEXTFIELD2 = 2;

	  /// <summary>
	  /// The separator to be able to store encoding and mimetype inside the same
	  /// text field. Please be aware that the separator only works when it is a
	  /// character that is not allowed in the first component.
	  /// </summary>
	  protected internal const string MIMETYPE_ENCODING_SEPARATOR = "#";

	  public FileValueSerializer() : base(ValueType.FILE)
	  {
	  }

	  public virtual void writeValue(FileValue value, ValueFields valueFields)
	  {
		sbyte[] data = ((FileValueImpl) value).ByteArray;
		valueFields.ByteArrayValue = data;
		valueFields.TextValue = value.Filename;
		if (value.MimeType == null && value.Encoding != null)
		{
		  valueFields.TextValue2 = MIMETYPE_ENCODING_SEPARATOR + value.Encoding;
		}
		else if (value.MimeType != null && value.Encoding == null)
		{
		  valueFields.TextValue2 = value.MimeType + MIMETYPE_ENCODING_SEPARATOR;
		}
		else if (value.MimeType != null && value.Encoding != null)
		{
		  valueFields.TextValue2 = value.MimeType + MIMETYPE_ENCODING_SEPARATOR + value.Encoding;
		}
	  }

	  public override FileValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		throw new System.NotSupportedException("Currently no automatic conversation from UntypedValue to FileValue");
	  }

	  public override FileValue readValue(ValueFields valueFields, bool deserializeValue)
	  {
		string fileName = valueFields.TextValue;
		if (string.ReferenceEquals(fileName, null))
		{
		  // ensure file name is not null
		  fileName = "";
		}
		FileValueBuilder builder = Variables.fileValue(fileName);
		if (valueFields.ByteArrayValue != null)
		{
		  builder.file(valueFields.ByteArrayValue);
		}
		// to ensure the same array size all the time
		if (!string.ReferenceEquals(valueFields.TextValue2, null))
		{
		  string[] split = Arrays.copyOf(valueFields.TextValue2.Split(MIMETYPE_ENCODING_SEPARATOR, NR_OF_VALUES_IN_TEXTFIELD2), NR_OF_VALUES_IN_TEXTFIELD2);

		  string mimeType = returnNullIfEmptyString(split[0]);
		  string encoding = returnNullIfEmptyString(split[1]);

		  builder.mimeType(mimeType);
		  builder.encoding(encoding);
		}
		return builder.create();
	  }

	  protected internal virtual string returnNullIfEmptyString(string s)
	  {
		if (s.Length == 0)
		{
		  return null;
		}
		return s;
	  }

	  public override string Name
	  {
		  get
		  {
			return valueType.Name;
		  }
	  }

	  protected internal override bool canWriteValue(TypedValue value)
	  {
		if (value == null || value.Type == null)
		{
		  // untyped value
		  return false;
		}
		return value.Type.Name.Equals(Name);
	  }

	}

}