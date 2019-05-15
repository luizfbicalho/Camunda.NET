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
namespace org.camunda.bpm.integrationtest.jobexecutor
{
	using FailingSLSB = org.camunda.bpm.integrationtest.jobexecutor.beans.FailingSLSB;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class FailedJobCommandTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class FailedJobCommandTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		return initWebArchiveDeployment().addClass(typeof(FailingSLSB)).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/FailedJobCommandTest.bpmn20.xml");

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobRetriesDecremented()
	  public virtual void testJobRetriesDecremented()
	  {
		runtimeService.startProcessInstanceByKey("theProcess");

		Assert.assertEquals(1, managementService.createJobQuery().withRetriesLeft().count());

		waitForJobExecutorToProcessAllJobs();

		// now the retries = 0

		Assert.assertEquals(0, managementService.createJobQuery().withRetriesLeft().count());
		Assert.assertEquals(1, managementService.createJobQuery().noRetriesLeft().count());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobRetriesDecremented_multiple()
	  public virtual void testJobRetriesDecremented_multiple()
	  {

		for (int i = 0; i < 50; i++)
		{
		  runtimeService.startProcessInstanceByKey("theProcess");
		}

		Assert.assertEquals(50, managementService.createJobQuery().withRetriesLeft().count());

		waitForJobExecutorToProcessAllJobs(6 * 60 * 1000);

		// now the retries = 0

		Assert.assertEquals(0, managementService.createJobQuery().withRetriesLeft().count());
		Assert.assertEquals(51, managementService.createJobQuery().noRetriesLeft().count());

	  }

	}

}