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
	using HierarchyMode = org.springframework.test.annotation.DirtiesContext.HierarchyMode;
	using TestContext = org.springframework.test.context.TestContext;
	using TestContextManager = org.springframework.test.context.TestContextManager;
	using TestExecutionListener = org.springframework.test.context.TestExecutionListener;

	/// <summary>
	/// This is the same as the Spring3Test helper, but it is
	/// important to compile the helper against Spring 5 for it to work.
	/// </summary>
	public class Spring5TestHelper : SpringTestHelper
	{

	  public virtual void beforeTestClass(TestContextManager testContextManager)
	  {
		testContextManager.registerTestExecutionListeners(new TestExecutionListenerAnonymousInnerClass(this));
	  }

	  private class TestExecutionListenerAnonymousInnerClass : TestExecutionListener
	  {
		  private readonly Spring5TestHelper outerInstance;

		  public TestExecutionListenerAnonymousInnerClass(Spring5TestHelper outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void prepareTestInstance(org.springframework.test.context.TestContext testContext) throws Exception
		  public override void prepareTestInstance(TestContext testContext)
		  {
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void beforeTestMethod(org.springframework.test.context.TestContext testContext) throws Exception
		  public override void beforeTestMethod(TestContext testContext)
		  {
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void beforeTestClass(org.springframework.test.context.TestContext testContext) throws Exception
		  public override void beforeTestClass(TestContext testContext)
		  {
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void afterTestMethod(org.springframework.test.context.TestContext testContext) throws Exception
		  public override void afterTestMethod(TestContext testContext)
		  {
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void afterTestClass(org.springframework.test.context.TestContext testContext) throws Exception
		  public override void afterTestClass(TestContext testContext)
		  {
			// ensures that the application context is not cached beyond this test
			testContext.markApplicationContextDirty(HierarchyMode.EXHAUSTIVE);
		  }
	  }
	}

}