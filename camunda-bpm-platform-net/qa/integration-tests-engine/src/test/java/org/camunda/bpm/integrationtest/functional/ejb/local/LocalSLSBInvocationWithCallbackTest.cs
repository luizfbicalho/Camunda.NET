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
namespace org.camunda.bpm.integrationtest.functional.ejb.local
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using CallbackBean = org.camunda.bpm.integrationtest.functional.ejb.local.bean.CallbackBean;
	using InvokeStartProcessDelegateSLSB = org.camunda.bpm.integrationtest.functional.ejb.local.bean.InvokeStartProcessDelegateSLSB;
	using StartProcessInterface = org.camunda.bpm.integrationtest.functional.ejb.local.bean.StartProcessInterface;
	using StartProcessSLSB = org.camunda.bpm.integrationtest.functional.ejb.local.bean.StartProcessSLSB;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;


	/// 
	/// <summary>
	/// This Test deploys two processes:
	/// - LocalSLSBInvocationTest.testStartProcess.bpmn20.xml  (1)
	/// - LocalSLSBInvocationTest.callbackProcess.bpmn20.xml (2)
	/// 
	/// Two applications are deployed:
	/// <ul>
	/// <li>test.war - Process Application providing Processes (1+2)</li>
	/// <li>service.war - application providing a Local SLSB starting Process (2)</li>
	/// </ul>
	/// 
	/// 
	/// Expected Control flow:
	/// 
	/// <pre>
	///    test.war                                 service.war
	///    ========                                 ===========
	/// 
	/// start (unit test)
	///   Process (1)
	///      |
	///      v
	///   InvokeStartProcessDelegateSLSB  ---->    StartProcessSLSB
	///                                               start Process (2)
	///                                                  |
	///                                                  V
	///       CallbackBean         <-----------  Process Engine
	///  </pre>
	/// 
	/// 
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class LocalSLSBInvocationWithCallbackTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class LocalSLSBInvocationWithCallbackTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name:"pa", order:2)]
		public static WebArchive processArchive()
		{
		return initWebArchiveDeployment().addClass(typeof(InvokeStartProcessDelegateSLSB)).addClass(typeof(CallbackBean)).addAsResource("org/camunda/bpm/integrationtest/functional/ejb/local/LocalSLSBInvocationTest.testStartProcess.bpmn20.xml").addAsResource("org/camunda/bpm/integrationtest/functional/ejb/local/LocalSLSBInvocationTest.callbackProcess.bpmn20.xml").addAsWebInfResource("org/camunda/bpm/integrationtest/functional/ejb/local/jboss-deployment-structure.xml","jboss-deployment-structure.xml");
		}

	  [Deployment(order:1)]
	  public static WebArchive delegateDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "service.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(StartProcessSLSB)).addClass(typeof(StartProcessInterface)).addAsManifestResource(new StringAsset("Dependencies: org.camunda.bpm.camunda-engine"), "MANIFEST.MF"); // get access to engine classes

		TestContainer.addContainerSpecificResourcesForNonPa(webArchive);

		return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("pa") public void testInvokeBean() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInvokeBean()
	  {

		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testInvokeBean");

		Assert.assertEquals(true, runtimeService.getVariable(pi.Id, "result"));

		taskService.complete(taskService.createTaskQuery().processInstanceId(pi.Id).singleResult().Id);
	  }

	}

}