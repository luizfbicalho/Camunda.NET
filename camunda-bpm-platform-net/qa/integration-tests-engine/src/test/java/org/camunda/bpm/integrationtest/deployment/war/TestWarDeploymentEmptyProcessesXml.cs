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
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	/// <summary>
	/// Assert that we can deploy a WAR which bundles
	/// the client and an empty processes.xml file
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestWarDeploymentEmptyProcessesXml extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestWarDeploymentEmptyProcessesXml : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment("test.war", "META-INF/empty_processes.xml").addAsResource("org/camunda/bpm/integrationtest/testDeployProcessArchive.bpmn20.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployProcessArchive()
	  public virtual void testDeployProcessArchive()
	  {
		Assert.assertNotNull(processEngine);
		RepositoryService repositoryService = processEngine.RepositoryService;

		IList<ProcessDefinition> processDefinitions = repositoryService.createProcessDefinitionQuery().processDefinitionKey("testDeployProcessArchive").list();

		Assert.assertEquals(1, processDefinitions.Count);
		org.camunda.bpm.engine.repository.Deployment deployment = repositoryService.createDeploymentQuery().deploymentId(processDefinitions[0].DeploymentId).singleResult();

		ISet<string> registeredProcessApplications = BpmPlatform.ProcessApplicationService.ProcessApplicationNames;

		bool containsProcessApplication = false;

		// the process application name is used as name for the db deployment
		foreach (string appName in registeredProcessApplications)
		{
		  if (appName.Equals(deployment.Name))
		  {
			containsProcessApplication = true;
		  }
		}
		assertTrue(containsProcessApplication);


		// manually delete process definition here (to clean up)
		repositoryService.deleteDeployment(deployment.Id, true);
	  }

	}
}