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
namespace org.camunda.bpm.engine.impl.util
{

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Sebastian Menski
	/// @author Roman Smirnov
	/// </summary>
	public sealed class EnsureUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  public static void ensureNotNull(string variableName, object value)
	  {
		ensureNotNull("", variableName, value);
	  }

	  public static void ensureNotNull(Type exceptionClass, string variableName, object value)
	  {
		ensureNotNull(exceptionClass, null, variableName, value);
	  }

	  public static void ensureNotNull(string message, string variableName, object value)
	  {
		ensureNotNull(typeof(NullValueException), message, variableName, value);
	  }

	  public static void ensureNotNull(Type exceptionClass, string message, string variableName, object value)
	  {
		if (value == null)
		{
		  throw generateException(exceptionClass, message, variableName, "is null");
		}
	  }

	  public static void ensureNull(Type exceptionClass, string message, string variableName, object value)
	  {
		if (value != null)
		{
		  throw generateException(exceptionClass, message, variableName, "is not null");
		}
	  }

	  public static void ensureNotNull(string variableName, params object[] values)
	  {
		ensureNotNull("", variableName, values);
	  }

	  public static void ensureNotNull(Type exceptionClass, string variableName, params object[] values)
	  {
		ensureNotNull(exceptionClass, null, variableName, values);
	  }

	  public static void ensureNotNull(string message, string variableName, params object[] values)
	  {
		ensureNotNull(typeof(NullValueException), message, variableName, values);
	  }

	  public static void ensureNotNull(Type exceptionClass, string message, string variableName, params object[] values)
	  {
		if (values == null)
		{
		  throw generateException(exceptionClass, message, variableName, "is null");
		}
		foreach (object value in values)
		{
		  if (value == null)
		  {
			throw generateException(exceptionClass, message, variableName, "contains null value");
		  }
		}
	  }

	  public static void ensureNotEmpty(string variableName, string value)
	  {
		ensureNotEmpty("", variableName, value);
	  }

	  public static void ensureNotEmpty(Type exceptionClass, string variableName, string value)
	  {
		ensureNotEmpty(exceptionClass, null, variableName, value);
	  }

	  public static void ensureNotEmpty(string message, string variableName, string value)
	  {
		ensureNotEmpty(typeof(ProcessEngineException), message, variableName, value);
	  }

	  public static void ensureNotEmpty(Type exceptionClass, string message, string variableName, string value)
	  {
		ensureNotNull(exceptionClass, message, variableName, value);
		if (value.Trim().Length == 0)
		{
		  throw generateException(exceptionClass, message, variableName, "is empty");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(String variableName, java.util.Collection collection)
	  public static void ensureNotEmpty(string variableName, System.Collections.ICollection collection)
	  {
		ensureNotEmpty("", variableName, collection);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(Class exceptionClass, String variableName, java.util.Collection collection)
	  public static void ensureNotEmpty(Type exceptionClass, string variableName, System.Collections.ICollection collection)
	  {
		ensureNotEmpty(exceptionClass, null, variableName, collection);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(String message, String variableName, java.util.Collection collection)
	  public static void ensureNotEmpty(string message, string variableName, System.Collections.ICollection collection)
	  {
		ensureNotEmpty(typeof(ProcessEngineException), message, variableName, collection);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(Class exceptionClass, String message, String variableName, java.util.Collection collection)
	  public static void ensureNotEmpty(Type exceptionClass, string message, string variableName, System.Collections.ICollection collection)
	  {
		ensureNotNull(exceptionClass, message, variableName, collection);
		if (collection.Count == 0)
		{
		  throw generateException(exceptionClass, message, variableName, "is empty");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(String variableName, java.util.Map map)
	  public static void ensureNotEmpty(string variableName, System.Collections.IDictionary map)
	  {
		ensureNotEmpty("", variableName, map);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(Class exceptionClass, String variableName, java.util.Map map)
	  public static void ensureNotEmpty(Type exceptionClass, string variableName, System.Collections.IDictionary map)
	  {
		ensureNotEmpty(exceptionClass, null, variableName, map);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(String message, String variableName, java.util.Map map)
	  public static void ensureNotEmpty(string message, string variableName, System.Collections.IDictionary map)
	  {
		ensureNotEmpty(typeof(ProcessEngineException), message, variableName, map);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNotEmpty(Class exceptionClass, String message, String variableName, java.util.Map map)
	  public static void ensureNotEmpty(Type exceptionClass, string message, string variableName, System.Collections.IDictionary map)
	  {
		ensureNotNull(exceptionClass, message, variableName, map);
		if (map.Count == 0)
		{
		  throw generateException(exceptionClass, message, variableName, "is empty");
		}
	  }

	  public static void ensureEquals(Type exceptionClass, string variableName, long obj1, long obj2)
	  {
		if (obj1 != obj2)
		{
		  throw generateException(exceptionClass, "", variableName, "value differs from expected");
		}
	  }

	  public static void ensureEquals(string variableName, long obj1, long obj2)
	  {
		ensureEquals(typeof(ProcessEngineException), variableName, obj1, obj2);
	  }

	  public static void ensurePositive(string variableName, long? value)
	  {
		ensurePositive("", variableName, value);
	  }

	  public static void ensurePositive(Type exceptionClass, string variableName, long? value)
	  {
		ensurePositive(exceptionClass, null, variableName, value);
	  }

	  public static void ensurePositive(string message, string variableName, long? value)
	  {
		ensurePositive(typeof(ProcessEngineException), message, variableName, value);
	  }

	  public static void ensurePositive(Type exceptionClass, string message, string variableName, long? value)
	  {
		ensureNotNull(exceptionClass, variableName, value);
		if (value <= 0)
		{
		  throw generateException(exceptionClass, message, variableName, "is not greater than 0");
		}
	  }

	  public static void ensureGreaterThanOrEqual(string variableName, long value1, long value2)
	  {
		ensureGreaterThanOrEqual("", variableName, value1, value2);
	  }

	  public static void ensureGreaterThanOrEqual(string message, string variableName, long value1, long value2)
	  {
		ensureGreaterThanOrEqual(typeof(ProcessEngineException), message, variableName, value1, value2);
	  }

	  public static void ensureGreaterThanOrEqual(Type exceptionClass, string message, string variableName, long value1, long value2)
	  {
		if (value1 < value2)
		{
		  throw generateException(exceptionClass, message, variableName, "is not greater than or equal to " + value2);
		}
	  }

	  public static void ensureInstanceOf(string variableName, object value, Type expectedClass)
	  {
		ensureInstanceOf("", variableName, value, expectedClass);
	  }

	  public static void ensureInstanceOf(Type exceptionClass, string variableName, object value, Type expectedClass)
	  {
		ensureInstanceOf(exceptionClass, null, variableName, value, expectedClass);
	  }

	  public static void ensureInstanceOf(string message, string variableName, object value, Type expectedClass)
	  {
		ensureInstanceOf(typeof(ProcessEngineException), message, variableName, value, expectedClass);
	  }

	  public static void ensureInstanceOf(Type exceptionClass, string message, string variableName, object value, Type expectedClass)
	  {
		ensureNotNull(exceptionClass, message, variableName, value);
		Type valueClass = value.GetType();
		if (!expectedClass.IsAssignableFrom(valueClass))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw generateException(exceptionClass, message, variableName, "has class " + valueClass.FullName + " and not " + expectedClass.FullName);
		}
	  }

	  public static void ensureOnlyOneNotNull(string message, params object[] values)
	  {
		ensureOnlyOneNotNull(typeof(NullValueException), message, values);
	  }

	  public static void ensureOnlyOneNotNull(Type exceptionClass, string message, params object[] values)
	  {
		bool oneNotNull = false;
		foreach (object value in values)
		{
		  if (value != null)
		  {
			if (oneNotNull)
			{
			  throw generateException(exceptionClass, null, null, message);
			}
			oneNotNull = true;
		  }
		}
		if (!oneNotNull)
		{
		  throw generateException(exceptionClass, null, null, message);
		}
	  }

	  public static void ensureAtLeastOneNotNull(string message, params object[] values)
	  {
		ensureAtLeastOneNotNull(typeof(NullValueException), message, values);
	  }

	  public static void ensureAtLeastOneNotNull(Type exceptionClass, string message, params object[] values)
	  {
		foreach (object value in values)
		{
		  if (value != null)
		  {
			return;
		  }
		}
		throw generateException(exceptionClass, null, null, message);
	  }

	  public static void ensureAtLeastOneNotEmpty(string message, params string[] values)
	  {
		ensureAtLeastOneNotEmpty(typeof(ProcessEngineException), message, values);
	  }

	  public static void ensureAtLeastOneNotEmpty(Type exceptionClass, string message, params string[] values)
	  {
		foreach (string value in values)
		{
		  if (!string.ReferenceEquals(value, null) && value.Length > 0)
		  {
			return;
		  }
		}
		throw generateException(exceptionClass, null, null, message);
	  }

	  public static void ensureNotContainsEmptyString(string variableName, ICollection<string> values)
	  {
		ensureNotContainsEmptyString((string) null, variableName, values);
	  }

	  public static void ensureNotContainsEmptyString(string message, string variableName, ICollection<string> values)
	  {
		ensureNotContainsEmptyString(typeof(NotValidException), message, variableName, values);
	  }

	  public static void ensureNotContainsEmptyString(Type exceptionClass, string variableName, ICollection<string> values)
	  {
		ensureNotContainsEmptyString(exceptionClass, null, variableName, values);
	  }

	  public static void ensureNotContainsEmptyString(Type exceptionClass, string message, string variableName, ICollection<string> values)
	  {
		ensureNotNull(exceptionClass, message, variableName, values);
		foreach (string value in values)
		{
		  if (value.Length == 0)
		  {
			throw generateException(exceptionClass, message, variableName, "contains empty string");
		  }
		}
	  }

	  public static void ensureNotContainsNull<T1>(string variableName, ICollection<T1> values)
	  {
		ensureNotContainsNull((string) null, variableName, values);
	  }

	  public static void ensureNotContainsNull<T1>(string message, string variableName, ICollection<T1> values)
	  {
		ensureNotContainsNull(typeof(NullValueException), message, variableName, values);
	  }

	  public static void ensureNotContainsNull<T1>(Type exceptionClass, string variableName, ICollection<T1> values)
	  {
		ensureNotContainsNull(exceptionClass, null, variableName, values);
	  }

	  public static void ensureNotContainsNull<T1>(Type exceptionClass, string message, string variableName, ICollection<T1> values)
	  {
		ensureNotNull(exceptionClass, message, variableName, values.toArray(new object[values.Count]));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNumberOfElements(String variableName, java.util.Collection collection, int elements)
	  public static void ensureNumberOfElements(string variableName, System.Collections.ICollection collection, int elements)
	  {
		ensureNumberOfElements("", variableName, collection, elements);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNumberOfElements(String message, String variableName, java.util.Collection collection, int elements)
	  public static void ensureNumberOfElements(string message, string variableName, System.Collections.ICollection collection, int elements)
	  {
		ensureNumberOfElements(typeof(ProcessEngineException), message, variableName, collection, elements);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNumberOfElements(Class exceptionClass, String variableName, java.util.Collection collection, int elements)
	  public static void ensureNumberOfElements(Type exceptionClass, string variableName, System.Collections.ICollection collection, int elements)
	  {
		ensureNumberOfElements(exceptionClass, "", variableName, collection, elements);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static void ensureNumberOfElements(Class exceptionClass, String message, String variableName, java.util.Collection collection, int elements)
	  public static void ensureNumberOfElements(Type exceptionClass, string message, string variableName, System.Collections.ICollection collection, int elements)
	  {
		ensureNotNull(exceptionClass, message, variableName, collection);
		if (collection.Count != elements)
		{
		  throw generateException(exceptionClass, message, variableName, "does not have " + elements + " elements");
		}
	  }

	  public static void ensureValidIndividualResourceId(string message, string id)
	  {
		ensureValidIndividualResourceId(typeof(ProcessEngineException), message, id);
	  }

	  public static void ensureValidIndividualResourceId(Type exceptionClass, string message, string id)
	  {
		ensureNotNull(exceptionClass, message, "id", id);
		if (org.camunda.bpm.engine.authorization.Authorization_Fields.ANY.Equals(id))
		{
		  throw generateException(exceptionClass, message, "id", "cannot be " + org.camunda.bpm.engine.authorization.Authorization_Fields.ANY + ". " + org.camunda.bpm.engine.authorization.Authorization_Fields.ANY + " is a reserved identifier.");
		}
	  }

	  public static void ensureValidIndividualResourceIds(string message, ICollection<string> ids)
	  {
		ensureValidIndividualResourceIds(typeof(ProcessEngineException), message, ids);
	  }

	  public static void ensureValidIndividualResourceIds(Type exceptionClass, string message, ICollection<string> ids)
	  {
		ensureNotNull(exceptionClass, message, "id", ids);
		foreach (string id in ids)
		{
		  ensureValidIndividualResourceId(exceptionClass, message, id);
		}
	  }

	  public static void ensureWhitelistedResourceId(CommandContext commandContext, string resourceType, string resourceId)
	  {
		string resourcePattern = determineResourceWhitelistPattern(commandContext.ProcessEngineConfiguration, resourceType);
		Pattern PATTERN = Pattern.compile(resourcePattern);

		if (!PATTERN.matcher(resourceId).matches())
		{
		  throw generateException(typeof(ProcessEngineException), resourceType + " has an invalid id", "'" + resourceId + "'", "is not a valid resource identifier.");
		}
	  }

	  protected internal static string determineResourceWhitelistPattern(ProcessEngineConfiguration processEngineConfiguration, string resourceType)
	  {
		string resourcePattern = null;

		if (resourceType.Equals("User"))
		{
		  resourcePattern = processEngineConfiguration.UserResourceWhitelistPattern;
		}

		if (resourceType.Equals("Group"))
		{
		  resourcePattern = processEngineConfiguration.GroupResourceWhitelistPattern;
		}

		if (resourceType.Equals("Tenant"))
		{
		  resourcePattern = processEngineConfiguration.TenantResourceWhitelistPattern;
		}

		if (!string.ReferenceEquals(resourcePattern, null) && resourcePattern.Length > 0)
		{
		  return resourcePattern;
		}

		return processEngineConfiguration.GeneralResourceWhitelistPattern;
	  }

	  protected internal static T generateException<T>(Type exceptionClass, string message, string variableName, string description) where T : org.camunda.bpm.engine.ProcessEngineException
	  {
			  exceptionClass = typeof(T);
		string formattedMessage = formatMessage(message, variableName, description);

		try
		{
		  System.Reflection.ConstructorInfo<T> constructor = exceptionClass.GetConstructor(typeof(string));

		  return constructor.newInstance(formattedMessage);

		}
		catch (Exception e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.exceptionWhileInstantiatingClass(exceptionClass.FullName, e);
		}

	  }

	  protected internal static string formatMessage(string message, string variableName, string description)
	  {
		return formatMessageElement(message, ": ") + formatMessageElement(variableName, " ") + description;
	  }

	  protected internal static string formatMessageElement(string element, string delimiter)
	  {
		if (!string.ReferenceEquals(element, null) && element.Length > 0)
		{
		  return element + delimiter;
		}
		else
		{
		  return "";
		}
	  }

	  public static void ensureActiveCommandContext(string operation)
	  {
		if (Context.CommandContext == null)
		{
		  throw LOG.notInsideCommandContext(operation);
		}
	  }
	}

}