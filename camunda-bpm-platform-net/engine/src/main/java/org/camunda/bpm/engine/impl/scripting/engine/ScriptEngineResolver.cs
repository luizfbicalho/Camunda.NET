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
namespace org.camunda.bpm.engine.impl.scripting.engine
{


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ScriptEngineResolver
	{

	  protected internal readonly ScriptEngineManager scriptEngineManager;

	  protected internal IDictionary<string, ScriptEngine> cachedEngines = new Dictionary<string, ScriptEngine>();

	  public ScriptEngineResolver(ScriptEngineManager scriptEngineManager)
	  {
		this.scriptEngineManager = scriptEngineManager;
	  }

	  public virtual void addScriptEngineFactory(ScriptEngineFactory scriptEngineFactory)
	  {
		scriptEngineManager.registerEngineName(scriptEngineFactory.EngineName, scriptEngineFactory);
	  }

	  public virtual ScriptEngineManager ScriptEngineManager
	  {
		  get
		  {
			return scriptEngineManager;
		  }
	  }


	  /// <summary>
	  /// Returns a cached script engine or creates a new script engine if no such engine is currently cached.
	  /// </summary>
	  /// <param name="language"> the language (such as 'groovy' for the script engine) </param>
	  /// <returns> the cached engine or null if no script engine can be created for the given language </returns>
	  public virtual ScriptEngine getScriptEngine(string language, bool resolveFromCache)
	  {

		ScriptEngine scriptEngine = null;

		if (resolveFromCache)
		{
		  scriptEngine = cachedEngines[language];

		  if (scriptEngine == null)
		  {
			scriptEngine = scriptEngineManager.getEngineByName(language);

			if (scriptEngine != null)
			{

			  if (ScriptingEngines.GROOVY_SCRIPTING_LANGUAGE.Equals(language))
			  {
				configureGroovyScriptEngine(scriptEngine);
			  }

			  if (isCachable(scriptEngine))
			  {
				cachedEngines[language] = scriptEngine;
			  }

			}

		  }

		}
		else
		{
		  scriptEngine = scriptEngineManager.getEngineByName(language);
		}

		return scriptEngine;
	  }

	  /// <summary>
	  /// Allows checking whether the script engine can be cached.
	  /// </summary>
	  /// <param name="scriptEngine"> the script engine to check. </param>
	  /// <returns> true if the script engine may be cached. </returns>
	  protected internal virtual bool isCachable(ScriptEngine scriptEngine)
	  {
		// Check if script-engine supports multithreading. If true it can be cached.
		object threadingParameter = scriptEngine.Factory.getParameter("THREADING");
		return threadingParameter != null;
	  }

	  /// <summary>
	  /// Allows providing custom configuration for the groovy script engine. </summary>
	  /// <param name="scriptEngine"> the groovy script engine to configure. </param>
	  protected internal virtual void configureGroovyScriptEngine(ScriptEngine scriptEngine)
	  {

		// make sure Groovy compiled scripts only hold weak references to java methods
		scriptEngine.Context.setAttribute("#jsr223.groovy.engine.keep.globals", "weak", ScriptContext.ENGINE_SCOPE);
	  }


	}

}