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
namespace org.camunda.bpm.engine.spring.test.components
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;
	using SpringJUnit4ClassRunner = org.springframework.test.context.junit4.SpringJUnit4ClassRunner;

	/// <summary>
	/// @author Josh Long
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(SpringJUnit4ClassRunner.class) @ContextConfiguration("classpath:org/camunda/bpm/engine/spring/test/components/ProcessStartingBeanPostProcessorTest-context.xml") public class ProcessStartingBeanPostProcessorTest
	public class ProcessStartingBeanPostProcessorTest
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		private Logger log = Logger.getLogger(this.GetType().FullName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.ProcessEngine processEngine;
		private ProcessEngine processEngine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private ProcessInitiatingPojo processInitiatingPojo;
		private ProcessInitiatingPojo processInitiatingPojo;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.RepositoryService repositoryService;
		private RepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void before()
		public virtual void before()
		{
		  repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/spring/test/autodeployment/autodeploy.b.bpmn20.xml").addClasspathResource("org/camunda/bpm/engine/spring/test/components/waiter.bpmn20.xml").deploy();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void after()
		public virtual void after()
		{
		foreach (Deployment deployment in repositoryService.createDeploymentQuery().list())
		{
		  repositoryService.deleteDeployment(deployment.Id, true);
		}
		processEngine.close();
		processEngine = null;
		processInitiatingPojo = null;
		repositoryService = null;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReturnedProcessInstance() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testReturnedProcessInstance()
		{
			string processInstanceId = this.processInitiatingPojo.startProcessA(22);
			assertNotNull("the process instance id should not be null", processInstanceId);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReflectingSideEffects() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testReflectingSideEffects()
		{
			assertNotNull("the processInitiatingPojo mustn't be null.", this.processInitiatingPojo);

			this.processInitiatingPojo.reset();

			assertEquals(this.processInitiatingPojo.MethodState, 0);

			this.processInitiatingPojo.startProcess(53);

			assertEquals(this.processInitiatingPojo.MethodState, 1);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUsingBusinessKey() throws Throwable
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testUsingBusinessKey()
		{
			long id = 5;
			string businessKey = "usersKey" + DateTimeHelper.CurrentUnixTimeMillis();
			ProcessInstance pi = processInitiatingPojo.enrollCustomer(businessKey, id);
			assertEquals("the business key of the resultant ProcessInstance should match " + "the one specified through the AOP-intercepted method",businessKey, pi.BusinessKey);

		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLaunchingProcessInstance()
		public virtual void testLaunchingProcessInstance()
		{
			long id = 343;
			string processInstance = processInitiatingPojo.startProcessA(id);
			long? customerId = (long?) processEngine.RuntimeService.getVariable(processInstance, "customerId");
			assertEquals("the process variable should both exist and be equal to the value given, " + id, customerId, (long?) id);
			log.info("the customerId fromt he ProcessInstance is " + customerId);
			assertNotNull("processInstanc can't be null", processInstance);
			assertNotNull("the variable should be non-null", customerId);
		}
	}

}