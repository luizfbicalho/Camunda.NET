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
namespace org.camunda.bpm.engine.spring.test.container
{
	using ManagedProcessEngineFactoryBean = org.camunda.bpm.engine.spring.container.ManagedProcessEngineFactoryBean;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using AbstractApplicationContext = org.springframework.context.support.AbstractApplicationContext;
	using ClassPathXmlApplicationContext = org.springframework.context.support.ClassPathXmlApplicationContext;

	/// <summary>
	/// <para>Testcase for <seealso cref="ManagedProcessEngineFactoryBean"/></para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ManagedProcessEngineFactoryBeanTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessApplicationDeployment()
	  public virtual void testProcessApplicationDeployment()
	  {

		// initially, no process engine is registered:
		Assert.assertNull(BpmPlatform.DefaultProcessEngine);
		Assert.assertEquals(0, BpmPlatform.ProcessEngineService.ProcessEngines.Count);

		// start spring application context
		AbstractApplicationContext applicationContext = new ClassPathXmlApplicationContext("org/camunda/bpm/engine/spring/test/container/ManagedProcessEngineFactoryBean-context.xml");
		applicationContext.start();

		// assert that now the process engine is registered:
		Assert.assertNotNull(BpmPlatform.DefaultProcessEngine);

		// close the spring application context
		applicationContext.close();

		// after closing the application context, the process engine is gone
		Assert.assertNull(BpmPlatform.DefaultProcessEngine);
		Assert.assertEquals(0, BpmPlatform.ProcessEngineService.ProcessEngines.Count);

	  }

	}

}