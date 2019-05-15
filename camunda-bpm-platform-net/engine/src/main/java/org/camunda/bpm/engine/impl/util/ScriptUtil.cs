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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureAtLeastOneNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptFactory = org.camunda.bpm.engine.impl.scripting.ScriptFactory;
	using JuelScriptEngineFactory = org.camunda.bpm.engine.impl.scripting.engine.JuelScriptEngineFactory;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public sealed class ScriptUtil
	{

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a source or resource. It excepts static and
	  /// dynamic sources and resources. Dynamic means that the source or resource is an expression
	  /// which will be evaluated during execution.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="source"> the source code of the script or an expression which evaluates to the source code </param>
	  /// <param name="resource"> the resource path of the script code or an expression which evaluates to the resource path </param>
	  /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language is null or empty or both of source and resource are null or empty </exception>
	  public static ExecutableScript getScript(string language, string source, string resource, ExpressionManager expressionManager)
	  {
		return getScript(language, source, resource, expressionManager, ScriptFactory);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a source or resource. It excepts static and
	  /// dynamic sources and resources. Dynamic means that the source or resource is an expression
	  /// which will be evaluated during execution.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="source"> the source code of the script or an expression which evaluates to the source code </param>
	  /// <param name="resource"> the resource path of the script code or an expression which evaluates to the resource path </param>
	  /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
	  /// <param name="scriptFactory"> the script factory used to create the script </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language is null or empty or both of source and resource are invalid </exception>
	  public static ExecutableScript getScript(string language, string source, string resource, ExpressionManager expressionManager, ScriptFactory scriptFactory)
	  {
		ensureNotEmpty(typeof(NotValidException), "Script language", language);
		ensureAtLeastOneNotNull(typeof(NotValidException), "No script source or resource was given", source, resource);
		if (!string.ReferenceEquals(resource, null) && resource.Length > 0)
		{
		  return getScriptFromResource(language, resource, expressionManager, scriptFactory);
		}
		else
		{
		  return getScriptFormSource(language, source, expressionManager, scriptFactory);
		}
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a source. It excepts static and dynamic sources.
	  /// Dynamic means that the source is an expression which will be evaluated during execution.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="source"> the source code of the script or an expression which evaluates to the source code </param>
	  /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
	  /// <param name="scriptFactory"> the script factory used to create the script </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language is null or empty or source is null </exception>
	  public static ExecutableScript getScriptFormSource(string language, string source, ExpressionManager expressionManager, ScriptFactory scriptFactory)
	  {
		ensureNotEmpty(typeof(NotValidException), "Script language", language);
		ensureNotNull(typeof(NotValidException), "Script source", source);
		if (isDynamicScriptExpression(language, source))
		{
		  Expression sourceExpression = expressionManager.createExpression(source);
		  return getScriptFromSourceExpression(language, sourceExpression, scriptFactory);
		}
		else
		{
		  return getScriptFromSource(language, source, scriptFactory);
		}
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a static source.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="source"> the source code of the script </param>
	  /// <param name="scriptFactory"> the script factory used to create the script </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language is null or empty or source is null </exception>
	  public static ExecutableScript getScriptFromSource(string language, string source, ScriptFactory scriptFactory)
	  {
		ensureNotEmpty(typeof(NotValidException), "Script language", language);
		ensureNotNull(typeof(NotValidException), "Script source", source);
		return scriptFactory.createScriptFromSource(language, source);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a dynamic source. Dynamic means that the source
	  /// is an expression which will be evaluated during execution.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="sourceExpression"> the expression which evaluates to the source code </param>
	  /// <param name="scriptFactory"> the script factory used to create the script </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language is null or empty or sourceExpression is null </exception>
	  public static ExecutableScript getScriptFromSourceExpression(string language, Expression sourceExpression, ScriptFactory scriptFactory)
	  {
		ensureNotEmpty(typeof(NotValidException), "Script language", language);
		ensureNotNull(typeof(NotValidException), "Script source expression", sourceExpression);
		return scriptFactory.createScriptFromSource(language, sourceExpression);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a resource. It excepts static and dynamic resources.
	  /// Dynamic means that the resource is an expression which will be evaluated during execution.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="resource"> the resource path of the script code or an expression which evaluates to the resource path </param>
	  /// <param name="expressionManager"> the expression manager to use to generate the expressions of dynamic scripts </param>
	  /// <param name="scriptFactory"> the script factory used to create the script </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language or resource are null or empty </exception>
	  public static ExecutableScript getScriptFromResource(string language, string resource, ExpressionManager expressionManager, ScriptFactory scriptFactory)
	  {
		ensureNotEmpty(typeof(NotValidException), "Script language", language);
		ensureNotEmpty(typeof(NotValidException), "Script resource", resource);
		if (isDynamicScriptExpression(language, resource))
		{
		  Expression resourceExpression = expressionManager.createExpression(resource);
		  return getScriptFromResourceExpression(language, resourceExpression, scriptFactory);
		}
		else
		{
		  return getScriptFromResource(language, resource, scriptFactory);
		}
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a static resource.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="resource"> the resource path of the script code </param>
	  /// <param name="scriptFactory"> the script factory used to create the script </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language or resource are null or empty </exception>
	  public static ExecutableScript getScriptFromResource(string language, string resource, ScriptFactory scriptFactory)
	  {
		ensureNotEmpty(typeof(NotValidException), "Script language", language);
		ensureNotEmpty(typeof(NotValidException), "Script resource", resource);
		return scriptFactory.createScriptFromResource(language, resource);
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="ExecutableScript"/> from a dynamic resource. Dynamic means that the source
	  /// is an expression which will be evaluated during execution.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="resourceExpression"> the expression which evaluates to the resource path </param>
	  /// <param name="scriptFactory"> the script factory used to create the script </param>
	  /// <returns> the newly created script </returns>
	  /// <exception cref="NotValidException"> if language is null or empty or resourceExpression is null </exception>
	  public static ExecutableScript getScriptFromResourceExpression(string language, Expression resourceExpression, ScriptFactory scriptFactory)
	  {
		ensureNotEmpty(typeof(NotValidException), "Script language", language);
		ensureNotNull(typeof(NotValidException), "Script resource expression", resourceExpression);
		return scriptFactory.createScriptFromResource(language, resourceExpression);
	  }

	  /// <summary>
	  /// Checks if the value is an expression for a dynamic script source or resource.
	  /// </summary>
	  /// <param name="language"> the language of the script </param>
	  /// <param name="value"> the value to check </param>
	  /// <returns> true if the value is an expression for a dynamic script source/resource, otherwise false </returns>
	  public static bool isDynamicScriptExpression(string language, string value)
	  {
		return StringUtil.isExpression(value) && (!string.ReferenceEquals(language, null) && !JuelScriptEngineFactory.names.Contains(language.ToLower()));
	  }

	  /// <summary>
	  /// Returns the configured script factory in the context or a new one.
	  /// </summary>
	  public static ScriptFactory ScriptFactory
	  {
		  get
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
			if (processEngineConfiguration != null)
			{
			  return processEngineConfiguration.ScriptFactory;
			}
			else
			{
			  return new ScriptFactory();
			}
		  }
	  }
	}

}