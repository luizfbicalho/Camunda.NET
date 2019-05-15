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
namespace org.camunda.bpm.engine.test.standalone.scripting
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptFactory = org.camunda.bpm.engine.impl.scripting.ScriptFactory;
	using SourceExecutableScript = org.camunda.bpm.engine.impl.scripting.SourceExecutableScript;
	using ScriptingEnvironment = org.camunda.bpm.engine.impl.scripting.env.ScriptingEnvironment;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class ScriptCompilationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string SCRIPT_LANGUAGE = "groovy";
	  protected internal const string EXAMPLE_SCRIPT = "println 'hello world'";

	  protected internal ScriptFactory scriptFactory;

	  public virtual void setUp()
	  {
		scriptFactory = processEngineConfiguration.ScriptFactory;
	  }

	  protected internal virtual SourceExecutableScript createScript(string language, string source)
	  {
		return (SourceExecutableScript) scriptFactory.createScriptFromSource(language, source);
	  }

	  public virtual void testScriptShouldBeCompiledByDefault()
	  {
		// when a script is created
		SourceExecutableScript script = createScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
		assertNotNull(script);

		// then it should not be compiled on creation
		assertTrue(script.ShouldBeCompiled);
		assertNull(script.CompiledScript);

		// but after first execution
		executeScript(script);

		// it was compiled
		assertFalse(script.ShouldBeCompiled);
		assertNotNull(script.CompiledScript);
	  }

	  public virtual void testDisableScriptCompilation()
	  {
		// when script compilation is disabled and a script is created
		processEngineConfiguration.EnableScriptCompilation = false;
		SourceExecutableScript script = createScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
		assertNotNull(script);

		// then it should not be compiled on creation
		assertTrue(script.ShouldBeCompiled);
		assertNull(script.CompiledScript);

		// and after first execution
		executeScript(script);

		// it was also not compiled
		assertFalse(script.ShouldBeCompiled);
		assertNull(script.CompiledScript);

		// re-enable script compilation
		processEngineConfiguration.EnableScriptCompilation = true;
	  }

	  public virtual void testDisableScriptCompilationByDisabledScriptEngineCaching()
	  {
		// when script engine caching is disabled and a script is created
		processEngineConfiguration.EnableScriptEngineCaching = false;
		SourceExecutableScript script = createScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
		assertNotNull(script);

		// then it should not be compiled on creation
		assertTrue(script.ShouldBeCompiled);
		assertNull(script.CompiledScript);

		// and after first execution
		executeScript(script);

		// it was also not compiled
		assertFalse(script.ShouldBeCompiled);
		assertNull(script.CompiledScript);

		// re-enable script engine caching
		processEngineConfiguration.EnableScriptEngineCaching = true;
	  }

	  public virtual void testOverrideScriptSource()
	  {
		// when a script is created and executed
		SourceExecutableScript script = createScript(SCRIPT_LANGUAGE, EXAMPLE_SCRIPT);
		assertNotNull(script);
		executeScript(script);

		// it was compiled
		assertFalse(script.ShouldBeCompiled);
		assertNotNull(script.CompiledScript);

		// if the script source changes
		script.ScriptSource = EXAMPLE_SCRIPT;

		// then it should not be compiled after change
		assertTrue(script.ShouldBeCompiled);
		assertNull(script.CompiledScript);

		// but after next execution
		executeScript(script);

		// it is compiled again
		assertFalse(script.ShouldBeCompiled);
		assertNotNull(script.CompiledScript);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected Object executeScript(final org.camunda.bpm.engine.impl.scripting.ExecutableScript script)
	  protected internal virtual object executeScript(ExecutableScript script)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.scripting.env.ScriptingEnvironment scriptingEnvironment = processEngineConfiguration.getScriptingEnvironment();
		ScriptingEnvironment scriptingEnvironment = processEngineConfiguration.ScriptingEnvironment;
		return processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, script, scriptingEnvironment));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly ScriptCompilationTest outerInstance;

		  private ExecutableScript script;
		  private ScriptingEnvironment scriptingEnvironment;

		  public CommandAnonymousInnerClass(ScriptCompilationTest outerInstance, ExecutableScript script, ScriptingEnvironment scriptingEnvironment)
		  {
			  this.outerInstance = outerInstance;
			  this.script = script;
			  this.scriptingEnvironment = scriptingEnvironment;
		  }

		  public object execute(CommandContext commandContext)
		  {
			return scriptingEnvironment.execute(script, null);
		  }
	  }

	}

}