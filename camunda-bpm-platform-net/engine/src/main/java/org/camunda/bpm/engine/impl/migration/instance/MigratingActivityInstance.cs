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
namespace org.camunda.bpm.engine.impl.migration.instance
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;
	using MigrationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.MigrationObserverBehavior;
	using ModificationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ModificationObserverBehavior;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigratingActivityInstance : MigratingScopeInstance, MigratingInstance
	{

	  public static readonly MigrationLogger MIGRATION_LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal ActivityInstance activityInstance;
	  // scope execution for actual scopes,
	  // concurrent execution in case of non-scope activity with expanded tree
	  protected internal ExecutionEntity representativeExecution;
	  protected internal bool activeState;

	  protected internal IList<RemovingInstance> removingDependentInstances = new List<RemovingInstance>();
	  protected internal IList<MigratingInstance> migratingDependentInstances = new List<MigratingInstance>();
	  protected internal IList<EmergingInstance> emergingDependentInstances = new List<EmergingInstance>();

	  protected internal ISet<MigratingActivityInstance> childActivityInstances = new HashSet<MigratingActivityInstance>();
	  protected internal ISet<MigratingTransitionInstance> childTransitionInstances = new HashSet<MigratingTransitionInstance>();
	  protected internal ISet<MigratingEventScopeInstance> childCompensationInstances = new HashSet<MigratingEventScopeInstance>();
	  protected internal ISet<MigratingCompensationEventSubscriptionInstance> childCompensationSubscriptionInstances = new HashSet<MigratingCompensationEventSubscriptionInstance>();

	  // behaves differently if the current activity is scope or not
	  protected internal MigratingActivityInstanceBehavior instanceBehavior;

	  /// <summary>
	  /// Creates a migrating activity instances
	  /// </summary>
	  public MigratingActivityInstance(ActivityInstance activityInstance, MigrationInstruction migrationInstruction, ScopeImpl sourceScope, ScopeImpl targetScope, ExecutionEntity scopeExecution)
	  {

		this.activityInstance = activityInstance;
		this.migrationInstruction = migrationInstruction;
		this.sourceScope = sourceScope;
		this.currentScope = sourceScope;
		this.targetScope = targetScope;
		this.representativeExecution = scopeExecution;
		this.instanceBehavior = determineBehavior(sourceScope);

		if (activityInstance.ChildActivityInstances.Length == 0 && activityInstance.ChildTransitionInstances.Length == 0)
		{
		  // active state is only relevant for child activity instances;
		  // for all other instances, their respective executions are always inactive
		  activeState = representativeExecution.Active;
		}
	  }

	  /// <summary>
	  /// Creates an emerged activity instance
	  /// </summary>
	  public MigratingActivityInstance(ScopeImpl targetScope, ExecutionEntity scopeExecution)
	  {

		this.targetScope = targetScope;
		this.currentScope = targetScope;
		this.representativeExecution = scopeExecution;
		this.instanceBehavior = determineBehavior(targetScope);
	  }


	  protected internal virtual MigratingActivityInstanceBehavior determineBehavior(ScopeImpl scope)
	  {
		if (scope.Scope)
		{
		  return new MigratingScopeActivityInstanceBehavior(this);
		}
		else
		{
		  return new MigratingNonScopeActivityInstanceBehavior(this);
		}
	  }

	  public override void detachChildren()
	  {
		ISet<MigratingActivityInstance> childrenCopy = new HashSet<MigratingActivityInstance>(childActivityInstances);
		// First detach all dependent entities, only then detach the activity instances.
		// This is because detaching activity instances may trigger execution tree compaction which in turn
		// may overwrite certain dependent entities (e.g. variables)
		foreach (MigratingActivityInstance child in childrenCopy)
		{
		  child.detachDependentInstances();
		}

		foreach (MigratingActivityInstance child in childrenCopy)
		{
		  child.detachState();
		}

		ISet<MigratingTransitionInstance> transitionChildrenCopy = new HashSet<MigratingTransitionInstance>(childTransitionInstances);
		foreach (MigratingTransitionInstance child in transitionChildrenCopy)
		{
		  child.detachState();
		}

		ISet<MigratingEventScopeInstance> compensationChildrenCopy = new HashSet<MigratingEventScopeInstance>(childCompensationInstances);
		foreach (MigratingEventScopeInstance child in compensationChildrenCopy)
		{
		  child.detachState();
		}

		ISet<MigratingCompensationEventSubscriptionInstance> compensationSubscriptionsChildrenCopy = new HashSet<MigratingCompensationEventSubscriptionInstance>(childCompensationSubscriptionInstances);
		foreach (MigratingCompensationEventSubscriptionInstance child in compensationSubscriptionsChildrenCopy)
		{
		  child.detachState();
		}
	  }

	  public virtual void detachDependentInstances()
	  {
		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  if (!dependentInstance.Detached)
		  {
			dependentInstance.detachState();
		  }
		}
	  }

	  public override bool Detached
	  {
		  get
		  {
			return instanceBehavior.Detached;
		  }
	  }

	  public override void detachState()
	  {

		detachDependentInstances();

		instanceBehavior.detachState();

		setParent(null);
	  }

	  public override void attachState(MigratingScopeInstance activityInstance)
	  {

		this.setParent(activityInstance);
		instanceBehavior.attachState();

		foreach (MigratingInstance dependentInstance in migratingDependentInstances)
		{
		  dependentInstance.attachState(this);
		}
	  }

	  public override void attachState(MigratingTransitionInstance targetTransitionInstance)
	  {
		throw MIGRATION_LOGGER.cannotAttachToTransitionInstance(this);
	  }

	  public override void migrateDependentEntities()
	  {
		foreach (MigratingInstance migratingInstance in migratingDependentInstances)
		{
		  migratingInstance.migrateState();
		  migratingInstance.migrateDependentEntities();
		}

		ExecutionEntity representativeExecution = resolveRepresentativeExecution();
		foreach (EmergingInstance emergingInstance in emergingDependentInstances)
		{
		  emergingInstance.create(representativeExecution);
		}
	  }

	  public override ExecutionEntity resolveRepresentativeExecution()
	  {
		return instanceBehavior.resolveRepresentativeExecution();
	  }

	  public override void addMigratingDependentInstance(MigratingInstance migratingInstance)
	  {
		migratingDependentInstances.Add(migratingInstance);
	  }

	  public virtual IList<MigratingInstance> MigratingDependentInstances
	  {
		  get
		  {
			return migratingDependentInstances;
		  }
	  }

	  public virtual void addRemovingDependentInstance(RemovingInstance removingInstance)
	  {
		removingDependentInstances.Add(removingInstance);
	  }

	  public virtual void addEmergingDependentInstance(EmergingInstance emergingInstance)
	  {
		emergingDependentInstances.Add(emergingInstance);
	  }

	  public virtual void addChild(MigratingTransitionInstance transitionInstance)
	  {
		this.childTransitionInstances.Add(transitionInstance);
	  }

	  public virtual void removeChild(MigratingTransitionInstance transitionInstance)
	  {
		this.childTransitionInstances.remove(transitionInstance);
	  }

	  public virtual void addChild(MigratingActivityInstance activityInstance)
	  {
		this.childActivityInstances.Add(activityInstance);
	  }

	  public virtual void removeChild(MigratingActivityInstance activityInstance)
	  {
		this.childActivityInstances.remove(activityInstance);
	  }

	  public override void addChild(MigratingScopeInstance migratingActivityInstance)
	  {
		if (migratingActivityInstance is MigratingActivityInstance)
		{
		  addChild((MigratingActivityInstance) migratingActivityInstance);
		}
		else if (migratingActivityInstance is MigratingEventScopeInstance)
		{
		  addChild((MigratingEventScopeInstance) migratingActivityInstance);
		}
		else
		{
		  throw MIGRATION_LOGGER.cannotHandleChild(this, migratingActivityInstance);
		}
	  }

	  public override void removeChild(MigratingScopeInstance child)
	  {
		if (child is MigratingActivityInstance)
		{
		  removeChild((MigratingActivityInstance) child);
		}
		else if (child is MigratingEventScopeInstance)
		{
		  removeChild((MigratingEventScopeInstance) child);
		}
		else
		{
		  throw MIGRATION_LOGGER.cannotHandleChild(this, child);
		}
	  }

	  public virtual void addChild(MigratingEventScopeInstance compensationInstance)
	  {
		this.childCompensationInstances.Add(compensationInstance);
	  }

	  public virtual void removeChild(MigratingEventScopeInstance compensationInstance)
	  {
		this.childCompensationInstances.remove(compensationInstance);
	  }

	  public override void addChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription)
	  {
		this.childCompensationSubscriptionInstances.Add(migratingEventSubscription);
	  }

	  public override void removeChild(MigratingCompensationEventSubscriptionInstance migratingEventSubscription)
	  {
		this.childCompensationSubscriptionInstances.remove(migratingEventSubscription);
	  }

	  public virtual ActivityInstance ActivityInstance
	  {
		  get
		  {
			return activityInstance;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			if (activityInstance != null)
			{
			  return activityInstance.Id;
			}
			else
			{
			  // - this branch is only executed for emerging activity instances
			  // - emerging activity instances are never leaf activities
			  // - therefore it is fine to always look up the activity instance id on the parent
			  ExecutionEntity execution = resolveRepresentativeExecution();
			  return execution.ParentActivityInstanceId;
			}
		  }
	  }

	  public override MigratingActivityInstance getParent()
	  {
		return (MigratingActivityInstance) base.Parent;
	  }

	  /// <summary>
	  /// Returns a copy of all children, modifying the returned set does not have any further effect.
	  /// </summary>
	  public override ISet<MigratingProcessElementInstance> Children
	  {
		  get
		  {
			ISet<MigratingProcessElementInstance> childInstances = new HashSet<MigratingProcessElementInstance>();
			childInstances.addAll(childActivityInstances);
			childInstances.addAll(childTransitionInstances);
			childInstances.addAll(childCompensationInstances);
			childInstances.addAll(childCompensationSubscriptionInstances);
			return childInstances;
		  }
	  }

	  public override ICollection<MigratingScopeInstance> ChildScopeInstances
	  {
		  get
		  {
			ISet<MigratingScopeInstance> childInstances = new HashSet<MigratingScopeInstance>();
			childInstances.addAll(childActivityInstances);
			childInstances.addAll(childCompensationInstances);
			return childInstances;
		  }
	  }

	  public virtual ISet<MigratingActivityInstance> ChildActivityInstances
	  {
		  get
		  {
			return childActivityInstances;
		  }
	  }

	  public virtual ISet<MigratingTransitionInstance> ChildTransitionInstances
	  {
		  get
		  {
			return childTransitionInstances;
		  }
	  }

	  public virtual ISet<MigratingEventScopeInstance> ChildCompensationInstances
	  {
		  get
		  {
			return childCompensationInstances;
		  }
	  }

	  public override bool migrates()
	  {
		return targetScope != null;
	  }

	  public override void removeUnmappedDependentInstances()
	  {
		foreach (RemovingInstance removingInstance in removingDependentInstances)
		{
		  removingInstance.remove();
		}
	  }

	  public override void remove(bool skipCustomListeners, bool skipIoMappings)
	  {
		instanceBehavior.remove(skipCustomListeners, skipIoMappings);
	  }

	  public override void migrateState()
	  {
		instanceBehavior.migrateState();
	  }

	  protected internal virtual void migrateHistory(DelegateExecution execution)
	  {
		if (activityInstance.Id.Equals(activityInstance.ProcessInstanceId))
		{
		  migrateProcessInstanceHistory(execution);
		}
		else
		{
		  migrateActivityInstanceHistory(execution);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void migrateProcessInstanceHistory(final org.camunda.bpm.engine.delegate.DelegateExecution execution)
	  protected internal virtual void migrateProcessInstanceHistory(DelegateExecution execution)
	  {
		HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
		if (!historyLevel.isHistoryEventProduced(HistoryEventTypes.PROCESS_INSTANCE_MIGRATE, this))
		{
		  return;
		}

		HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, execution));
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly MigratingActivityInstance outerInstance;

		  private DelegateExecution execution;

		  public HistoryEventCreatorAnonymousInnerClass(MigratingActivityInstance outerInstance, DelegateExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createProcessInstanceUpdateEvt(execution);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void migrateActivityInstanceHistory(final org.camunda.bpm.engine.delegate.DelegateExecution execution)
	  protected internal virtual void migrateActivityInstanceHistory(DelegateExecution execution)
	  {
		HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
		if (!historyLevel.isHistoryEventProduced(HistoryEventTypes.ACTIVITY_INSTANCE_MIGRATE, this))
		{
		  return;
		}

		HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass2(this));
	  }

	  private class HistoryEventCreatorAnonymousInnerClass2 : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly MigratingActivityInstance outerInstance;

		  public HistoryEventCreatorAnonymousInnerClass2(MigratingActivityInstance outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createActivityInstanceMigrateEvt(outerInstance);
		  }
	  }

	  public virtual ExecutionEntity createAttachableExecution()
	  {
		return instanceBehavior.createAttachableExecution();
	  }

	  public virtual void destroyAttachableExecution(ExecutionEntity execution)
	  {
		instanceBehavior.destroyAttachableExecution(execution);
	  }

	  public override void setParent(MigratingScopeInstance parentInstance)
	  {
		if (this.parentInstance != null)
		{
		  this.parentInstance.removeChild(this);
		}

		this.parentInstance = parentInstance;

		if (parentInstance != null)
		{
		  parentInstance.addChild(this);
		}
	  }


	  protected internal interface MigratingActivityInstanceBehavior
	  {

		bool Detached {get;}

		void detachState();

		void attachState();

		void migrateState();

		void remove(bool skipCustomListeners, bool skipIoMappings);

		ExecutionEntity resolveRepresentativeExecution();

		ExecutionEntity createAttachableExecution();

		void destroyAttachableExecution(ExecutionEntity execution);
	  }

	  protected internal class MigratingNonScopeActivityInstanceBehavior : MigratingActivityInstanceBehavior
	  {
		  private readonly MigratingActivityInstance outerInstance;

		  public MigratingNonScopeActivityInstanceBehavior(MigratingActivityInstance outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public virtual bool Detached
		{
			get
			{
			  return resolveRepresentativeExecution().getActivity() == null;
			}
		}

		public virtual void detachState()
		{
		  ExecutionEntity currentExecution = resolveRepresentativeExecution();

		  currentExecution.setActivity(null);
		  currentExecution.leaveActivityInstance();
		  currentExecution.Active = false;

		  outerInstance.getParent().destroyAttachableExecution(currentExecution);
		}

		public virtual void attachState()
		{

		  outerInstance.representativeExecution = outerInstance.getParent().createAttachableExecution();

		  outerInstance.representativeExecution.setActivity((PvmActivity) outerInstance.sourceScope);
		  outerInstance.representativeExecution.ActivityInstanceId = outerInstance.activityInstance.Id;
		  outerInstance.representativeExecution.Active = outerInstance.activeState;

		}

		public virtual void migrateState()
		{
		  ExecutionEntity currentExecution = resolveRepresentativeExecution();
		  currentExecution.setProcessDefinition(outerInstance.targetScope.ProcessDefinition);
		  currentExecution.setActivity((PvmActivity) outerInstance.targetScope);

		  outerInstance.currentScope = outerInstance.targetScope;

		  if (outerInstance.targetScope.Scope)
		  {
			becomeScope();
		  }

		  outerInstance.migrateHistory(currentExecution);
		}

		protected internal virtual void becomeScope()
		{
		  foreach (MigratingInstance dependentInstance in outerInstance.migratingDependentInstances)
		  {
			dependentInstance.detachState();
		  }

		  ExecutionEntity currentExecution = resolveRepresentativeExecution();

		  currentExecution = currentExecution.createExecution();
		  ExecutionEntity parent = currentExecution.Parent;
		  parent.setActivity(null);

		  if (!parent.Concurrent)
		  {
			parent.leaveActivityInstance();
		  }

		  outerInstance.representativeExecution = currentExecution;
		  foreach (MigratingInstance dependentInstance in outerInstance.migratingDependentInstances)
		  {
			dependentInstance.attachState(outerInstance);
		  }

		  outerInstance.instanceBehavior = new MigratingScopeActivityInstanceBehavior(outerInstance);
		}

		public virtual ExecutionEntity resolveRepresentativeExecution()
		{
		  if (outerInstance.representativeExecution.ReplacedBy != null)
		  {
			return outerInstance.representativeExecution.resolveReplacedBy();
		  }
		  else
		  {
			return outerInstance.representativeExecution;
		  }
		}

		public virtual void remove(bool skipCustomListeners, bool skipIoMappings)
		{
		  // nothing to do; we don't remove non-scope instances
		}

		public virtual ExecutionEntity createAttachableExecution()
		{
		  throw MIGRATION_LOGGER.cannotBecomeSubordinateInNonScope(outerInstance);
		}

		public virtual void destroyAttachableExecution(ExecutionEntity execution)
		{
		  throw MIGRATION_LOGGER.cannotDestroySubordinateInNonScope(outerInstance);
		}
	  }

	  protected internal class MigratingScopeActivityInstanceBehavior : MigratingActivityInstanceBehavior
	  {
		  private readonly MigratingActivityInstance outerInstance;

		  public MigratingScopeActivityInstanceBehavior(MigratingActivityInstance outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		public virtual bool Detached
		{
			get
			{
			  ExecutionEntity representativeExecution = resolveRepresentativeExecution();
			  return representativeExecution != representativeExecution.getProcessInstance() && representativeExecution.Parent == null;
			}
		}

		public virtual void detachState()
		{
		  ExecutionEntity currentScopeExecution = resolveRepresentativeExecution();

		  ExecutionEntity parentExecution = currentScopeExecution.Parent;
		  currentScopeExecution.Parent = null;

		  if (outerInstance.sourceScope.ActivityBehavior is CompositeActivityBehavior)
		  {
			parentExecution.leaveActivityInstance();
		  }

		  outerInstance.getParent().destroyAttachableExecution(parentExecution);
		}

		public virtual void attachState()
		{
		  ExecutionEntity newParentExecution = outerInstance.getParent().createAttachableExecution();

		  ExecutionEntity currentScopeExecution = resolveRepresentativeExecution();
		  currentScopeExecution.Parent = newParentExecution;

		  if (outerInstance.sourceScope.ActivityBehavior is CompositeActivityBehavior)
		  {
			newParentExecution.ActivityInstanceId = outerInstance.activityInstance.Id;
		  }

		}

		public virtual void migrateState()
		{
		  ExecutionEntity currentScopeExecution = resolveRepresentativeExecution();
		  currentScopeExecution.setProcessDefinition(outerInstance.targetScope.ProcessDefinition);

		  ExecutionEntity parentExecution = currentScopeExecution.Parent;

		  if (parentExecution != null && parentExecution.Concurrent)
		  {
			parentExecution.setProcessDefinition(outerInstance.targetScope.ProcessDefinition);
		  }

		  outerInstance.currentScope = outerInstance.targetScope;

		  if (!outerInstance.targetScope.Scope)
		  {
			becomeNonScope();
			currentScopeExecution = resolveRepresentativeExecution();
		  }

		  if (isLeafActivity(outerInstance.targetScope))
		  {
			currentScopeExecution.setActivity((PvmActivity) outerInstance.targetScope);
		  }

		  if (outerInstance.sourceScope.ActivityBehavior is MigrationObserverBehavior)
		  {
			((MigrationObserverBehavior) outerInstance.sourceScope.ActivityBehavior).migrateScope(currentScopeExecution);
		  }

		  outerInstance.migrateHistory(currentScopeExecution);
		}

		protected internal virtual void becomeNonScope()
		{
		  foreach (MigratingInstance dependentInstance in outerInstance.migratingDependentInstances)
		  {
			dependentInstance.detachState();
		  }

		  ExecutionEntity parentExecution = outerInstance.representativeExecution.Parent;

		  parentExecution.setActivity(outerInstance.representativeExecution.getActivity());
		  parentExecution.ActivityInstanceId = outerInstance.representativeExecution.ActivityInstanceId;

		  outerInstance.representativeExecution.remove();
		  outerInstance.representativeExecution = parentExecution;

		  foreach (MigratingInstance dependentInstance in outerInstance.migratingDependentInstances)
		  {
			dependentInstance.attachState(outerInstance);
		  }

		  outerInstance.instanceBehavior = new MigratingNonScopeActivityInstanceBehavior(outerInstance);
		}

		protected internal virtual bool isLeafActivity(ScopeImpl scope)
		{
		  return scope.Activities.Count == 0;
		}

		public virtual ExecutionEntity resolveRepresentativeExecution()
		{
		  return outerInstance.representativeExecution;
		}

		public virtual void remove(bool skipCustomListeners, bool skipIoMappings)
		{

		  ExecutionEntity currentExecution = resolveRepresentativeExecution();
		  ExecutionEntity parentExecution = currentExecution.Parent;

		  currentExecution.setActivity((PvmActivity) outerInstance.sourceScope);
		  currentExecution.ActivityInstanceId = outerInstance.activityInstance.Id;

		  currentExecution.deleteCascade("migration", skipCustomListeners, skipIoMappings);

		  outerInstance.getParent().destroyAttachableExecution(parentExecution);

		  outerInstance.setParent(null);
		  foreach (MigratingTransitionInstance child in outerInstance.childTransitionInstances)
		  {
			child.setParent(null);
		  }
		  foreach (MigratingActivityInstance child in outerInstance.childActivityInstances)
		  {
			child.setParent(null);
		  }
		  foreach (MigratingEventScopeInstance child in outerInstance.childCompensationInstances)
		  {
			child.Parent = null;
		  }
		}

		public virtual ExecutionEntity createAttachableExecution()
		{
		  ExecutionEntity scopeExecution = resolveRepresentativeExecution();
		  ExecutionEntity attachableExecution = scopeExecution;

		  if (outerInstance.currentScope.ActivityBehavior is ModificationObserverBehavior)
		  {
			ModificationObserverBehavior behavior = (ModificationObserverBehavior) outerInstance.currentScope.ActivityBehavior;
			attachableExecution = (ExecutionEntity) behavior.createInnerInstance(scopeExecution);
		  }
		  else
		  {
			if (scopeExecution.NonEventScopeExecutions.Count > 0 || scopeExecution.getActivity() != null)
			{
			  attachableExecution = (ExecutionEntity) scopeExecution.createConcurrentExecution();
			  attachableExecution.Active = false;
			  scopeExecution.forceUpdate();
			}
		  }

		  return attachableExecution;
		}

		public virtual void destroyAttachableExecution(ExecutionEntity execution)
		{

		  if (outerInstance.currentScope.ActivityBehavior is ModificationObserverBehavior)
		  {
			ModificationObserverBehavior behavior = (ModificationObserverBehavior) outerInstance.currentScope.ActivityBehavior;
			behavior.destroyInnerInstance(execution);
		  }
		  else
		  {
			if (execution.Concurrent)
			{
			  execution.remove();
			  execution.Parent.tryPruneLastConcurrentChild();
			  execution.Parent.forceUpdate();
			}
		  }
		}
	  }
	}



}