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
namespace org.camunda.bpm.engine.rest.sub.impl
{
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class VariableResponseProvider
	{

	  public virtual Response getResponseForTypedVariable(TypedValue typedVariableValue, string id)
	  {
		if (typedVariableValue is BytesValue || ValueType.BYTES.Equals(typedVariableValue.Type))
		{
		  return responseForByteVariable(typedVariableValue);
		}
		else if (ValueType.FILE.Equals(typedVariableValue.Type))
		{
		  return responseForFileVariable((FileValue) typedVariableValue);
		}
		else
		{
		  throw new InvalidRequestException(Response.Status.BAD_REQUEST, string.Format("Value of variable with id {0} is not a binary value.", id));
		}
	  }


	  /// <summary>
	  /// Creates a response for a variable of type <seealso cref="ValueType.FILE"/>.
	  /// </summary>
	  protected internal virtual Response responseForFileVariable(FileValue fileValue)
	  {
		string type = fileValue.MimeType != null ? fileValue.MimeType : MediaType.APPLICATION_OCTET_STREAM;
		if (fileValue.Encoding != null)
		{
		  type += "; charset=" + fileValue.Encoding;
		}
		object value = fileValue.Value == null ? "" : fileValue.Value;
		return Response.ok(value, type).header("Content-Disposition", "attachment; filename=" + fileValue.Filename).build();
	  }

	  /// <summary>
	  /// Creates a response for a variable of type <seealso cref="ValueType.BYTES"/>.
	  /// </summary>
	  protected internal virtual Response responseForByteVariable(TypedValue variableInstance)
	  {
		sbyte[] valueBytes = (sbyte[]) variableInstance.Value;
		if (valueBytes == null)
		{
		  valueBytes = new sbyte[0];
		}
		return Response.ok(new MemoryStream(valueBytes), MediaType.APPLICATION_OCTET_STREAM).build();
	  }
	}

}