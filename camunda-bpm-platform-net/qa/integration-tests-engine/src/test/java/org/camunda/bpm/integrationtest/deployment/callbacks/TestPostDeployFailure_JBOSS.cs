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
namespace org.camunda.bpm.integrationtest.deployment.callbacks
{
	using Assert = org.junit.Assert;

	using PostDeployFailureApp = org.camunda.bpm.integrationtest.deployment.callbacks.apps.PostDeployFailureApp;
	using Deployer = org.jboss.arquillian.container.test.api.Deployer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ArquillianResource = org.jboss.arquillian.test.api.ArquillianResource;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestPostDeployFailure_JBOSS
	public class TestPostDeployFailure_JBOSS
	{

	  private const string DEPLOYMENT = "DEPLOYMENT";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ArquillianResource private org.jboss.arquillian.container.test.api.Deployer deployer;
	  private Deployer deployer;

	  [Deployment(managed:false, name:DEPLOYMENT)]
	  public static WebArchive createDeployment1()
	  {

		WebArchive archive = ShrinkWrap.create(typeof(WebArchive), "failingDeployment.war").addAsResource("META-INF/processes.xml", "META-INF/processes.xml").addAsResource("org/camunda/bpm/integrationtest/invoice-it.bpmn20.xml").addClass(typeof(PostDeployFailureApp));

		return archive;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void test()
	  public virtual void test()
	  {

		try
		{
		  deployer.deploy(DEPLOYMENT);
		  Assert.fail("failure expected");
		}
		catch (Exception)
		{
		  // expected
		}

	  }

	}

}