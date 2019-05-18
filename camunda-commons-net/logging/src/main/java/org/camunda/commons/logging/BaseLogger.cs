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
namespace org.camunda.commons.logging
{
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;
	using MessageFormatter = org.slf4j.helpers.MessageFormatter;

	/// <summary>
	/// Base class for implementing a logger class. A logger class is a class with
	/// dedicated methods for each log message:
	/// 
	/// <pre>
	/// public class MyLogger extends BaseLogger {
	/// 
	///   public static MyLogger LOG = createLogger(MyLogger.class, "MYPROJ", "org.example", "01");
	/// 
	///   public void engineStarted(long currentTime) {
	///     logInfo("100", "My super engine has started at '{}'", currentTime);
	///   }
	/// 
	/// }
	/// </pre>
	/// 
	/// The logger can then be used in the following way:
	/// 
	/// <pre>
	/// LOG.engineStarted(System.currentTimeMilliseconds());
	/// </pre>
	/// 
	/// This will print the following message:
	/// <pre>
	/// INFO  org.example - MYPROJ-01100 My super engine has started at '4234234523'
	/// </pre>
	/// 
	/// <h2>Slf4j</h2>
	/// This class uses slf4j as logging API. The class ensures that log messages and exception
	/// messages are always formatted using the same template.
	/// 
	/// <h2>Log message format</h2>
	/// The log message format produced by this class is as follows:
	/// <pre>
	/// [PROJECT_CODE]-[COMPONENT_ID][MESSAGE_ID] message
	/// </pre>
	/// Example:
	/// <pre>
	/// MYPROJ-01100 My super engine has started at '4234234523'
	/// </pre>
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public abstract class BaseLogger
	{

	  /// <summary>
	  /// the slf4j logger we delegate to </summary>
	  protected internal Logger delegateLogger;

	  /// <summary>
	  /// the project code of the logger </summary>
	  protected internal string projectCode;

	  /// <summary>
	  /// the component Id of the logger. </summary>
	  protected internal string componentId;

	  protected internal BaseLogger()
	  {
	  }

	  /// <summary>
	  /// Creates a new instance of the <seealso cref="BaseLogger Logger"/>.
	  /// </summary>
	  /// <param name="loggerClass"> the type of the logger </param>
	  /// <param name="projectCode"> the unique code for a complete project. </param>
	  /// <param name="name"> the name of the slf4j logger to use. </param>
	  /// <param name="componentId"> the unique id of the component. </param>
	  public static T createLogger<T>(Type<T> loggerClass, string projectCode, string name, string componentId) where T : BaseLogger
	  {
		try
		{
		  T logger = System.Activator.CreateInstance(loggerClass);
		  logger.projectCode = projectCode;
		  logger.componentId = componentId;
		  logger.delegateLogger = LoggerFactory.getLogger(name);

		  return logger;

		}
		catch (InstantiationException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new Exception("Unable to instantiate logger '" + loggerClass.FullName + "'", e);

		}
		catch (IllegalAccessException e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new Exception("Unable to instantiate logger '" + loggerClass.FullName + "'", e);

		}
	  }

	  /// <summary>
	  /// Logs a 'DEBUG' message
	  /// </summary>
	  /// <param name="id"> the unique id of this log message </param>
	  /// <param name="messageTemplate"> the message template to use </param>
	  /// <param name="parameters"> a list of optional parameters </param>
	  protected internal virtual void logDebug(string id, string messageTemplate, params object[] parameters)
	  {
		if (delegateLogger.DebugEnabled)
		{
		  string msg = formatMessageTemplate(id, messageTemplate);
		  delegateLogger.debug(msg, parameters);
		}
	  }

	  /// <summary>
	  /// Logs an 'INFO' message
	  /// </summary>
	  /// <param name="id"> the unique id of this log message </param>
	  /// <param name="messageTemplate"> the message template to use </param>
	  /// <param name="parameters"> a list of optional parameters </param>
	  protected internal virtual void logInfo(string id, string messageTemplate, params object[] parameters)
	  {
		if (delegateLogger.InfoEnabled)
		{
		  string msg = formatMessageTemplate(id, messageTemplate);
		  delegateLogger.info(msg, parameters);
		}
	  }

	  /// <summary>
	  /// Logs an 'WARN' message
	  /// </summary>
	  /// <param name="id"> the unique id of this log message </param>
	  /// <param name="messageTemplate"> the message template to use </param>
	  /// <param name="parameters"> a list of optional parameters </param>
	  protected internal virtual void logWarn(string id, string messageTemplate, params object[] parameters)
	  {
		if (delegateLogger.WarnEnabled)
		{
		  string msg = formatMessageTemplate(id, messageTemplate);
		  delegateLogger.warn(msg, parameters);
		}
	  }

	  /// <summary>
	  /// Logs an 'ERROR' message
	  /// </summary>
	  /// <param name="id"> the unique id of this log message </param>
	  /// <param name="messageTemplate"> the message template to use </param>
	  /// <param name="parameters"> a list of optional parameters </param>
	  protected internal virtual void logError(string id, string messageTemplate, params object[] parameters)
	  {
		if (delegateLogger.ErrorEnabled)
		{
		  string msg = formatMessageTemplate(id, messageTemplate);
		  delegateLogger.error(msg, parameters);
		}
	  }

	  /// <returns> true if the logger will log 'DEBUG' messages </returns>
	  public virtual bool DebugEnabled
	  {
		  get
		  {
			return delegateLogger.DebugEnabled;
		  }
	  }

	  /// <returns> true if the logger will log 'INFO' messages </returns>
	  public virtual bool InfoEnabled
	  {
		  get
		  {
			return delegateLogger.InfoEnabled;
		  }
	  }

	  /// <returns> true if the logger will log 'WARN' messages </returns>
	  public virtual bool WarnEnabled
	  {
		  get
		  {
			return delegateLogger.WarnEnabled;
		  }
	  }

	  /// <returns> true if the logger will log 'ERROR' messages </returns>
	  public virtual bool ErrorEnabled
	  {
		  get
		  {
			return delegateLogger.ErrorEnabled;
		  }
	  }

	  /// <summary>
	  /// Formats a message template
	  /// </summary>
	  /// <param name="id"> the id of the message </param>
	  /// <param name="messageTemplate"> the message template to use
	  /// </param>
	  /// <returns> the formatted template </returns>
	  protected internal virtual string formatMessageTemplate(string id, string messageTemplate)
	  {
		return projectCode + "-" + componentId + id + " " + messageTemplate;
	  }

	  /// <summary>
	  /// Prepares an exception message
	  /// </summary>
	  /// <param name="id"> the id of the message </param>
	  /// <param name="messageTemplate"> the message template to use </param>
	  /// <param name="parameters"> the parameters for the message (optional)
	  /// </param>
	  /// <returns> the prepared exception message </returns>
	  protected internal virtual string exceptionMessage(string id, string messageTemplate, params object[] parameters)
	  {
		string formattedTemplate = formatMessageTemplate(id, messageTemplate);
		if (parameters == null || parameters.Length == 0)
		{
		  return formattedTemplate;

		}
		else
		{
		  return MessageFormatter.arrayFormat(formattedTemplate, parameters).Message;

		}
	  }

	}

}