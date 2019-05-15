using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

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
namespace org.camunda.bpm.engine.impl.test
{
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProcessApplicationManager = org.camunda.bpm.engine.impl.application.ProcessApplicationManager;
	using BpmnDeployer = org.camunda.bpm.engine.impl.bpmn.deployer.BpmnDeployer;
	using IdGenerator = org.camunda.bpm.engine.impl.cfg.IdGenerator;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CmmnDeployer = org.camunda.bpm.engine.impl.cmmn.deployer.CmmnDeployer;
	using DbIdGenerator = org.camunda.bpm.engine.impl.db.DbIdGenerator;
	using PersistenceSession = org.camunda.bpm.engine.impl.db.PersistenceSession;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DecisionDefinitionDeployer = org.camunda.bpm.engine.impl.dmn.deployer.DecisionDefinitionDeployer;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using DatabasePurgeReport = org.camunda.bpm.engine.impl.management.DatabasePurgeReport;
	using PurgeReport = org.camunda.bpm.engine.impl.management.PurgeReport;
	using CachePurgeReport = org.camunda.bpm.engine.impl.persistence.deploy.cache.CachePurgeReport;
	using PropertyEntity = org.camunda.bpm.engine.impl.persistence.entity.PropertyEntity;
	using ClassNameUtil = org.camunda.bpm.engine.impl.util.ClassNameUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using RequiredHistoryLevel = org.camunda.bpm.engine.test.RequiredHistoryLevel;
	using Assert = org.junit.Assert;
	using Description = org.junit.runner.Description;
	using Logger = org.slf4j.Logger;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class TestHelper
	{

	  private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  public const string EMPTY_LINE = "                                                                                           ";

	  public static readonly IList<string> TABLENAMES_EXCLUDED_FROM_DB_CLEAN_CHECK = new IList<string> {"ACT_GE_PROPERTY", "ACT_GE_SCHEMA_LOG"};

	  internal static IDictionary<string, ProcessEngine> processEngines = new Dictionary<string, ProcessEngine>();

	  public static readonly IList<string> RESOURCE_SUFFIXES = new List<string>();

	  static TestHelper()
	  {
		((IList<string>)RESOURCE_SUFFIXES).AddRange(Arrays.asList(BpmnDeployer.BPMN_RESOURCE_SUFFIXES));
		((IList<string>)RESOURCE_SUFFIXES).AddRange(Arrays.asList(CmmnDeployer.CMMN_RESOURCE_SUFFIXES));
		((IList<string>)RESOURCE_SUFFIXES).AddRange(Arrays.asList(DecisionDefinitionDeployer.DMN_RESOURCE_SUFFIXES));
	  }

	  /// <summary>
	  /// use <seealso cref="ProcessEngineAssert"/> instead.
	  /// </summary>
	  [Obsolete]
	  public static void assertProcessEnded(ProcessEngine processEngine, string processInstanceId)
	  {
		ProcessEngineAssert.assertProcessEnded(processEngine, processInstanceId);
	  }

	  public static string annotationDeploymentSetUp(ProcessEngine processEngine, Type testClass, string methodName, Deployment deploymentAnnotation)
	  {
		string deploymentId = null;
		System.Reflection.MethodInfo method = null;
		bool onMethod = true;

		try
		{
		  method = getMethod(testClass, methodName);
		}
		catch (Exception)
		{
		  if (deploymentAnnotation == null)
		  {
			// we have neither the annotation, nor can look it up from the method
			return null;
		  }
		}

		if (deploymentAnnotation == null)
		{
		  deploymentAnnotation = method.getAnnotation(typeof(Deployment));
		}
		// if not found on method, try on class level
		if (deploymentAnnotation == null)
		{
		  onMethod = false;
		  Type lookForAnnotationClass = testClass;
		  while (lookForAnnotationClass != typeof(object))
		  {
			deploymentAnnotation = lookForAnnotationClass.getAnnotation(typeof(Deployment));
			if (deploymentAnnotation != null)
			{
			  testClass = lookForAnnotationClass;
			  break;
			}
			lookForAnnotationClass = lookForAnnotationClass.BaseType;
		  }
		}

		if (deploymentAnnotation != null)
		{
		  LOG.debug("annotation @Deployment creates deployment for {}.{}", ClassNameUtil.getClassNameWithoutPackage(testClass), methodName);
		  string[] resources = deploymentAnnotation.resources();
		  if (resources.Length == 0 && method != null)
		  {
			string name = onMethod ? method.Name : null;
			string resource = getBpmnProcessDefinitionResource(testClass, name);
			resources = new string[]{resource};
		  }

		  DeploymentBuilder deploymentBuilder = processEngine.RepositoryService.createDeployment().name(ClassNameUtil.getClassNameWithoutPackage(testClass) + "." + methodName);

		  foreach (string resource in resources)
		  {
			deploymentBuilder.addClasspathResource(resource);
		  }

		  deploymentId = deploymentBuilder.deploy().Id;
		}

		return deploymentId;
	  }

	  public static string annotationDeploymentSetUp(ProcessEngine processEngine, Type testClass, string methodName)
	  {
		return annotationDeploymentSetUp(processEngine, testClass, methodName, null);
	  }

	  public static void annotationDeploymentTearDown(ProcessEngine processEngine, string deploymentId, Type testClass, string methodName)
	  {
		LOG.debug("annotation @Deployment deletes deployment for {}.{}", ClassNameUtil.getClassNameWithoutPackage(testClass), methodName);
		deleteDeployment(processEngine, deploymentId);
	  }

	  public static void deleteDeployment(ProcessEngine processEngine, string deploymentId)
	  {
		if (!string.ReferenceEquals(deploymentId, null))
		{
		  processEngine.RepositoryService.deleteDeployment(deploymentId, true);
		}
	  }

	  /// <summary>
	  /// get a resource location by convention based on a class (type) and a
	  /// relative resource name. The return value will be the full classpath
	  /// location of the type, plus a suffix built from the name parameter:
	  /// <code>BpmnDeployer.BPMN_RESOURCE_SUFFIXES</code>.
	  /// The first resource matching a suffix will be returned.
	  /// </summary>
	  public static string getBpmnProcessDefinitionResource(Type type, string name)
	  {
		foreach (string suffix in RESOURCE_SUFFIXES)
		{
		  string resource = createResourceName(type, name, suffix);
		  Stream inputStream = ReflectUtil.getResourceAsStream(resource);
		  if (inputStream == null)
		  {
			continue;
		  }
		  else
		  {
			return resource;
		  }
		}
		return createResourceName(type, name, BpmnDeployer.BPMN_RESOURCE_SUFFIXES[0]);
	  }

	  private static string createResourceName(Type type, string name, string suffix)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		StringBuilder r = new StringBuilder(type.FullName.Replace('.', '/'));
		if (!string.ReferenceEquals(name, null))
		{
		  r.Append("." + name);
		}
		return r.Append("." + suffix).ToString();
	  }

	  public static bool annotationRequiredHistoryLevelCheck(ProcessEngine processEngine, Description description)
	  {
		RequiredHistoryLevel annotation = description.getAnnotation(typeof(RequiredHistoryLevel));

		if (annotation != null)
		{
		  return historyLevelCheck(processEngine, annotation);

		}
		else
		{
		  return annotationRequiredHistoryLevelCheck(processEngine, description.TestClass, description.MethodName);
		}
	  }

	  private static bool historyLevelCheck(ProcessEngine processEngine, RequiredHistoryLevel annotation)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;

		HistoryLevel requiredHistoryLevel = getHistoryLevelForName(processEngineConfiguration.HistoryLevels, annotation.value());
		HistoryLevel currentHistoryLevel = processEngineConfiguration.HistoryLevel;

		return currentHistoryLevel.Id >= requiredHistoryLevel.Id;
	  }

	  private static HistoryLevel getHistoryLevelForName(IList<HistoryLevel> historyLevels, string name)
	  {
		foreach (HistoryLevel historyLevel in historyLevels)
		{

		  if (historyLevel.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
		  {
			return historyLevel;
		  }
		}
		throw new System.ArgumentException("Unknown history level: " + name);
	  }

	  public static bool annotationRequiredHistoryLevelCheck(ProcessEngine processEngine, Type testClass, string methodName)
	  {
		RequiredHistoryLevel annotation = getAnnotation(processEngine, testClass, methodName, typeof(RequiredHistoryLevel));

		if (annotation != null)
		{
		  return historyLevelCheck(processEngine, annotation);
		}
		else
		{
		  return true;
		}
	  }

	  private static T getAnnotation<T>(ProcessEngine processEngine, Type testClass, string methodName, Type<T> annotationClass) where T : Annotation
	  {
		System.Reflection.MethodInfo method = null;
		T annotation = null;

		try
		{
		  method = getMethod(testClass, methodName);
		  annotation = method.getAnnotation(annotationClass);
		}
		catch (Exception)
		{
		  // - ignore if we cannot access the method
		  // - just try again with the class
		  // => can for example be the case for parameterized tests where methodName does not correspond to the actual method name
		  //    (note that method-level annotations still work in this
		  //     scenario due to Description#getAnnotation in annotationRequiredHistoryLevelCheck)
		}

		// if not found on method, try on class level
		if (annotation == null)
		{
		  annotation = testClass.getAnnotation(annotationClass);
		}
		return annotation;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static Method getMethod(Class clazz, String methodName) throws SecurityException, NoSuchMethodException
	  protected internal static System.Reflection.MethodInfo getMethod(Type clazz, string methodName)
	  {
		return clazz.GetMethod(methodName, (Type[]) null);
	  }

	  /// <summary>
	  /// Ensures that the deployment cache and database is clean after a test. If not the cache
	  /// and database will be cleared.
	  /// </summary>
	  /// <param name="processEngine"> the <seealso cref="ProcessEngine"/> to test </param>
	  /// <exception cref="AssertionError"> if the deployment cache or database was not clean </exception>
	  public static void assertAndEnsureCleanDbAndCache(ProcessEngine processEngine)
	  {
		assertAndEnsureCleanDbAndCache(processEngine, true);
	  }

	  /// <summary>
	  /// Ensures that the deployment cache and database is clean after a test. If not the cache
	  /// and database will be cleared.
	  /// </summary>
	  /// <param name="processEngine"> the <seealso cref="ProcessEngine"/> to test </param>
	  /// <param name="fail"> if true the method will throw an <seealso cref="AssertionError"/> if the deployment cache or database is not clean </param>
	  /// <exception cref="AssertionError"> if the deployment cache or database was not clean </exception>
	  public static string assertAndEnsureCleanDbAndCache(ProcessEngine processEngine, bool fail)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;

		// clear user operation log in case some operations are
		// executed with an authenticated user
		clearUserOperationLog(processEngineConfiguration);

		LOG.debug("verifying that db is clean after test");
		PurgeReport purgeReport = ((ManagementServiceImpl) processEngine.ManagementService).purge();

		string paRegistrationMessage = assertAndEnsureNoProcessApplicationsRegistered(processEngine);

		StringBuilder message = new StringBuilder();
		CachePurgeReport cachePurgeReport = purgeReport.CachePurgeReport;
		if (!cachePurgeReport.Empty)
		{
		  message.Append("Deployment cache is not clean:\n").Append(cachePurgeReport.PurgeReportAsString);
		}
		else
		{
		  LOG.debug("Deployment cache was clean.");
		}
		DatabasePurgeReport databasePurgeReport = purgeReport.DatabasePurgeReport;
		if (!databasePurgeReport.Empty)
		{
		  message.Append("Database is not clean:\n").Append(databasePurgeReport.PurgeReportAsString);
		}
		else
		{
		  LOG.debug("Database was clean.");
		}
		if (!string.ReferenceEquals(paRegistrationMessage, null))
		{
		  message.Append(paRegistrationMessage);
		}

		if (fail && message.Length > 0)
		{
		  Assert.fail(message.ToString());
		}
		return message.ToString();
	  }

	  /// <summary>
	  /// Ensures that the deployment cache is empty after a test. If not the cache
	  /// will be cleared.
	  /// </summary>
	  /// <param name="processEngine"> the <seealso cref="ProcessEngine"/> to test </param>
	  /// <exception cref="AssertionError"> if the deployment cache was not clean </exception>
	  public static void assertAndEnsureCleanDeploymentCache(ProcessEngine processEngine)
	  {
		assertAndEnsureCleanDeploymentCache(processEngine, true);
	  }

	  /// <summary>
	  /// Ensures that the deployment cache is empty after a test. If not the cache
	  /// will be cleared.
	  /// </summary>
	  /// <param name="processEngine"> the <seealso cref="ProcessEngine"/> to test </param>
	  /// <param name="fail"> if true the method will throw an <seealso cref="AssertionError"/> if the deployment cache is not clean </param>
	  /// <returns> the deployment cache summary if fail is set to false or null if deployment cache was clean </returns>
	  /// <exception cref="AssertionError"> if the deployment cache was not clean and fail is set to true </exception>
	  public static string assertAndEnsureCleanDeploymentCache(ProcessEngine processEngine, bool fail)
	  {
		StringBuilder outputMessage = new StringBuilder();
		ProcessEngineConfigurationImpl processEngineConfiguration = ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
		CachePurgeReport cachePurgeReport = processEngineConfiguration.DeploymentCache.purgeCache();

		outputMessage.Append(cachePurgeReport.PurgeReportAsString);
		if (outputMessage.Length > 0)
		{
		  outputMessage.Insert(0, "Deployment cache not clean:\n");
		  LOG.error(outputMessage.ToString());

		  if (fail)
		  {
			Assert.fail(outputMessage.ToString());
		  }

		  return outputMessage.ToString();
		}
		else
		{
		  LOG.debug("Deployment cache was clean");
		  return null;
		}
	  }


	  public static string assertAndEnsureNoProcessApplicationsRegistered(ProcessEngine processEngine)
	  {
		ProcessEngineConfigurationImpl engineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
		ProcessApplicationManager processApplicationManager = engineConfiguration.ProcessApplicationManager;

		if (processApplicationManager.hasRegistrations())
		{
		  processApplicationManager.clearRegistrations();
		  return "There are still process applications registered";
		}
		else
		{
		  return null;
		}

	  }

	  public static void waitForJobExecutorToProcessAllJobs(ProcessEngineConfigurationImpl processEngineConfiguration, long maxMillisToWait, long intervalMillis)
	  {
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		jobExecutor.start();

		try
		{
		  Timer timer = new Timer();
		  InteruptTask task = new InteruptTask(Thread.CurrentThread);
		  timer.schedule(task, maxMillisToWait);
		  bool areJobsAvailable = true;
		  try
		  {
			while (areJobsAvailable && !task.TimeLimitExceeded)
			{
			  Thread.Sleep(intervalMillis);
			  areJobsAvailable = areJobsAvailable(processEngineConfiguration);
			}
		  }
		  catch (InterruptedException)
		  {
		  }
		  finally
		  {
			timer.cancel();
		  }
		  if (areJobsAvailable)
		  {
			throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
		  }

		}
		finally
		{
		  jobExecutor.shutdown();
		}
	  }

	  public static bool areJobsAvailable(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return !processEngineConfiguration.ManagementService.createJobQuery().executable().list().Empty;
	  }

	  public static void resetIdGenerator(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		IdGenerator idGenerator = processEngineConfiguration.IdGenerator;

		if (idGenerator is DbIdGenerator)
		{
		  ((DbIdGenerator) idGenerator).reset();
		}
	  }

	  private class InteruptTask : TimerTask
	  {
		protected internal bool timeLimitExceeded = false;
		protected internal Thread thread;
		public InteruptTask(Thread thread)
		{
		  this.thread = thread;
		}
		public virtual bool TimeLimitExceeded
		{
			get
			{
			  return timeLimitExceeded;
			}
		}
		public override void run()
		{
		  timeLimitExceeded = true;
		  thread.Interrupt();
		}
	  }

	  public static ProcessEngine getProcessEngine(string configurationResource)
	  {
		ProcessEngine processEngine = processEngines[configurationResource];
		if (processEngine == null)
		{
		  LOG.debug("==== BUILDING PROCESS ENGINE ========================================================================");
		  processEngine = ProcessEngineConfiguration.createProcessEngineConfigurationFromResource(configurationResource).buildProcessEngine();
		  LOG.debug("==== PROCESS ENGINE CREATED =========================================================================");
		  processEngines[configurationResource] = processEngine;
		}
		return processEngine;
	  }

	  public static void closeProcessEngines()
	  {
		foreach (ProcessEngine processEngine in processEngines.Values)
		{
		  processEngine.close();
		}
		processEngines.Clear();
	  }

	  public static void createSchema(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass());
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  public object execute(CommandContext commandContext)
		  {

			commandContext.getSession(typeof(PersistenceSession)).dbSchemaCreate();
			return null;
		  }
	  }

	  public static void dropSchema(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2());
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  public object execute(CommandContext commandContext)
		  {
			commandContext.DbSqlSession.dbSchemaDrop();
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static void createOrUpdateHistoryLevel(final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration)
	  public static void createOrUpdateHistoryLevel(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(processEngineConfiguration));
	  }

	  private class CommandAnonymousInnerClass3 : Command<object>
	  {
		  private ProcessEngineConfigurationImpl processEngineConfiguration;

		  public CommandAnonymousInnerClass3(ProcessEngineConfigurationImpl processEngineConfiguration)
		  {
			  this.processEngineConfiguration = processEngineConfiguration;
		  }

		  public object execute(CommandContext commandContext)
		  {
			DbEntityManager dbEntityManager = commandContext.DbEntityManager;
			PropertyEntity historyLevelProperty = dbEntityManager.selectById(typeof(PropertyEntity), "historyLevel");
			if (historyLevelProperty != null)
			{
			  if (processEngineConfiguration.HistoryLevel.Id != new int?(historyLevelProperty.Value))
			  {
				historyLevelProperty.Value = Convert.ToString(processEngineConfiguration.HistoryLevel.Id);
				dbEntityManager.merge(historyLevelProperty);
			  }
			}
			else
			{
			  HistoryLevelSetupCommand.dbCreateHistoryLevel(commandContext);
			}
			return null;
		  }
	  }

	  public static void deleteHistoryLevel(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass4());
	  }

	  private class CommandAnonymousInnerClass4 : Command<object>
	  {
		  public object execute(CommandContext commandContext)
		  {
			DbEntityManager dbEntityManager = commandContext.DbEntityManager;
			PropertyEntity historyLevelProperty = dbEntityManager.selectById(typeof(PropertyEntity), "historyLevel");
			if (historyLevelProperty != null)
			{
			  dbEntityManager.delete(historyLevelProperty);
			}
			return null;
		  }
	  }

	  public static void clearUserOperationLog(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		if (processEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
		{
		  HistoryService historyService = processEngineConfiguration.HistoryService;
		  IList<UserOperationLogEntry> logs = historyService.createUserOperationLogQuery().list();
		  foreach (UserOperationLogEntry log in logs)
		  {
			historyService.deleteUserOperationLogEntry(log.Id);
		  }
		}
	  }




	}

}