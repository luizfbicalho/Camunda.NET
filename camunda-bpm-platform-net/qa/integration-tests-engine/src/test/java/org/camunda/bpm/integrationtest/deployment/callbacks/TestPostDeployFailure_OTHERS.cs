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
namespace org.camunda.bpm.integrationtest.deployment.callbacks
{
	using Assert = org.junit.Assert;

	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) @Ignore public class TestPostDeployFailure_OTHERS extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestPostDeployFailure_OTHERS : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"fail")]
		public static WebArchive createDeployment1()
		{
	   return TestPostDeployFailure_JBOSS.createDeployment1();
		}

	  [Deployment(name:"checker")]
	  public static WebArchive createDeployment2()
	  {
		return initWebArchiveDeployment("checker.war");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("checker") public void test()
	  public virtual void test()
	  {

		// make sure the deployment of the first app was rolled back

		long count = processEngine.RepositoryService.createDeploymentQuery().count();

		Assert.assertEquals(1, count);

	  }

	}

}