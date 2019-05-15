using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.scripting.env
{


	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;

	/// <summary>
	/// <para>The scripting environment contains scripts that provide an environment to
	/// a user provided script. The environment may contain additional libraries
	/// or imports.</para>
	/// 
	/// <para>The environment performs lazy initialization of scripts: the first time a script of a given
	/// script language is executed, the environment will use the <seealso cref="ScriptEnvResolver ScriptEnvResolvers"/>
	/// for resolving the environment scripts for that language. The scripts (if any) are then pre-compiled
	/// and cached for reuse.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ScriptingEnvironment
	{

	  /// <summary>
	  /// the cached environment scripts per script language </summary>
	  protected internal IDictionary<string, IList<ExecutableScript>> env = new Dictionary<string, IList<ExecutableScript>>();

	  /// <summary>
	  /// the resolvers </summary>
	  protected internal IList<ScriptEnvResolver> envResolvers;

	  /// <summary>
	  /// the script factory used for compiling env scripts </summary>
	  protected internal ScriptFactory scriptFactory;

	  /// <summary>
	  /// the scripting engines </summary>
	  protected internal ScriptingEngines scriptingEngines;

	  public ScriptingEnvironment(ScriptFactory scriptFactory, IList<ScriptEnvResolver> scriptEnvResolvers, ScriptingEngines scriptingEngines)
	  {
		this.scriptFactory = scriptFactory;
		this.envResolvers = scriptEnvResolvers;
		this.scriptingEngines = scriptingEngines;
	  }

	  /// <summary>
	  /// execute a given script in the environment
	  /// </summary>
	  /// <param name="script"> the <seealso cref="ExecutableScript"/> to execute </param>
	  /// <param name="scope"> the scope in which to execute the script </param>
	  /// <returns> the result of the script evaluation </returns>
	  public virtual object execute(ExecutableScript script, VariableScope scope)
	  {

		// get script engine
		ScriptEngine scriptEngine = scriptingEngines.getScriptEngineForLanguage(script.Language);

		// create bindings
		Bindings bindings = scriptingEngines.createBindings(scriptEngine, scope);

		return execute(script, scope, bindings, scriptEngine);
	  }

	  public virtual object execute(ExecutableScript script, VariableScope scope, Bindings bindings, ScriptEngine scriptEngine)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String scriptLanguage = script.getLanguage();
		string scriptLanguage = script.Language;

		// first, evaluate the env scripts (if any)
		IList<ExecutableScript> envScripts = getEnvScripts(scriptLanguage);
		foreach (ExecutableScript envScript in envScripts)
		{
		  envScript.execute(scriptEngine, scope, bindings);
		}

		// next evaluate the actual script
		return script.execute(scriptEngine, scope, bindings);
	  }

	  protected internal virtual IDictionary<string, IList<ExecutableScript>> getEnv(string language)
	  {
		ProcessEngineConfigurationImpl config = Context.ProcessEngineConfiguration;
		ProcessApplicationReference processApplication = Context.CurrentProcessApplication;

		IDictionary<string, IList<ExecutableScript>> result = null;
		if (config.EnableFetchScriptEngineFromProcessApplication)
		{
		  if (processApplication != null)
		  {
			result = getPaEnvScripts(processApplication);
		  }
		}

		return result != null ? result : env;
	  }

	  protected internal virtual IDictionary<string, IList<ExecutableScript>> getPaEnvScripts(ProcessApplicationReference pa)
	  {
		try
		{
		  ProcessApplicationInterface processApplication = pa.ProcessApplication;
		  ProcessApplicationInterface rawObject = processApplication.RawObject;

		  if (rawObject is AbstractProcessApplication)
		  {
			AbstractProcessApplication abstractProcessApplication = (AbstractProcessApplication) rawObject;
			return abstractProcessApplication.EnvironmentScripts;
		  }
		  return null;
		}
		catch (ProcessApplicationUnavailableException e)
		{
		  throw new ProcessEngineException("Process Application is unavailable.", e);
		}
	  }

	  /// <summary>
	  /// Returns the env scripts for the given language. Performs lazy initialization of the env scripts.
	  /// </summary>
	  /// <param name="scriptLanguage"> the language </param>
	  /// <returns> a list of executable environment scripts. Never null. </returns>
	  protected internal virtual IList<ExecutableScript> getEnvScripts(string scriptLanguage)
	  {
		IDictionary<string, IList<ExecutableScript>> environment = getEnv(scriptLanguage);
		IList<ExecutableScript> envScripts = environment[scriptLanguage];
		if (envScripts == null)
		{
		  lock (this)
		  {
			envScripts = environment[scriptLanguage];
			if (envScripts == null)
			{
			  envScripts = initEnvForLanguage(scriptLanguage);
			  environment[scriptLanguage] = envScripts;
			}
		  }
		}
		return envScripts;
	  }

	  /// <summary>
	  /// Initializes the env scripts for a given language.
	  /// </summary>
	  /// <param name="language"> the language </param>
	  /// <returns> the list of env scripts. Never null. </returns>
	  protected internal virtual IList<ExecutableScript> initEnvForLanguage(string language)
	  {

		IList<ExecutableScript> scripts = new List<ExecutableScript>();
		foreach (ScriptEnvResolver resolver in envResolvers)
		{
		  string[] resolvedScripts = resolver.resolve(language);
		  if (resolvedScripts != null)
		  {
			foreach (string resolvedScript in resolvedScripts)
			{
			  scripts.Add(scriptFactory.createScriptFromSource(language, resolvedScript));
			}
		  }
		}

		return scripts;
	  }

	}

}