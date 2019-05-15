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
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using CaseVariableBean = org.camunda.bpm.integrationtest.functional.cdi.beans.CaseVariableBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CdiBeanCaseTaskResolutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CdiBeanCaseTaskResolutionTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"pa1")]
		public static WebArchive createCallingProcessDeployment()
		{
		return initWebArchiveDeployment("pa1.war").addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiBeanCallActivityResolutionTest.callingCase.cmmn");
		}

	  [Deployment(name:"pa2")]
	  public static WebArchive createCalledProcessDeployment()
	  {
		return initWebArchiveDeployment("pa2.war").addClass(typeof(CaseVariableBean)).addAsResource("org/camunda/bpm/integrationtest/functional/cdi/CdiBeanCallActivityResolutionTest.calledCase.cmmn");
	  }

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive deployment = ShrinkWrap.create(typeof(WebArchive), "client.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsLibraries(DeploymentHelper.EngineCdi);

		TestContainer.addContainerSpecificResourcesForNonPa(deployment);

		return deployment;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveBeanInCmmnCase()
	  public virtual void testResolveBeanInCmmnCase()
	  {
		CaseInstance caseInstance = caseService.withCaseDefinitionByKey("callingCase").create();

		CaseExecution caseTaskInstance = caseService.createCaseExecutionQuery().activityId("PI_CaseTask_1").singleResult();

		CaseExecution calledCaseHumanTaskInstance = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult();

		Task calledCaseTask = taskService.createTaskQuery().singleResult();

		taskService.complete(calledCaseTask.Id, Variables.createVariables().putValue("var", "value"));

		CaseInstance calledInstance = caseService.createCaseInstanceQuery().caseDefinitionKey("calledCase").singleResult();

		caseService.withCaseExecution(calledInstance.Id).close();

		// then
		string variable = (string) caseService.getVariable(caseInstance.Id, "var");
		Assert.assertEquals("valuevalue", variable);
	  }
	}

}