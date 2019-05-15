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
namespace org.camunda.bpm.integrationtest.functional.scriptengine
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;

	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationUnavailableException = org.camunda.bpm.application.ProcessApplicationUnavailableException;
	using ProcessApplicationManager = org.camunda.bpm.engine.impl.application.ProcessApplicationManager;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public abstract class AbstractPaLocalScriptEngineTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public abstract class AbstractPaLocalScriptEngineTest : AbstractFoxPlatformIntegrationTest
	{

	  public const string PROCESS_ID = "testProcess";
	  public const string SCRIPT_TEXT = "my-script";
	  public const string SCRIPT_FORMAT = "dummy";
	  public const string DUMMY_SCRIPT_ENGINE_FACTORY_SPI = "org.camunda.bpm.integrationtest.functional.scriptengine.engine.DummyScriptEngineFactory";
	  public const string SCRIPT_ENGINE_FACTORY_PATH = "META-INF/services/javax.script.ScriptEngineFactory";

	  protected internal static StringAsset createScriptTaskProcess(string scriptFormat, string scriptText)
	  {
		BpmnModelInstance modelInstance = Bpmn.createExecutableProcess(PROCESS_ID).startEvent().scriptTask().scriptFormat(scriptFormat).scriptText(scriptText).camundaResultVariable("scriptValue").userTask().endEvent().done();
		return new StringAsset(Bpmn.convertToString(modelInstance));
	  }

	  protected internal virtual ProcessApplicationInterface ProcessApplication
	  {
		  get
		  {
			ProcessApplicationReference reference = processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
    
			assertNotNull(reference);
    
			ProcessApplicationInterface processApplication = null;
			try
			{
			  processApplication = reference.ProcessApplication;
			}
			catch (ProcessApplicationUnavailableException)
			{
			  fail("Could not retrieve process application");
			}
    
			return processApplication.RawObject;
		  }
	  }

	  private class CommandAnonymousInnerClass : Command<ProcessApplicationReference>
	  {
		  private readonly AbstractPaLocalScriptEngineTest outerInstance;

		  public CommandAnonymousInnerClass(AbstractPaLocalScriptEngineTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ProcessApplicationReference execute(CommandContext commandContext)
		  {
			ProcessDefinitionEntity definition = commandContext.ProcessDefinitionManager.findLatestProcessDefinitionByKey(PROCESS_ID);
			string deploymentId = definition.DeploymentId;
			ProcessApplicationManager processApplicationManager = outerInstance.processEngineConfiguration.ProcessApplicationManager;
			return processApplicationManager.getProcessApplicationForDeployment(deploymentId);
		  }
	  }

	}

}