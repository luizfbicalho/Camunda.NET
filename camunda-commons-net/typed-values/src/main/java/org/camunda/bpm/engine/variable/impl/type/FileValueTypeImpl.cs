using System;
using System.Collections.Generic;
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
namespace org.camunda.bpm.engine.variable.impl.type
{

	using FileValueType = org.camunda.bpm.engine.variable.type.FileValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using FileValueBuilder = org.camunda.bpm.engine.variable.value.builder.FileValueBuilder;

	/// <summary>
	/// Valuetype to save files from byte arrays, inputstreams or just files as
	/// process variables and retrieve them via an <seealso cref="System.IO.Stream_Input"/>.
	/// 
	/// @author Ronny Bräunlich
	/// @since 7.4
	/// 
	/// </summary>
	[Serializable]
	public class FileValueTypeImpl : AbstractValueTypeImpl, FileValueType
	{

	  private const long serialVersionUID = 1L;

	  public FileValueTypeImpl() : base("file")
	  {
	  }

	  public override TypedValue createValue(object value, IDictionary<string, object> valueInfo)
	  {
		if (valueInfo == null)
		{
		  throw new System.ArgumentException("Cannot create file without valueInfo.");
		}
		object filename = valueInfo[org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_NAME];
		if (filename == null)
		{
		  throw new System.ArgumentException("Cannot create file without filename! Please set a name into ValueInfo with key " + org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_NAME);
		}
		FileValueBuilder builder = Variables.fileValue(filename.ToString());
		if (value is File)
		{
		  builder.file((File) value);
		}
		else if (value is Stream)
		{
		  builder.file((Stream) value);
		}
		else if (value is sbyte[])
		{
		  builder.file((sbyte[]) value);
		}
		else
		{
		  throw new System.ArgumentException("Provided value is not of File, InputStream or byte[] type.");
		}

		if (valueInfo.ContainsKey(org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_MIME_TYPE))
		{
		  object mimeType = valueInfo[org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_MIME_TYPE];

		  if (mimeType == null)
		  {
			throw new System.ArgumentException("The provided mime type is null. Set a non-null value info property with key '" + org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_NAME + "'");
		  }

		  builder.mimeType(mimeType.ToString());
		}
		if (valueInfo.ContainsKey(org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_ENCODING))
		{
		  object encoding = valueInfo[org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_ENCODING];

		  if (encoding == null)
		  {
			throw new System.ArgumentException("The provided encoding is null. Set a non-null value info property with key '" + org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_ENCODING + "'");
		  }

		  builder.encoding(encoding.ToString());
		}

		builder.Transient = isTransient(valueInfo);
		return builder.create();
	  }

	  public override IDictionary<string, object> getValueInfo(TypedValue typedValue)
	  {
		if (!(typedValue is FileValue))
		{
		  throw new System.ArgumentException("Value not of type FileValue");
		}
		FileValue fileValue = (FileValue) typedValue;
		IDictionary<string, object> result = new Dictionary<string, object>(2);
		result[org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_NAME] = fileValue.Filename;
		if (!string.ReferenceEquals(fileValue.MimeType, null))
		{
		  result[org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_MIME_TYPE] = fileValue.MimeType;
		}
		if (!string.ReferenceEquals(fileValue.Encoding, null))
		{
		  result[org.camunda.bpm.engine.variable.type.FileValueType_Fields.VALUE_INFO_FILE_ENCODING] = fileValue.Encoding;
		}
		if (fileValue.Transient)
		{
		  result[org.camunda.bpm.engine.variable.type.ValueType_Fields.VALUE_INFO_TRANSIENT] = fileValue.Transient;
		}
		return result;
	  }

	  public override bool PrimitiveValueType
	  {
		  get
		  {
			return true;
		  }
	  }

	}

}