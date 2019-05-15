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
namespace org.camunda.bpm.engine.spring.test
{

	using AbstractProcessEngineTestCase = org.camunda.bpm.engine.impl.test.AbstractProcessEngineTestCase;
	using CachedIntrospectionResults = org.springframework.beans.CachedIntrospectionResults;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using ApplicationContext = org.springframework.context.ApplicationContext;
	using ApplicationContextAware = org.springframework.context.ApplicationContextAware;
	using ConfigurableApplicationContext = org.springframework.context.ConfigurableApplicationContext;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;
	using TestContextManager = org.springframework.test.context.TestContextManager;
	using TestExecutionListeners = org.springframework.test.context.TestExecutionListeners;
	using DependencyInjectionTestExecutionListener = org.springframework.test.context.support.DependencyInjectionTestExecutionListener;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TestExecutionListeners(DependencyInjectionTestExecutionListener.class) public abstract class SpringProcessEngineTestCase extends org.camunda.bpm.engine.impl.test.AbstractProcessEngineTestCase implements org.springframework.context.ApplicationContextAware
	public abstract class SpringProcessEngineTestCase : AbstractProcessEngineTestCase, ApplicationContextAware
	{

	  protected internal TestContextManager testContextManager;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired protected org.springframework.context.ConfigurableApplicationContext applicationContext;
	  protected internal ConfigurableApplicationContext applicationContext;

	  public SpringProcessEngineTestCase() : base()
	  {
		this.testContextManager = new TestContextManager(this.GetType());

		SpringTestHelper testHelper = lookupTestHelper();
		testHelper.beforeTestClass(testContextManager);
	  }

	  protected internal virtual SpringTestHelper lookupTestHelper()
	  {
		ServiceLoader<SpringTestHelper> serviceLoader = ServiceLoader.load(typeof(SpringTestHelper));
		return serviceLoader.GetEnumerator().next();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void runBare() throws Throwable
	  public override void runBare()
	  {
		testContextManager.prepareTestInstance(this); // this will initialize all dependencies
		try
		{
		  base.runBare();
		}
		finally
		{
		  testContextManager.afterTestClass();
		  applicationContext.close();
		  applicationContext = null;
		  processEngine = null;
		  testContextManager = null;
		  CachedIntrospectionResults.clearClassLoader(this.GetType().ClassLoader);
		}
	  }

	  protected internal override void initializeProcessEngine()
	  {
		ContextConfiguration contextConfiguration = this.GetType().getAnnotation(typeof(ContextConfiguration));
		processEngine = applicationContext.getBean(typeof(ProcessEngine));
	  }

	  public virtual ApplicationContext ApplicationContext
	  {
		  set
		  {
			this.applicationContext = (ConfigurableApplicationContext) value;
		  }
	  }

	}

}