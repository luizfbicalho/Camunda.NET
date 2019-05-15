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
namespace org.camunda.bpm.integrationtest.deployment.war
{

	using ProcessApplicationDeploymentInfo = org.camunda.bpm.application.ProcessApplicationDeploymentInfo;
	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestWarDeploymentResumePreviousOff extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestWarDeploymentResumePreviousOff : AbstractFoxPlatformIntegrationTest
	{

	  private const string PA1 = "PA1";
	  private const string PA2 = "PA2";

	  [Deployment(order:1, name:PA1)]
	  public static WebArchive processArchive1()
	  {
		return initWebArchiveDeployment("pa1.war").addAsResource("org/camunda/bpm/integrationtest/deployment/war/testDeployProcessArchiveV1.bpmn20.xml");
	  }

	  [Deployment(order:2, name:PA2)]
	  public static WebArchive processArchive2()
	  {
		return initWebArchiveDeployment("pa2.war", "org/camunda/bpm/integrationtest/deployment/war/resumePreviousVersionsOff_processes.xml").addAsResource("org/camunda/bpm/integrationtest/deployment/war/testDeployProcessArchiveV2.bpmn20.xml");

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment(value=PA2) public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {
		Assert.assertNotNull(processEngine);
		RepositoryService repositoryService = processEngine.RepositoryService;
		long count = repositoryService.createProcessDefinitionQuery().processDefinitionKey("testDeployProcessArchive").count();

		Assert.assertEquals(2, count);

		// validate registrations:
		ProcessApplicationService processApplicationService = BpmPlatform.ProcessApplicationService;
		ISet<string> processApplicationNames = processApplicationService.ProcessApplicationNames;
		foreach (string paName in processApplicationNames)
		{
		  ProcessApplicationInfo processApplicationInfo = processApplicationService.getProcessApplicationInfo(paName);
		  IList<ProcessApplicationDeploymentInfo> deploymentInfo = processApplicationInfo.DeploymentInfo;
		  if (deploymentInfo.Count == 2)
		  {
			Assert.fail("Previous version of the deployment must not be resumed");
		  }
		}

	  }

	}

}