using System;

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
namespace org.camunda.bpm.engine.test
{

	using ProcessEngineAssert = org.camunda.bpm.engine.impl.test.ProcessEngineAssert;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	using TestCase = junit.framework.TestCase;


	/// <summary>
	/// Convenience for ProcessEngine and services initialization in the form of a JUnit base class.
	/// 
	/// <para>Usage: <code>public class YourTest extends ProcessEngineTestCase</code></para>
	/// 
	/// <para>The ProcessEngine and the services available to subclasses through protected member fields.
	/// The processEngine will be initialized by default with the camunda.cfg.xml resource
	/// on the classpath.  To specify a different configuration file, override the
	/// <seealso cref="#getConfigurationResource()"/> method.
	/// Process engines will be cached statically.  The first time the setUp is called for a given
	/// configuration resource, the process engine will be constructed.</para>
	/// 
	/// <para>You can declare a deployment with the <seealso cref="Deployment"/> annotation.
	/// This base class will make sure that this deployment gets deployed in the
	/// setUp and <seealso cref="RepositoryService#deleteDeploymentCascade(String, boolean) cascade deleted"/>
	/// in the tearDown.
	/// </para>
	/// 
	/// <para>This class also lets you {@link #setCurrentTime(Date) set the current time used by the
	/// process engine}. This can be handy to control the exact time that is used by the engine
	/// in order to verify e.g. e.g. due dates of timers.  Or start, end and duration times
	/// in the history service.  In the tearDown, the internal clock will automatically be
	/// reset to use the current system time rather then the time that was set during
	/// a test method.  In other words, you don't have to clean up your own time messing mess ;-)
	/// </para>
	/// 
	/// @author Tom Baeyens
	/// @author Falko Menge (camunda)
	/// </summary>
	public class ProcessEngineTestCase : TestCase
	{

	  protected internal string configurationResource = "camunda.cfg.xml";
	  protected internal string configurationResourceCompat = "activiti.cfg.xml";
	  protected internal string deploymentId = null;

	  protected internal ProcessEngine processEngine;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  [Obsolete]
	  protected internal HistoryService historicDataService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;
	  protected internal ManagementService managementService;
	  protected internal FormService formService;
	  protected internal FilterService filterService;
	  protected internal AuthorizationService authorizationService;
	  protected internal CaseService caseService;

	  protected internal bool skipTest = false;

	  /// <summary>
	  /// uses 'camunda.cfg.xml' as it's configuration resource </summary>
	  public ProcessEngineTestCase()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void assertProcessEnded(final String processInstanceId)
	  public virtual void assertProcessEnded(string processInstanceId)
	  {
		ProcessEngineAssert.assertProcessEnded(processEngine, processInstanceId);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();

		if (processEngine == null)
		{
		  initializeProcessEngine();
		  initializeServices();
		}

		bool hasRequiredHistoryLevel = TestHelper.annotationRequiredHistoryLevelCheck(processEngine, this.GetType(), Name);
		// ignore test case when current history level is too low
		skipTest = !hasRequiredHistoryLevel;

		if (!skipTest)
		{
		  deploymentId = TestHelper.annotationDeploymentSetUp(processEngine, this.GetType(), Name);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void runTest() throws Throwable
	  protected internal override void runTest()
	  {
		if (!skipTest)
		{
		  base.runTest();
		}
	  }

	  protected internal virtual void initializeProcessEngine()
	  {
		try
		{
		  processEngine = TestHelper.getProcessEngine(ConfigurationResource);
		}
		catch (Exception ex)
		{
		  if (ex.InnerException != null && ex.InnerException is FileNotFoundException)
		  {
			processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource(configurationResourceCompat).buildProcessEngine();
		  }
		  else
		  {
			throw ex;
		  }
		}
	  }

	  protected internal virtual void initializeServices()
	  {
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		historicDataService = processEngine.HistoryService;
		historyService = processEngine.HistoryService;
		identityService = processEngine.IdentityService;
		managementService = processEngine.ManagementService;
		formService = processEngine.FormService;
		filterService = processEngine.FilterService;
		authorizationService = processEngine.AuthorizationService;
		caseService = processEngine.CaseService;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		TestHelper.annotationDeploymentTearDown(processEngine, deploymentId, this.GetType(), Name);

		ClockUtil.reset();

		base.tearDown();
	  }

	  public static void closeProcessEngines()
	  {
		TestHelper.closeProcessEngines();
	  }

	  public virtual DateTime CurrentTime
	  {
		  set
		  {
			ClockUtil.CurrentTime = value;
		  }
	  }

	  public virtual string ConfigurationResource
	  {
		  get
		  {
			return configurationResource;
		  }
		  set
		  {
			this.configurationResource = value;
		  }
	  }


	}

}