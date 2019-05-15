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

	using JacksonAwareStringToTypeConverter = org.camunda.bpm.engine.rest.dto.converter.JacksonAwareStringToTypeConverter;
	using StringToTypeConverter = org.camunda.bpm.engine.rest.dto.converter.StringToTypeConverter;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;


	using JsonIgnore = com.fasterxml.jackson.annotation.JsonIgnore;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class AbstractSearchQueryDto
	{

	  protected internal ObjectMapper objectMapper;

	  // required for populating via jackson
	  public AbstractSearchQueryDto()
	  {

	  }

	  public AbstractSearchQueryDto(ObjectMapper objectMapper, MultivaluedMap<string, string> queryParameters)
	  {
		this.objectMapper = objectMapper;
		foreach (KeyValuePair<string, IList<string>> param in queryParameters.entrySet())
		{
		  string key = param.Key;
		  string value = param.Value.GetEnumerator().next();
		  this.setValueBasedOnAnnotation(key, value);
		}
	  }

	  // note: with Jackson version >= 1.9, it would be better to use @JacksonInject and
	  // configure the object mapper in the JacksonConfigurator class to be an injectable value.
	  // then, explicitly calling this method with every query is not necessary any longer
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @JsonIgnore public void setObjectMapper(com.fasterxml.jackson.databind.ObjectMapper objectMapper)
	  public virtual ObjectMapper ObjectMapper
	  {
		  set
		  {
			this.objectMapper = value;
		  }
	  }

	  /// <summary>
	  /// Finds the methods that are annotated with a <seealso cref="CamundaQueryParam"/> with a value that matches the key parameter.
	  /// Before invoking these methods, the annotated <seealso cref="StringToTypeConverter"/> is used to convert the String value to the desired Java type. </summary>
	  /// <param name="key"> </param>
	  /// <param name="value"> </param>
	  protected internal virtual void setValueBasedOnAnnotation(string key, string value)
	  {
		IList<System.Reflection.MethodInfo> matchingMethods = findMatchingAnnotatedMethods(key);
		foreach (System.Reflection.MethodInfo method in matchingMethods)
		{
		  Type converterClass = findAnnotatedTypeConverter(method);
		  if (converterClass == null)
		  {
			continue;
		  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.rest.dto.converter.JacksonAwareStringToTypeConverter<?> converter = null;
		  JacksonAwareStringToTypeConverter<object> converter = null;
		  try
		  {
			converter = System.Activator.CreateInstance(converterClass);
			converter.ObjectMapper = objectMapper;
			object convertedValue = converter.convertQueryParameterToType(value);
			method.invoke(this, convertedValue);
		  }
		  catch (InstantiationException e)
		  {
			throw new RestException(Status.INTERNAL_SERVER_ERROR, e, "Server error.");
		  }
		  catch (IllegalAccessException e)
		  {
			throw new RestException(Status.INTERNAL_SERVER_ERROR, e, "Server error.");
		  }
		  catch (InvocationTargetException e)
		  {
			throw new InvalidRequestException(Status.BAD_REQUEST, e, "Cannot set query parameter '" + key + "' to value '" + value + "'");
		  }
		  catch (RestException e)
		  {
			throw new InvalidRequestException(e.Status, e, "Cannot set query parameter '" + key + "' to value '" + value + "': " + e.Message);
		  }
		}
	  }

	  private IList<System.Reflection.MethodInfo> findMatchingAnnotatedMethods(string parameterName)
	  {
		IList<System.Reflection.MethodInfo> result = new List<System.Reflection.MethodInfo>();
		System.Reflection.MethodInfo[] methods = this.GetType().GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
		  System.Reflection.MethodInfo method = methods[i];
		  Annotation[] methodAnnotations = method.GetCustomAttributes(true);

		  for (int j = 0; j < methodAnnotations.Length; j++)
		  {
			Annotation annotation = methodAnnotations[j];
			if (annotation is CamundaQueryParam)
			{
			  CamundaQueryParam parameterAnnotation = (CamundaQueryParam) annotation;
			  if (parameterAnnotation.value().Equals(parameterName))
			  {
				result.Add(method);
			  }
			}
		  }
		}
		return result;
	  }

	  private Type findAnnotatedTypeConverter(System.Reflection.MethodInfo method)
	  {
		Annotation[] methodAnnotations = method.GetCustomAttributes(true);

		for (int j = 0; j < methodAnnotations.Length; j++)
		{
		  Annotation annotation = methodAnnotations[j];
		  if (annotation is CamundaQueryParam)
		  {
			CamundaQueryParam parameterAnnotation = (CamundaQueryParam) annotation;
			return parameterAnnotation.converter();
		  }
		}
		return null;
	  }

	}

}