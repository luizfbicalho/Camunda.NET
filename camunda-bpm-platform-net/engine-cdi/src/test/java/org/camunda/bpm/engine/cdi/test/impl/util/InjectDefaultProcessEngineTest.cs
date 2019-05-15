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
namespace org.camunda.bpm.engine.cdi.test.impl.util
{
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using InjectedProcessEngineBean = org.camunda.bpm.engine.cdi.test.impl.beans.InjectedProcessEngineBean;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using org.junit;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class InjectDefaultProcessEngineTest
	public class InjectDefaultProcessEngineTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.JavaArchive createDeployment()
		public static JavaArchive createDeployment()
		{
		return ShrinkWrap.create(typeof(JavaArchive)).addPackages(true, "org.camunda.bpm.engine.cdi").addAsManifestResource("org/camunda/bpm/engine/cdi/test/impl/util/beans.xml", "beans.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule processEngineRule = new org.camunda.bpm.engine.test.ProcessEngineRule();
	  public ProcessEngineRule processEngineRule = new ProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		if (BpmPlatform.ProcessEngineService.DefaultProcessEngine == null)
		{
		  org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().registerProcessEngine(processEngineRule.ProcessEngine);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDownCdiProcessEngineTestCase() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDownCdiProcessEngineTestCase()
	  {
		org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.get().unregisterProcessEngine(processEngineRule.ProcessEngine);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessEngineInject()
	  public virtual void testProcessEngineInject()
	  {
		//given only default engine exist

		//when TestClass is created
		InjectedProcessEngineBean testClass = ProgrammaticBeanLookup.lookup(typeof(InjectedProcessEngineBean));
		Assert.assertNotNull(testClass);

		//then default engine is injected
		Assert.assertEquals("default", testClass.processEngine.Name);
	  }
	}

}