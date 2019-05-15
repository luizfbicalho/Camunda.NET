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
namespace org.camunda.bpm.engine.impl.bpmn.deployer
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using BpmnParseLogger = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseLogger;
	using BpmnParser = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParser;
	using EventSubscriptionDeclaration = org.camunda.bpm.engine.impl.bpmn.parser.EventSubscriptionDeclaration;
	using DeleteJobsCmd = org.camunda.bpm.engine.impl.cmd.DeleteJobsCmd;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Properties = org.camunda.bpm.engine.impl.core.model.Properties;
	using PropertyMapKey = org.camunda.bpm.engine.impl.core.model.PropertyMapKey;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using TimerDeclarationImpl = org.camunda.bpm.engine.impl.jobexecutor.TimerDeclarationImpl;
	using TimerStartEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventJobHandler;
	using Deployer = org.camunda.bpm.engine.impl.persistence.deploy.Deployer;
	using DeploymentCache = org.camunda.bpm.engine.impl.persistence.deploy.cache.DeploymentCache;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;
	using EventSubscriptionManager = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionManager;
	using IdentityLinkEntity = org.camunda.bpm.engine.impl.persistence.entity.IdentityLinkEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionManager;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using LegacyBehavior = org.camunda.bpm.engine.impl.pvm.runtime.LegacyBehavior;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using IdentityLinkType = org.camunda.bpm.engine.task.IdentityLinkType;

	/// <summary>
	/// <seealso cref="Deployer"/> responsible to parse BPMN 2.0 XML files and create the proper
	/// <seealso cref="ProcessDefinitionEntity"/>s. Overwrite this class if you want to gain some control over
	/// this mechanism, e.g. setting different version numbers, or you want to use your own <seealso cref="BpmnParser"/>.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Bernd Ruecker
	/// </summary>
	public class BpmnDeployer : AbstractDefinitionDeployer<ProcessDefinitionEntity>
	{

	  public static BpmnParseLogger LOG = ProcessEngineLogger.BPMN_PARSE_LOGGER;

	  public static readonly string[] BPMN_RESOURCE_SUFFIXES = new string[] {"bpmn20.xml", "bpmn"};

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected static final org.camunda.bpm.engine.impl.core.model.PropertyMapKey<String, java.util.List<org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?>>> JOB_DECLARATIONS_PROPERTY = new org.camunda.bpm.engine.impl.core.model.PropertyMapKey<String, java.util.List<org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?>>>("JOB_DECLARATIONS_PROPERTY");
	  protected internal static readonly PropertyMapKey<string, IList<JobDeclaration<object, ?>>> JOB_DECLARATIONS_PROPERTY = new PropertyMapKey<string, IList<JobDeclaration<object, ?>>>("JOB_DECLARATIONS_PROPERTY");

	  protected internal ExpressionManager expressionManager;
	  protected internal BpmnParser bpmnParser;

	  /// <summary>
	  /// <!> DON'T KEEP DEPLOYMENT-SPECIFIC STATE <!> * </summary>

	  protected internal override string[] ResourcesSuffixes
	  {
		  get
		  {
			return BPMN_RESOURCE_SUFFIXES;
		  }
	  }

	  protected internal override IList<ProcessDefinitionEntity> transformDefinitions(DeploymentEntity deployment, ResourceEntity resource, Properties properties)
	  {
		sbyte[] bytes = resource.Bytes;
		MemoryStream inputStream = new MemoryStream(bytes);

		BpmnParse bpmnParse = bpmnParser.createParse().sourceInputStream(inputStream).deployment(deployment).name(resource.Name);

		if (!deployment.ValidatingSchema)
		{
		  bpmnParse.SchemaResource = null;
		}

		bpmnParse.execute();

		if (!properties.contains(JOB_DECLARATIONS_PROPERTY))
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: properties.set(JOB_DECLARATIONS_PROPERTY, new java.util.HashMap<String, java.util.List<org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?>>>());
		  properties.set(JOB_DECLARATIONS_PROPERTY, new Dictionary<string, IList<JobDeclaration<object, ?>>>());
		}
		properties.get(JOB_DECLARATIONS_PROPERTY).putAll(bpmnParse.JobDeclarations);

		return bpmnParse.ProcessDefinitions;
	  }

	  protected internal override ProcessDefinitionEntity findDefinitionByDeploymentAndKey(string deploymentId, string definitionKey)
	  {
		return ProcessDefinitionManager.findProcessDefinitionByDeploymentAndKey(deploymentId, definitionKey);
	  }

	  protected internal override ProcessDefinitionEntity findLatestDefinitionByKeyAndTenantId(string definitionKey, string tenantId)
	  {
		return ProcessDefinitionManager.findLatestProcessDefinitionByKeyAndTenantId(definitionKey, tenantId);
	  }

	  protected internal override void persistDefinition(ProcessDefinitionEntity definition)
	  {
		ProcessDefinitionManager.insertProcessDefinition(definition);
	  }

	  protected internal override void addDefinitionToDeploymentCache(DeploymentCache deploymentCache, ProcessDefinitionEntity definition)
	  {
		deploymentCache.addProcessDefinition(definition);
	  }

	  protected internal override void definitionAddedToDeploymentCache(DeploymentEntity deployment, ProcessDefinitionEntity definition, Properties properties)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?>> declarations = properties.get(JOB_DECLARATIONS_PROPERTY).get(definition.getKey());
		IList<JobDeclaration<object, ?>> declarations = properties.get(JOB_DECLARATIONS_PROPERTY)[definition.Key];

		updateJobDeclarations(declarations, definition, deployment.New);

		ProcessDefinitionEntity latestDefinition = findLatestDefinitionByKeyAndTenantId(definition.Key, definition.TenantId);

		if (deployment.New)
		{
		  adjustStartEventSubscriptions(definition, latestDefinition);
		}

		// add "authorizations"
		addAuthorizations(definition);
	  }

	  protected internal override void persistedDefinitionLoaded(DeploymentEntity deployment, ProcessDefinitionEntity definition, ProcessDefinitionEntity persistedDefinition)
	  {
		definition.SuspensionState = persistedDefinition.SuspensionState;
	  }

	  protected internal override void handlePersistedDefinition(ProcessDefinitionEntity definition, ProcessDefinitionEntity persistedDefinition, DeploymentEntity deployment, Properties properties)
	  {
		//check if persisted definition is not null, since the process definition can be deleted by the user
		//in such cases we don't want to handle them
		//we can't do this in the parent method, since other siblings want to handle them like {@link DecisionDefinitionDeployer}
		if (persistedDefinition != null)
		{
		  base.handlePersistedDefinition(definition, persistedDefinition, deployment, properties);
		}
	  }

	  protected internal virtual void updateJobDeclarations<T1>(IList<T1> jobDeclarations, ProcessDefinitionEntity processDefinition, bool isNewDeployment)
	  {

		if (jobDeclarations == null || jobDeclarations.Count == 0)
		{
		  return;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager jobDefinitionManager = getJobDefinitionManager();
		JobDefinitionManager jobDefinitionManager = JobDefinitionManager;

		if (isNewDeployment)
		{
		  // create new job definitions:
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?> jobDeclaration : jobDeclarations)
		  foreach (JobDeclaration<object, ?> jobDeclaration in jobDeclarations)
		  {
			createJobDefinition(processDefinition, jobDeclaration);
		  }

		}
		else
		{
		  // query all job definitions and update the declarations with their Ids
		  IList<JobDefinitionEntity> existingDefinitions = jobDefinitionManager.findByProcessDefinitionId(processDefinition.Id);

		  LegacyBehavior.migrateMultiInstanceJobDefinitions(processDefinition, existingDefinitions);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, ?> jobDeclaration : jobDeclarations)
		  foreach (JobDeclaration<object, ?> jobDeclaration in jobDeclarations)
		  {
			bool jobDefinitionExists = false;
			foreach (JobDefinition jobDefinitionEntity in existingDefinitions)
			{

			  // <!> Assumption: there can be only one job definition per activity and type
			  if (jobDeclaration.ActivityId.Equals(jobDefinitionEntity.ActivityId) && jobDeclaration.JobHandlerType.Equals(jobDefinitionEntity.JobType))
			  {
				jobDeclaration.JobDefinitionId = jobDefinitionEntity.Id;
				jobDefinitionExists = true;
				break;
			  }
			}

			if (!jobDefinitionExists)
			{
			  // not found: create new definition
			  createJobDefinition(processDefinition, jobDeclaration);
			}

		  }
		}

	  }

	  protected internal virtual void createJobDefinition<T1>(ProcessDefinition processDefinition, JobDeclaration<T1> jobDeclaration)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager jobDefinitionManager = getJobDefinitionManager();
		JobDefinitionManager jobDefinitionManager = JobDefinitionManager;

		JobDefinitionEntity jobDefinitionEntity = new JobDefinitionEntity(jobDeclaration);
		jobDefinitionEntity.ProcessDefinitionId = processDefinition.Id;
		jobDefinitionEntity.ProcessDefinitionKey = processDefinition.Key;
		jobDefinitionEntity.TenantId = processDefinition.TenantId;
		jobDefinitionManager.insert(jobDefinitionEntity);
		jobDeclaration.JobDefinitionId = jobDefinitionEntity.Id;
	  }

	  /// <summary>
	  /// adjust all event subscriptions responsible to start process instances
	  /// (timer start event, message start event). The default behavior is to remove the old
	  /// subscriptions and add new ones for the new deployed process definitions.
	  /// </summary>
	  protected internal virtual void adjustStartEventSubscriptions(ProcessDefinitionEntity newLatestProcessDefinition, ProcessDefinitionEntity oldLatestProcessDefinition)
	  {
		  removeObsoleteTimers(newLatestProcessDefinition);
		  addTimerDeclarations(newLatestProcessDefinition);

		  removeObsoleteEventSubscriptions(newLatestProcessDefinition, oldLatestProcessDefinition);
		  addEventSubscriptions(newLatestProcessDefinition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addTimerDeclarations(org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition)
	  protected internal virtual void addTimerDeclarations(ProcessDefinitionEntity processDefinition)
	  {
		IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) processDefinition.getProperty(BpmnParse.PROPERTYNAME_START_TIMER);
		if (timerDeclarations != null)
		{
		  foreach (TimerDeclarationImpl timerDeclaration in timerDeclarations)
		  {
			string deploymentId = processDefinition.DeploymentId;
			timerDeclaration.createStartTimerInstance(deploymentId);
		  }
		}
	  }

	  protected internal virtual void removeObsoleteTimers(ProcessDefinitionEntity processDefinition)
	  {
		IList<JobEntity> jobsToDelete = JobManager.findJobsByConfiguration(TimerStartEventJobHandler.TYPE, processDefinition.Key, processDefinition.TenantId);

		foreach (JobEntity job in jobsToDelete)
		{
			(new DeleteJobsCmd(job.Id)).execute(Context.CommandContext);
		}
	  }

	  protected internal virtual void removeObsoleteEventSubscriptions(ProcessDefinitionEntity processDefinition, ProcessDefinitionEntity latestProcessDefinition)
	  {
		// remove all subscriptions for the previous version
		if (latestProcessDefinition != null)
		{
		  EventSubscriptionManager eventSubscriptionManager = EventSubscriptionManager;

		  IList<EventSubscriptionEntity> subscriptionsToDelete = new List<EventSubscriptionEntity>();

		  IList<EventSubscriptionEntity> messageEventSubscriptions = eventSubscriptionManager.findEventSubscriptionsByConfiguration(EventType.MESSAGE.name(), latestProcessDefinition.Id);
		  ((IList<EventSubscriptionEntity>)subscriptionsToDelete).AddRange(messageEventSubscriptions);

		  IList<EventSubscriptionEntity> signalEventSubscriptions = eventSubscriptionManager.findEventSubscriptionsByConfiguration(EventType.SIGNAL.name(), latestProcessDefinition.Id);
		  ((IList<EventSubscriptionEntity>)subscriptionsToDelete).AddRange(signalEventSubscriptions);

		  IList<EventSubscriptionEntity> conditionalEventSubscriptions = eventSubscriptionManager.findEventSubscriptionsByConfiguration(EventType.CONDITONAL.name(), latestProcessDefinition.Id);
		  ((IList<EventSubscriptionEntity>)subscriptionsToDelete).AddRange(conditionalEventSubscriptions);

		  foreach (EventSubscriptionEntity eventSubscriptionEntity in subscriptionsToDelete)
		  {
			eventSubscriptionEntity.delete();
		  }
		}
	  }

	  public virtual void addEventSubscriptions(ProcessDefinitionEntity processDefinition)
	  {
		IDictionary<string, EventSubscriptionDeclaration> eventDefinitions = processDefinition.Properties.get(BpmnProperties.EVENT_SUBSCRIPTION_DECLARATIONS);
		foreach (EventSubscriptionDeclaration eventDefinition in eventDefinitions.Values)
		{
		  addEventSubscription(processDefinition, eventDefinition);
		}
	  }

	  protected internal virtual void addEventSubscription(ProcessDefinitionEntity processDefinition, EventSubscriptionDeclaration eventDefinition)
	  {
		if (eventDefinition.StartEvent)
		{
		  string eventType = eventDefinition.EventType;

		  if (eventType.Equals(EventType.MESSAGE.name()))
		  {
			addMessageStartEventSubscription(eventDefinition, processDefinition);
		  }
		  else if (eventType.Equals(EventType.SIGNAL.name()))
		  {
			addSignalStartEventSubscription(eventDefinition, processDefinition);
		  }
		  else if (eventType.Equals(EventType.CONDITONAL.name()))
		  {
			addConditionalStartEventSubscription(eventDefinition, processDefinition);
		  }
		}
	  }

	  protected internal virtual void addMessageStartEventSubscription(EventSubscriptionDeclaration messageEventDefinition, ProcessDefinitionEntity processDefinition)
	  {

		string tenantId = processDefinition.TenantId;

		if (isSameMessageEventSubscriptionAlreadyPresent(messageEventDefinition, tenantId))
		{
		  throw LOG.messageEventSubscriptionWithSameNameExists(processDefinition.ResourceName, messageEventDefinition.UnresolvedEventName);
		}

		EventSubscriptionEntity newSubscription = messageEventDefinition.createSubscriptionForStartEvent(processDefinition);
		newSubscription.insert();

	  }

	  protected internal virtual bool isSameMessageEventSubscriptionAlreadyPresent(EventSubscriptionDeclaration eventSubscription, string tenantId)
	  {
		// look for subscriptions for the same name in db:
		IList<EventSubscriptionEntity> subscriptionsForSameMessageName = EventSubscriptionManager.findEventSubscriptionsByNameAndTenantId(EventType.MESSAGE.name(), eventSubscription.UnresolvedEventName, tenantId);

		// also look for subscriptions created in the session:
		IList<EventSubscriptionEntity> cachedSubscriptions = DbEntityManager.getCachedEntitiesByType(typeof(EventSubscriptionEntity));

		foreach (EventSubscriptionEntity cachedSubscription in cachedSubscriptions)
		{

		  if (eventSubscription.UnresolvedEventName.Equals(cachedSubscription.EventName) && hasTenantId(cachedSubscription, tenantId) && !subscriptionsForSameMessageName.Contains(cachedSubscription))
		  {

			subscriptionsForSameMessageName.Add(cachedSubscription);
		  }
		}

		// remove subscriptions deleted in the same command
		subscriptionsForSameMessageName = DbEntityManager.pruneDeletedEntities(subscriptionsForSameMessageName);

		// remove subscriptions for different type of event (i.e. remove intermediate message event subscriptions)
		subscriptionsForSameMessageName = filterSubscriptionsOfDifferentType(eventSubscription, subscriptionsForSameMessageName);

		return subscriptionsForSameMessageName.Count > 0;
	  }

	  protected internal virtual bool hasTenantId(EventSubscriptionEntity cachedSubscription, string tenantId)
	  {
		if (string.ReferenceEquals(tenantId, null))
		{
		  return string.ReferenceEquals(cachedSubscription.TenantId, null);
		}
		else
		{
		  return tenantId.Equals(cachedSubscription.TenantId);
		}
	  }

	  /// <summary>
	  /// It is possible to deploy a process containing a start and intermediate
	  /// message event that wait for the same message or to have two processes, one
	  /// with a message start event and the other one with a message intermediate
	  /// event, that subscribe for the same message. Therefore we have to find out
	  /// if there are subscriptions for the other type of event and remove those.
	  /// </summary>
	  /// <param name="eventSubscription"> </param>
	  /// <param name="subscriptionsForSameMessageName"> </param>
	  protected internal virtual IList<EventSubscriptionEntity> filterSubscriptionsOfDifferentType(EventSubscriptionDeclaration eventSubscription, IList<EventSubscriptionEntity> subscriptionsForSameMessageName)
	  {
		List<EventSubscriptionEntity> filteredSubscriptions = new List<EventSubscriptionEntity>(subscriptionsForSameMessageName);

		foreach (EventSubscriptionEntity subscriptionEntity in new List<EventSubscriptionEntity>(subscriptionsForSameMessageName))
		{

		  if (isSubscriptionOfDifferentTypeAsDeclaration(subscriptionEntity, eventSubscription))
		  {
			filteredSubscriptions.Remove(subscriptionEntity);
		  }
		}

		return filteredSubscriptions;
	  }

	  protected internal virtual bool isSubscriptionOfDifferentTypeAsDeclaration(EventSubscriptionEntity subscriptionEntity, EventSubscriptionDeclaration declaration)
	  {

		return (declaration.StartEvent && isSubscriptionForIntermediateEvent(subscriptionEntity)) || (!declaration.StartEvent && isSubscriptionForStartEvent(subscriptionEntity));
	  }

	  protected internal virtual bool isSubscriptionForStartEvent(EventSubscriptionEntity subscriptionEntity)
	  {
		return string.ReferenceEquals(subscriptionEntity.ExecutionId, null);
	  }

	  protected internal virtual bool isSubscriptionForIntermediateEvent(EventSubscriptionEntity subscriptionEntity)
	  {
		return !string.ReferenceEquals(subscriptionEntity.ExecutionId, null);
	  }

	  protected internal virtual void addSignalStartEventSubscription(EventSubscriptionDeclaration signalEventDefinition, ProcessDefinitionEntity processDefinition)
	  {
		EventSubscriptionEntity newSubscription = signalEventDefinition.createSubscriptionForStartEvent(processDefinition);

		newSubscription.insert();
	  }

	  protected internal virtual void addConditionalStartEventSubscription(EventSubscriptionDeclaration conditionalEventDefinition, ProcessDefinitionEntity processDefinition)
	  {
		EventSubscriptionEntity newSubscription = conditionalEventDefinition.createSubscriptionForStartEvent(processDefinition);

		newSubscription.insert();
	  }

	  internal enum ExprType
	  {
		  USER,
		  GROUP

	  }

	  protected internal virtual void addAuthorizationsFromIterator(ISet<Expression> exprSet, ProcessDefinitionEntity processDefinition, ExprType exprType)
	  {
		if (exprSet != null)
		{
		  foreach (Expression expr in exprSet)
		  {
			IdentityLinkEntity identityLink = new IdentityLinkEntity();
			identityLink.ProcessDef = processDefinition;
			if (exprType.Equals(ExprType.USER))
			{
			  identityLink.UserId = expr.ToString();
			}
			else if (exprType.Equals(ExprType.GROUP))
			{
			  identityLink.GroupId = expr.ToString();
			}
			identityLink.Type = IdentityLinkType.CANDIDATE;
			identityLink.TenantId = processDefinition.TenantId;
			identityLink.insert();
		  }
		}
	  }

	  protected internal virtual void addAuthorizations(ProcessDefinitionEntity processDefinition)
	  {
		addAuthorizationsFromIterator(processDefinition.CandidateStarterUserIdExpressions, processDefinition, ExprType.USER);
		addAuthorizationsFromIterator(processDefinition.CandidateStarterGroupIdExpressions, processDefinition, ExprType.GROUP);
	  }

	  // context ///////////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual DbEntityManager DbEntityManager
	  {
		  get
		  {
			return CommandContext.DbEntityManager;
		  }
	  }

	  protected internal virtual JobManager JobManager
	  {
		  get
		  {
			return CommandContext.JobManager;
		  }
	  }

	  protected internal virtual JobDefinitionManager JobDefinitionManager
	  {
		  get
		  {
			return CommandContext.JobDefinitionManager;
		  }
	  }

	  protected internal virtual EventSubscriptionManager EventSubscriptionManager
	  {
		  get
		  {
			return CommandContext.EventSubscriptionManager;
		  }
	  }

	  protected internal virtual ProcessDefinitionManager ProcessDefinitionManager
	  {
		  get
		  {
			return CommandContext.ProcessDefinitionManager;
		  }
	  }

	  // getters/setters ///////////////////////////////////////////////////////////////////////////////////

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	  public virtual BpmnParser BpmnParser
	  {
		  get
		  {
			return bpmnParser;
		  }
		  set
		  {
			this.bpmnParser = value;
		  }
	  }


	}

}