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
namespace org.camunda.bpm.engine.impl.interceptor
{
	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using org.camunda.bpm.engine;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TransactionContext = org.camunda.bpm.engine.impl.cfg.TransactionContext;
	using TransactionContextFactory = org.camunda.bpm.engine.impl.cfg.TransactionContextFactory;
	using CaseDefinitionManager = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionManager;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CaseExecutionManager = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionManager;
	using CaseSentryPartManager = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseSentryPartManager;
	using CmmnAtomicOperation = org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DbSqlSession = org.camunda.bpm.engine.impl.db.sql.DbSqlSession;
	using DecisionDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionManager;
	using DecisionRequirementsDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionManager;
	using HistoricDecisionInstanceManager = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceManager;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using ReadOnlyIdentityProvider = org.camunda.bpm.engine.impl.identity.ReadOnlyIdentityProvider;
	using WritableIdentityProvider = org.camunda.bpm.engine.impl.identity.WritableIdentityProvider;
	using FailedJobCommandFactory = org.camunda.bpm.engine.impl.jobexecutor.FailedJobCommandFactory;
	using OptimizeManager = org.camunda.bpm.engine.impl.optimize.OptimizeManager;
	using org.camunda.bpm.engine.impl.persistence.entity;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Agim Emruli
	/// @author Daniel Meyer
	/// </summary>
	public class CommandContext
	{

	  private static readonly ContextLogger LOG = ProcessEngineLogger.CONTEXT_LOGGER;

	  protected internal bool authorizationCheckEnabled = true;
	  protected internal bool userOperationLogEnabled = true;
	  protected internal bool tenantCheckEnabled = true;
	  protected internal bool restrictUserOperationLogToAuthenticatedUsers;

	  protected internal TransactionContext transactionContext;
	  protected internal IDictionary<Type, SessionFactory> sessionFactories;
	  protected internal IDictionary<Type, Session> sessions = new Dictionary<Type, Session>();
	  protected internal IList<Session> sessionList = new List<Session>();
	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal FailedJobCommandFactory failedJobCommandFactory;

	  protected internal JobEntity currentJob = null;

	  protected internal IList<CommandContextListener> commandContextListeners = new LinkedList<CommandContextListener>();

	  protected internal string operationId;

	  public CommandContext(ProcessEngineConfigurationImpl processEngineConfiguration) : this(processEngineConfiguration, processEngineConfiguration.TransactionContextFactory)
	  {
	  }

	  public CommandContext(ProcessEngineConfigurationImpl processEngineConfiguration, TransactionContextFactory transactionContextFactory)
	  {
		this.processEngineConfiguration = processEngineConfiguration;
		this.failedJobCommandFactory = processEngineConfiguration.FailedJobCommandFactory;
		sessionFactories = processEngineConfiguration.SessionFactories;
		this.transactionContext = transactionContextFactory.openTransactionContext(this);
		this.restrictUserOperationLogToAuthenticatedUsers = processEngineConfiguration.RestrictUserOperationLogToAuthenticatedUsers;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void performOperation(final org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation executionOperation, final org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity execution)
	  public virtual void performOperation(CmmnAtomicOperation executionOperation, CaseExecutionEntity execution)
	  {
		ProcessApplicationReference targetProcessApplication = getTargetProcessApplication(execution);

		if (requiresContextSwitch(targetProcessApplication))
		{
		  Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, executionOperation, execution)
		 , targetProcessApplication, new InvocationContext(execution));

		}
		else
		{
		  try
		  {
			Context.ExecutionContext = execution;
			LOG.debugExecutingAtomicOperation(executionOperation, execution);

			executionOperation.execute(execution);
		  }
		  finally
		  {
			Context.removeExecutionContext();
		  }
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly CommandContext outerInstance;

		  private CmmnAtomicOperation executionOperation;
		  private CaseExecutionEntity execution;

		  public CallableAnonymousInnerClass(CommandContext outerInstance, CmmnAtomicOperation executionOperation, CaseExecutionEntity execution)
		  {
			  this.outerInstance = outerInstance;
			  this.executionOperation = executionOperation;
			  this.execution = execution;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.performOperation(executionOperation, execution);
			return null;
		  }

	  }

	  public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
	  {
		  get
		  {
			return processEngineConfiguration;
		  }
	  }

	  protected internal virtual ProcessApplicationReference getTargetProcessApplication(CaseExecutionEntity execution)
	  {
		return ProcessApplicationContextUtil.getTargetProcessApplication(execution);
	  }

	  protected internal virtual bool requiresContextSwitch(ProcessApplicationReference processApplicationReference)
	  {
		return ProcessApplicationContextUtil.requiresContextSwitch(processApplicationReference);
	  }

	  public virtual void close(CommandInvocationContext commandInvocationContext)
	  {
		// the intention of this method is that all resources are closed properly,
		// even
		// if exceptions occur in close or flush methods of the sessions or the
		// transaction context.

		try
		{
		  try
		  {
			try
			{

			  if (commandInvocationContext.Throwable == null)
			  {
				fireCommandContextClose();
				flushSessions();
			  }

			}
			catch (Exception exception)
			{
			  commandInvocationContext.trySetThrowable(exception);
			}
			finally
			{

			  try
			  {
				if (commandInvocationContext.Throwable == null)
				{
				  transactionContext.commit();
				}
			  }
			  catch (Exception exception)
			  {
				commandInvocationContext.trySetThrowable(exception);
			  }

			  if (commandInvocationContext.Throwable != null)
			  {
				// fire command failed (must not fail itself)
				fireCommandFailed(commandInvocationContext.Throwable);

				if (shouldLogInfo(commandInvocationContext.Throwable))
				{
				  LOG.infoException(commandInvocationContext.Throwable);
				}
				else if (shouldLogFine(commandInvocationContext.Throwable))
				{
				  LOG.debugException(commandInvocationContext.Throwable);
				}
				else
				{
				  LOG.errorException(commandInvocationContext.Throwable);
				}
				transactionContext.rollback();
			  }
			}
		  }
		  catch (Exception exception)
		  {
			commandInvocationContext.trySetThrowable(exception);
		  }
		  finally
		  {
			closeSessions(commandInvocationContext);
		  }
		}
		catch (Exception exception)
		{
		  commandInvocationContext.trySetThrowable(exception);
		}

		// rethrow the original exception if there was one
		commandInvocationContext.rethrow();
	  }

	  protected internal virtual bool shouldLogInfo(Exception exception)
	  {
		return exception is TaskAlreadyClaimedException;
	  }

	  protected internal virtual bool shouldLogFine(Exception exception)
	  {
		return exception is OptimisticLockingException || exception is BadUserRequestException;
	  }

	  protected internal virtual void fireCommandContextClose()
	  {
		foreach (CommandContextListener listener in commandContextListeners)
		{
		  listener.onCommandContextClose(this);
		}
	  }

	  protected internal virtual void fireCommandFailed(Exception t)
	  {
		foreach (CommandContextListener listener in commandContextListeners)
		{
		  try
		  {
			listener.onCommandFailed(this, t);
		  }
		  catch (Exception)
		  {
			LOG.exceptionWhileInvokingOnCommandFailed(t);
		  }
		}
	  }

	  protected internal virtual void flushSessions()
	  {
		for (int i = 0; i < sessionList.Count; i++)
		{
		  sessionList[i].flush();
		}
	  }

	  protected internal virtual void closeSessions(CommandInvocationContext commandInvocationContext)
	  {
		foreach (Session session in sessionList)
		{
		  try
		  {
			session.close();
		  }
		  catch (Exception exception)
		  {
			commandInvocationContext.trySetThrowable(exception);
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked"}) public <T> T getSession(Class<T> sessionClass)
	  public virtual T getSession<T>(Type<T> sessionClass)
	  {
		Session session = sessions[sessionClass];
		if (session == null)
		{
		  SessionFactory sessionFactory = sessionFactories[sessionClass];
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  ensureNotNull("no session factory configured for " + sessionClass.FullName, "sessionFactory", sessionFactory);
		  session = sessionFactory.openSession();
		  sessions[sessionClass] = session;
		  sessionList.Insert(0, session);
		}

		return (T) session;
	  }

	  public virtual DbEntityManager DbEntityManager
	  {
		  get
		  {
			return getSession(typeof(DbEntityManager));
		  }
	  }

	  public virtual DbSqlSession DbSqlSession
	  {
		  get
		  {
			return getSession(typeof(DbSqlSession));
		  }
	  }

	  public virtual DeploymentManager DeploymentManager
	  {
		  get
		  {
			return getSession(typeof(DeploymentManager));
		  }
	  }

	  public virtual ResourceManager ResourceManager
	  {
		  get
		  {
			return getSession(typeof(ResourceManager));
		  }
	  }

	  public virtual ByteArrayManager ByteArrayManager
	  {
		  get
		  {
			return getSession(typeof(ByteArrayManager));
		  }
	  }

	  public virtual ProcessDefinitionManager ProcessDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(ProcessDefinitionManager));
		  }
	  }

	  public virtual ExecutionManager ExecutionManager
	  {
		  get
		  {
			return getSession(typeof(ExecutionManager));
		  }
	  }

	  public virtual TaskManager TaskManager
	  {
		  get
		  {
			return getSession(typeof(TaskManager));
		  }
	  }

	  public virtual TaskReportManager TaskReportManager
	  {
		  get
		  {
			return getSession(typeof(TaskReportManager));
		  }
	  }

	  public virtual MeterLogManager MeterLogManager
	  {
		  get
		  {
			return getSession(typeof(MeterLogManager));
		  }
	  }

	  public virtual IdentityLinkManager IdentityLinkManager
	  {
		  get
		  {
			return getSession(typeof(IdentityLinkManager));
		  }
	  }

	  public virtual VariableInstanceManager VariableInstanceManager
	  {
		  get
		  {
			return getSession(typeof(VariableInstanceManager));
		  }
	  }

	  public virtual HistoricProcessInstanceManager HistoricProcessInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricProcessInstanceManager));
		  }
	  }

	  public virtual HistoricCaseInstanceManager HistoricCaseInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricCaseInstanceManager));
		  }
	  }

	  public virtual HistoricDetailManager HistoricDetailManager
	  {
		  get
		  {
			return getSession(typeof(HistoricDetailManager));
		  }
	  }

	  public virtual UserOperationLogManager OperationLogManager
	  {
		  get
		  {
			return getSession(typeof(UserOperationLogManager));
		  }
	  }

	  public virtual HistoricVariableInstanceManager HistoricVariableInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricVariableInstanceManager));
		  }
	  }

	  public virtual HistoricActivityInstanceManager HistoricActivityInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricActivityInstanceManager));
		  }
	  }

	  public virtual HistoricCaseActivityInstanceManager HistoricCaseActivityInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricCaseActivityInstanceManager));
		  }
	  }

	  public virtual HistoricTaskInstanceManager HistoricTaskInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricTaskInstanceManager));
		  }
	  }

	  public virtual HistoricIncidentManager HistoricIncidentManager
	  {
		  get
		  {
			return getSession(typeof(HistoricIncidentManager));
		  }
	  }

	  public virtual HistoricIdentityLinkLogManager HistoricIdentityLinkManager
	  {
		  get
		  {
			return getSession(typeof(HistoricIdentityLinkLogManager));
		  }
	  }

	  public virtual JobManager JobManager
	  {
		  get
		  {
			return getSession(typeof(JobManager));
		  }
	  }

	  public virtual BatchManager BatchManager
	  {
		  get
		  {
			return getSession(typeof(BatchManager));
		  }
	  }

	  public virtual HistoricBatchManager HistoricBatchManager
	  {
		  get
		  {
			return getSession(typeof(HistoricBatchManager));
		  }
	  }

	  public virtual JobDefinitionManager JobDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(JobDefinitionManager));
		  }
	  }

	  public virtual IncidentManager IncidentManager
	  {
		  get
		  {
			return getSession(typeof(IncidentManager));
		  }
	  }

	  public virtual IdentityInfoManager IdentityInfoManager
	  {
		  get
		  {
			return getSession(typeof(IdentityInfoManager));
		  }
	  }

	  public virtual AttachmentManager AttachmentManager
	  {
		  get
		  {
			return getSession(typeof(AttachmentManager));
		  }
	  }

	  public virtual TableDataManager TableDataManager
	  {
		  get
		  {
			return getSession(typeof(TableDataManager));
		  }
	  }

	  public virtual CommentManager CommentManager
	  {
		  get
		  {
			return getSession(typeof(CommentManager));
		  }
	  }

	  public virtual EventSubscriptionManager EventSubscriptionManager
	  {
		  get
		  {
			return getSession(typeof(EventSubscriptionManager));
		  }
	  }

	  public virtual IDictionary<Type, SessionFactory> SessionFactories
	  {
		  get
		  {
			return sessionFactories;
		  }
	  }

	  public virtual PropertyManager PropertyManager
	  {
		  get
		  {
			return getSession(typeof(PropertyManager));
		  }
	  }

	  public virtual StatisticsManager StatisticsManager
	  {
		  get
		  {
			return getSession(typeof(StatisticsManager));
		  }
	  }

	  public virtual HistoricStatisticsManager HistoricStatisticsManager
	  {
		  get
		  {
			return getSession(typeof(HistoricStatisticsManager));
		  }
	  }

	  public virtual HistoricJobLogManager HistoricJobLogManager
	  {
		  get
		  {
			return getSession(typeof(HistoricJobLogManager));
		  }
	  }

	  public virtual HistoricExternalTaskLogManager HistoricExternalTaskLogManager
	  {
		  get
		  {
			return getSession(typeof(HistoricExternalTaskLogManager));
		  }
	  }

	  public virtual ReportManager HistoricReportManager
	  {
		  get
		  {
			return getSession(typeof(ReportManager));
		  }
	  }

	  public virtual AuthorizationManager AuthorizationManager
	  {
		  get
		  {
			return getSession(typeof(AuthorizationManager));
		  }
	  }

	  public virtual ReadOnlyIdentityProvider ReadOnlyIdentityProvider
	  {
		  get
		  {
			return getSession(typeof(ReadOnlyIdentityProvider));
		  }
	  }

	  public virtual WritableIdentityProvider WritableIdentityProvider
	  {
		  get
		  {
			return getSession(typeof(WritableIdentityProvider));
		  }
	  }

	  public virtual TenantManager TenantManager
	  {
		  get
		  {
			return getSession(typeof(TenantManager));
		  }
	  }

	  public virtual SchemaLogManager SchemaLogManager
	  {
		  get
		  {
			return getSession(typeof(SchemaLogManager));
		  }
	  }

	  // CMMN /////////////////////////////////////////////////////////////////////

	  public virtual CaseDefinitionManager CaseDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(CaseDefinitionManager));
		  }
	  }

	  public virtual CaseExecutionManager CaseExecutionManager
	  {
		  get
		  {
			return getSession(typeof(CaseExecutionManager));
		  }
	  }

	  public virtual CaseSentryPartManager CaseSentryPartManager
	  {
		  get
		  {
			return getSession(typeof(CaseSentryPartManager));
		  }
	  }

	  // DMN //////////////////////////////////////////////////////////////////////

	  public virtual DecisionDefinitionManager DecisionDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(DecisionDefinitionManager));
		  }
	  }

	  public virtual DecisionRequirementsDefinitionManager DecisionRequirementsDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(DecisionRequirementsDefinitionManager));
		  }
	  }

	  public virtual HistoricDecisionInstanceManager HistoricDecisionInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricDecisionInstanceManager));
		  }
	  }

	  // Filter ////////////////////////////////////////////////////////////////////

	  public virtual FilterManager FilterManager
	  {
		  get
		  {
			return getSession(typeof(FilterManager));
		  }
	  }

	  // External Tasks ////////////////////////////////////////////////////////////

	  public virtual ExternalTaskManager ExternalTaskManager
	  {
		  get
		  {
			return getSession(typeof(ExternalTaskManager));
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual void registerCommandContextListener(CommandContextListener commandContextListener)
	  {
		if (!commandContextListeners.Contains(commandContextListener))
		{
		  commandContextListeners.Add(commandContextListener);
		}
	  }

	  public virtual TransactionContext TransactionContext
	  {
		  get
		  {
			return transactionContext;
		  }
	  }

	  public virtual IDictionary<Type, Session> Sessions
	  {
		  get
		  {
			return sessions;
		  }
	  }

	  public virtual FailedJobCommandFactory FailedJobCommandFactory
	  {
		  get
		  {
			return failedJobCommandFactory;
		  }
	  }

	  public virtual Authentication Authentication
	  {
		  get
		  {
			IdentityService identityService = processEngineConfiguration.IdentityService;
			return identityService.CurrentAuthentication;
		  }
	  }

	  public virtual T runWithoutAuthorization<T>(Callable<T> runnable)
	  {
		CommandContext commandContext = Context.CommandContext;
		bool authorizationEnabled = commandContext.AuthorizationCheckEnabled;
		try
		{
		  commandContext.disableAuthorizationCheck();
		  return runnable.call();
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException(e);
		}
		finally
		{
		  if (authorizationEnabled)
		  {
			commandContext.enableAuthorizationCheck();
		  }
		}
	  }

	  public virtual string AuthenticatedUserId
	  {
		  get
		  {
			IdentityService identityService = processEngineConfiguration.IdentityService;
			Authentication currentAuthentication = identityService.CurrentAuthentication;
			if (currentAuthentication == null)
			{
			  return null;
			}
			else
			{
			  return currentAuthentication.UserId;
			}
		  }
	  }

	  public virtual IList<string> AuthenticatedGroupIds
	  {
		  get
		  {
			IdentityService identityService = processEngineConfiguration.IdentityService;
			Authentication currentAuthentication = identityService.CurrentAuthentication;
			if (currentAuthentication == null)
			{
			  return null;
			}
			else
			{
			  return currentAuthentication.GroupIds;
			}
		  }
	  }

	  public virtual void enableAuthorizationCheck()
	  {
		authorizationCheckEnabled = true;
	  }

	  public virtual void disableAuthorizationCheck()
	  {
		authorizationCheckEnabled = false;
	  }

	  public virtual bool AuthorizationCheckEnabled
	  {
		  get
		  {
			return authorizationCheckEnabled;
		  }
		  set
		  {
			this.authorizationCheckEnabled = value;
		  }
	  }


	  public virtual void enableUserOperationLog()
	  {
		userOperationLogEnabled = true;
	  }

	  public virtual void disableUserOperationLog()
	  {
		userOperationLogEnabled = false;
	  }

	  public virtual bool UserOperationLogEnabled
	  {
		  get
		  {
			return userOperationLogEnabled;
		  }
	  }

	  public virtual bool LogUserOperationEnabled
	  {
		  set
		  {
			this.userOperationLogEnabled = value;
		  }
	  }

	  public virtual void enableTenantCheck()
	  {
		tenantCheckEnabled = true;
	  }

	  public virtual void disableTenantCheck()
	  {
		tenantCheckEnabled = false;
	  }

	  public virtual bool TenantCheckEnabled
	  {
		  set
		  {
			this.tenantCheckEnabled = value;
		  }
		  get
		  {
			return tenantCheckEnabled;
		  }
	  }


	  public virtual JobEntity CurrentJob
	  {
		  get
		  {
			return currentJob;
		  }
		  set
		  {
			this.currentJob = value;
		  }
	  }


	  public virtual bool RestrictUserOperationLogToAuthenticatedUsers
	  {
		  get
		  {
			return restrictUserOperationLogToAuthenticatedUsers;
		  }
		  set
		  {
			this.restrictUserOperationLogToAuthenticatedUsers = value;
		  }
	  }


	  public virtual string OperationId
	  {
		  get
		  {
			if (!OperationLogManager.UserOperationLogEnabled)
			{
			  return null;
			}
			if (string.ReferenceEquals(operationId, null))
			{
			  operationId = Context.ProcessEngineConfiguration.IdGenerator.NextId;
			}
			return operationId;
		  }
		  set
		  {
			this.operationId = value;
		  }
	  }


	  public virtual OptimizeManager OptimizeManager
	  {
		  get
		  {
			return getSession(typeof(OptimizeManager));
		  }
	  }
	}

}