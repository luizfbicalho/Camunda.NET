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
namespace org.camunda.bpm.application
{

	using ProcessApplicationContextImpl = org.camunda.bpm.application.impl.ProcessApplicationContextImpl;
	using ProcessApplicationIdentifier = org.camunda.bpm.application.impl.ProcessApplicationIdentifier;

	/// <summary>
	/// <para>A utility to declare the process application in which subsequent engine API calls
	/// are executed. Process application context is important for the engine
	/// to access custom classes as well as process-application-level entities like
	/// script engines or Spin data formats.
	/// 
	/// </para>
	/// <para>By default, the process engine only guarantees to switch into the context
	/// of the process application when it executes custom code (e.g. a JavaDelegate).
	/// This utility allows to declare a process application into which the process engine
	/// then switches as soon as it begins executing a command.
	/// 
	/// Example using a variable that is serialized with a Camunda Spin data format:
	/// 
	/// <pre>
	///  try {
	///    ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
	///    runtimeService.setVariable(
	///      "processInstanceId",
	///      "variableName",
	///      Variables.objectValue(anObject).serializationDataFormat(SerializationDataFormats.JSON).create());
	///  } finally {
	///    ProcessApplicationContext.clear();
	///  }
	/// </pre>
	/// 
	/// </para>
	/// <para>Declaring the process application context allows the engine to access the Spin JSON data format
	/// as configured in that process application to serialize the object value. Without declaring the context,
	/// the global json data format is used.
	/// 
	/// </para>
	/// <para>Declaring the context process application affects only engine API invocations. It DOES NOT affect
	/// the context class loader for subsequent code.
	/// 
	/// @author Thorben Lindhauer
	/// </para>
	/// </summary>
	public class ProcessApplicationContext
	{

	  /// <summary>
	  /// Declares the context process application for all subsequent engine API invocations
	  /// until <seealso cref="clear()"/> is called. The context is bound to the current thread.
	  /// This method should always be used in a try-finally block to ensure that <seealso cref="clear()"/>
	  /// is called under any circumstances.
	  /// </summary>
	  /// <param name="processApplicationName"> the name of the process application to switch into </param>
	  public static string CurrentProcessApplication
	  {
		  set
		  {
			ProcessApplicationContextImpl.set(new ProcessApplicationIdentifier(value));
		  }
	  }

	  /// <summary>
	  /// Declares the context process application for all subsequent engine API invocations
	  /// until <seealso cref="clear()"/> is called. The context is bound to the current thread.
	  /// This method should always be used in a try-finally block to ensure that <seealso cref="clear()"/>
	  /// is called under any circumstances.
	  /// </summary>
	  /// <param name="reference"> a reference to the process application to switch into </param>
	  public static ProcessApplicationReference CurrentProcessApplication
	  {
		  set
		  {
			ProcessApplicationContextImpl.set(new ProcessApplicationIdentifier(value));
		  }
	  }

	  /// <summary>
	  /// Declares the context process application for all subsequent engine API invocations
	  /// until <seealso cref="clear()"/> is called. The context is bound to the current thread.
	  /// This method should always be used in a try-finally block to ensure that <seealso cref="clear()"/>
	  /// is called under any circumstances.
	  /// </summary>
	  /// <param name="processApplication"> the process application to switch into </param>
	  public static ProcessApplicationInterface CurrentProcessApplication
	  {
		  set
		  {
			ProcessApplicationContextImpl.set(new ProcessApplicationIdentifier(value));
		  }
	  }

	  /// <summary>
	  /// Clears the currently declared context process application.
	  /// </summary>
	  public static void clear()
	  {
		ProcessApplicationContextImpl.clear();
	  }

	  /// <summary>
	  /// <para>Takes a callable and executes all engine API invocations within that callable in the context
	  /// of the given process application
	  /// 
	  /// </para>
	  /// <para>Equivalent to
	  /// <pre>
	  ///   try {
	  ///     ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
	  ///     callable.call();
	  ///   } finally {
	  ///     ProcessApplicationContext.clear();
	  ///   }
	  /// </pre>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="callable"> the callable to execute </param>
	  /// <param name="name"> the name of the process application to switch into </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T> T withProcessApplicationContext(java.util.concurrent.Callable<T> callable, String processApplicationName) throws Exception
	  public static T withProcessApplicationContext<T>(Callable<T> callable, string processApplicationName)
	  {
		try
		{
		  CurrentProcessApplication = processApplicationName;
		  return callable.call();
		}
		finally
		{
		  clear();
		}
	  }

	  /// <summary>
	  /// <para>Takes a callable and executes all engine API invocations within that callable in the context
	  /// of the given process application
	  /// 
	  /// </para>
	  /// <para>Equivalent to
	  /// <pre>
	  ///   try {
	  ///     ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
	  ///     callable.call();
	  ///   } finally {
	  ///     ProcessApplicationContext.clear();
	  ///   }
	  /// </pre>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="callable"> the callable to execute </param>
	  /// <param name="reference"> a reference of the process application to switch into </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T> T withProcessApplicationContext(java.util.concurrent.Callable<T> callable, ProcessApplicationReference reference) throws Exception
	  public static T withProcessApplicationContext<T>(Callable<T> callable, ProcessApplicationReference reference)
	  {
		try
		{
		  CurrentProcessApplication = reference;
		  return callable.call();
		}
		finally
		{
		  clear();
		}
	  }

	  /// <summary>
	  /// <para>Takes a callable and executes all engine API invocations within that callable in the context
	  /// of the given process application
	  /// 
	  /// </para>
	  /// <para>Equivalent to
	  /// <pre>
	  ///   try {
	  ///     ProcessApplicationContext.setCurrentProcessApplication("someProcessApplication");
	  ///     callable.call();
	  ///   } finally {
	  ///     ProcessApplicationContext.clear();
	  ///   }
	  /// </pre>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="callable"> the callable to execute </param>
	  /// <param name="processApplication"> the process application to switch into </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T> T withProcessApplicationContext(java.util.concurrent.Callable<T> callable, ProcessApplicationInterface processApplication) throws Exception
	  public static T withProcessApplicationContext<T>(Callable<T> callable, ProcessApplicationInterface processApplication)
	  {
		try
		{
		  CurrentProcessApplication = processApplication;
		  return callable.call();
		}
		finally
		{
		  clear();
		}
	  }
	}

}