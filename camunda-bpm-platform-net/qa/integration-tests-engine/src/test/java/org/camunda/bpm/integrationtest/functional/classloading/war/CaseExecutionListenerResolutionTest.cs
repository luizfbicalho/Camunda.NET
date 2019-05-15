using System;

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
namespace org.camunda.bpm.integrationtest.functional.classloading.war
{
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using ExampleCaseExecutionListener = org.camunda.bpm.integrationtest.functional.classloading.beans.ExampleCaseExecutionListener;
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
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class CaseExecutionListenerResolutionTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class CaseExecutionListenerResolutionTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessArchiveDeplyoment()
		public static WebArchive createProcessArchiveDeplyoment()
		{
		return initWebArchiveDeployment().addClass(typeof(ExampleCaseExecutionListener)).addAsResource("org/camunda/bpm/integrationtest/functional/classloading/CaseExecutionListenerResolutionTest.cmmn");
		}

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "client.war").addClass(typeof(AbstractFoxPlatformIntegrationTest));

		TestContainer.addContainerSpecificResources(webArchive);

		return webArchive;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testResolveCaseExecutionListenerClass()
	  public virtual void testResolveCaseExecutionListenerClass()
	  {
		// assert that we cannot load the delegate here:
		try
		{
		  Type.GetType("org.camunda.bpm.integrationtest.functional.classloading.beans.ExampleCaseExecutionListener");
		  Assert.fail("CNFE expected");
		}
		catch (ClassNotFoundException)
		{
		  // expected
		}

		string caseInstanceId = caseService.withCaseDefinitionByKey("case").create().Id;

		string humanTaskId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().variableName("listener").caseInstanceIdIn(caseInstanceId);

		Assert.assertNotNull(query.singleResult());
		Assert.assertEquals("listener-notified", query.singleResult().Value);

		caseService.withCaseExecution(caseInstanceId).removeVariable("listener").execute();

		Assert.assertEquals(0, query.count());

		// the delegate expression listener should execute successfully
		caseService.withCaseExecution(humanTaskId).complete();

		Assert.assertNotNull(query.singleResult());
		Assert.assertEquals("listener-notified", query.singleResult().Value);

	  }

	}

}