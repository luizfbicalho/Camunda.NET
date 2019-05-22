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
namespace org.camunda.commons.utils
{
	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class EnsureUtil
	{

	  private static readonly EnsureUtilLogger LOG = UtilsLogger.ENSURE_UTIL_LOGGER;

	  /// <summary>
	  /// Ensures that the parameter is not null.
	  /// </summary>
	  /// <param name="parameterName"> the parameter name </param>
	  /// <param name="value"> the value to ensure to be not null </param>
	  /// <exception cref="IllegalArgumentException"> if the parameter value is null </exception>
	  public static void ensureNotNull(string parameterName, object value)
	  {
		if (value == null)
		{
		  throw LOG.parameterIsNullException(parameterName);
		}
	  }

	  /// <summary>
	  /// Ensure the object is of a given type and return the casted object
	  /// </summary>
	  /// <param name="objectName"> the name of the parameter </param>
	  /// <param name="object"> the parameter value </param>
	  /// <param name="type"> the expected type </param>
	  /// <returns> the parameter casted to the requested type </returns>
	  /// <exception cref="IllegalArgumentException"> in case object cannot be casted to type </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T ensureParamInstanceOf(String objectName, Object object, Class<T> type)
	  public static T ensureParamInstanceOf<T>(string objectName, object @object, Type type)
	  {
			  type = typeof(T);
		if (type.IsAssignableFrom(@object.GetType()))
		{
		  return (T) @object;
		}
		else
		{
		  throw LOG.unsupportedParameterType(objectName, @object, type);
		}
	  }
	}

}