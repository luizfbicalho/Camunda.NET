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
namespace org.camunda.bpm.integrationtest.functional.slf4j
{
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using ILoggerFactory = org.slf4j.ILoggerFactory;
	using LoggerFactory = org.slf4j.LoggerFactory;
	using NOPLoggerFactory = org.slf4j.helpers.NOPLoggerFactory;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class Slf4jClassloadingTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class Slf4jClassloadingTest : AbstractFoxPlatformIntegrationTest
	{

	  public const string JDK14_LOGGER_FACTORY = "org.slf4j.impl.JDK14LoggerFactory";
	  public const string JBOSS_SLF4J_LOGGER_FACTORY = "org.slf4j.impl.Slf4jLoggerFactory";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
	  public static WebArchive createDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "test.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(TestLogger));

		TestContainer.addContainerSpecificResourcesWithoutWeld(webArchive);
		TestContainer.addCommonLoggingDependency(webArchive);

		return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotUseNopLoggerFactory()
	  public virtual void shouldNotUseNopLoggerFactory()
	  {
		ILoggerFactory loggerFactory = LoggerFactory.ILoggerFactory;

		// verify that a SLF4J backend is used which is not the NOP logger
		assertFalse("Should not use NOPLoggerFactory", loggerFactory is NOPLoggerFactory);

		// should either use slf4j-jdk14 or slf4j-jboss-logmanager
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		string loggerFactoryClassName = loggerFactory.GetType().FullName;
		assertTrue("Should use slf4j-jdk14 or slf4j-jboss-logmanager", JDK14_LOGGER_FACTORY.Equals(loggerFactoryClassName) || JBOSS_SLF4J_LOGGER_FACTORY.Equals(loggerFactoryClassName));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBeAbleToLogMessageWithFormatParameters()
	  public virtual void shouldBeAbleToLogMessageWithFormatParameters()
	  {
		TestLogger logger = TestLogger.INSTANCE;

		// verify that we can use different formatting methods
		logger.testLogWithSingleFormatParameter();
		logger.testLogWithTwoFormatParameters();
		logger.testLogWithArrayFormatter();
	  }


	}

}