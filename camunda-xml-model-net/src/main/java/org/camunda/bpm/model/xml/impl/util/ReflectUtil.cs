using System;
using System.Threading;
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
namespace org.camunda.bpm.model.xml.impl.util
{



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class ReflectUtil
	{

	  public static Stream getResourceAsStream(string name)
	  {
		// Try the current Thread context class loader
		ClassLoader classLoader = Thread.CurrentThread.ContextClassLoader;
		Stream resourceStream = classLoader.getResourceAsStream(name);
		if (resourceStream == null)
		{
		  // Finally, try the class loader for this class
		  classLoader = typeof(ReflectUtil).ClassLoader;
		  resourceStream = classLoader.getResourceAsStream(name);
		}

		return resourceStream;
	  }

	  public static URL getResource(string name)
	  {
		return getResource(name, null);
	  }

	  public static URL getResource(string name, ClassLoader classLoader)
	  {
		if (classLoader == null)
		{
		  // Try the current Thread context class loader
		  classLoader = Thread.CurrentThread.ContextClassLoader;
		}
		URL url = classLoader.getResource(name);
		if (url == null)
		{
		  // Finally, try the class loader for this class
		  classLoader = typeof(ReflectUtil).ClassLoader;
		  url = classLoader.getResource(name);
		}

		return url;
	  }

	  public static File getResourceAsFile(string path)
	  {
		URL resource = getResource(path);
		try
		{
		  return new File(resource.toURI());
		}
		catch (URISyntaxException e)
		{
		  throw new ModelException("Exception while loading resource file " + path, e);
		}
	  }

	  /// <summary>
	  /// Create a new instance of the provided type
	  /// </summary>
	  /// <param name="type"> the class to create a new instance of </param>
	  /// <param name="parameters"> the parameters to pass to the constructor </param>
	  /// <returns> the created instance </returns>
	  public static T createInstance<T>(Type<T> type, params object[] parameters)
	  {

		// get types for parameters
		Type[] parameterTypes = new Type[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
		  object parameter = parameters[i];
		  parameterTypes[i] = parameter.GetType();
		}

		try
		{
		  // create instance
		  System.Reflection.ConstructorInfo<T> constructor = type.GetConstructor(parameterTypes);
		  return constructor.newInstance(parameters);

		}
		catch (Exception e)
		{
		  throw new ModelException("Exception while creating an instance of type " + type, e);
		}
	  }

	}

}