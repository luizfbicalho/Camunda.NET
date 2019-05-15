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
namespace org.camunda.bpm.engine.test.standalone.scripting
{

	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using EmbeddedProcessApplication = org.camunda.bpm.application.impl.EmbeddedProcessApplication;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptFactory = org.camunda.bpm.engine.impl.scripting.ScriptFactory;
	using SourceExecutableScript = org.camunda.bpm.engine.impl.scripting.SourceExecutableScript;
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;
	using ScriptEnvResolver = org.camunda.bpm.engine.impl.scripting.env.ScriptEnvResolver;
	using ScriptingEnvironment = org.camunda.bpm.engine.impl.scripting.env.ScriptingEnvironment;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class EnvScriptCachingTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_PATH = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string SCRIPT_LANGUAGE = "groovy";
	  protected internal const string SCRIPT = "println 'hello world'";
	  protected internal const string ENV_SCRIPT = "println 'hello world from env script'";
	  protected internal static readonly ScriptEnvResolver RESOLVER;

	  static EnvScriptCachingTest()
	  {
		RESOLVER = new ScriptEnvResolverAnonymousInnerClass();
	  }

	  private class ScriptEnvResolverAnonymousInnerClass : ScriptEnvResolver
	  {
		  public string[] resolve(string language)
		  {
			return new string[] {ENV_SCRIPT};
		  }
	  }

	  protected internal ScriptFactory scriptFactory;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setUp() throws Exception
	  public virtual void setUp()
	  {
		base.setUp();
		scriptFactory = processEngineConfiguration.ScriptFactory;
		processEngineConfiguration.EnvScriptResolvers.Add(RESOLVER);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		base.tearDown();
		processEngineConfiguration.EnvScriptResolvers.Remove(RESOLVER);
	  }

	  public virtual void testEnabledPaEnvScriptCaching()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource(PROCESS_PATH).deploy();

		// when
		executeScript(processApplication);

		// then
		IDictionary<string, IList<ExecutableScript>> environmentScripts = processApplication.EnvironmentScripts;
		assertNotNull(environmentScripts);

		IList<ExecutableScript> groovyEnvScripts = environmentScripts[SCRIPT_LANGUAGE];

		assertNotNull(groovyEnvScripts);
		assertFalse(groovyEnvScripts.Count == 0);
		assertEquals(processEngineConfiguration.EnvScriptResolvers.Count, groovyEnvScripts.Count);

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testDisabledPaEnvScriptCaching()
	  {
		// given
		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = false;

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource(PROCESS_PATH).deploy();

		// when
		executeScript(processApplication);

		// then
		IDictionary<string, IList<ExecutableScript>> environmentScripts = processApplication.EnvironmentScripts;
		assertNotNull(environmentScripts);
		assertNull(environmentScripts[SCRIPT_LANGUAGE]);

		repositoryService.deleteDeployment(deployment.Id, true);

		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = true;
	  }

	  protected internal virtual SourceExecutableScript createScript(string language, string source)
	  {
		return (SourceExecutableScript) scriptFactory.createScriptFromSource(language, source);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void executeScript(final org.camunda.bpm.application.ProcessApplicationInterface processApplication)
	  protected internal virtual void executeScript(ProcessApplicationInterface processApplication)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, processApplication));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly EnvScriptCachingTest outerInstance;

		  private ProcessApplicationInterface processApplication;

		  public CommandAnonymousInnerClass(EnvScriptCachingTest outerInstance, ProcessApplicationInterface processApplication)
		  {
			  this.outerInstance = outerInstance;
			  this.processApplication = processApplication;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			return Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this)
		   , processApplication.Reference);
		  }

		  private class CallableAnonymousInnerClass : Callable<Void>
		  {
			  private readonly CommandAnonymousInnerClass outerInstance;

			  public CallableAnonymousInnerClass(CommandAnonymousInnerClass outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
			  public Void call()
			  {
				ScriptingEngines scriptingEngines = outerInstance.outerInstance.processEngineConfiguration.ScriptingEngines;
				ScriptEngine scriptEngine = scriptingEngines.getScriptEngineForLanguage(SCRIPT_LANGUAGE);

				SourceExecutableScript script = outerInstance.outerInstance.createScript(SCRIPT_LANGUAGE, SCRIPT);

				ScriptingEnvironment scriptingEnvironment = outerInstance.outerInstance.processEngineConfiguration.ScriptingEnvironment;
				scriptingEnvironment.execute(script, null, null, scriptEngine);

				return null;
			  }
		  }
	  }

	}

}