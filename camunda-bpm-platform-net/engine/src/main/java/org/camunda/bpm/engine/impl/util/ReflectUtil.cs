using System;
using System.Collections.Generic;
using System.Reflection;
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
namespace org.camunda.bpm.engine.impl.util
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class ReflectUtil
	{

	  private static readonly EngineUtilLogger LOG = ProcessEngineLogger.UTIL_LOGGER;

	  private static readonly IDictionary<string, string> charEncodings = new Dictionary<string, string>();

	  static ReflectUtil()
	  {
		charEncodings["ä"] = "%C3%A4";
		charEncodings["ö"] = "%C3%B6";
		charEncodings["ü"] = "%C3%BC";
		charEncodings["Ä"] = "%C3%84";
		charEncodings["Ö"] = "%C3%96";
		charEncodings["Ü"] = "%C3%9C";
	  }

	  public static ClassLoader ClassLoader
	  {
		  get
		  {
			ClassLoader loader = CustomClassLoader;
			if (loader == null)
			{
			  loader = Thread.CurrentThread.ContextClassLoader;
			}
			return loader;
		  }
	  }

	  public static Type loadClass(string className)
	  {
	   Type clazz = null;
	   ClassLoader classLoader = CustomClassLoader;

	   // First exception in chain of classloaders will be used as cause when no class is found in any of them
	   Exception throwable = null;

	   if (classLoader != null)
	   {
		 try
		 {
		   LOG.debugClassLoading(className, "custom classloader", classLoader);
		   clazz = Type.GetType(className, true, classLoader);
		 }
		 catch (Exception t)
		 {
		   throwable = t;
		 }
	   }
	   if (clazz == null)
	   {
		 try
		 {
		   ClassLoader contextClassloader = ClassLoaderUtil.ContextClassloader;
		   if (contextClassloader != null)
		   {
			 LOG.debugClassLoading(className, "current thread context classloader", contextClassloader);
			 clazz = Type.GetType(className, true, contextClassloader);
		   }
		 }
		 catch (Exception t)
		 {
		   if (throwable == null)
		   {
			 throwable = t;
		   }
		 }
		 if (clazz == null)
		 {
		   try
		   {
			 ClassLoader localClassloader = ClassLoaderUtil.getClassloader(typeof(ReflectUtil));
			 LOG.debugClassLoading(className, "local classloader", localClassloader);
			 clazz = Type.GetType(className, true, localClassloader);
		   }
		   catch (Exception t)
		   {
			 if (throwable == null)
			 {
			   throwable = t;
			 }
		   }
		 }
	   }

	   if (clazz == null)
	   {
		 throw LOG.classLoadingException(className, throwable);
	   }
	   return clazz;
	  }

	  public static Stream getResourceAsStream(string name)
	  {
		Stream resourceStream = null;
		ClassLoader classLoader = CustomClassLoader;
		if (classLoader != null)
		{
		  resourceStream = classLoader.getResourceAsStream(name);
		}

		if (resourceStream == null)
		{
		  // Try the current Thread context classloader
		  classLoader = Thread.CurrentThread.ContextClassLoader;
		  resourceStream = classLoader.getResourceAsStream(name);
		  if (resourceStream == null)
		  {
			// Finally, try the classloader for this class
			classLoader = typeof(ReflectUtil).ClassLoader;
			resourceStream = classLoader.getResourceAsStream(name);
		  }
		}
		return resourceStream;
	  }

	  public static URL getResource(string name)
	  {
		URL url = null;
		ClassLoader classLoader = CustomClassLoader;
		if (classLoader != null)
		{
		  url = classLoader.getResource(name);
		}
		if (url == null)
		{
		  // Try the current Thread context classloader
		  classLoader = Thread.CurrentThread.ContextClassLoader;
		  url = classLoader.getResource(name);
		  if (url == null)
		  {
			// Finally, try the classloader for this class
			classLoader = typeof(ReflectUtil).ClassLoader;
			url = classLoader.getResource(name);
		  }
		}

		return url;
	  }

	  public static string getResourceUrlAsString(string name)
	  {
		string url = getResource(name).ToString();
		foreach (KeyValuePair<string, string> mapping in charEncodings.SetOfKeyValuePairs())
		{
		  url = url.replaceAll(mapping.Key, mapping.Value);
		}
		return url;
	  }

	  /// <summary>
	  /// Converts an url to an uri. Escapes whitespaces if needed.
	  /// </summary>
	  /// <param name="url">  the url to convert </param>
	  /// <returns> the resulting uri </returns>
	  /// <exception cref="ProcessEngineException"> if the url has invalid syntax </exception>
	  public static URI urlToURI(URL url)
	  {
		try
		{
		  return url.toURI();
		}
		catch (URISyntaxException e)
		{
		  throw LOG.cannotConvertUrlToUri(url, e);
		}
	  }


	  public static object instantiate(string className)
	  {
		try
		{
		  Type clazz = loadClass(className);
		  return System.Activator.CreateInstance(clazz);
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileInstantiatingClass(className, e);
		}
	  }

	  public static T instantiate<T>(Type<T> type)
	  {
		try
		{
		  return System.Activator.CreateInstance(type);
		}
		catch (Exception e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.exceptionWhileInstantiatingClass(type.FullName, e);
		}
	  }

	  public static object invoke(object target, string methodName, object[] args)
	  {
		try
		{
		  Type clazz = target.GetType();
		  System.Reflection.MethodInfo method = findMethod(clazz, methodName, args);
		  method.Accessible = true;
		  return method.invoke(target, args);
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileInvokingMethod(methodName, target, e);
		}
	  }

	  /// <summary>
	  /// Returns the field of the given object or null if it doesnt exist.
	  /// </summary>
	  public static System.Reflection.FieldInfo getField(string fieldName, object @object)
	  {
		return getField(fieldName, @object.GetType());
	  }

	  /// <summary>
	  /// Returns the field of the given class or null if it doesnt exist.
	  /// </summary>
	  public static System.Reflection.FieldInfo getField(string fieldName, Type clazz)
	  {
		System.Reflection.FieldInfo field = null;
		try
		{
		  field = clazz.getDeclaredField(fieldName);
		}
		catch (SecurityException)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.unableToAccessField(field, clazz.FullName);
		}
		catch (NoSuchFieldException)
		{
		  // for some reason getDeclaredFields doesnt search superclasses
		  // (which getFields() does ... but that gives only public fields)
		  Type superClass = clazz.BaseType;
		  if (superClass != null)
		  {
			return getField(fieldName, superClass);
		  }
		}
		return field;
	  }

	  public static void setField(System.Reflection.FieldInfo field, object @object, object value)
	  {
		try
		{
		  field.Accessible = true;
		  field.set(@object, value);
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileSettingField(field, @object, value, e);
		}
	  }

	  /// <summary>
	  /// Returns the setter-method for the given field name or null if no setter exists.
	  /// </summary>
	  public static System.Reflection.MethodInfo getSetter(string fieldName, Type clazz, Type fieldType)
	  {
		string setterName = buildSetterName(fieldName);
		try
		{
		  // Using getMathods(), getMathod(...) expects exact parameter type
		  // matching and ignores inheritance-tree.
		  System.Reflection.MethodInfo[] methods = clazz.GetMethods();
		  foreach (System.Reflection.MethodInfo method in methods)
		  {
			if (method.Name.Equals(setterName))
			{
			  Type[] paramTypes = method.ParameterTypes;
			  if (paramTypes != null && paramTypes.Length == 1 && paramTypes[0].IsAssignableFrom(fieldType))
			  {
				return method;
			  }
			}
		  }
		  return null;
		}
		catch (SecurityException)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.unableToAccessMethod(setterName, clazz.FullName);
		}
	  }

	  /// <summary>
	  /// Returns a setter method based on the fieldName and the java beans setter naming convention or null if none exists.
	  /// If multiple setters with different parameter types are present, an exception is thrown.
	  /// If they have the same parameter type, one of those methods is returned.
	  /// </summary>
	  public static System.Reflection.MethodInfo getSingleSetter(string fieldName, Type clazz)
	  {
		string setterName = buildSetterName(fieldName);
		try
		{
		  // Using getMathods(), getMathod(...) expects exact parameter type
		  // matching and ignores inheritance-tree.
		  System.Reflection.MethodInfo[] methods = clazz.GetMethods();
		  IList<System.Reflection.MethodInfo> candidates = new List<System.Reflection.MethodInfo>();
		  ISet<Type> parameterTypes = new HashSet<Type>();
		  foreach (System.Reflection.MethodInfo method in methods)
		  {
			if (method.Name.Equals(setterName))
			{
			  Type[] paramTypes = method.ParameterTypes;

			  if (paramTypes != null && paramTypes.Length == 1)
			  {
				candidates.Add(method);
				parameterTypes.Add(paramTypes[0]);
			  }
			}
		  }

		  if (parameterTypes.Count > 1)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw LOG.ambiguousSetterMethod(setterName, clazz.FullName);
		  }
		  if (candidates.Count >= 1)
		  {
			return candidates[0];
		  }

		  return null;
		}
		catch (SecurityException)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw LOG.unableToAccessMethod(setterName, clazz.FullName);
		}
	  }

	  private static string buildSetterName(string fieldName)
	  {
		return "set" + Character.toTitleCase(fieldName[0]) + fieldName.Substring(1, fieldName.Length - 1);
	  }

	  private static System.Reflection.MethodInfo findMethod(Type clazz, string methodName, object[] args)
	  {
		foreach (System.Reflection.MethodInfo method in clazz.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
		{
		  // TODO add parameter matching
		  if (method.Name.Equals(methodName) && matches(method.ParameterTypes, args))
		  {
			return method;
		  }
		}
		Type superClass = clazz.BaseType;
		if (superClass != null)
		{
		  return findMethod(superClass, methodName, args);
		}
		return null;
	  }

	  public static object instantiate(string className, object[] args)
	  {
		Type clazz = loadClass(className);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Constructor<?> constructor = findMatchingConstructor(clazz, args);
		System.Reflection.ConstructorInfo<object> constructor = findMatchingConstructor(clazz, args);
		ensureNotNull("couldn't find constructor for " + className + " with args " + Arrays.asList(args), "constructor", constructor);
		try
		{
		  return constructor.newInstance(args);
		}
		catch (Exception e)
		{
		  throw LOG.exceptionWhileInstantiatingClass(className, e);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) private static <T> Constructor<T> findMatchingConstructor(Class<T> clazz, Object[] args)
	  private static System.Reflection.ConstructorInfo<T> findMatchingConstructor<T>(Type<T> clazz, object[] args)
	  {
		foreach (System.Reflection.ConstructorInfo constructor in clazz.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
		{ // cannot use <?> or <T> due to JDK 5/6 incompatibility
		  if (matches(constructor.ParameterTypes, args))
		  {
			return constructor;
		  }
		}
		return null;
	  }

	  private static bool matches(Type[] parameterTypes, object[] args)
	  {
		if ((parameterTypes == null) || (parameterTypes.Length == 0))
		{
		  return ((args == null) || (args.Length == 0));
		}
		if ((args == null) || (parameterTypes.Length != args.Length))
		{
		  return false;
		}
		for (int i = 0; i < parameterTypes.Length; i++)
		{
		  if ((args[i] != null) && (!parameterTypes[i].IsAssignableFrom(args[i].GetType())))
		  {
			return false;
		  }
		}
		return true;
	  }

	  private static ClassLoader CustomClassLoader
	  {
		  get
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
			if (processEngineConfiguration != null)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ClassLoader classLoader = processEngineConfiguration.getClassLoader();
			  ClassLoader classLoader = processEngineConfiguration.ClassLoader;
			  if (classLoader != null)
			  {
				return classLoader;
			  }
			}
			return null;
		  }
	  }

	  /// <summary>
	  /// Finds a method by name and parameter types.
	  /// </summary>
	  /// <param name="declaringType"> the name of the class </param>
	  /// <param name="methodName"> the name of the method to look for </param>
	  /// <param name="parameterTypes"> the types of the parameters </param>
	  public static System.Reflection.MethodInfo getMethod(Type declaringType, string methodName, params Type[] parameterTypes)
	  {
		return findMethod(declaringType, methodName, parameterTypes);
	  }
	}

}