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
namespace org.camunda.bpm.integrationtest.service
{

	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class ProcessApplicationServiceTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class ProcessApplicationServiceTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"test1")]
		public static WebArchive app1()
		{
		return initWebArchiveDeployment("test1.war").addAsResource("org/camunda/bpm/integrationtest/testDeployProcessArchive.bpmn20.xml");
		}

	  [Deployment(name:"test2")]
	  public static WebArchive app2()
	  {
		return initWebArchiveDeployment("test2.war").addAsResource("org/camunda/bpm/integrationtest/testDeployProcessArchiveWithoutActivitiCdi.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("test1") public void testProcessApplicationsDeployed()
	  public virtual void testProcessApplicationsDeployed()
	  {

		ProcessApplicationService processApplicationService = BpmPlatform.ProcessApplicationService;

		ISet<string> processApplicationNames = processApplicationService.ProcessApplicationNames;

		// check if the new applications are deployed with allowed names
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'retainAll' method:
		processApplicationNames.retainAll(Arrays.asList(new string [] {"test1", "test2", "/test1", "/test2"}));

		Assert.assertEquals(2, processApplicationNames.Count);

		foreach (string appName in processApplicationNames)
		{
		  ProcessApplicationInfo processApplicationInfo = processApplicationService.getProcessApplicationInfo(appName);

		  Assert.assertNotNull(processApplicationInfo);
		  Assert.assertNotNull(processApplicationInfo.Name);
		  Assert.assertEquals(1, processApplicationInfo.DeploymentInfo.Count);
		}

	  }


	}

}