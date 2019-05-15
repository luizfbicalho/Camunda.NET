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
namespace org.camunda.bpm.engine.rest.dto.converter
{
	using JsonParseException = com.fasterxml.jackson.core.JsonParseException;
	using JsonMappingException = com.fasterxml.jackson.databind.JsonMappingException;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;


	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public abstract class JacksonAwareStringToTypeConverter<T> : StringToTypeConverter<T>
	{

	  protected internal ObjectMapper objectMapper;

	  public abstract T convertQueryParameterToType(string value);

	  public virtual ObjectMapper ObjectMapper
	  {
		  set
		  {
			this.objectMapper = value;
		  }
	  }

	  protected internal virtual T mapToType(string value, Type<T> typeClass)
	  {
		try
		{
		  return objectMapper.readValue(value, typeClass);
		}
		catch (JsonParseException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, string.Format("Cannot convert value {0} to java type {1}", value, typeClass.FullName));
		}
		catch (JsonMappingException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, string.Format("Cannot convert value {0} to java type {1}", value, typeClass.FullName));
		}
		catch (IOException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, string.Format("Cannot convert value {0} to java type {1}", value, typeClass.FullName));
		}
	  }
	}

}