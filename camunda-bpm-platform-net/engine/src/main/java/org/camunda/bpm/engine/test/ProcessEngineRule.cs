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
namespace org.camunda.bpm.engine.test
{

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TestHelper = org.camunda.bpm.engine.impl.test.TestHelper;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Assume = org.junit.Assume;
	using TestWatcher = org.junit.rules.TestWatcher;
	using Description = org.junit.runner.Description;
	using Statement = org.junit.runners.model.Statement;


	/// <summary>
	/// Convenience for ProcessEngine and services initialization in the form of a
	/// JUnit rule.
	/// <para>
	/// Usage:
	/// </para>
	/// 
	/// <pre>
	/// public class YourTest {
	/// 
	///   &#64;Rule
	///   public ProcessEngineRule processEngineRule = new ProcessEngineRule();
	/// 
	///   ...
	/// }
	/// </pre>
	/// <para>
	/// The ProcessEngine and the services will be made available to the test class
	/// through the getters of the processEngineRule. The processEngine will be
	/// initialized by default with the camunda.cfg.xml resource on the classpath. To
	/// specify a different configuration file, pass the resource location in
	/// <seealso cref="#ProcessEngineRule(String) the appropriate constructor"/>. Process
	/// engines will be cached statically. Right before the first time the setUp is
	/// called for a given configuration resource, the process engine will be
	/// constructed.
	/// </para>
	/// <para>
	/// You can declare a deployment with the <seealso cref="Deployment"/> annotation. This
	/// base class will make sure that this deployment gets deployed before the setUp
	/// and {@link RepositoryService#deleteDeployment(String, boolean) cascade
	/// deleted} after the tearDown.
	/// </para>
	/// <para>
	/// The processEngineRule also lets you
	/// {@link ProcessEngineRule#setCurrentTime(Date) set the current time used by
	/// the process engine}. This can be handy to control the exact time that is used
	/// by the engine in order to verify e.g. e.g. due dates of timers. Or start, end
	/// and duration times in the history service. In the tearDown, the internal
	/// clock will automatically be reset to use the current system time rather then
	/// the time that was set during a test method. In other words, you don't have to
	/// clean up your own time messing mess ;-)
	/// </para>
	/// <para>
	/// If you need the history service for your tests then you can specify the
	/// required history level of the test method or class, using the
	/// <seealso cref="RequiredHistoryLevel"/> annotation. If the current history level of the
	/// process engine is lower than the specified one then the test is skipped.
	/// </para>
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public class ProcessEngineRule : TestWatcher, ProcessEngineServices
	{

	  protected internal string configurationResource = "camunda.cfg.xml";
	  protected internal string configurationResourceCompat = "activiti.cfg.xml";
	  protected internal string deploymentId = null;
	  protected internal IList<string> additionalDeployments = new List<string>();

	  protected internal bool ensureCleanAfterTest = false;

	  protected internal ProcessEngine processEngine;
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal RepositoryService repositoryService;
	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;
	  protected internal ManagementService managementService;
	  protected internal FormService formService;
	  protected internal FilterService filterService;
	  protected internal AuthorizationService authorizationService;
	  protected internal CaseService caseService;
	  protected internal ExternalTaskService externalTaskService;
	  protected internal DecisionService decisionService;

	  public ProcessEngineRule() : this(false)
	  {
	  }

	  public ProcessEngineRule(bool ensureCleanAfterTest)
	  {
		this.ensureCleanAfterTest = ensureCleanAfterTest;
	  }

	  public ProcessEngineRule(string configurationResource) : this(configurationResource, false)
	  {
	  }

	  public ProcessEngineRule(string configurationResource, bool ensureCleanAfterTest)
	  {
		this.configurationResource = configurationResource;
		this.ensureCleanAfterTest = ensureCleanAfterTest;
	  }

	  public ProcessEngineRule(ProcessEngine processEngine) : this(processEngine, false)
	  {
	  }

	  public ProcessEngineRule(ProcessEngine processEngine, bool ensureCleanAfterTest)
	  {
		this.processEngine = processEngine;
		this.ensureCleanAfterTest = ensureCleanAfterTest;
	  }

	  public override void starting(Description description)
	  {
		deploymentId = TestHelper.annotationDeploymentSetUp(processEngine, description.TestClass, description.MethodName, description.getAnnotation(typeof(Deployment)));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public org.junit.runners.model.Statement apply(final org.junit.runners.model.Statement super, final org.junit.runner.Description description)
	  public override Statement apply(Statement @base, Description description)
	  {

		if (processEngine == null)
		{
		  initializeProcessEngine();
		}

		initializeServices();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hasRequiredHistoryLevel = org.camunda.bpm.engine.impl.test.TestHelper.annotationRequiredHistoryLevelCheck(processEngine, description);
		bool hasRequiredHistoryLevel = TestHelper.annotationRequiredHistoryLevelCheck(processEngine, description);
		return new StatementAnonymousInnerClass(this, @base, description, hasRequiredHistoryLevel);
	  }

	  private class StatementAnonymousInnerClass : Statement
	  {
		  private readonly ProcessEngineRule outerInstance;

		  private Statement @base;
		  private Description description;
		  private bool hasRequiredHistoryLevel;

		  public StatementAnonymousInnerClass(ProcessEngineRule outerInstance, Statement @base, Description description, bool hasRequiredHistoryLevel)
		  {
			  this.outerInstance = outerInstance;
			  this.@base = @base;
			  this.description = description;
			  this.hasRequiredHistoryLevel = hasRequiredHistoryLevel;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void evaluate() throws Throwable
		  public override void evaluate()
		  {
			Assume.assumeTrue("ignored because the current history level is too low", hasRequiredHistoryLevel);
			outerInstance.apply(@base, description).evaluate();
		  }
	  }

	  protected internal virtual void initializeProcessEngine()
	  {
		try
		{
		  processEngine = TestHelper.getProcessEngine(configurationResource);
		}
		catch (Exception ex)
		{
		  if (ex.InnerException != null && ex.InnerException is FileNotFoundException)
		  {
			processEngine = TestHelper.getProcessEngine(configurationResourceCompat);
		  }
		  else
		  {
			throw ex;
		  }
		}
	  }

	  protected internal virtual void initializeServices()
	  {
		processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
		repositoryService = processEngine.RepositoryService;
		runtimeService = processEngine.RuntimeService;
		taskService = processEngine.TaskService;
		historyService = processEngine.HistoryService;
		identityService = processEngine.IdentityService;
		managementService = processEngine.ManagementService;
		formService = processEngine.FormService;
		authorizationService = processEngine.AuthorizationService;
		caseService = processEngine.CaseService;
		filterService = processEngine.FilterService;
		externalTaskService = processEngine.ExternalTaskService;
		decisionService = processEngine.DecisionService;
	  }

	  protected internal virtual void clearServiceReferences()
	  {
		processEngineConfiguration = null;
		repositoryService = null;
		runtimeService = null;
		taskService = null;
		formService = null;
		historyService = null;
		identityService = null;
		managementService = null;
		authorizationService = null;
		caseService = null;
		filterService = null;
		externalTaskService = null;
		decisionService = null;
	  }

	  public override void finished(Description description)
	  {
		identityService.clearAuthentication();
		processEngine.ProcessEngineConfiguration.TenantCheckEnabled = true;

		TestHelper.annotationDeploymentTearDown(processEngine, deploymentId, description.TestClass, description.MethodName);
		foreach (string additionalDeployment in additionalDeployments)
		{
		  TestHelper.deleteDeployment(processEngine, additionalDeployment);
		}

		if (ensureCleanAfterTest)
		{
		  TestHelper.assertAndEnsureCleanDbAndCache(processEngine);
		}

		TestHelper.resetIdGenerator(processEngineConfiguration);
		ClockUtil.reset();


		clearServiceReferences();
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


	  public virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			return processEngine;
		  }
		  set
		  {
			this.processEngine = value;
		  }
	  }


	  public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return processEngineConfiguration;
		  }
		  set
		  {
			this.processEngineConfiguration = value;
		  }
	  }


	  public virtual RepositoryService RepositoryService
	  {
		  get
		  {
			return repositoryService;
		  }
		  set
		  {
			this.repositoryService = value;
		  }
	  }


	  public virtual RuntimeService RuntimeService
	  {
		  get
		  {
			return runtimeService;
		  }
		  set
		  {
			this.runtimeService = value;
		  }
	  }


	  public virtual TaskService TaskService
	  {
		  get
		  {
			return taskService;
		  }
		  set
		  {
			this.taskService = value;
		  }
	  }


	  public virtual HistoryService HistoryService
	  {
		  get
		  {
			return historyService;
		  }
		  set
		  {
			this.historyService = value;
		  }
	  }


	  /// <seealso cref= #setHistoryService(HistoryService) </seealso>
	  /// <param name="historicService">
	  ///          the historiy service instance </param>
	  public virtual HistoryService HistoricDataService
	  {
		  set
		  {
			this.HistoryService = value;
		  }
	  }

	  public virtual IdentityService IdentityService
	  {
		  get
		  {
			return identityService;
		  }
		  set
		  {
			this.identityService = value;
		  }
	  }


	  public virtual ManagementService ManagementService
	  {
		  get
		  {
			return managementService;
		  }
		  set
		  {
			this.managementService = value;
		  }
	  }

	  public virtual AuthorizationService AuthorizationService
	  {
		  get
		  {
			return authorizationService;
		  }
		  set
		  {
			this.authorizationService = value;
		  }
	  }


	  public virtual CaseService CaseService
	  {
		  get
		  {
			return caseService;
		  }
		  set
		  {
			this.caseService = value;
		  }
	  }


	  public virtual FormService FormService
	  {
		  get
		  {
			return formService;
		  }
		  set
		  {
			this.formService = value;
		  }
	  }



	  public virtual FilterService FilterService
	  {
		  get
		  {
			return filterService;
		  }
		  set
		  {
			this.filterService = value;
		  }
	  }


	  public virtual ExternalTaskService ExternalTaskService
	  {
		  get
		  {
			return externalTaskService;
		  }
		  set
		  {
			this.externalTaskService = value;
		  }
	  }


	  public virtual DecisionService DecisionService
	  {
		  get
		  {
			return decisionService;
		  }
		  set
		  {
			this.decisionService = value;
		  }
	  }


	  public virtual void manageDeployment(org.camunda.bpm.engine.repository.Deployment deployment)
	  {
		this.additionalDeployments.Add(deployment.Id);
	  }

	}

}