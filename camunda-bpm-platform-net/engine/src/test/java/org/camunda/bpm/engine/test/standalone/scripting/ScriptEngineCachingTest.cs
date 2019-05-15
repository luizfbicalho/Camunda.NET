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
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessApplicationDeployment = org.camunda.bpm.engine.repository.ProcessApplicationDeployment;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ScriptEngineCachingTest : PluggableProcessEngineTestCase
	{

	  protected internal const string PROCESS_PATH = "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml";
	  protected internal const string SCRIPT_LANGUAGE = "groovy";

	  public virtual void testGlobalCachingOfScriptEngine()
	  {
		// when
		ScriptEngine engine = getScriptEngine(SCRIPT_LANGUAGE);

		// then
		assertNotNull(engine);
		assertEquals(engine, getScriptEngine(SCRIPT_LANGUAGE));
	  }

	  public virtual void testGlobalDisableCachingOfScriptEngine()
	  {
		// then
		processEngineConfiguration.EnableScriptEngineCaching = false;
		ScriptingEngines.EnableScriptEngineCaching = false;

		// when
		ScriptEngine engine = getScriptEngine(SCRIPT_LANGUAGE);

		// then
		assertNotNull(engine);
		assertFalse(engine.Equals(getScriptEngine(SCRIPT_LANGUAGE)));

		processEngineConfiguration.EnableScriptEngineCaching = true;
		ScriptingEngines.EnableScriptEngineCaching = true;
	  }

	  public virtual void testCachingOfScriptEngineInProcessApplication()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		// when
		ScriptEngine engine = processApplication.getScriptEngineForName(SCRIPT_LANGUAGE, true);

		// then
		assertNotNull(engine);
		assertEquals(engine, processApplication.getScriptEngineForName(SCRIPT_LANGUAGE, true));
	  }

	  public virtual void testDisableCachingOfScriptEngineInProcessApplication()
	  {
		// given
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		// when
		ScriptEngine engine = processApplication.getScriptEngineForName(SCRIPT_LANGUAGE, false);

		// then
		assertNotNull(engine);
		assertFalse(engine.Equals(processApplication.getScriptEngineForName(SCRIPT_LANGUAGE, false)));
	  }

	  public virtual void testFetchScriptEngineFromPaEnableCaching()
	  {
		// then
		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource(PROCESS_PATH).deploy();

		// when
		ScriptEngine engine = getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication);

		// then
		assertNotNull(engine);
		assertEquals(engine, getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication));

		// cached in pa
		assertEquals(engine, processApplication.getScriptEngineForName(SCRIPT_LANGUAGE, true));

		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	  public virtual void testFetchScriptEngineFromPaDisableCaching()
	  {
		// then
		processEngineConfiguration.EnableScriptEngineCaching = false;
		ScriptingEngines.EnableScriptEngineCaching = false;

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource(PROCESS_PATH).deploy();

		// when
		ScriptEngine engine = getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication);

		// then
		assertNotNull(engine);
		assertFalse(engine.Equals(getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication)));

		// not cached in pa
		assertFalse(engine.Equals(processApplication.getScriptEngineForName(SCRIPT_LANGUAGE, false)));

		repositoryService.deleteDeployment(deployment.Id, true);

		processEngineConfiguration.EnableScriptEngineCaching = true;
		ScriptingEngines.EnableScriptEngineCaching = true;
	  }

	  public virtual void testDisableFetchScriptEngineFromProcessApplication()
	  {
		// when
		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = false;

		EmbeddedProcessApplication processApplication = new EmbeddedProcessApplication();

		ProcessApplicationDeployment deployment = repositoryService.createDeployment(processApplication.Reference).addClasspathResource(PROCESS_PATH).deploy();

		// when
		ScriptEngine engine = getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication);

		// then
		assertNotNull(engine);
		assertEquals(engine, getScriptEngineFromPa(SCRIPT_LANGUAGE, processApplication));

		// not cached in pa
		assertFalse(engine.Equals(processApplication.getScriptEngineForName(SCRIPT_LANGUAGE, true)));

		repositoryService.deleteDeployment(deployment.Id, true);

		processEngineConfiguration.EnableFetchScriptEngineFromProcessApplication = true;
	  }

	  protected internal virtual ScriptingEngines ScriptingEngines
	  {
		  get
		  {
			return processEngineConfiguration.ScriptingEngines;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected javax.script.ScriptEngine getScriptEngine(final String name)
	  protected internal virtual ScriptEngine getScriptEngine(string name)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines scriptingEngines = getScriptingEngines();
		ScriptingEngines scriptingEngines = ScriptingEngines;
		return processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, name, scriptingEngines));
	  }

	  private class CommandAnonymousInnerClass : Command<ScriptEngine>
	  {
		  private readonly ScriptEngineCachingTest outerInstance;

		  private string name;
		  private ScriptingEngines scriptingEngines;

		  public CommandAnonymousInnerClass(ScriptEngineCachingTest outerInstance, string name, ScriptingEngines scriptingEngines)
		  {
			  this.outerInstance = outerInstance;
			  this.name = name;
			  this.scriptingEngines = scriptingEngines;
		  }

		  public ScriptEngine execute(CommandContext commandContext)
		  {
			return scriptingEngines.getScriptEngineForLanguage(name);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected javax.script.ScriptEngine getScriptEngineFromPa(final String name, final org.camunda.bpm.application.ProcessApplicationInterface processApplication)
	  protected internal virtual ScriptEngine getScriptEngineFromPa(string name, ProcessApplicationInterface processApplication)
	  {
		return processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this, name, processApplication));
	  }

	  private class CommandAnonymousInnerClass2 : Command<ScriptEngine>
	  {
		  private readonly ScriptEngineCachingTest outerInstance;

		  private string name;
		  private ProcessApplicationInterface processApplication;

		  public CommandAnonymousInnerClass2(ScriptEngineCachingTest outerInstance, string name, ProcessApplicationInterface processApplication)
		  {
			  this.outerInstance = outerInstance;
			  this.name = name;
			  this.processApplication = processApplication;
		  }

		  public ScriptEngine execute(CommandContext commandContext)
		  {
			return Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this)
		   , processApplication.Reference);
		  }

		  private class CallableAnonymousInnerClass : Callable<ScriptEngine>
		  {
			  private readonly CommandAnonymousInnerClass2 outerInstance;

			  public CallableAnonymousInnerClass(CommandAnonymousInnerClass2 outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.script.ScriptEngine call() throws Exception
			  public ScriptEngine call()
			  {
				return outerInstance.outerInstance.getScriptEngine(outerInstance.name);
			  }
		  }
	  }

	}

}