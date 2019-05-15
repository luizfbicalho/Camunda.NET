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
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class PeriodUnitConverter : JacksonAwareStringToTypeConverter<PeriodUnit>
	{

	  public override PeriodUnit convertQueryParameterToType(string value)
	  {
		return mapToEnum(value, typeof(PeriodUnit));
	  }

	  protected internal virtual T mapToEnum<T>(string value, Type<T> type) where T : Enum<T>
	  {
		try
		{
		  return Enum.valueOf(type, value.ToUpper());
		}
		catch (System.ArgumentException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, string.Format("Cannot convert value {0} to java enum type {1}", value, type.FullName));
		}
		catch (System.NullReferenceException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new InvalidRequestException(Status.BAD_REQUEST, e, string.Format("Cannot convert value {0} to java enum type {1}", value, type.FullName));
		}
	  }

	}

}