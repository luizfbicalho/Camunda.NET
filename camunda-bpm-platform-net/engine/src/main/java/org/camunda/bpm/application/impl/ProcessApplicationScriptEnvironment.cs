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
namespace org.camunda.bpm.application.impl
{


	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptEngineResolver = org.camunda.bpm.engine.impl.scripting.engine.ScriptEngineResolver;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessApplicationScriptEnvironment
	{

	  protected internal ProcessApplicationInterface processApplication;

	  protected internal ScriptEngineResolver processApplicationScriptEngineResolver;
	  protected internal IDictionary<string, IList<ExecutableScript>> environmentScripts = new Dictionary<string, IList<ExecutableScript>>();

	  public ProcessApplicationScriptEnvironment(ProcessApplicationInterface processApplication)
	  {
		this.processApplication = processApplication;
	  }

	  /// <summary>
	  /// <para>Returns an instance of <seealso cref="ScriptEngine"/> for the given <code>scriptEngineName</code>.</para>
	  /// 
	  /// <para>Iff the given parameter <code>cache</code> is set <code>true</code>,
	  /// then the instance <seealso cref="ScriptEngine"/> will be cached.</para>
	  /// </summary>
	  /// <param name="scriptEngineName"> the name of the <seealso cref="ScriptEngine"/> to return </param>
	  /// <param name="cache"> a boolean value which indicates whether the <seealso cref="ScriptEngine"/> should
	  ///              be cached or not.
	  /// </param>
	  /// <returns> a <seealso cref="ScriptEngine"/> </returns>
	  public virtual ScriptEngine getScriptEngineForName(string scriptEngineName, bool cache)
	  {
		if (processApplicationScriptEngineResolver == null)
		{
		  lock (this)
		  {
			if (processApplicationScriptEngineResolver == null)
			{
			  processApplicationScriptEngineResolver = new ScriptEngineResolver(new ScriptEngineManager(ProcessApplicationClassloader));
			}
		  }
		}
		return processApplicationScriptEngineResolver.getScriptEngine(scriptEngineName, cache);
	  }

	  /// <summary>
	  /// Returns a map of cached environment scripts per script language.
	  /// </summary>
	  public virtual IDictionary<string, IList<ExecutableScript>> EnvironmentScripts
	  {
		  get
		  {
			return environmentScripts;
		  }
	  }

	  protected internal virtual ClassLoader ProcessApplicationClassloader
	  {
		  get
		  {
			return processApplication.ProcessApplicationClassloader;
		  }
	  }

	}

}