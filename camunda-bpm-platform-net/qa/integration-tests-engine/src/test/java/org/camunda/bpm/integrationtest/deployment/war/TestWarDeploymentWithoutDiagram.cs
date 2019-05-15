﻿/*
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

	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestHelper = org.camunda.bpm.integrationtest.util.TestHelper;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// <summary>
	/// @author Christian Lipphardt
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestWarDeploymentWithoutDiagram extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestWarDeploymentWithoutDiagram : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive processArchive()
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(TestHelper)).addAsResource("org/camunda/bpm/integrationtest/testDeployProcessArchive.bpmn20.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeployProcessArchiveDiagramCreationDisabled() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeployProcessArchiveDiagramCreationDisabled()
	  {
		string expectedDiagramResource = "/org/camunda/bpm/integrationtest/testDeployProcessArchive.png";
		string processDefinitionKey = "testDeployProcessArchive";
		TestHelper.assertDiagramIsDeployed(false, this.GetType(), expectedDiagramResource, processDefinitionKey);
	  }

	}

}