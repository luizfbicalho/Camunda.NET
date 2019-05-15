using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.dto
{


	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using FormPart = org.camunda.bpm.engine.rest.mapper.MultipartFormData.FormPart;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using AbstractValueTypeImpl = org.camunda.bpm.engine.variable.impl.type.AbstractValueTypeImpl;
	using FileValueType = org.camunda.bpm.engine.variable.type.FileValueType;
	using PrimitiveValueType = org.camunda.bpm.engine.variable.type.PrimitiveValueType;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ValueTypeResolver = org.camunda.bpm.engine.variable.type.ValueTypeResolver;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class VariableValueDto
	{

	  protected internal string type;
	  protected internal object value;
	  protected internal IDictionary<string, object> valueInfo;

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual object Value
	  {
		  get
		  {
			return value;
		  }
		  set
		  {
			this.value = value;
		  }
	  }


	  public virtual IDictionary<string, object> ValueInfo
	  {
		  get
		  {
			return valueInfo;
		  }
		  set
		  {
			this.valueInfo = value;
		  }
	  }


	  public virtual TypedValue toTypedValue(ProcessEngine processEngine, ObjectMapper objectMapper)
	  {
		ValueTypeResolver valueTypeResolver = processEngine.ProcessEngineConfiguration.ValueTypeResolver;

		if (string.ReferenceEquals(type, null))
		{
		  if (valueInfo != null && valueInfo[ValueType.VALUE_INFO_TRANSIENT] is bool?)
		  {
			return Variables.untypedValue(value, (bool?) valueInfo[ValueType.VALUE_INFO_TRANSIENT]);
		  }
		  return Variables.untypedValue(value);
		}

		ValueType valueType = valueTypeResolver.typeForName(fromRestApiTypeName(type));
		if (valueType == null)
		{
		  throw new RestException(Status.BAD_REQUEST, string.Format("Unsupported value type '{0}'", type));
		}
		else
		{
		  if (valueType is PrimitiveValueType)
		  {
			PrimitiveValueType primitiveValueType = (PrimitiveValueType) valueType;
			Type javaType = primitiveValueType.JavaType;
			object mappedValue = null;
			try
			{
			  if (value != null)
			  {
				if (javaType.IsAssignableFrom(value.GetType()))
				{
				  mappedValue = value;
				}
				else
				{
				  // use jackson to map the value to the requested java type
				  mappedValue = objectMapper.readValue("\"" + value + "\"", javaType);
				}
			  }
			  return valueType.createValue(mappedValue, valueInfo);
			}
			catch (Exception e)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  throw new InvalidRequestException(Status.BAD_REQUEST, e, string.Format("Cannot convert value '{0}' of type '{1}' to java type {2}", value, type, javaType.FullName));
			}
		  }
		  else if (valueType is SerializableValueType)
		  {
			if (value != null && !(value is string))
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "Must provide 'null' or String value for value of SerializableValue type '" + type + "'.");
			}
			return ((SerializableValueType) valueType).createValueFromSerialized((string) value, valueInfo);
		  }
		  else if (valueType is FileValueType)
		  {

			if (value is string)
			{
			  value = Base64.decodeBase64((string) value);
			}

			return valueType.createValue(value, valueInfo);
		  }
		  else
		  {
			return valueType.createValue(value, valueInfo);
		  }
		}

	  }

	  protected internal virtual FileValue fileValueWithDecodedString(FileValue fileValue, string value)
	  {
		return Variables.fileValue(fileValue.Filename).file(Base64.decodeBase64(value)).mimeType(fileValue.MimeType).encoding(fileValue.Encoding).create();
	  }

	  public static VariableMap toMap(IDictionary<string, VariableValueDto> variables, ProcessEngine processEngine, ObjectMapper objectMapper)
	  {
		if (variables == null)
		{
		  return null;
		}

		VariableMap result = Variables.createVariables();
		foreach (KeyValuePair<string, VariableValueDto> variableEntry in variables.SetOfKeyValuePairs())
		{
		  result.put(variableEntry.Key, variableEntry.Value.toTypedValue(processEngine, objectMapper));
		}

		return result;
	  }

	  public static IDictionary<string, VariableValueDto> fromMap(VariableMap variables)
	  {
		return fromMap(variables, false);
	  }

	  public static IDictionary<string, VariableValueDto> fromMap(VariableMap variables, bool preferSerializedValue)
	  {
		IDictionary<string, VariableValueDto> result = new Dictionary<string, VariableValueDto>();
		foreach (string variableName in variables.Keys)
		{
		  VariableValueDto valueDto = VariableValueDto.fromTypedValue(variables.getValueTyped(variableName), preferSerializedValue);
		  result[variableName] = valueDto;
		}

		return result;
	  }

	  public static VariableValueDto fromTypedValue(TypedValue typedValue)
	  {
		VariableValueDto dto = new VariableValueDto();
		fromTypedValue(dto, typedValue);
		return dto;
	  }

	  public static VariableValueDto fromTypedValue(TypedValue typedValue, bool preferSerializedValue)
	  {
		VariableValueDto dto = new VariableValueDto();
		fromTypedValue(dto, typedValue, preferSerializedValue);
		return dto;
	  }

	  public static void fromTypedValue(VariableValueDto dto, TypedValue typedValue)
	  {
		fromTypedValue(dto, typedValue, false);
	  }

	  public static void fromTypedValue(VariableValueDto dto, TypedValue typedValue, bool preferSerializedValue)
	  {

		ValueType type = typedValue.Type;
		if (type != null)
		{
		  string typeName = type.Name;
		  dto.Type = toRestApiTypeName(typeName);
		  dto.ValueInfo = type.getValueInfo(typedValue);
		}

		if (typedValue is SerializableValue)
		{
		  SerializableValue serializableValue = (SerializableValue) typedValue;

		  if (serializableValue.Deserialized && !preferSerializedValue)
		  {
			dto.Value = serializableValue.Value;
		  }
		  else
		  {
			dto.Value = serializableValue.ValueSerialized;
		  }

		}
		else if (typedValue is FileValue)
		{
		  //do not set the value for FileValues since we don't want to send megabytes over the network without explicit request
		}
		else
		{
		  dto.Value = typedValue.Value;
		}

	  }

	  public static string toRestApiTypeName(string name)
	  {
		return name.Substring(0, 1).ToUpper() + name.Substring(1);
	  }

	  public static string fromRestApiTypeName(string name)
	  {
		return name.Substring(0, 1).ToLower() + name.Substring(1);
	  }

	  public static VariableValueDto fromFormPart(string type, FormPart binaryDataFormPart)
	  {
		VariableValueDto dto = new VariableValueDto();

		dto.type = type;
		dto.value = binaryDataFormPart.BinaryContent;

		if (ValueType.FILE.Name.Equals(fromRestApiTypeName(type)))
		{

		  string contentType = binaryDataFormPart.ContentType;
		  if (string.ReferenceEquals(contentType, null))
		  {
			contentType = MediaType.APPLICATION_OCTET_STREAM;
		  }

		  dto.valueInfo = new Dictionary<>();
		  dto.valueInfo[FileValueType.VALUE_INFO_FILE_NAME] = binaryDataFormPart.FileName;
		  MimeType mimeType = null;
		  try
		  {
			mimeType = new MimeType(contentType);
		  }
		  catch (MimeTypeParseException)
		  {
			throw new RestException(Status.BAD_REQUEST, "Invalid mime type given");
		  }

		  dto.valueInfo[FileValueType.VALUE_INFO_FILE_MIME_TYPE] = mimeType.BaseType;

		  string encoding = mimeType.getParameter("encoding");
		  if (!string.ReferenceEquals(encoding, null))
		  {
			dto.valueInfo[FileValueType.VALUE_INFO_FILE_ENCODING] = encoding;
		  }

		  string transientString = mimeType.getParameter("transient");
		  bool isTransient = bool.Parse(transientString);
		  if (isTransient)
		  {
			dto.valueInfo[AbstractValueTypeImpl.VALUE_INFO_TRANSIENT] = isTransient;
		  }
		}

		return dto;


	  }

	}

}