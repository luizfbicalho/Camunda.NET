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
namespace org.camunda.bpm.engine.rest.util
{
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ProvidersUtil
	{

	  public static T resolveFromContext<T>(Providers providers, Type clazz)
	  {
			  clazz = typeof(T);
		return resolveFromContext(providers, clazz, null);
	  }

	  public static T resolveFromContext<T>(Providers providers, Type clazz, Type type)
	  {
			  clazz = typeof(T);
		return resolveFromContext(providers, clazz, null, type);
	  }

	  public static T resolveFromContext<T>(Providers providers, Type clazz, MediaType mediaType, Type type)
	  {
			  clazz = typeof(T);
		ContextResolver<T> contextResolver = providers.getContextResolver(clazz, mediaType);

		if (contextResolver == null)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new RestException("No context resolver found for class " + clazz.FullName);
		}

		return contextResolver.getContext(type);
	  }
	}

}