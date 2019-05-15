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
namespace org.camunda.bpm.integrationtest.functional.cdi
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ConditionalFlowBean = org.camunda.bpm.integrationtest.functional.cdi.beans.ConditionalFlowBean;
	using ProcessVariableBean = org.camunda.bpm.integrationtest.functional.cdi.beans.ProcessVariableBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CdiBeanCallActivityResolutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CdiBeanCallActivityResolutionTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"pa1")]
		public static WebArchive createCallingProcessDeployment()
		{
		return initWebArchiveDeployment("pa1.war").addClass(typeof(ConditionalFlowBean)).addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiBeanCallActivityResolutionTest.callingProcess.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiBeanCallActivityResolutionTest.callingProcessConditionalFlow.bpmn20.xml");

		}

	  [Deployment(name:"pa2")]
	  public static WebArchive createCalledProcessDeployment()
	  {
		return initWebArchiveDeployment("pa2.war").addClass(typeof(ProcessVariableBean)).addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiBeanCallActivityResolutionTest.calledProcess.bpmn20.xml");
	  }

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsLibraries(DeploymentHelper.EngineCdi);

		TestContainer.addContainerSpecificResourcesForNonPa(deployment);

		return deployment;
	  }

	  protected internal ProcessInstance processInstance;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (processInstance != null)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, null);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveBeanInBpmnProcess()
	  public virtual void testResolveBeanInBpmnProcess()
	  {
		processInstance = runtimeService.startProcessInstanceByKey("callingProcess");

		Task calledProcessTask = taskService.createTaskQuery().singleResult();

		taskService.complete(calledProcessTask.Id, Variables.createVariables().putValue("var", "value"));

		Task afterCallActivityTask = taskService.createTaskQuery().singleResult();
		Assert.assertNotNull(afterCallActivityTask);
		Assert.assertEquals("afterCallActivity", afterCallActivityTask.TaskDefinitionKey);

		string variable = (string) runtimeService.getVariable(processInstance.Id, "var");
		Assert.assertEquals("valuevalue", variable);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveBeanInBpmnProcessConditionalFlow()
	  public virtual void testResolveBeanInBpmnProcessConditionalFlow()
	  {
		// given
		processInstance = runtimeService.startProcessInstanceByKey("callingProcessConditionalFlow", Variables.createVariables().putValue("takeFlow", true));

		Task calledProcessTask = taskService.createTaskQuery().singleResult();

		// when
		taskService.complete(calledProcessTask.Id);

		// then
		Task afterCallActivityTask = taskService.createTaskQuery().singleResult();
		Assert.assertNotNull(afterCallActivityTask);
		Assert.assertEquals("afterCallActivityTask", afterCallActivityTask.TaskDefinitionKey);
	  }

	}

}