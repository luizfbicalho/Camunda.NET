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
namespace org.camunda.bpm.integrationtest.functional.el
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using DmnDecisionTableResult = org.camunda.bpm.dmn.engine.DmnDecisionTableResult;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using GreeterBean = org.camunda.bpm.integrationtest.functional.el.beans.GreeterBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class DecisionContextSwitchTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class DecisionContextSwitchTest : AbstractFoxPlatformIntegrationTest
	{

	  protected internal const string DMN_RESOURCE_NAME = "org/camunda/bpm/integrationtest/functional/el/BeanResolvingDecision.dmn11.xml";

	  [Deployment(name:"bpmnDeployment")]
	  public static WebArchive createBpmnDeployment()
	  {
		return initWebArchiveDeployment("bpmn-deployment.war").addAsResource("org/camunda/bpm/integrationtest/functional/el/BusinessRuleProcess.bpmn20.xml");
	  }

	  [Deployment(name:"dmnDeployment")]
	  public static WebArchive createDmnDeployment()
	  {
		return initWebArchiveDeployment("dmn-deployment.war").addClass(typeof(GreeterBean)).addAsResource(DMN_RESOURCE_NAME);
	  }


	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "client.war").addClass(typeof(AbstractFoxPlatformIntegrationTest));

		TestContainer.addContainerSpecificResources(webArchive);

		return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void shouldSwitchContextWhenUsingDecisionService()
	  public virtual void shouldSwitchContextWhenUsingDecisionService()
	  {
		DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey("decision", Variables.createVariables());
		assertEquals("ok", decisionResult.FirstResult.FirstEntry);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @SuppressWarnings("unchecked") @OperateOnDeployment("clientDeployment") public void shouldSwitchContextWhenCallingFromBpmn()
	  public virtual void shouldSwitchContextWhenCallingFromBpmn()
	  {
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		VariableInstance decisionResult = runtimeService.createVariableInstanceQuery().processInstanceIdIn(pi.Id).variableName("result").singleResult();
		IList<IDictionary<string, object>> result = (IList<IDictionary<string, object>>) decisionResult.Value;
		assertEquals("ok", result[0]["result"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void shouldSwitchContextWhenUsingDecisionServiceAfterRedeployment()
	  public virtual void shouldSwitchContextWhenUsingDecisionServiceAfterRedeployment()
	  {

		// given
		IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();

		// find dmn deployment
		org.camunda.bpm.engine.repository.Deployment dmnDeployment = null;
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in deployments)
		{
		  IList<string> resourceNames = repositoryService.getDeploymentResourceNames(deployment.Id);
		  if (resourceNames.Contains(DMN_RESOURCE_NAME))
		  {
			dmnDeployment = deployment;
		  }
		}

		if (dmnDeployment == null)
		{
		  Assert.fail("Expected to find DMN deployment");
		}

		org.camunda.bpm.engine.repository.Deployment deployment2 = repositoryService.createDeployment().nameFromDeployment(dmnDeployment.Id).addDeploymentResources(dmnDeployment.Id).deploy();

		try
		{
		  // when then
		  DmnDecisionTableResult decisionResult = decisionService.evaluateDecisionTableByKey("decision", Variables.createVariables());
		  assertEquals("ok", decisionResult.FirstResult.FirstEntry);
		}
		finally
		{
		  repositoryService.deleteDeployment(deployment2.Id, true);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @SuppressWarnings("unchecked") @OperateOnDeployment("clientDeployment") public void shouldSwitchContextWhenCallingFromBpmnAfterRedeployment()
	  public virtual void shouldSwitchContextWhenCallingFromBpmnAfterRedeployment()
	  {
		// given
		IList<org.camunda.bpm.engine.repository.Deployment> deployments = repositoryService.createDeploymentQuery().list();

		// find dmn deployment
		org.camunda.bpm.engine.repository.Deployment dmnDeployment = null;
		foreach (org.camunda.bpm.engine.repository.Deployment deployment in deployments)
		{
		  IList<string> resourceNames = repositoryService.getDeploymentResourceNames(deployment.Id);
		  if (resourceNames.Contains(DMN_RESOURCE_NAME))
		  {
			dmnDeployment = deployment;
		  }
		}

		if (dmnDeployment == null)
		{
		  Assert.fail("Expected to find DMN deployment");
		}

		org.camunda.bpm.engine.repository.Deployment deployment2 = repositoryService.createDeployment().nameFromDeployment(dmnDeployment.Id).addDeploymentResources(dmnDeployment.Id).deploy();

		try
		{
		  // when then
		  ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess");

		  VariableInstance decisionResult = runtimeService.createVariableInstanceQuery().processInstanceIdIn(pi.Id).variableName("result").singleResult();
		  IList<IDictionary<string, object>> result = (IList<IDictionary<string, object>>) decisionResult.Value;
		  assertEquals("ok", result[0]["result"]);
		}
		finally
		{
		  repositoryService.deleteDeployment(deployment2.Id, true);
		}
	  }

	}

}