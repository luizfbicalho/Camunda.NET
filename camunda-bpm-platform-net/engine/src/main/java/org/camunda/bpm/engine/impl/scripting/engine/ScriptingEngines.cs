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
namespace org.camunda.bpm.engine.impl.scripting.engine
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using DmnScriptEngineResolver = org.camunda.bpm.dmn.engine.impl.spi.el.DmnScriptEngineResolver;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;

	/// <summary>
	/// <para>Manager for JSR-223 <seealso cref="ScriptEngine"/> handling.</para>
	/// 
	/// <para><strong>Resolving a script engine:</strong>
	/// This class supports resolving a script engine for a given 'language name' (eg. 'groovy').
	/// If the configuration option <seealso cref="#enableScriptEngineCaching"/> is set to true,
	/// the class will attempt to cache 'cachable' script engines. We assume a <seealso cref="ScriptEngine"/> is
	/// 'cachable' if it declares to be threadsafe (see <seealso cref="#isCachable(ScriptEngine)"/>)</para>
	/// 
	/// <para><strong>Custom Bindings:</strong> this class supports custom <seealso cref="Bindings"/>
	/// implementations through the <seealso cref="#scriptBindingsFactory"/>. See <seealso cref="ScriptBindingsFactory"/>.</para>
	/// </p>
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class ScriptingEngines : DmnScriptEngineResolver
	{

	  public const string DEFAULT_SCRIPTING_LANGUAGE = "juel";
	  public const string GROOVY_SCRIPTING_LANGUAGE = "groovy";

	  protected internal ScriptEngineResolver scriptEngineResolver;
	  protected internal ScriptBindingsFactory scriptBindingsFactory;

	  protected internal bool enableScriptEngineCaching = true;

	  public ScriptingEngines(ScriptBindingsFactory scriptBindingsFactory) : this(new ScriptEngineManager())
	  {
		this.scriptBindingsFactory = scriptBindingsFactory;
	  }

	  public ScriptingEngines(ScriptEngineManager scriptEngineManager)
	  {
		this.scriptEngineResolver = new ScriptEngineResolver(scriptEngineManager);
	  }

	  public virtual bool EnableScriptEngineCaching
	  {
		  get
		  {
			return enableScriptEngineCaching;
		  }
		  set
		  {
			this.enableScriptEngineCaching = value;
		  }
	  }


	  public virtual ScriptEngineManager ScriptEngineManager
	  {
		  get
		  {
			return scriptEngineResolver.ScriptEngineManager;
		  }
	  }

	  public virtual ScriptingEngines addScriptEngineFactory(ScriptEngineFactory scriptEngineFactory)
	  {
		scriptEngineResolver.addScriptEngineFactory(scriptEngineFactory);
		return this;
	  }

	  /// <summary>
	  /// Loads the given script engine by language name. Will throw an exception if no script engine can be loaded for the given language name.
	  /// </summary>
	  /// <param name="language"> the name of the script language to lookup an implementation for </param>
	  /// <returns> the script engine </returns>
	  /// <exception cref="ProcessEngineException"> if no such engine can be found. </exception>
	  public virtual ScriptEngine getScriptEngineForLanguage(string language)
	  {

		if (!string.ReferenceEquals(language, null))
		{
		  language = language.ToLower();
		}

		ProcessApplicationReference pa = Context.CurrentProcessApplication;
		ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;

		ScriptEngine engine = null;
		if (config.EnableFetchScriptEngineFromProcessApplication)
		{
		  if (pa != null)
		  {
			engine = getPaScriptEngine(language, pa);
		  }
		}

		if (engine == null)
		{
		  engine = getGlobalScriptEngine(language);
		}

		return engine;
	  }

	  protected internal virtual ScriptEngine getPaScriptEngine(string language, ProcessApplicationReference pa)
	  {
		try
		{
		  ProcessApplicationInterface processApplication = pa.ProcessApplication;
		  ProcessApplicationInterface rawObject = processApplication.RawObject;

		  if (rawObject is AbstractProcessApplication)
		  {
			AbstractProcessApplication abstractProcessApplication = (AbstractProcessApplication) rawObject;
			return abstractProcessApplication.getScriptEngineForName(language, enableScriptEngineCaching);
		  }
		  return null;
		}
		catch (ProcessApplicationUnavailableException e)
		{
		  throw new ProcessEngineException("Process Application is unavailable.", e);
		}
	  }

	  protected internal virtual ScriptEngine getGlobalScriptEngine(string language)
	  {

		ScriptEngine scriptEngine = scriptEngineResolver.getScriptEngine(language, enableScriptEngineCaching);

		ensureNotNull("Can't find scripting engine for '" + language + "'", "scriptEngine", scriptEngine);

		return scriptEngine;
	  }

	  /// <summary>
	  /// override to build a spring aware ScriptingEngines </summary>
	  /// <param name="engineBindin"> </param>
	  /// <param name="scriptEngine">  </param>
	  public virtual Bindings createBindings(ScriptEngine scriptEngine, VariableScope variableScope)
	  {
		return scriptBindingsFactory.createBindings(variableScope, scriptEngine.createBindings());
	  }

	  public virtual ScriptBindingsFactory ScriptBindingsFactory
	  {
		  get
		  {
			return scriptBindingsFactory;
		  }
		  set
		  {
			this.scriptBindingsFactory = value;
		  }
	  }

	}

}