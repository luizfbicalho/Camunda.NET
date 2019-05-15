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
	using Task = org.camunda.bpm.engine.task.Task;
	using AbstractScriptEngineFactory = org.camunda.bpm.integrationtest.functional.scriptengine.engine.AbstractScriptEngineFactory;
	using AlwaysTrueScriptEngineFactory = org.camunda.bpm.integrationtest.functional.scriptengine.engine.AlwaysTrueScriptEngineFactory;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaLocalScriptEngineCallActivityConditionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaLocalScriptEngineCallActivityConditionTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"pa1")]
		public static WebArchive createCallingProcessDeployment()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return initWebArchiveDeployment("pa1.war").addClass(typeof(AbstractScriptEngineFactory)).addClass(typeof(AlwaysTrueScriptEngineFactory)).addAsResource(new StringAsset(typeof(AlwaysTrueScriptEngineFactory).FullName), PaLocalScriptEngineSupportTest.SCRIPT_ENGINE_FACTORY_PATH).addAsResource("org/camunda/bpm/integrationtest/functional/scriptengine/PaLocalScriptEngineCallActivityConditionTest.callingProcessScriptConditionalFlow.bpmn20.xml");
		}

	  [Deployment(name:"pa2")]
	  public static WebArchive createCalledProcessDeployment()
	  {
		return initWebArchiveDeployment("pa2.war").addAsResource("org/camunda/bpm/integrationtest/functional/scriptengine/PaLocalScriptEngineCallActivityConditionTest.calledProcess.bpmn20.xml");
	  }

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest));

		TestContainer.addContainerSpecificResourcesForNonPa(deployment);

		return deployment;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void shouldEvaluateCondition()
	  public virtual void shouldEvaluateCondition()
	  {
		// given
		runtimeService.startProcessInstanceByKey("callingProcessScriptConditionalFlow").Id;

		Task calledProcessTask = taskService.createTaskQuery().singleResult();

		// when the called process instance returns
		taskService.complete(calledProcessTask.Id);

		// then the conditional flow leaving the call activity has been taken
		Task afterCallActivityTask = taskService.createTaskQuery().singleResult();
		Assert.assertNotNull(afterCallActivityTask);
		Assert.assertEquals("afterCallActivityTask", afterCallActivityTask.TaskDefinitionKey);
	  }

	}

}