using System;
using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.container.impl.metadata
{

	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PropertyHelper
	{

	  private static readonly ContainerIntegrationLogger LOG = ProcessEngineLogger.CONTAINER_INTEGRATION_LOGGER;

	  /// <summary>
	  /// Regex for Ant-style property placeholders
	  /// </summary>
	  private static readonly Pattern PROPERTY_TEMPLATE = Pattern.compile("([^\\$]*)\\$\\{(.+?)\\}([^\\$]*)");

	  public static bool getBooleanProperty(IDictionary<string, string> properties, string name, bool defaultValue)
	  {
		string value = properties[name];
		if (string.ReferenceEquals(value, null))
		{
		  return defaultValue;
		}
		else
		{
		  return bool.Parse(value);
		}
	  }

	  /// <summary>
	  /// Converts a value to the type of the given field. </summary>
	  /// <param name="value"> </param>
	  /// <param name="field">
	  /// @return </param>
	  public static object convertToClass(string value, Type clazz)
	  {
		object propertyValue;
		if (clazz.IsAssignableFrom(typeof(int)))
		{
		  propertyValue = int.Parse(value);
		}
		else if (clazz.IsAssignableFrom(typeof(long)))
		{
		  propertyValue = long.Parse(value);
		}
		else if (clazz.IsAssignableFrom(typeof(float)))
		{
		  propertyValue = float.Parse(value);
		}
		else if (clazz.IsAssignableFrom(typeof(bool)))
		{
		  propertyValue = bool.Parse(value);
		}
		else
		{
		  propertyValue = value;
		}
		return propertyValue;
	  }

	  public static void applyProperty(object configuration, string key, string stringValue)
	  {
		Type configurationClass = configuration.GetType();

		System.Reflection.MethodInfo setter = ReflectUtil.getSingleSetter(key, configurationClass);

		if (setter != null)
		{
		  try
		  {
			Type parameterClass = setter.ParameterTypes[0];
			object value = PropertyHelper.convertToClass(stringValue, parameterClass);

			setter.invoke(configuration, value);
		  }
		  catch (Exception e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			throw LOG.cannotSetValueForProperty(key, configurationClass.FullName, e);
		  }
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		  throw LOG.cannotFindSetterForProperty(key, configurationClass.FullName);
		}
	  }

	  /// <summary>
	  /// Sets an objects fields via reflection from String values.
	  /// Depending on the field's type the respective values are converted to int or boolean.
	  /// </summary>
	  /// <param name="configuration"> </param>
	  /// <param name="properties"> </param>
	  /// <exception cref="ProcessEngineException"> if a property is supplied that matches no field or
	  /// if the field's type is not String, nor int, nor boolean. </exception>
	  public static void applyProperties(object configuration, IDictionary<string, string> properties)
	  {
		foreach (KeyValuePair<string, string> property in properties.SetOfKeyValuePairs())
		{
		  applyProperty(configuration, property.Key, property.Value);
		}
	  }


	  /// <summary>
	  /// Replaces Ant-style property references if the corresponding keys exist in the provided <seealso cref="Properties"/>.
	  /// </summary>
	  /// <param name="props"> contains possible replacements </param>
	  /// <param name="original"> may contain Ant-style templates </param>
	  /// <returns> the original string with replaced properties or the unchanged original string if no placeholder found. </returns>
	  public static string resolveProperty(Properties props, string original)
	  {
		Matcher matcher = PROPERTY_TEMPLATE.matcher(original);
		StringBuilder buffer = new StringBuilder();
		bool found = false;
		while (matcher.find())
		{
		  found = true;
		  string propertyName = matcher.group(2).Trim();
		  buffer.Append(matcher.group(1)).Append(props.containsKey(propertyName) ? props.getProperty(propertyName) : "").Append(matcher.group(3));
		}
		return found ? buffer.ToString() : original;
	  }

	}

}