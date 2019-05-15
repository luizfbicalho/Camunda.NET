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
namespace org.camunda.bpm.integrationtest.functional.ejb
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using SLSBClientDelegate = org.camunda.bpm.integrationtest.functional.ejb.beans.SLSBClientDelegate;
	using SLSBThrowExceptionDelegate = org.camunda.bpm.integrationtest.functional.ejb.beans.SLSBThrowExceptionDelegate;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// Testcase verifying that if an exception is thrown inside an EJB the original
	/// exception reaches the caller
	/// 
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class SLSBExceptionInDelegateTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class SLSBExceptionInDelegateTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(SLSBThrowExceptionDelegate)).addClass(typeof(SLSBClientDelegate)).addAsResource("org/camunda/bpm/integrationtest/functional/ejb/SLSBExceptionInDelegateTest.testOriginalExceptionFromEjbReachesCaller.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/ejb/SLSBExceptionInDelegateTest.callProcess.bpmn20.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOriginalExceptionFromEjbReachesCaller()
	  public virtual void testOriginalExceptionFromEjbReachesCaller()
	  {
		  runtimeService.startProcessInstanceByKey("callProcessWithExceptionFromEjb");
		  Job job = managementService.createJobQuery().singleResult();
		  managementService.setJobRetries(job.Id, 1);

		  waitForJobExecutorToProcessAllJobs();

		  Incident incident = runtimeService.createIncidentQuery().activityId("servicetask1").singleResult();
		  assertThat(incident.IncidentMessage, @is("error"));
	  }

	}

}