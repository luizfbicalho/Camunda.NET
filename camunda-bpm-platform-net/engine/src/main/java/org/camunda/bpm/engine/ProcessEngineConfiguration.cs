using System;
using System.Collections.Generic;
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
namespace org.camunda.bpm.engine
{

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using PasswordPolicy = org.camunda.bpm.engine.identity.PasswordPolicy;
	using BootstrapEngineCommand = org.camunda.bpm.engine.impl.BootstrapEngineCommand;
	using HistoryLevelSetupCommand = org.camunda.bpm.engine.impl.HistoryLevelSetupCommand;
	using SchemaOperationsProcessEngineBuild = org.camunda.bpm.engine.impl.SchemaOperationsProcessEngineBuild;
	using BeansConfigurationHelper = org.camunda.bpm.engine.impl.cfg.BeansConfigurationHelper;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ValueTypeResolver = org.camunda.bpm.engine.variable.type.ValueTypeResolver;


	/// <summary>
	/// Configuration information from which a process engine can be build.
	/// 
	/// <para>Most common is to create a process engine based on the default configuration file:
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createProcessEngineConfigurationFromResourceDefault()
	///   .buildProcessEngine();
	/// </pre>
	/// </para>
	/// 
	/// <para>To create a process engine programatic, without a configuration file,
	/// the first option is <seealso cref="#createStandaloneProcessEngineConfiguration()"/>
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createStandaloneProcessEngineConfiguration()
	///   .buildProcessEngine();
	/// </pre>
	/// This creates a new process engine with all the defaults to connect to
	/// a remote h2 database (jdbc:h2:tcp://localhost/activiti) in standalone
	/// mode.  Standalone mode means that Activiti will manage the transactions
	/// on the JDBC connections that it creates.  One transaction per
	/// service method.
	/// For a description of how to write the configuration files, see the
	/// userguide.
	/// </para>
	/// 
	/// <para>The second option is great for testing: <seealso cref="#createStandaloneInMemProcessEngineConfiguration()"/>
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createStandaloneInMemProcessEngineConfiguration()
	///   .buildProcessEngine();
	/// </pre>
	/// This creates a new process engine with all the defaults to connect to
	/// an memory h2 database (jdbc:h2:tcp://localhost/activiti) in standalone
	/// mode.  The DB schema strategy default is in this case <code>create-drop</code>.
	/// Standalone mode means that Activiti will manage the transactions
	/// on the JDBC connections that it creates.  One transaction per
	/// service method.
	/// </para>
	/// 
	/// <para>On all forms of creating a process engine, you can first customize the configuration
	/// before calling the <seealso cref="#buildProcessEngine()"/> method by calling any of the
	/// setters like this:
	/// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
	///   .createProcessEngineConfigurationFromResourceDefault()
	///   .setMailServerHost("gmail.com")
	///   .setJdbcUsername("mickey")
	///   .setJdbcPassword("mouse")
	///   .buildProcessEngine();
	/// </pre>
	/// </para>
	/// </summary>
	/// <seealso cref= ProcessEngines
	/// @author Tom Baeyens </seealso>
	public abstract class ProcessEngineConfiguration
	{

	  /// <summary>
	  /// Checks the version of the DB schema against the library when
	  /// the process engine is being created and throws an exception
	  /// if the versions don't match.
	  /// </summary>
	  public const string DB_SCHEMA_UPDATE_FALSE = "false";

	  /// <summary>
	  /// Creates the schema when the process engine is being created and
	  /// drops the schema when the process engine is being closed.
	  /// </summary>
	  public const string DB_SCHEMA_UPDATE_CREATE_DROP = "create-drop";

	  /// <summary>
	  /// Upon building of the process engine, a check is performed and
	  /// an update of the schema is performed if it is necessary.
	  /// </summary>
	  public const string DB_SCHEMA_UPDATE_TRUE = "true";

	  /// <summary>
	  /// Value for <seealso cref="#setHistory(String)"/> to ensure that no history is being recorded.
	  /// </summary>
	  public const string HISTORY_NONE = "none";
	  /// <summary>
	  /// Value for <seealso cref="#setHistory(String)"/> to ensure that only historic process instances and
	  /// historic activity instances are being recorded.
	  /// This means no details for those entities.
	  /// </summary>
	  public const string HISTORY_ACTIVITY = "activity";
	  /// <summary>
	  /// Value for <seealso cref="#setHistory(String)"/> to ensure that only historic process instances,
	  /// historic activity instances and last process variable values are being recorded.
	  /// <para><strong>NOTE:</strong> This history level has been deprecated. Use level <seealso cref="#HISTORY_ACTIVITY"/> instead.</para>
	  /// </summary>
	  [Obsolete]
	  public const string HISTORY_VARIABLE = "variable";
	  /// <summary>
	  /// Value for <seealso cref="#setHistory(String)"/> to ensure that only historic process instances,
	  /// historic activity instances and submitted form property values are being recorded.
	  /// </summary>
	  public const string HISTORY_AUDIT = "audit";
	  /// <summary>
	  /// Value for <seealso cref="#setHistory(String)"/> to ensure that all historic information is
	  /// being recorded, including the variable updates.
	  /// </summary>
	  public const string HISTORY_FULL = "full";

	  /// <summary>
	  /// Value for <seealso cref="#setHistory(String)"/>. Choosing auto causes the configuration to choose the level
	  /// already present on the database. If none can be found, "audit" is taken.
	  /// </summary>
	  public const string HISTORY_AUTO = "auto";

	  /// <summary>
	  /// The default history level that is used when no history level is configured
	  /// </summary>
	  public const string HISTORY_DEFAULT = HISTORY_AUDIT;

	  /// <summary>
	  /// History cleanup is performed based on end time.
	  /// </summary>
	  public const string HISTORY_CLEANUP_STRATEGY_END_TIME_BASED = "endTimeBased";

	  /// <summary>
	  /// History cleanup is performed based on removal time.
	  /// </summary>
	  public const string HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED = "removalTimeBased";

	  /// <summary>
	  /// Removal time for historic entities is set on execution start.
	  /// </summary>
	  public const string HISTORY_REMOVAL_TIME_STRATEGY_START = "start";

	  /// <summary>
	  /// Removal time for historic entities is set if execution has been ended.
	  /// </summary>
	  public const string HISTORY_REMOVAL_TIME_STRATEGY_END = "end";

	  /// <summary>
	  /// Removal time for historic entities is not set.
	  /// </summary>
	  public const string HISTORY_REMOVAL_TIME_STRATEGY_NONE = "none";

	  /// <summary>
	  /// Always enables check for <seealso cref="Authorization#AUTH_TYPE_REVOKE revoke"/> authorizations.
	  /// This mode is equal to the &lt; 7.5 behavior.
	  /// <p />
	  /// *NOTE:* Checking revoke authorizations is very expensive for resources with a high potential
	  /// cardinality like tasks or process instances and can render authorized access to the process engine
	  /// effectively unusable on most databases. You are therefore strongly discouraged from using this mode.
	  /// 
	  /// </summary>
	  public const string AUTHORIZATION_CHECK_REVOKE_ALWAYS = "always";

	  /// <summary>
	  /// Never checks for <seealso cref="Authorization#AUTH_TYPE_REVOKE revoke"/> authorizations. This mode
	  /// has best performance effectively disables the use of <seealso cref="Authorization#AUTH_TYPE_REVOKE"/>.
	  /// *Note*: It is strongly recommended to use this mode.
	  /// </summary>
	  public const string AUTHORIZATION_CHECK_REVOKE_NEVER = "never";

	  /// <summary>
	  /// This mode only checks for <seealso cref="Authorization#AUTH_TYPE_REVOKE revoke"/> authorizations if at least
	  /// one revoke authorization currently exits for the current user or one of the groups the user is a member
	  /// of. To achieve this it is checked once per command whether potentially applicable revoke authorizations
	  /// exist. Based on the outcome, the authorization check then uses revoke or not.
	  /// <p />
	  /// *NOTE:* Checking revoke authorizations is very expensive for resources with a high potential
	  /// cardinality like tasks or process instances and can render authorized access to the process engine
	  /// effectively unusable on most databases.
	  /// </summary>
	  public const string AUTHORIZATION_CHECK_REVOKE_AUTO = "auto";

	  protected internal string processEngineName = ProcessEngines.NAME_DEFAULT;
	  protected internal int idBlockSize = 100;
	  protected internal string history = HISTORY_DEFAULT;
	  protected internal bool jobExecutorActivate;
	  protected internal bool jobExecutorDeploymentAware = false;
	  protected internal bool jobExecutorPreferTimerJobs = false;
	  protected internal bool jobExecutorAcquireByDueDate = false;
	  protected internal bool jobExecutorAcquireByPriority = false;

	  protected internal bool ensureJobDueDateNotNull = false;
	  protected internal bool producePrioritizedJobs = true;
	  protected internal bool producePrioritizedExternalTasks = true;

	  /// <summary>
	  /// The flag will be used inside the method "JobManager#send()". It will be used to decide whether to notify the
	  /// job executor that a new job has been created. It will be used for performance improvement, so that the new job could
	  /// be executed in some situations immediately.
	  /// </summary>
	  protected internal bool hintJobExecutor = true;

	  protected internal string mailServerHost = "localhost";
	  protected internal string mailServerUsername; // by default no name and password are provided, which
	  protected internal string mailServerPassword; // means no authentication for mail server
	  protected internal int mailServerPort = 25;
	  protected internal bool useTLS = false;
	  protected internal string mailServerDefaultFrom = "camunda@localhost";

	  protected internal string databaseType;
	  protected internal string databaseSchemaUpdate = DB_SCHEMA_UPDATE_FALSE;
	  protected internal string jdbcDriver = "org.h2.Driver";
	  protected internal string jdbcUrl = "jdbc:h2:tcp://localhost/activiti";
	  protected internal string jdbcUsername = "sa";
	  protected internal string jdbcPassword = "";
	  protected internal string dataSourceJndiName = null;
	  protected internal int jdbcMaxActiveConnections;
	  protected internal int jdbcMaxIdleConnections;
	  protected internal int jdbcMaxCheckoutTime;
	  protected internal int jdbcMaxWaitTime;
	  protected internal bool jdbcPingEnabled = false;
	  protected internal string jdbcPingQuery = null;
	  protected internal int jdbcPingConnectionNotUsedFor;
	  protected internal DataSource dataSource;
	  protected internal SchemaOperationsCommand schemaOperationsCommand = new SchemaOperationsProcessEngineBuild();
	  protected internal ProcessEngineBootstrapCommand bootstrapCommand = new BootstrapEngineCommand();
	  protected internal HistoryLevelSetupCommand historyLevelCommand = new HistoryLevelSetupCommand();
	  protected internal bool transactionsExternallyManaged = false;
	  /// <summary>
	  /// the number of seconds the jdbc driver will wait for a response from the database </summary>
	  protected internal int? jdbcStatementTimeout;
	  protected internal bool jdbcBatchProcessing = true;

	  protected internal string jpaPersistenceUnitName;
	  protected internal object jpaEntityManagerFactory;
	  protected internal bool jpaHandleTransaction;
	  protected internal bool jpaCloseEntityManager;
	  protected internal int defaultNumberOfRetries = JobEntity.DEFAULT_RETRIES;

	  protected internal ClassLoader classLoader;

	  protected internal bool createIncidentOnFailedJobEnabled = true;

	  /// <summary>
	  /// configuration of password policy
	  /// </summary>
	  protected internal bool enablePasswordPolicy;
	  protected internal PasswordPolicy passwordPolicy;

	  /// <summary>
	  /// switch for controlling whether the process engine performs authorization checks.
	  /// The default value is false.
	  /// </summary>
	  protected internal bool authorizationEnabled = false;

	  /// <summary>
	  /// Provides the default task permission for the user related to a task
	  /// User can be related to a task in the following ways
	  /// - Candidate user
	  /// - Part of candidate group
	  /// - Assignee
	  /// - Owner
	  /// The default value is UPDATE.
	  /// </summary>
	  protected internal string defaultUserPermissionNameForTask = "UPDATE";

	  /// <summary>
	  /// <para>The following flag <code>authorizationEnabledForCustomCode</code> will
	  /// only be taken into account iff <code>authorizationEnabled</code> is set
	  /// <code>true</code>.</para>
	  /// 
	  /// <para>If the value of the flag <code>authorizationEnabledForCustomCode</code>
	  /// is set <code>true</code> then an authorization check will be performed by
	  /// executing commands inside custom code (e.g. inside <seealso cref="JavaDelegate"/>).</para>
	  /// 
	  /// <para>The default value is <code>false</code>.</para>
	  /// 
	  /// </summary>
	  protected internal bool authorizationEnabledForCustomCode = false;

	  /// <summary>
	  /// If the value of this flag is set <code>true</code> then the process engine
	  /// performs tenant checks to ensure that an authenticated user can only access
	  /// data that belongs to one of his tenants.
	  /// </summary>
	  protected internal bool tenantCheckEnabled = true;

	  protected internal ValueTypeResolver valueTypeResolver;

	  protected internal string authorizationCheckRevokes = AUTHORIZATION_CHECK_REVOKE_AUTO;

	  /// <summary>
	  /// A parameter used for defining acceptable values for the User, Group
	  /// and Tenant IDs. The pattern can be defined by using the standard
	  /// Java Regular Expression syntax should be used.
	  /// 
	  /// <para>By default only alphanumeric values (or 'camunda-admin') will be accepted.</para>
	  /// </summary>
	  protected internal string generalResourceWhitelistPattern = "[a-zA-Z0-9]+|camunda-admin";

	  /// <summary>
	  /// A parameter used for defining acceptable values for the User IDs.
	  /// The pattern can be defined by using the standard Java Regular
	  /// Expression syntax should be used.
	  /// 
	  /// <para>If not defined, the general pattern is used. Only alphanumeric
	  /// values (or 'camunda-admin') will be accepted.</para>
	  /// </summary>
	  protected internal string userResourceWhitelistPattern;

	  /// <summary>
	  /// A parameter used for defining acceptable values for the Group IDs.
	  /// The pattern can be defined by using the standard Java Regular
	  /// Expression syntax should be used.
	  /// 
	  /// <para>If not defined, the general pattern is used. Only alphanumeric
	  /// values (or 'camunda-admin') will be accepted.</para>
	  /// </summary>
	  protected internal string groupResourceWhitelistPattern;

	  /// <summary>
	  /// A parameter used for defining acceptable values for the Tenant IDs.
	  /// The pattern can be defined by using the standard Java Regular
	  /// Expression syntax should be used.
	  /// 
	  /// <para>If not defined, the general pattern is used. Only alphanumeric
	  /// values (or 'camunda-admin') will be accepted.</para>
	  /// </summary>
	  protected internal string tenantResourceWhitelistPattern;

	  /// <summary>
	  /// If the value of this flag is set <code>true</code> then the process engine
	  /// throws <seealso cref="ProcessEngineException"/> when no catching boundary event was
	  /// defined for an error event.
	  /// 
	  /// <para>The default value is <code>false</code>.</para>
	  /// </summary>
	  protected internal bool enableExceptionsAfterUnhandledBpmnError = false;

	  /// <summary>
	  /// If the value of this flag is set to <code>false</code>, <seealso cref="OptimisticLockingException"/>s
	  /// are not skipped for UPDATE or DELETE operations applied to historic entities.
	  /// 
	  /// <para>The default value is <code>true</code>.</para>
	  /// </summary>
	  protected internal bool skipHistoryOptimisticLockingExceptions = true;

	  /// <summary>
	  /// If the value of this flag is set to <code>true</code>,
	  /// READ_INSTANCE_VARIABLE,
	  /// READ_HISTORY_VARIABLE, or
	  /// READ_TASK_VARIABLE on Process Definition resource, and
	  /// READ_VARIABLE on Task resource
	  /// will be required to fetch variables when the autorizations are enabled.
	  /// </summary>
	  protected internal bool enforceSpecificVariablePermission = false;

	  /// <summary>
	  /// Specifies which permissions will not be taken into account in the
	  /// authorizations checks if authorization is enabled.
	  /// </summary>
	  protected internal IList<string> disabledPermissions = Collections.emptyList();

	  /// <summary>
	  /// use one of the static createXxxx methods instead </summary>
	  protected internal ProcessEngineConfiguration()
	  {
	  }

	  public abstract ProcessEngine buildProcessEngine();

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromResourceDefault()
	  {
		ProcessEngineConfiguration processEngineConfiguration = null;
		try
		{
		  processEngineConfiguration = createProcessEngineConfigurationFromResource("camunda.cfg.xml", "processEngineConfiguration");
		}
		catch (Exception)
		{
		  processEngineConfiguration = createProcessEngineConfigurationFromResource("activiti.cfg.xml", "processEngineConfiguration");
		}
		return processEngineConfiguration;
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromResource(string resource)
	  {
		return createProcessEngineConfigurationFromResource(resource, "processEngineConfiguration");
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromResource(string resource, string beanName)
	  {
		return BeansConfigurationHelper.parseProcessEngineConfigurationFromResource(resource, beanName);
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromInputStream(Stream inputStream)
	  {
		return createProcessEngineConfigurationFromInputStream(inputStream, "processEngineConfiguration");
	  }

	  public static ProcessEngineConfiguration createProcessEngineConfigurationFromInputStream(Stream inputStream, string beanName)
	  {
		return BeansConfigurationHelper.parseProcessEngineConfigurationFromInputStream(inputStream, beanName);
	  }

	  public static ProcessEngineConfiguration createStandaloneProcessEngineConfiguration()
	  {
		return new StandaloneProcessEngineConfiguration();
	  }

	  public static ProcessEngineConfiguration createStandaloneInMemProcessEngineConfiguration()
	  {
		return new StandaloneInMemProcessEngineConfiguration();
	  }

	// TODO add later when we have test coverage for this
	//  public static ProcessEngineConfiguration createJtaProcessEngineConfiguration() {
	//    return new JtaProcessEngineConfiguration();
	//  }


	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string ProcessEngineName
	  {
		  get
		  {
			return processEngineName;
		  }
	  }

	  public virtual ProcessEngineConfiguration setProcessEngineName(string processEngineName)
	  {
		this.processEngineName = processEngineName;
		return this;
	  }

	  public virtual int IdBlockSize
	  {
		  get
		  {
			return idBlockSize;
		  }
	  }

	  public virtual ProcessEngineConfiguration setIdBlockSize(int idBlockSize)
	  {
		this.idBlockSize = idBlockSize;
		return this;
	  }

	  public virtual string History
	  {
		  get
		  {
			return history;
		  }
	  }

	  public virtual ProcessEngineConfiguration setHistory(string history)
	  {
		this.history = history;
		return this;
	  }

	  public virtual string MailServerHost
	  {
		  get
		  {
			return mailServerHost;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerHost(string mailServerHost)
	  {
		this.mailServerHost = mailServerHost;
		return this;
	  }

	  public virtual string MailServerUsername
	  {
		  get
		  {
			return mailServerUsername;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerUsername(string mailServerUsername)
	  {
		this.mailServerUsername = mailServerUsername;
		return this;
	  }

	  public virtual string MailServerPassword
	  {
		  get
		  {
			return mailServerPassword;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerPassword(string mailServerPassword)
	  {
		this.mailServerPassword = mailServerPassword;
		return this;
	  }

	  public virtual int MailServerPort
	  {
		  get
		  {
			return mailServerPort;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerPort(int mailServerPort)
	  {
		this.mailServerPort = mailServerPort;
		return this;
	  }

	  public virtual bool MailServerUseTLS
	  {
		  get
		  {
			return useTLS;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerUseTLS(bool useTLS)
	  {
		this.useTLS = useTLS;
		return this;
	  }

	  public virtual string MailServerDefaultFrom
	  {
		  get
		  {
			return mailServerDefaultFrom;
		  }
	  }

	  public virtual ProcessEngineConfiguration setMailServerDefaultFrom(string mailServerDefaultFrom)
	  {
		this.mailServerDefaultFrom = mailServerDefaultFrom;
		return this;
	  }

	  public virtual string DatabaseType
	  {
		  get
		  {
			return databaseType;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseType(string databaseType)
	  {
		this.databaseType = databaseType;
		return this;
	  }

	  public virtual string DatabaseSchemaUpdate
	  {
		  get
		  {
			return databaseSchemaUpdate;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDatabaseSchemaUpdate(string databaseSchemaUpdate)
	  {
		this.databaseSchemaUpdate = databaseSchemaUpdate;
		return this;
	  }

	  public virtual DataSource DataSource
	  {
		  get
		  {
			return dataSource;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDataSource(DataSource dataSource)
	  {
		this.dataSource = dataSource;
		return this;
	  }

	  public virtual SchemaOperationsCommand SchemaOperationsCommand
	  {
		  get
		  {
			return schemaOperationsCommand;
		  }
		  set
		  {
			this.schemaOperationsCommand = value;
		  }
	  }


	  public virtual ProcessEngineBootstrapCommand ProcessEngineBootstrapCommand
	  {
		  get
		  {
			return bootstrapCommand;
		  }
		  set
		  {
			this.bootstrapCommand = value;
		  }
	  }


	  public virtual HistoryLevelSetupCommand HistoryLevelCommand
	  {
		  get
		  {
			return historyLevelCommand;
		  }
		  set
		  {
			this.historyLevelCommand = value;
		  }
	  }


	  public virtual string JdbcDriver
	  {
		  get
		  {
			return jdbcDriver;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcDriver(string jdbcDriver)
	  {
		this.jdbcDriver = jdbcDriver;
		return this;
	  }

	  public virtual string JdbcUrl
	  {
		  get
		  {
			return jdbcUrl;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcUrl(string jdbcUrl)
	  {
		this.jdbcUrl = jdbcUrl;
		return this;
	  }

	  public virtual string JdbcUsername
	  {
		  get
		  {
			return jdbcUsername;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcUsername(string jdbcUsername)
	  {
		this.jdbcUsername = jdbcUsername;
		return this;
	  }

	  public virtual string JdbcPassword
	  {
		  get
		  {
			return jdbcPassword;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPassword(string jdbcPassword)
	  {
		this.jdbcPassword = jdbcPassword;
		return this;
	  }

	  public virtual bool TransactionsExternallyManaged
	  {
		  get
		  {
			return transactionsExternallyManaged;
		  }
	  }

	  public virtual ProcessEngineConfiguration setTransactionsExternallyManaged(bool transactionsExternallyManaged)
	  {
		this.transactionsExternallyManaged = transactionsExternallyManaged;
		return this;
	  }

	  public virtual int JdbcMaxActiveConnections
	  {
		  get
		  {
			return jdbcMaxActiveConnections;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxActiveConnections(int jdbcMaxActiveConnections)
	  {
		this.jdbcMaxActiveConnections = jdbcMaxActiveConnections;
		return this;
	  }

	  public virtual int JdbcMaxIdleConnections
	  {
		  get
		  {
			return jdbcMaxIdleConnections;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxIdleConnections(int jdbcMaxIdleConnections)
	  {
		this.jdbcMaxIdleConnections = jdbcMaxIdleConnections;
		return this;
	  }

	  public virtual int JdbcMaxCheckoutTime
	  {
		  get
		  {
			return jdbcMaxCheckoutTime;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxCheckoutTime(int jdbcMaxCheckoutTime)
	  {
		this.jdbcMaxCheckoutTime = jdbcMaxCheckoutTime;
		return this;
	  }

	  public virtual int JdbcMaxWaitTime
	  {
		  get
		  {
			return jdbcMaxWaitTime;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcMaxWaitTime(int jdbcMaxWaitTime)
	  {
		this.jdbcMaxWaitTime = jdbcMaxWaitTime;
		return this;
	  }

	  public virtual bool JdbcPingEnabled
	  {
		  get
		  {
			return jdbcPingEnabled;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPingEnabled(bool jdbcPingEnabled)
	  {
		this.jdbcPingEnabled = jdbcPingEnabled;
		return this;
	  }

	  public virtual string JdbcPingQuery
	  {
		  get
		  {
			return jdbcPingQuery;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPingQuery(string jdbcPingQuery)
	  {
		this.jdbcPingQuery = jdbcPingQuery;
		return this;
	  }

	  public virtual int JdbcPingConnectionNotUsedFor
	  {
		  get
		  {
			return jdbcPingConnectionNotUsedFor;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcPingConnectionNotUsedFor(int jdbcPingNotUsedFor)
	  {
		this.jdbcPingConnectionNotUsedFor = jdbcPingNotUsedFor;
		return this;
	  }

	  /// <summary>
	  /// Gets the number of seconds the jdbc driver will wait for a response from the database. </summary>
	  public virtual int? JdbcStatementTimeout
	  {
		  get
		  {
			return jdbcStatementTimeout;
		  }
	  }

	  /// <summary>
	  /// Sets the number of seconds the jdbc driver will wait for a response from the database. </summary>
	  public virtual ProcessEngineConfiguration setJdbcStatementTimeout(int? jdbcStatementTimeout)
	  {
		this.jdbcStatementTimeout = jdbcStatementTimeout;
		return this;
	  }

	  public virtual bool JdbcBatchProcessing
	  {
		  get
		  {
			return jdbcBatchProcessing;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJdbcBatchProcessing(bool jdbcBatchProcessing)
	  {
		this.jdbcBatchProcessing = jdbcBatchProcessing;
		return this;
	  }

	  public virtual bool JobExecutorActivate
	  {
		  get
		  {
			return jobExecutorActivate;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJobExecutorActivate(bool jobExecutorActivate)
	  {
		this.jobExecutorActivate = jobExecutorActivate;
		return this;
	  }

	  public virtual bool JobExecutorDeploymentAware
	  {
		  get
		  {
			return jobExecutorDeploymentAware;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJobExecutorDeploymentAware(bool jobExecutorDeploymentAware)
	  {
		this.jobExecutorDeploymentAware = jobExecutorDeploymentAware;
		return this;
	  }

	  public virtual bool JobExecutorAcquireByDueDate
	  {
		  get
		  {
			return jobExecutorAcquireByDueDate;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJobExecutorAcquireByDueDate(bool jobExecutorAcquireByDueDate)
	  {
		this.jobExecutorAcquireByDueDate = jobExecutorAcquireByDueDate;
		return this;
	  }

	  public virtual bool JobExecutorPreferTimerJobs
	  {
		  get
		  {
			return jobExecutorPreferTimerJobs;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJobExecutorPreferTimerJobs(bool jobExecutorPreferTimerJobs)
	  {
		this.jobExecutorPreferTimerJobs = jobExecutorPreferTimerJobs;
		return this;
	  }

	  public virtual bool HintJobExecutor
	  {
		  get
		  {
			return hintJobExecutor;
		  }
	  }

	  public virtual ProcessEngineConfiguration setHintJobExecutor(bool hintJobExecutor)
	  {
		this.hintJobExecutor = hintJobExecutor;
		return this;
	  }

	  public virtual ClassLoader ClassLoader
	  {
		  get
		  {
			return classLoader;
		  }
	  }

	  public virtual ProcessEngineConfiguration setClassLoader(ClassLoader classLoader)
	  {
		this.classLoader = classLoader;
		return this;
	  }

	  public virtual object JpaEntityManagerFactory
	  {
		  get
		  {
			return jpaEntityManagerFactory;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJpaEntityManagerFactory(object jpaEntityManagerFactory)
	  {
		this.jpaEntityManagerFactory = jpaEntityManagerFactory;
		return this;
	  }

	  public virtual bool JpaHandleTransaction
	  {
		  get
		  {
			return jpaHandleTransaction;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJpaHandleTransaction(bool jpaHandleTransaction)
	  {
		this.jpaHandleTransaction = jpaHandleTransaction;
		return this;
	  }

	  public virtual bool JpaCloseEntityManager
	  {
		  get
		  {
			return jpaCloseEntityManager;
		  }
	  }

	  public virtual ProcessEngineConfiguration setJpaCloseEntityManager(bool jpaCloseEntityManager)
	  {
		this.jpaCloseEntityManager = jpaCloseEntityManager;
		return this;
	  }

	  public virtual string JpaPersistenceUnitName
	  {
		  get
		  {
			return jpaPersistenceUnitName;
		  }
		  set
		  {
			this.jpaPersistenceUnitName = value;
		  }
	  }


	  public virtual string DataSourceJndiName
	  {
		  get
		  {
			return dataSourceJndiName;
		  }
		  set
		  {
			this.dataSourceJndiName = value;
		  }
	  }


	  public virtual bool CreateIncidentOnFailedJobEnabled
	  {
		  get
		  {
			return createIncidentOnFailedJobEnabled;
		  }
	  }

	  public virtual ProcessEngineConfiguration setCreateIncidentOnFailedJobEnabled(bool createIncidentOnFailedJobEnabled)
	  {
		this.createIncidentOnFailedJobEnabled = createIncidentOnFailedJobEnabled;
		return this;
	  }

	  public virtual bool AuthorizationEnabled
	  {
		  get
		  {
			return authorizationEnabled;
		  }
	  }

	  public virtual ProcessEngineConfiguration setAuthorizationEnabled(bool isAuthorizationChecksEnabled)
	  {
		this.authorizationEnabled = isAuthorizationChecksEnabled;
		return this;
	  }

	  public virtual string DefaultUserPermissionNameForTask
	  {
		  get
		  {
			return defaultUserPermissionNameForTask;
		  }
	  }

	  public virtual ProcessEngineConfiguration setDefaultUserPermissionNameForTask(string defaultUserPermissionNameForTask)
	  {
		this.defaultUserPermissionNameForTask = defaultUserPermissionNameForTask;
		return this;
	  }

	  public virtual bool AuthorizationEnabledForCustomCode
	  {
		  get
		  {
			return authorizationEnabledForCustomCode;
		  }
	  }

	  public virtual ProcessEngineConfiguration setAuthorizationEnabledForCustomCode(bool authorizationEnabledForCustomCode)
	  {
		this.authorizationEnabledForCustomCode = authorizationEnabledForCustomCode;
		return this;
	  }

	  public virtual bool TenantCheckEnabled
	  {
		  get
		  {
			return tenantCheckEnabled;
		  }
	  }

	  public virtual ProcessEngineConfiguration setTenantCheckEnabled(bool isTenantCheckEnabled)
	  {
		this.tenantCheckEnabled = isTenantCheckEnabled;
		return this;
	  }

	  public virtual string GeneralResourceWhitelistPattern
	  {
		  get
		  {
			return generalResourceWhitelistPattern;
		  }
		  set
		  {
			this.generalResourceWhitelistPattern = value;
		  }
	  }


	  public virtual string UserResourceWhitelistPattern
	  {
		  get
		  {
			return userResourceWhitelistPattern;
		  }
		  set
		  {
			this.userResourceWhitelistPattern = value;
		  }
	  }


	  public virtual string GroupResourceWhitelistPattern
	  {
		  get
		  {
			return groupResourceWhitelistPattern;
		  }
		  set
		  {
			this.groupResourceWhitelistPattern = value;
		  }
	  }


	  public virtual string TenantResourceWhitelistPattern
	  {
		  get
		  {
			return tenantResourceWhitelistPattern;
		  }
		  set
		  {
			this.tenantResourceWhitelistPattern = value;
		  }
	  }


	  public virtual int DefaultNumberOfRetries
	  {
		  get
		  {
			return defaultNumberOfRetries;
		  }
		  set
		  {
			this.defaultNumberOfRetries = value;
		  }
	  }


	  public virtual ValueTypeResolver ValueTypeResolver
	  {
		  get
		  {
			return valueTypeResolver;
		  }
	  }

	  public virtual ProcessEngineConfiguration setValueTypeResolver(ValueTypeResolver valueTypeResolver)
	  {
		this.valueTypeResolver = valueTypeResolver;
		return this;
	  }

	  public virtual bool EnsureJobDueDateNotNull
	  {
		  get
		  {
			return ensureJobDueDateNotNull;
		  }
		  set
		  {
			this.ensureJobDueDateNotNull = value;
		  }
	  }


	  public virtual bool ProducePrioritizedJobs
	  {
		  get
		  {
			return producePrioritizedJobs;
		  }
		  set
		  {
			this.producePrioritizedJobs = value;
		  }
	  }


	  public virtual bool JobExecutorAcquireByPriority
	  {
		  get
		  {
			return jobExecutorAcquireByPriority;
		  }
		  set
		  {
			this.jobExecutorAcquireByPriority = value;
		  }
	  }


	  public virtual bool ProducePrioritizedExternalTasks
	  {
		  get
		  {
			return producePrioritizedExternalTasks;
		  }
		  set
		  {
			this.producePrioritizedExternalTasks = value;
		  }
	  }


	  public virtual string AuthorizationCheckRevokes
	  {
		  set
		  {
			this.authorizationCheckRevokes = value;
		  }
		  get
		  {
			return authorizationCheckRevokes;
		  }
	  }


	  public virtual bool EnableExceptionsAfterUnhandledBpmnError
	  {
		  get
		  {
			return enableExceptionsAfterUnhandledBpmnError;
		  }
		  set
		  {
			this.enableExceptionsAfterUnhandledBpmnError = value;
		  }
	  }


	  public virtual bool SkipHistoryOptimisticLockingExceptions
	  {
		  get
		  {
			return skipHistoryOptimisticLockingExceptions;
		  }
	  }

	  public virtual ProcessEngineConfiguration setSkipHistoryOptimisticLockingExceptions(bool skipHistoryOptimisticLockingExceptions)
	  {
		this.skipHistoryOptimisticLockingExceptions = skipHistoryOptimisticLockingExceptions;
		return this;
	  }

	  public virtual bool EnforceSpecificVariablePermission
	  {
		  get
		  {
			return enforceSpecificVariablePermission;
		  }
		  set
		  {
			this.enforceSpecificVariablePermission = value;
		  }
	  }


	  public virtual IList<string> DisabledPermissions
	  {
		  get
		  {
			return disabledPermissions;
		  }
		  set
		  {
			this.disabledPermissions = value;
		  }
	  }


	  public virtual bool EnablePasswordPolicy
	  {
		  get
		  {
			return enablePasswordPolicy;
		  }
	  }

	  public virtual ProcessEngineConfiguration setEnablePasswordPolicy(bool enablePasswordPolicy)
	  {
		this.enablePasswordPolicy = enablePasswordPolicy;
		return this;
	  }

	  public virtual PasswordPolicy PasswordPolicy
	  {
		  get
		  {
			return passwordPolicy;
		  }
	  }

	  public virtual ProcessEngineConfiguration setPasswordPolicy(PasswordPolicy passwordPolicy)
	  {
		this.passwordPolicy = passwordPolicy;
		return this;
	  }
	}
}