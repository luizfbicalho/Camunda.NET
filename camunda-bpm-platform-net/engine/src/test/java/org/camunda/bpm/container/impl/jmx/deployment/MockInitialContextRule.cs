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
namespace org.camunda.bpm.container.impl.jmx.deployment
{
	using TestRule = org.junit.rules.TestRule;
	using Description = org.junit.runner.Description;
	using Statement = org.junit.runners.model.Statement;

	public class MockInitialContextRule : TestRule
	{

	  private readonly Context context;

	  public MockInitialContextRule(Context context)
	  {
		this.context = context;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public org.junit.runners.model.Statement apply(final org.junit.runners.model.Statement super, org.junit.runner.Description description)
	  public override Statement apply(Statement @base, Description description)
	  {
		return new StatementAnonymousInnerClass(this, @base);
	  }

	  private class StatementAnonymousInnerClass : Statement
	  {
		  private readonly MockInitialContextRule outerInstance;

		  private Statement @base;

		  public StatementAnonymousInnerClass(MockInitialContextRule outerInstance, Statement @base)
		  {
			  this.outerInstance = outerInstance;
			  this.@base = @base;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void evaluate() throws Throwable
		  public override void evaluate()
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			System.setProperty(Context.INITIAL_CONTEXT_FACTORY, typeof(MockInitialContextFactory).FullName);
			MockInitialContextFactory.CurrentContext = outerInstance.context;
			try
			{
			  @base.evaluate();
			}
			finally
			{
			  System.clearProperty(Context.INITIAL_CONTEXT_FACTORY);
			  MockInitialContextFactory.clearCurrentContext();
			}
		  }
	  }

	}

}