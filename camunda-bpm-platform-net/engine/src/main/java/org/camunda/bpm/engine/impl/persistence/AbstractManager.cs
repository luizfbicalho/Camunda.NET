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
namespace org.camunda.bpm.engine.impl.persistence
{

	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using ResourceAuthorizationProvider = org.camunda.bpm.engine.impl.cfg.auth.ResourceAuthorizationProvider;
	using CaseDefinitionManager = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionManager;
	using CaseExecutionManager = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionManager;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DbSqlSession = org.camunda.bpm.engine.impl.db.sql.DbSqlSession;
	using DecisionDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionDefinitionManager;
	using DecisionRequirementsDefinitionManager = org.camunda.bpm.engine.impl.dmn.entity.repository.DecisionRequirementsDefinitionManager;
	using HistoricDecisionInstanceManager = org.camunda.bpm.engine.impl.history.@event.HistoricDecisionInstanceManager;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using AttachmentManager = org.camunda.bpm.engine.impl.persistence.entity.AttachmentManager;
	using AuthorizationEntity = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using BatchManager = org.camunda.bpm.engine.impl.persistence.entity.BatchManager;
	using ByteArrayManager = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayManager;
	using DeploymentManager = org.camunda.bpm.engine.impl.persistence.entity.DeploymentManager;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;
	using ExecutionManager = org.camunda.bpm.engine.impl.persistence.entity.ExecutionManager;
	using HistoricActivityInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricActivityInstanceManager;
	using HistoricBatchManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricBatchManager;
	using HistoricCaseActivityInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseActivityInstanceManager;
	using HistoricCaseInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricCaseInstanceManager;
	using HistoricDetailManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricDetailManager;
	using HistoricExternalTaskLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricExternalTaskLogManager;
	using HistoricIdentityLinkLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricIdentityLinkLogManager;
	using HistoricIncidentManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentManager;
	using HistoricJobLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogManager;
	using HistoricProcessInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricProcessInstanceManager;
	using ReportManager = org.camunda.bpm.engine.impl.persistence.entity.ReportManager;
	using HistoricTaskInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricTaskInstanceManager;
	using HistoricVariableInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceManager;
	using IdentityInfoManager = org.camunda.bpm.engine.impl.persistence.entity.IdentityInfoManager;
	using IdentityLinkManager = org.camunda.bpm.engine.impl.persistence.entity.IdentityLinkManager;
	using JobDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using ResourceManager = org.camunda.bpm.engine.impl.persistence.entity.ResourceManager;
	using TaskManager = org.camunda.bpm.engine.impl.persistence.entity.TaskManager;
	using TaskReportManager = org.camunda.bpm.engine.impl.persistence.entity.TaskReportManager;
	using TenantManager = org.camunda.bpm.engine.impl.persistence.entity.TenantManager;
	using UserOperationLogManager = org.camunda.bpm.engine.impl.persistence.entity.UserOperationLogManager;
	using VariableInstanceManager = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceManager;



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class AbstractManager : Session
	{

	  public virtual void insert(DbEntity dbEntity)
	  {
		DbEntityManager.insert(dbEntity);
	  }

	  public virtual void delete(DbEntity dbEntity)
	  {
		DbEntityManager.delete(dbEntity);
	  }

	  protected internal virtual DbEntityManager DbEntityManager
	  {
		  get
		  {
			return getSession(typeof(DbEntityManager));
		  }
	  }

	  protected internal virtual DbSqlSession DbSqlSession
	  {
		  get
		  {
			return getSession(typeof(DbSqlSession));
		  }
	  }

	  protected internal virtual T getSession<T>(Type<T> sessionClass)
	  {
		return Context.CommandContext.getSession(sessionClass);
	  }

	  protected internal virtual DeploymentManager DeploymentManager
	  {
		  get
		  {
			return getSession(typeof(DeploymentManager));
		  }
	  }

	  protected internal virtual ResourceManager ResourceManager
	  {
		  get
		  {
			return getSession(typeof(ResourceManager));
		  }
	  }

	  protected internal virtual ByteArrayManager ByteArrayManager
	  {
		  get
		  {
			return getSession(typeof(ByteArrayManager));
		  }
	  }

	  protected internal virtual ProcessDefinitionManager ProcessDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(ProcessDefinitionManager));
		  }
	  }

	  protected internal virtual CaseDefinitionManager CaseDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(CaseDefinitionManager));
		  }
	  }

	  protected internal virtual DecisionDefinitionManager DecisionDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(DecisionDefinitionManager));
		  }
	  }

	  protected internal virtual DecisionRequirementsDefinitionManager DecisionRequirementsDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(DecisionRequirementsDefinitionManager));
		  }
	  }

	  protected internal virtual HistoricDecisionInstanceManager HistoricDecisionInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricDecisionInstanceManager));
		  }
	  }

	  protected internal virtual CaseExecutionManager CaseInstanceManager
	  {
		  get
		  {
			return getSession(typeof(CaseExecutionManager));
		  }
	  }

	  protected internal virtual CaseExecutionManager CaseExecutionManager
	  {
		  get
		  {
			return getSession(typeof(CaseExecutionManager));
		  }
	  }

	  protected internal virtual ExecutionManager ProcessInstanceManager
	  {
		  get
		  {
			return getSession(typeof(ExecutionManager));
		  }
	  }

	  protected internal virtual TaskManager TaskManager
	  {
		  get
		  {
			return getSession(typeof(TaskManager));
		  }
	  }

	  protected internal virtual TaskReportManager TaskReportManager
	  {
		  get
		  {
			return getSession(typeof(TaskReportManager));
		  }
	  }

	  protected internal virtual IdentityLinkManager IdentityLinkManager
	  {
		  get
		  {
			return getSession(typeof(IdentityLinkManager));
		  }
	  }

	  protected internal virtual VariableInstanceManager VariableInstanceManager
	  {
		  get
		  {
			return getSession(typeof(VariableInstanceManager));
		  }
	  }

	  protected internal virtual HistoricProcessInstanceManager HistoricProcessInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricProcessInstanceManager));
		  }
	  }

	  protected internal virtual HistoricCaseInstanceManager HistoricCaseInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricCaseInstanceManager));
		  }
	  }

	  protected internal virtual HistoricDetailManager HistoricDetailManager
	  {
		  get
		  {
			return getSession(typeof(HistoricDetailManager));
		  }
	  }

	  protected internal virtual HistoricVariableInstanceManager HistoricVariableInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricVariableInstanceManager));
		  }
	  }

	  protected internal virtual HistoricActivityInstanceManager HistoricActivityInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricActivityInstanceManager));
		  }
	  }

	  protected internal virtual HistoricCaseActivityInstanceManager HistoricCaseActivityInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricCaseActivityInstanceManager));
		  }
	  }

	  protected internal virtual HistoricTaskInstanceManager HistoricTaskInstanceManager
	  {
		  get
		  {
			return getSession(typeof(HistoricTaskInstanceManager));
		  }
	  }

	  protected internal virtual HistoricIncidentManager HistoricIncidentManager
	  {
		  get
		  {
			return getSession(typeof(HistoricIncidentManager));
		  }
	  }

	  protected internal virtual HistoricIdentityLinkLogManager HistoricIdentityLinkManager
	  {
		  get
		  {
			return getSession(typeof(HistoricIdentityLinkLogManager));
		  }
	  }

	  protected internal virtual HistoricJobLogManager HistoricJobLogManager
	  {
		  get
		  {
			return getSession(typeof(HistoricJobLogManager));
		  }
	  }

	  protected internal virtual HistoricExternalTaskLogManager HistoricExternalTaskLogManager
	  {
		  get
		  {
			return getSession(typeof(HistoricExternalTaskLogManager));
		  }
	  }

	  protected internal virtual JobManager JobManager
	  {
		  get
		  {
			return getSession(typeof(JobManager));
		  }
	  }

	  protected internal virtual JobDefinitionManager JobDefinitionManager
	  {
		  get
		  {
			return getSession(typeof(JobDefinitionManager));
		  }
	  }

	  protected internal virtual UserOperationLogManager UserOperationLogManager
	  {
		  get
		  {
			return getSession(typeof(UserOperationLogManager));
		  }
	  }

	  protected internal virtual EventSubscriptionManager EventSubscriptionManager
	  {
		  get
		  {
			return getSession(typeof(EventSubscriptionManager));
		  }
	  }

	  protected internal virtual IdentityInfoManager IdentityInfoManager
	  {
		  get
		  {
			return getSession(typeof(IdentityInfoManager));
		  }
	  }

	  protected internal virtual AttachmentManager AttachmentManager
	  {
		  get
		  {
			return getSession(typeof(AttachmentManager));
		  }
	  }

	  protected internal virtual ReportManager HistoricReportManager
	  {
		  get
		  {
			return getSession(typeof(ReportManager));
		  }
	  }

	  protected internal virtual BatchManager BatchManager
	  {
		  get
		  {
			return getSession(typeof(BatchManager));
		  }
	  }

	  protected internal virtual HistoricBatchManager HistoricBatchManager
	  {
		  get
		  {
			return getSession(typeof(HistoricBatchManager));
		  }
	  }

	  protected internal virtual TenantManager TenantManager
	  {
		  get
		  {
			return getSession(typeof(TenantManager));
		  }
	  }

	  public virtual void close()
	  {
	  }

	  public virtual void flush()
	  {
	  }

	  // authorizations ///////////////////////////////////////

	  protected internal virtual CommandContext CommandContext
	  {
		  get
		  {
			return Context.CommandContext;
		  }
	  }

	  protected internal virtual AuthorizationManager AuthorizationManager
	  {
		  get
		  {
			return getSession(typeof(AuthorizationManager));
		  }
	  }

	  protected internal virtual void configureQuery<T1>(AbstractQuery<T1> query, Resource resource)
	  {
		AuthorizationManager.configureQuery(query, resource);
	  }

	  protected internal virtual void checkAuthorization(Permission permission, Resource resource, string resourceId)
	  {
		AuthorizationManager.checkAuthorization(permission, resource, resourceId);
	  }

	  public virtual bool AuthorizationEnabled
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.AuthorizationEnabled;
		  }
	  }

	  protected internal virtual Authentication CurrentAuthentication
	  {
		  get
		  {
			return Context.CommandContext.Authentication;
		  }
	  }

	  protected internal virtual ResourceAuthorizationProvider ResourceAuthorizationProvider
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ResourceAuthorizationProvider;
		  }
	  }

	  protected internal virtual void deleteAuthorizations(Resource resource, string resourceId)
	  {
		AuthorizationManager.deleteAuthorizationsByResourceId(resource, resourceId);
	  }

	  protected internal virtual void deleteAuthorizationsForUser(Resource resource, string resourceId, string userId)
	  {
		AuthorizationManager.deleteAuthorizationsByResourceIdAndUserId(resource, resourceId, userId);
	  }

	  protected internal virtual void deleteAuthorizationsForGroup(Resource resource, string resourceId, string groupId)
	  {
		AuthorizationManager.deleteAuthorizationsByResourceIdAndGroupId(resource, resourceId, groupId);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void saveDefaultAuthorizations(final org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity[] authorizations)
	  public virtual void saveDefaultAuthorizations(AuthorizationEntity[] authorizations)
	  {
		if (authorizations != null && authorizations.Length > 0)
		{
		  Context.CommandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, authorizations));
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly AbstractManager outerInstance;

		  private AuthorizationEntity[] authorizations;

		  public CallableAnonymousInnerClass(AbstractManager outerInstance, AuthorizationEntity[] authorizations)
		  {
			  this.outerInstance = outerInstance;
			  this.authorizations = authorizations;
		  }

		  public Void call()
		  {
			AuthorizationManager authorizationManager = outerInstance.AuthorizationManager;
			foreach (AuthorizationEntity authorization in authorizations)
			{

			  if (string.ReferenceEquals(authorization.Id, null))
			  {
				authorizationManager.insert(authorization);
			  }
			  else
			  {
				authorizationManager.update(authorization);
			  }

			}
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void deleteDefaultAuthorizations(final org.camunda.bpm.engine.impl.persistence.entity.AuthorizationEntity[] authorizations)
	  public virtual void deleteDefaultAuthorizations(AuthorizationEntity[] authorizations)
	  {
		if (authorizations != null && authorizations.Length > 0)
		{
		  Context.CommandContext.runWithoutAuthorization(new CallableAnonymousInnerClass2(this, authorizations));
		}
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly AbstractManager outerInstance;

		  private AuthorizationEntity[] authorizations;

		  public CallableAnonymousInnerClass2(AbstractManager outerInstance, AuthorizationEntity[] authorizations)
		  {
			  this.outerInstance = outerInstance;
			  this.authorizations = authorizations;
		  }

		  public Void call()
		  {
			AuthorizationManager authorizationManager = outerInstance.AuthorizationManager;
			foreach (AuthorizationEntity authorization in authorizations)
			{
			  authorizationManager.delete(authorization);
			}
			return null;
		  }
	  }

	}

}