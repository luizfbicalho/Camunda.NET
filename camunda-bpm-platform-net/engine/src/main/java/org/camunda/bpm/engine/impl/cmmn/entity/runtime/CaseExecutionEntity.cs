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
namespace org.camunda.bpm.engine.impl.cmmn.entity.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_ACTIVITY_DESCRIPTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_ACTIVITY_TYPE;


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using TenantIdProvider = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProvider;
	using TenantIdProviderCaseInstanceContext = org.camunda.bpm.engine.impl.cfg.multitenancy.TenantIdProviderCaseInstanceContext;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnSentryPart = org.camunda.bpm.engine.impl.cmmn.execution.CmmnSentryPart;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnCaseDefinition = org.camunda.bpm.engine.impl.cmmn.model.CmmnCaseDefinition;
	using CmmnAtomicOperation = org.camunda.bpm.engine.impl.cmmn.operation.CmmnAtomicOperation;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;
	using CoreVariableInstance = org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance;
	using CmmnVariableInvocationListener = org.camunda.bpm.engine.impl.core.variable.scope.CmmnVariableInvocationListener;
	using VariableInstanceFactory = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceFactory;
	using VariableInstanceLifecycleListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener;
	using VariableOnPartListener = org.camunda.bpm.engine.impl.core.variable.scope.VariableOnPartListener;
	using VariableStore = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore;
	using VariablesProvider = org.camunda.bpm.engine.impl.core.variable.scope.VariableStore.VariablesProvider;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventHandler = org.camunda.bpm.engine.impl.history.handler.HistoryEventHandler;
	using CmmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.CmmnHistoryEventProducer;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CaseExecutionEntityReferencer = org.camunda.bpm.engine.impl.persistence.entity.CaseExecutionEntityReferencer;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using VariableInstanceEntityFactory = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntityFactory;
	using VariableInstanceEntityPersistenceListener = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntityPersistenceListener;
	using VariableInstanceHistoryListener = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceHistoryListener;
	using VariableInstanceSequenceCounterListener = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceSequenceCounterListener;
	using PvmProcessDefinition = org.camunda.bpm.engine.impl.pvm.PvmProcessDefinition;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseExecution = org.camunda.bpm.engine.runtime.CaseExecution;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;
	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementType = org.camunda.bpm.model.xml.type.ModelElementType;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CaseExecutionEntity : CmmnExecution, CaseExecution, CaseInstance, DbEntity, HasDbRevision, HasDbReferences, VariableStore.VariablesProvider<VariableInstanceEntity>
	{
		private bool InstanceFieldsInitialized = false;

		public CaseExecutionEntity()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			variableStore = new VariableStore<VariableInstanceEntity>(this, new CaseExecutionEntityReferencer(this));
		}


	  private const long serialVersionUID = 1L;

	  // current position /////////////////////////////////////////////////////////

	  /// <summary>
	  /// the case instance.  this is the root of the execution tree.
	  /// the caseInstance of a case instance is a self reference. 
	  /// </summary>
	  [NonSerialized]
	  protected internal CaseExecutionEntity caseInstance;

	  /// <summary>
	  /// the parent execution </summary>
	  [NonSerialized]
	  protected internal CaseExecutionEntity parent;

	  /// <summary>
	  /// nested executions </summary>
	  protected internal IList<CaseExecutionEntity> caseExecutions;

	  /// <summary>
	  /// nested case sentry parts </summary>
	  protected internal IList<CaseSentryPartEntity> caseSentryParts;
	  protected internal IDictionary<string, IList<CmmnSentryPart>> sentries;

	  /// <summary>
	  /// reference to a sub process instance, not-null if currently subprocess is started from this execution </summary>
	  [NonSerialized]
	  protected internal ExecutionEntity subProcessInstance;

	  [NonSerialized]
	  protected internal ExecutionEntity superExecution;

	  [NonSerialized]
	  protected internal CaseExecutionEntity subCaseInstance;

	  [NonSerialized]
	  protected internal CaseExecutionEntity superCaseExecution;

	  // associated entities /////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked" }) protected org.camunda.bpm.engine.impl.core.variable.scope.VariableStore<org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity> variableStore = new org.camunda.bpm.engine.impl.core.variable.scope.VariableStore<org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity>(this, new org.camunda.bpm.engine.impl.persistence.entity.CaseExecutionEntityReferencer(this));
	  protected internal VariableStore<VariableInstanceEntity> variableStore;

	  // Persistence //////////////////////////////////////////////////////////////

	  protected internal int revision = 1;
	  protected internal string caseDefinitionId;
	  protected internal string activityId;
	  protected internal string caseInstanceId;
	  protected internal string parentId;
	  protected internal string superCaseExecutionId;
	  protected internal string superExecutionId;

	  // activity properites //////////////////////////////////////////////////////

	  protected internal string activityName;
	  protected internal string activityType;
	  protected internal string activityDescription;

	  // case definition ///////////////////////////////////////////////////////////

	  public override string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId;
		  }
	  }

	  /// <summary>
	  /// ensures initialization and returns the case definition. </summary>
	  public override CmmnCaseDefinition CaseDefinition
	  {
		  get
		  {
			ensureCaseDefinitionInitialized();
			return caseDefinition;
		  }
		  set
		  {
			base.CaseDefinition = value;
    
			caseDefinitionId = null;
			if (value != null)
			{
			  caseDefinitionId = value.Id;
			}
    
		  }
	  }


	  protected internal virtual void ensureCaseDefinitionInitialized()
	  {
		if ((caseDefinition == null) && (!string.ReferenceEquals(caseDefinitionId, null)))
		{

		  CaseDefinitionEntity deployedCaseDefinition = Context.ProcessEngineConfiguration.DeploymentCache.getCaseDefinitionById(caseDefinitionId);

		  CaseDefinition = deployedCaseDefinition;
		}
	  }

	  // parent ////////////////////////////////////////////////////////////////////

	  public override CaseExecutionEntity getParent()
	  {
		ensureParentInitialized();
		return parent;
	  }

	  public override void setParent(CmmnExecution parent)
	  {
		this.parent = (CaseExecutionEntity) parent;

		if (parent != null)
		{
		  this.parentId = parent.Id;
		}
		else
		{
		  this.parentId = null;
		}
	  }

	  protected internal virtual void ensureParentInitialized()
	  {
		if (parent == null && !string.ReferenceEquals(parentId, null))
		{
		  if (ExecutionTreePrefetchEnabled)
		  {
			ensureCaseExecutionTreeInitialized();

		  }
		  else
		  {
			parent = Context.CommandContext.CaseExecutionManager.findCaseExecutionById(parentId);

		  }
		}
	  }

	  /// <seealso cref= ExecutionEntity#ensureExecutionTreeInitialized </seealso>
	  protected internal virtual void ensureCaseExecutionTreeInitialized()
	  {
		IList<CaseExecutionEntity> executions = Context.CommandContext.CaseExecutionManager.findChildCaseExecutionsByCaseInstanceId(caseInstanceId);

		CaseExecutionEntity caseInstance = null;

		IDictionary<string, CaseExecutionEntity> executionMap = new Dictionary<string, CaseExecutionEntity>();
		foreach (CaseExecutionEntity execution in executions)
		{
		  execution.caseExecutions = new List<CaseExecutionEntity>();
		  executionMap[execution.Id] = execution;
		  if (execution.CaseInstanceExecution)
		  {
			caseInstance = execution;
		  }
		}

		foreach (CaseExecutionEntity execution in executions)
		{
		  string parentId = execution.ParentId;
		  CaseExecutionEntity parent = executionMap[parentId];
		  if (!execution.CaseInstanceExecution)
		  {
			execution.caseInstance = caseInstance;
			execution.parent = parent;
			parent.caseExecutions.Add(execution);
		  }
		  else
		  {
			execution.caseInstance = execution;
		  }
		}
	  }

	  /// <returns> true if execution tree prefetching is enabled </returns>
	  protected internal virtual bool ExecutionTreePrefetchEnabled
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ExecutionTreePrefetchEnabled;
		  }
	  }

	  public override string ParentId
	  {
		  get
		  {
			return parentId;
		  }
	  }

	  // activity //////////////////////////////////////////////////////////////////

	  public override CmmnActivity Activity
	  {
		  get
		  {
			ensureActivityInitialized();
			return base.Activity;
		  }
		  set
		  {
			base.Activity = value;
			if (value != null)
			{
			  this.activityId = value.Id;
			  this.activityName = value.Name;
			  this.activityType = getActivityProperty(value, PROPERTY_ACTIVITY_TYPE);
			  this.activityDescription = getActivityProperty(value, PROPERTY_ACTIVITY_DESCRIPTION);
			}
			else
			{
			  this.activityId = null;
			  this.activityName = null;
			  this.activityType = null;
			  this.activityDescription = null;
			}
		  }
	  }


	  protected internal virtual void ensureActivityInitialized()
	  {
		if ((activity == null) && (!string.ReferenceEquals(activityId, null)))
		{
		  Activity = CaseDefinition.findActivity(activityId);
		}
	  }

	  protected internal virtual string getActivityProperty(CmmnActivity activity, string property)
	  {
		string result = null;

		if (activity != null)
		{
		  object value = activity.getProperty(property);
		  if (value != null && value is string)
		  {
			result = (string) value;
		  }
		}

		return result;
	  }

	  // activity properties //////////////////////////////////////////////////////

	  public override string ActivityId
	  {
		  get
		  {
			return activityId;
		  }
	  }

	  public override string ActivityName
	  {
		  get
		  {
			return activityName;
		  }
	  }

	  public virtual string ActivityType
	  {
		  get
		  {
			return activityType;
		  }
	  }

	  public virtual string ActivityDescription
	  {
		  get
		  {
			return activityDescription;
		  }
	  }

	  // case executions ////////////////////////////////////////////////////////////////

	  public override IList<CaseExecutionEntity> CaseExecutions
	  {
		  get
		  {
			return new List<CaseExecutionEntity>(CaseExecutionsInternal);
		  }
	  }

	  protected internal override IList<CaseExecutionEntity> CaseExecutionsInternal
	  {
		  get
		  {
			ensureCaseExecutionsInitialized();
			return caseExecutions;
		  }
	  }

	  protected internal virtual void ensureCaseExecutionsInitialized()
	  {
		if (caseExecutions == null)
		{
		  this.caseExecutions = Context.CommandContext.CaseExecutionManager.findChildCaseExecutionsByParentCaseExecutionId(id);
		}
	  }

	  // task ///////////////////////////////////////////////////////////////////

	  public override TaskEntity Task
	  {
		  get
		  {
			ensureTaskInitialized();
			return task;
		  }
	  }

	  protected internal virtual void ensureTaskInitialized()
	  {
		if (task == null)
		{
		  task = Context.CommandContext.TaskManager.findTaskByCaseExecutionId(id);
		}
	  }

	  public override TaskEntity createTask(TaskDecorator taskDecorator)
	  {
		TaskEntity task = base.createTask(taskDecorator);
		fireHistoricCaseActivityInstanceUpdate();
		return task;
	  }

	  // case instance /////////////////////////////////////////////////////////

	  public override string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId;
		  }
	  }

	  public override CaseExecutionEntity getCaseInstance()
	  {
		ensureCaseInstanceInitialized();
		return caseInstance;
	  }

	  public override void setCaseInstance(CmmnExecution caseInstance)
	  {
		this.caseInstance = (CaseExecutionEntity) caseInstance;

		if (caseInstance != null)
		{
		  this.caseInstanceId = this.caseInstance.Id;
		}
	  }

	  protected internal virtual void ensureCaseInstanceInitialized()
	  {
		if ((caseInstance == null) && (!string.ReferenceEquals(caseInstanceId, null)))
		{

		  caseInstance = Context.CommandContext.CaseExecutionManager.findCaseExecutionById(caseInstanceId);

		}
	  }

	  public override bool CaseInstanceExecution
	  {
		  get
		  {
			return string.ReferenceEquals(parentId, null);
		  }
	  }

	  public override void create(IDictionary<string, object> variables)
	  {
		// determine tenant Id if null
		if (string.ReferenceEquals(tenantId, null))
		{
		  provideTenantId(variables);
		}
		base.create(variables);
	  }

	  protected internal virtual void provideTenantId(IDictionary<string, object> variables)
	  {
		TenantIdProvider tenantIdProvider = Context.ProcessEngineConfiguration.TenantIdProvider;

		if (tenantIdProvider != null)
		{
		  VariableMap variableMap = Variables.fromMap(variables);
		  CaseDefinition caseDefinition = (CaseDefinition) CaseDefinition;

		  TenantIdProviderCaseInstanceContext ctx = null;

		  if (!string.ReferenceEquals(superExecutionId, null))
		  {
			ctx = new TenantIdProviderCaseInstanceContext(caseDefinition, variableMap, getSuperExecution());
		  }
		  else if (!string.ReferenceEquals(superCaseExecutionId, null))
		  {
			ctx = new TenantIdProviderCaseInstanceContext(caseDefinition, variableMap, SuperCaseExecution);
		  }
		  else
		  {
			ctx = new TenantIdProviderCaseInstanceContext(caseDefinition, variableMap);
		  }

		  tenantId = tenantIdProvider.provideTenantIdForCaseInstance(ctx);
		}
	  }

	  protected internal override CaseExecutionEntity createCaseExecution(CmmnActivity activity)
	  {
		CaseExecutionEntity child = newCaseExecution();

		// set activity to execute
		child.Activity = activity;

		// handle child/parent-relation
		child.setParent(this);
		CaseExecutionsInternal.Add(child);

		// set case instance
		child.setCaseInstance(getCaseInstance());

		// set case definition
		child.CaseDefinition = CaseDefinition;

		// inherit the tenant id from parent case execution
		if (!string.ReferenceEquals(tenantId, null))
		{
		  child.TenantId = tenantId;
		}

		return child;
	  }

	  protected internal override CaseExecutionEntity newCaseExecution()
	  {
		CaseExecutionEntity newCaseExecution = new CaseExecutionEntity();

		Context.CommandContext.CaseExecutionManager.insertCaseExecution(newCaseExecution);

		return newCaseExecution;
	  }

	  // super execution //////////////////////////////////////////////////////

	  public virtual string SuperExecutionId
	  {
		  get
		  {
			return superExecutionId;
		  }
		  set
		  {
			this.superExecutionId = value;
		  }
	  }


	  public override ExecutionEntity getSuperExecution()
	  {
		ensureSuperExecutionInstanceInitialized();
		return superExecution;
	  }

	  public override void setSuperExecution(PvmExecutionImpl superExecution)
	  {
		if (!string.ReferenceEquals(this.superExecutionId, null))
		{
		  ensureSuperExecutionInstanceInitialized();
		  this.superExecution.setSubCaseInstance(null);
		}

		this.superExecution = (ExecutionEntity) superExecution;

		if (superExecution != null)
		{
		  this.superExecutionId = superExecution.Id;
		  this.superExecution.setSubCaseInstance(this);
		}
		else
		{
		  this.superExecutionId = null;
		}
	  }

	  protected internal virtual void ensureSuperExecutionInstanceInitialized()
	  {
		if (superExecution == null && !string.ReferenceEquals(superExecutionId, null))
		{
		  superExecution = Context.CommandContext.ExecutionManager.findExecutionById(superExecutionId);
		}
	  }

	  // sub process instance ///////////////////////////////////////////////////

	  public override ExecutionEntity getSubProcessInstance()
	  {
		ensureSubProcessInstanceInitialized();
		return subProcessInstance;
	  }

	  public override void setSubProcessInstance(PvmExecutionImpl subProcessInstance)
	  {
		this.subProcessInstance = (ExecutionEntity) subProcessInstance;
	  }

	  public override ExecutionEntity createSubProcessInstance(PvmProcessDefinition processDefinition)
	  {
		return createSubProcessInstance(processDefinition, null);
	  }

	  public override ExecutionEntity createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey)
	  {
		return createSubProcessInstance(processDefinition, businessKey, CaseInstanceId);
	  }

	  public override ExecutionEntity createSubProcessInstance(PvmProcessDefinition processDefinition, string businessKey, string caseInstanceId)
	  {
		ExecutionEntity subProcessInstance = (ExecutionEntity) processDefinition.createProcessInstance(businessKey, caseInstanceId);

		// inherit the tenant-id from the process definition
		string tenantId = ((ProcessDefinitionEntity) processDefinition).TenantId;
		if (!string.ReferenceEquals(tenantId, null))
		{
		  subProcessInstance.TenantId = tenantId;
		}
		else
		{
		  // if process definition has no tenant id, inherit this case instance's tenant id
		  subProcessInstance.TenantId = this.tenantId;
		}

		// manage bidirectional super-subprocess relation
		subProcessInstance.setSuperCaseExecution(this);
		setSubProcessInstance(subProcessInstance);

		fireHistoricCaseActivityInstanceUpdate();

		return subProcessInstance;
	  }

	  protected internal virtual void ensureSubProcessInstanceInitialized()
	  {
		if (subProcessInstance == null)
		{
		  subProcessInstance = Context.CommandContext.ExecutionManager.findSubProcessInstanceBySuperCaseExecutionId(id);
		}
	  }

	  // sub-/super- case instance ////////////////////////////////////////////////////

	  public override CaseExecutionEntity getSubCaseInstance()
	  {
		ensureSubCaseInstanceInitialized();
		return subCaseInstance;
	  }

	  public override void setSubCaseInstance(CmmnExecution subCaseInstance)
	  {
		this.subCaseInstance = (CaseExecutionEntity) subCaseInstance;
	  }

	  public override CaseExecutionEntity createSubCaseInstance(CmmnCaseDefinition caseDefinition)
	  {
		return createSubCaseInstance(caseDefinition, null);
	  }

	  public override CaseExecutionEntity createSubCaseInstance(CmmnCaseDefinition caseDefinition, string businessKey)
	  {
		CaseExecutionEntity subCaseInstance = (CaseExecutionEntity) caseDefinition.createCaseInstance(businessKey);

		// inherit the tenant-id from the case definition
		string tenantId = ((CaseDefinitionEntity) caseDefinition).TenantId;
		if (!string.ReferenceEquals(tenantId, null))
		{
		  subCaseInstance.TenantId = tenantId;
		}
		else
		{
		  // if case definition has no tenant id, inherit this case instance's tenant id
		  subCaseInstance.TenantId = this.tenantId;
		}

		// manage bidirectional super-sub-case-instances relation
		subCaseInstance.SuperCaseExecution = this;
		setSubCaseInstance(subCaseInstance);

		fireHistoricCaseActivityInstanceUpdate();

		return subCaseInstance;
	  }

	  public virtual void fireHistoricCaseActivityInstanceUpdate()
	  {
		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;
		HistoryLevel historyLevel = configuration.HistoryLevel;
		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.CASE_ACTIVITY_INSTANCE_UPDATE, this))
		{
		  CmmnHistoryEventProducer eventProducer = configuration.CmmnHistoryEventProducer;
		  HistoryEventHandler eventHandler = configuration.HistoryEventHandler;

		  HistoryEvent @event = eventProducer.createCaseActivityInstanceUpdateEvt(this);
		  eventHandler.handleEvent(@event);
		}
	  }

	  protected internal virtual void ensureSubCaseInstanceInitialized()
	  {
		if (subCaseInstance == null)
		{
		  subCaseInstance = Context.CommandContext.CaseExecutionManager.findSubCaseInstanceBySuperCaseExecutionId(id);
		}
	  }

	  public virtual string SuperCaseExecutionId
	  {
		  get
		  {
			return superCaseExecutionId;
		  }
		  set
		  {
			this.superCaseExecutionId = value;
		  }
	  }


	  public override CmmnExecution SuperCaseExecution
	  {
		  get
		  {
			ensureSuperCaseExecutionInitialized();
			return superCaseExecution;
		  }
		  set
		  {
			this.superCaseExecution = (CaseExecutionEntity) value;
    
			if (value != null)
			{
			  this.superCaseExecutionId = value.Id;
			}
			else
			{
			  this.superCaseExecutionId = null;
			}
		  }
	  }


	  protected internal virtual void ensureSuperCaseExecutionInitialized()
	  {
		if (superCaseExecution == null && !string.ReferenceEquals(superCaseExecutionId, null))
		{
		  superCaseExecution = Context.CommandContext.CaseExecutionManager.findCaseExecutionById(superCaseExecutionId);
		}
	  }

	  // sentry /////////////////////////////////////////////////////////////////////////

	  public override IList<CaseSentryPartEntity> CaseSentryParts
	  {
		  get
		  {
			ensureCaseSentryPartsInitialized();
			return caseSentryParts;
		  }
	  }

	  protected internal virtual void ensureCaseSentryPartsInitialized()
	  {
		if (caseSentryParts == null)
		{

		  caseSentryParts = Context.CommandContext.CaseSentryPartManager.findCaseSentryPartsByCaseExecutionId(id);

		  // create a map sentries: sentryId -> caseSentryParts
		  // for simple select to get all parts for one sentry
		  sentries = new Dictionary<string, IList<CmmnSentryPart>>();

		  foreach (CaseSentryPartEntity sentryPart in caseSentryParts)
		  {

			string sentryId = sentryPart.SentryId;
			IList<CmmnSentryPart> parts = sentries[sentryId];

			if (parts == null)
			{
			  parts = new List<CmmnSentryPart>();
			  sentries[sentryId] = parts;
			}

			parts.Add(sentryPart);
		  }
		}
	  }

	  protected internal override void addSentryPart(CmmnSentryPart sentryPart)
	  {
		CaseSentryPartEntity entity = (CaseSentryPartEntity) sentryPart;

		CaseSentryParts.Add(entity);

		string sentryId = sentryPart.SentryId;
		IList<CmmnSentryPart> parts = sentries[sentryId];

		if (parts == null)
		{
		  parts = new List<CmmnSentryPart>();
		  sentries[sentryId] = parts;
		}

		parts.Add(entity);
	  }

	  protected internal override IDictionary<string, IList<CmmnSentryPart>> Sentries
	  {
		  get
		  {
			ensureCaseSentryPartsInitialized();
			return sentries;
		  }
	  }

	  protected internal override IList<CmmnSentryPart> findSentry(string sentryId)
	  {
		ensureCaseSentryPartsInitialized();
		return sentries[sentryId];
	  }

	  protected internal override CaseSentryPartEntity newSentryPart()
	  {
		CaseSentryPartEntity caseSentryPart = new CaseSentryPartEntity();

		Context.CommandContext.CaseSentryPartManager.insertCaseSentryPart(caseSentryPart);

		return caseSentryPart;
	  }

	  // variables //////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) protected org.camunda.bpm.engine.impl.core.variable.scope.VariableStore<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance> getVariableStore()
	  protected internal override VariableStore<CoreVariableInstance> VariableStore
	  {
		  get
		  {
			return (VariableStore) variableStore;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "unchecked", "rawtypes" }) protected org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceFactory<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance> getVariableInstanceFactory()
	  protected internal override VariableInstanceFactory<CoreVariableInstance> VariableInstanceFactory
	  {
		  get
		  {
			return (VariableInstanceFactory) VariableInstanceEntityFactory.INSTANCE;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @SuppressWarnings({ "unchecked", "rawtypes" }) protected java.util.List<org.camunda.bpm.engine.impl.core.variable.scope.VariableInstanceLifecycleListener<org.camunda.bpm.engine.impl.core.variable.CoreVariableInstance>> getVariableInstanceLifecycleListeners()
	  protected internal override IList<VariableInstanceLifecycleListener<CoreVariableInstance>> VariableInstanceLifecycleListeners
	  {
		  get
		  {
			return Arrays.asList<VariableInstanceLifecycleListener<CoreVariableInstance>>((VariableInstanceLifecycleListener) VariableInstanceEntityPersistenceListener.INSTANCE, (VariableInstanceLifecycleListener) VariableInstanceSequenceCounterListener.INSTANCE, (VariableInstanceLifecycleListener) VariableInstanceHistoryListener.INSTANCE, (VariableInstanceLifecycleListener) CmmnVariableInvocationListener.INSTANCE, (VariableInstanceLifecycleListener) new VariableOnPartListener(this)
			 );
    
		  }
	  }

	  public virtual ICollection<VariableInstanceEntity> provideVariables()
	  {
		return Context.CommandContext.VariableInstanceManager.findVariableInstancesByCaseExecutionId(id);
	  }

	  public virtual ICollection<VariableInstanceEntity> provideVariables(ICollection<string> variableNames)
	  {
		return Context.CommandContext.VariableInstanceManager.findVariableInstancesByCaseExecutionIdAndVariableNames(id, variableNames);
	  }

	  // toString /////////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		if (CaseInstanceExecution)
		{
		  return "CaseInstance[" + ToStringIdentity + "]";
		}
		else
		{
		  return "CaseExecution[" + ToStringIdentity + "]";
		}
	  }

	  protected internal override string ToStringIdentity
	  {
		  get
		  {
			return id;
		  }
	  }

	  // delete/remove ///////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) public void remove()
	  public override void remove()
	  {
		base.remove();

		foreach (VariableInstanceEntity variableInstance in variableStore.Variables)
		{
		  invokeVariableLifecycleListenersDelete(variableInstance, this, Arrays.asList<VariableInstanceLifecycleListener<CoreVariableInstance>>((VariableInstanceLifecycleListener) VariableInstanceEntityPersistenceListener.INSTANCE));
		  variableStore.removeVariable(variableInstance.Name);
		}

		CommandContext commandContext = Context.CommandContext;

		foreach (CaseSentryPartEntity sentryPart in CaseSentryParts)
		{
		  commandContext.CaseSentryPartManager.deleteSentryPart(sentryPart);
		}

		// finally delete this execution
		commandContext.CaseExecutionManager.deleteCaseExecution(this);
	  }

	  // persistence /////////////////////////////////////////////////////////

	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  public virtual void forceUpdate()
	  {
		Context.CommandContext.DbEntityManager.forceUpdate(this);
	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referenceIds = new HashSet<string>();
    
			if (!string.ReferenceEquals(parentId, null))
			{
			  referenceIds.Add(parentId);
			}
			if (!string.ReferenceEquals(superCaseExecutionId, null))
			{
			  referenceIds.Add(superCaseExecutionId);
			}
    
			return referenceIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
    
			if (!string.ReferenceEquals(parentId, null))
			{
			  referenceIdAndClass[parentId] = typeof(CaseExecutionEntity);
			}
			if (!string.ReferenceEquals(superCaseExecutionId, null))
			{
			  referenceIdAndClass[superCaseExecutionId] = typeof(CaseExecutionEntity);
			}
			if (!string.ReferenceEquals(caseDefinitionId, null))
			{
			  referenceIdAndClass[caseDefinitionId] = typeof(CmmnCaseDefinition);
			}
    
			return referenceIdAndClass;
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["caseDefinitionId"] = caseDefinitionId;
			persistentState["businessKey"] = businessKey;
			persistentState["activityId"] = activityId;
			persistentState["parentId"] = parentId;
			persistentState["currentState"] = currentState;
			persistentState["previousState"] = previousState;
			persistentState["superExecutionId"] = superExecutionId;
			return persistentState;
		  }
	  }

	  public override CmmnModelInstance CmmnModelInstance
	  {
		  get
		  {
			if (!string.ReferenceEquals(caseDefinitionId, null))
			{
    
			  return Context.ProcessEngineConfiguration.DeploymentCache.findCmmnModelInstanceForCaseDefinition(caseDefinitionId);
    
			}
			else
			{
			  return null;
    
			}
		  }
	  }

	  public override CmmnElement CmmnModelElementInstance
	  {
		  get
		  {
			CmmnModelInstance cmmnModelInstance = CmmnModelInstance;
			if (cmmnModelInstance != null)
			{
    
			  ModelElementInstance modelElementInstance = cmmnModelInstance.getModelElementById(activityId);
    
			  try
			  {
				return (CmmnElement) modelElementInstance;
    
			  }
			  catch (System.InvalidCastException e)
			  {
				ModelElementType elementType = modelElementInstance.ElementType;
				throw new ProcessEngineException("Cannot cast " + modelElementInstance + " to CmmnElement. " + "Is of type " + elementType.TypeName + " Namespace " + elementType.TypeNamespace, e);
			  }
    
			}
			else
			{
			  return null;
			}
		  }
	  }

	  public override ProcessEngineServices ProcessEngineServices
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }

	  public override ProcessEngine ProcessEngine
	  {
		  get
		  {
			return Context.ProcessEngineConfiguration.ProcessEngine;
		  }
	  }

	  public override void performOperation<T>(CoreAtomicOperation<T> operation) where T : org.camunda.bpm.engine.impl.core.instance.CoreExecution
	  {
		Context.CommandContext.performOperation((CmmnAtomicOperation) operation, this);
	  }

	  public override void performOperationSync<T>(CoreAtomicOperation<T> operation) where T : org.camunda.bpm.engine.impl.core.instance.CoreExecution
	  {
		Context.CommandContext.performOperation((CmmnAtomicOperation) operation, this);
	  }
	}

}