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
namespace org.camunda.bpm.engine.cdi.test.impl.context
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using CreditCard = org.camunda.bpm.engine.cdi.test.impl.beans.CreditCard;
	using ProcessScopedMessageBean = org.camunda.bpm.engine.cdi.test.impl.beans.ProcessScopedMessageBean;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Test = org.junit.Test;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class BusinessProcessContextTest : CdiProcessEngineTestCase
	{


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testResolution() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testResolution()
	  {
		BusinessProcess businessProcess = getBeanInstance(typeof(BusinessProcess));

		businessProcess.startProcessByKey("testResolution").Id;

		assertNotNull(getBeanInstance(typeof(CreditCard)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResolutionBeforeProcessStart() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testResolutionBeforeProcessStart()
	  {
		// assert that @BusinessProcessScoped beans can be resolved in the absence of an underlying process instance:
		assertNotNull(getBeanInstance(typeof(CreditCard)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testConversationalBeanStoreFlush() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testConversationalBeanStoreFlush()
	  {

		getBeanInstance(typeof(BusinessProcess)).setVariable("testVariable", "testValue");
		string pid = getBeanInstance(typeof(BusinessProcess)).startProcessByKey("testConversationalBeanStoreFlush").Id;

		getBeanInstance(typeof(BusinessProcess)).associateExecutionById(pid);

		// assert that the variable assigned on the businessProcess bean is flushed 
		assertEquals("testValue", runtimeService.getVariable(pid, "testVariable"));

		// assert that the value set to the message bean in the first service task is flushed
		assertEquals("Hello from Activiti", getBeanInstance(typeof(ProcessScopedMessageBean)).Message);

		// complete the task to allow the process instance to terminate
		taskService.complete(taskService.createTaskQuery().singleResult().Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testChangeProcessScopedBeanProperty() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testChangeProcessScopedBeanProperty()
	  {

		// resolve the creditcard bean (@BusinessProcessScoped) and set a value:
		getBeanInstance(typeof(CreditCard)).CreditcardNumber = "123";
		string pid = getBeanInstance(typeof(BusinessProcess)).startProcessByKey("testConversationalBeanStoreFlush").Id;

		getBeanInstance(typeof(BusinessProcess)).startTask(taskService.createTaskQuery().singleResult().Id);

		// assert that the value of creditCardNumber is '123'
		assertEquals("123", getBeanInstance(typeof(CreditCard)).CreditcardNumber);
		// set a different value:
		getBeanInstance(typeof(CreditCard)).CreditcardNumber = "321";
		// complete the task
		getBeanInstance(typeof(BusinessProcess)).completeTask();

		getBeanInstance(typeof(BusinessProcess)).associateExecutionById(pid);

		// now assert that the value of creditcard is "321":
		assertEquals("321", getBeanInstance(typeof(CreditCard)).CreditcardNumber);

		// complete the task to allow the process instance to terminate
		taskService.complete(taskService.createTaskQuery().singleResult().Id);

	  }

	}

}