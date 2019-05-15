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
namespace org.camunda.bpm.engine.test.standalone.el
{
	using PropertyNotFoundException = org.camunda.bpm.engine.impl.javax.el.PropertyNotFoundException;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class ExpressionBeanAccessTest : ResourceProcessEngineTestCase
	{

	  public ExpressionBeanAccessTest() : base("org/camunda/bpm/engine/test/standalone/el/camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testConfigurationBeanAccess()
	  public virtual void testConfigurationBeanAccess()
	  {
		// Exposed bean returns 'I'm exposed' when to-string is called in first service-task
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("expressionBeanAccess");
		assertEquals("I'm exposed", runtimeService.getVariable(pi.Id, "exposedBeanResult"));

		// After signaling, an expression tries to use a bean that is present in the configuration but
		// is not added to the beans-list
		try
		{
		  runtimeService.signal(pi.Id);
		  fail("Exception expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertNotNull(ae.InnerException);
		  assertTrue(ae.InnerException is PropertyNotFoundException);
		}
	  }
	}

}