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
namespace org.camunda.bpm.engine.spring.test.plugin
{
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Autowired = org.springframework.beans.factory.annotation.Autowired;
	using Bean = org.springframework.context.annotation.Bean;
	using ContextConfiguration = org.springframework.test.context.ContextConfiguration;
	using SpringJUnit4ClassRunner = org.springframework.test.context.junit4.SpringJUnit4ClassRunner;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(SpringJUnit4ClassRunner.class) @ContextConfiguration(classes = SpringProcessEnginePluginTest.TestConfig.class) public class SpringProcessEnginePluginTest
	public class SpringProcessEnginePluginTest
	{

	  public class TestConfig
	  {

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Bean public org.camunda.bpm.engine.spring.SpringProcessEnginePlugin theBeanName()
		public virtual SpringProcessEnginePlugin theBeanName()
		{
		  return new SpringProcessEnginePluginAnonymousInnerClass(this);
		}

		private class SpringProcessEnginePluginAnonymousInnerClass : SpringProcessEnginePlugin
		{
			private readonly TestConfig outerInstance;

			public SpringProcessEnginePluginAnonymousInnerClass(TestConfig outerInstance)
			{
				this.outerInstance = outerInstance;
			}

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Autowired private org.camunda.bpm.engine.spring.SpringProcessEnginePlugin plugin;
	  private SpringProcessEnginePlugin plugin;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void verifyToString() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void verifyToString()
	  {
		Assert.assertEquals(plugin.ToString(), "theBeanName");
	  }
	}
}