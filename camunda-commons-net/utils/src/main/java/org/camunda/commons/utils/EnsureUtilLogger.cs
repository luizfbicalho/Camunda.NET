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
	public class EnsureUtilLogger : UtilsLogger
	{

	  public virtual System.ArgumentException parameterIsNullException(string parameterName)
	  {
		return new System.ArgumentException(exceptionMessage("001", "Parameter '{}' is null", parameterName));
	  }

	  public virtual System.ArgumentException unsupportedParameterType(string parameterName, object param, Type expectedType)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return new System.ArgumentException(exceptionMessage("002", "Unsupported parameter '{}' of type '{}'. Expected type '{}'.", parameterName, param.GetType(), expectedType.FullName));
	  }
	}

}