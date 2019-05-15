using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.rest.util.container
{

	using TestCustomResourceApplication = org.camunda.bpm.engine.rest.application.TestCustomResourceApplication;
	using ExternalResource = org.junit.rules.ExternalResource;
	using RuleChain = org.junit.rules.RuleChain;
	using TemporaryFolder = org.junit.rules.TemporaryFolder;
	using TestRule = org.junit.rules.TestRule;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JerseySpecifics : ContainerSpecifics
	{

	  protected internal static readonly TestRuleFactory DEFAULT_RULE_FACTORY = new EmbeddedServerRuleFactory(new JaxrsApplication());

	  protected internal static readonly IDictionary<Type, TestRuleFactory> TEST_RULE_FACTORIES = new Dictionary<Type, TestRuleFactory>();

	  static JerseySpecifics()
	  {
		TEST_RULE_FACTORIES[typeof(ExceptionHandlerTest)] = new EmbeddedServerRuleFactory(new TestCustomResourceApplication());
		TEST_RULE_FACTORIES[typeof(CustomJacksonDateFormatTest)] = new ServletContainerRuleFactory("custom-date-format-web.xml");
	  }

	  public virtual TestRule getTestRule(Type testClass)
	  {
		TestRuleFactory ruleFactory = DEFAULT_RULE_FACTORY;

		if (TEST_RULE_FACTORIES.ContainsKey(testClass))
		{
		  ruleFactory = TEST_RULE_FACTORIES[testClass];
		}

		return ruleFactory.createTestRule();
	  }

	  public class EmbeddedServerRuleFactory : TestRuleFactory
	  {

		protected internal Application jaxRsApplication;

		public EmbeddedServerRuleFactory(Application jaxRsApplication)
		{
		  this.jaxRsApplication = jaxRsApplication;
		}

		public virtual TestRule createTestRule()
		{
		  return new ExternalResourceAnonymousInnerClass(this);
		}

		private class ExternalResourceAnonymousInnerClass : ExternalResource
		{
			private readonly EmbeddedServerRuleFactory outerInstance;

			public ExternalResourceAnonymousInnerClass(EmbeddedServerRuleFactory outerInstance)
			{
				this.outerInstance = outerInstance;
				bootstrap = new JerseyServerBootstrap(outerInstance.jaxRsApplication);
			}


			internal JerseyServerBootstrap bootstrap;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void before() throws Throwable
			protected internal void before()
			{
			  bootstrap.start();
			}

			protected internal void after()
			{
			  bootstrap.stop();
			}
		}
	  }

	  public virtual TestRule getTestRule(string webXmlResource)
	  {
		throw new System.NotSupportedException();
	  }

	  public class ServletContainerRuleFactory : TestRuleFactory
	  {

		protected internal string webXmlResource;

		public ServletContainerRuleFactory(string webXmlResource)
		{
		  this.webXmlResource = webXmlResource;
		}

		public virtual TestRule createTestRule()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.junit.rules.TemporaryFolder tempFolder = new org.junit.rules.TemporaryFolder();
		  TemporaryFolder tempFolder = new TemporaryFolder();

		  return RuleChain.outerRule(tempFolder).around(new ExternalResourceAnonymousInnerClass(this, tempFolder));
		}

		private class ExternalResourceAnonymousInnerClass : ExternalResource
		{
			private readonly ServletContainerRuleFactory outerInstance;

			private TemporaryFolder tempFolder;

			public ExternalResourceAnonymousInnerClass(ServletContainerRuleFactory outerInstance, TemporaryFolder tempFolder)
			{
				this.outerInstance = outerInstance;
				this.tempFolder = tempFolder;
				bootstrap = new JerseyTomcatServerBootstrap(outerInstance.webXmlResource);
			}


			internal TomcatServerBootstrap bootstrap;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void before() throws Throwable
			protected internal void before()
			{
			  bootstrap.WorkingDir = tempFolder.Root.AbsolutePath;
			  bootstrap.start();
			}

			protected internal void after()
			{
			  bootstrap.stop();
			}
		}

	  }

	}

}