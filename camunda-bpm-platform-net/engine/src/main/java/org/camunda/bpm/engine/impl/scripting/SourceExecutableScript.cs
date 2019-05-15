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
namespace org.camunda.bpm.engine.impl.scripting
{

	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;

	/// <summary>
	/// A script which is provided as source code.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SourceExecutableScript : CompiledExecutableScript
	{

	  private static readonly ScriptLogger LOG = ProcessEngineLogger.SCRIPT_LOGGER;

	  /// <summary>
	  /// The source of the script. </summary>
	  protected internal string scriptSource;

	  /// <summary>
	  /// Flag to signal if the script should be compiled </summary>
	  protected internal bool shouldBeCompiled = true;

	  public SourceExecutableScript(string language, string source) : base(language)
	  {
		scriptSource = source;
	  }

	  public override object evaluate(ScriptEngine engine, VariableScope variableScope, Bindings bindings)
	  {
		if (shouldBeCompiled)
		{
		  compileScript(engine);
		}

		if (CompiledScript != null)
		{
		  return base.evaluate(engine, variableScope, bindings);
		}
		else
		{
		  try
		  {
			return evaluateScript(engine, bindings);
		  }
		  catch (ScriptException e)
		  {
			if (e.InnerException is BpmnError)
			{
			  throw (BpmnError) e.InnerException;
			}
			string activityIdMessage = getActivityIdExceptionMessage(variableScope);
			throw new ScriptEvaluationException("Unable to evaluate script" + activityIdMessage + ":" + e.Message, e);
		  }
		}
	  }

	  protected internal virtual void compileScript(ScriptEngine engine)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		if (processEngineConfiguration.EnableScriptEngineCaching && processEngineConfiguration.EnableScriptCompilation)
		{

		  if (CompiledScript == null && shouldBeCompiled)
		  {
			lock (this)
			{
			  if (CompiledScript == null && shouldBeCompiled)
			  {
				// try to compile script
				compiledScript = compile(engine, language, scriptSource);

				// either the script was successfully compiled or it can't be
				// compiled but we won't try it again
				shouldBeCompiled = false;
			  }
			}
		  }

		}
		else
		{
		  // if script compilation is disabled abort
		  shouldBeCompiled = false;
		}
	  }

	  public virtual CompiledScript compile(ScriptEngine scriptEngine, string language, string src)
	  {
		if (scriptEngine is Compilable && !scriptEngine.Factory.LanguageName.equalsIgnoreCase("ecmascript"))
		{
		  Compilable compilingEngine = (Compilable) scriptEngine;

		  try
		  {
			CompiledScript compiledScript = compilingEngine.compile(src);

			LOG.debugCompiledScriptUsing(language);

			return compiledScript;

		  }
		  catch (ScriptException e)
		  {
			throw new ScriptCompilationException("Unable to compile script: " + e.Message, e);

		  }

		}
		else
		{
		  // engine does not support compilation
		  return null;
		}

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object evaluateScript(javax.script.ScriptEngine engine, javax.script.Bindings bindings) throws javax.script.ScriptException
	  protected internal virtual object evaluateScript(ScriptEngine engine, Bindings bindings)
	  {
		LOG.debugEvaluatingNonCompiledScript(scriptSource);
		return engine.eval(scriptSource, bindings);
	  }

	  public virtual string ScriptSource
	  {
		  get
		  {
			return scriptSource;
		  }
		  set
		  {
			this.compiledScript = null;
			shouldBeCompiled = true;
			this.scriptSource = value;
		  }
	  }


	  public virtual bool ShouldBeCompiled
	  {
		  get
		  {
			return shouldBeCompiled;
		  }
	  }

	}

}