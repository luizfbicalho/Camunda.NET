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
namespace org.camunda.bpm.integrationtest.deployment.ear
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AnnotatedEjbPa = org.camunda.bpm.integrationtest.deployment.ear.beans.AnnotatedEjbPa;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using EnterpriseArchive = org.jboss.shrinkwrap.api.spec.EnterpriseArchive;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestPaAnnotatedEjb extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class TestPaAnnotatedEjb : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.EnterpriseArchive paAsEjbModule()
		public static EnterpriseArchive paAsEjbModule()
		{

		JavaArchive processArchive1Jar = ShrinkWrap.create(typeof(JavaArchive), "pa.jar").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestPaAnnotatedEjb)).addClass(typeof(AnnotatedEjbPa)).addAsResource("org/camunda/bpm/integrationtest/deployment/ear/process1.bpmn20.xml", "process.bpmn").addAsResource("META-INF/processes.xml", "deployment-descriptor-with-custom-filename.xml").addAsManifestResource(EmptyAsset.INSTANCE, "beans.xml");

		return ShrinkWrap.create(typeof(EnterpriseArchive), "paAsEjbModule.ear").addAsModule(processArchive1Jar).addAsLibrary(DeploymentHelper.EngineCdi);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPaAnnotatedEjb()
	  public virtual void testPaAnnotatedEjb()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process1");

		Assert.assertNotNull(processInstance);
	  }

	}

}