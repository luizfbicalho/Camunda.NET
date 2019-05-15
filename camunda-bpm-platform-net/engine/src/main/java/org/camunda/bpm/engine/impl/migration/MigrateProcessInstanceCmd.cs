using System.Collections.Generic;
using System.Threading;

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
namespace org.camunda.bpm.engine.impl.migration
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotContainsNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using CommandChecker = org.camunda.bpm.engine.impl.cfg.CommandChecker;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DeleteUnmappedInstanceVisitor = org.camunda.bpm.engine.impl.migration.instance.DeleteUnmappedInstanceVisitor;
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingActivityInstanceVisitor = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstanceVisitor;
	using MigratingCompensationEventSubscriptionInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingCompensationEventSubscriptionInstance;
	using MigratingEventScopeInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingEventScopeInstance;
	using MigratingProcessElementInstanceTopDownWalker = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessElementInstanceTopDownWalker;
	using MigratingProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance;
	using MigratingScopeInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingScopeInstance;
	using MigratingScopeInstanceBottomUpWalker = org.camunda.bpm.engine.impl.migration.instance.MigratingScopeInstanceBottomUpWalker;
	using MigratingTransitionInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingTransitionInstance;
	using MigrationCompensationInstanceVisitor = org.camunda.bpm.engine.impl.migration.instance.MigrationCompensationInstanceVisitor;
	using MigratingInstanceParser = org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParser;
	using MigratingActivityInstanceValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingActivityInstanceValidationReportImpl;
	using MigratingActivityInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingActivityInstanceValidator;
	using MigratingCompensationInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingCompensationInstanceValidator;
	using MigratingProcessInstanceValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingProcessInstanceValidationReportImpl;
	using MigratingTransitionInstanceValidationReportImpl = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingTransitionInstanceValidationReportImpl;
	using MigratingTransitionInstanceValidator = org.camunda.bpm.engine.impl.migration.validation.instance.MigratingTransitionInstanceValidator;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;

	/// <summary>
	/// How migration works:
	/// 
	/// <ol>
	///   <li>Validate migration instructions.
	///   <li>Delete activity instances that are not going to be migrated, invoking execution listeners
	///       and io mappings. This is performed in a bottom-up fashion in the activity instance tree and ensures
	///       that the "upstream" tree is always consistent with respect to the old process definition.
	///   <li>Migrate and create activity instances. Creation invokes execution listeners
	///       and io mappings. This is performed in a top-down fashion in the activity instance tree and
	///       ensures that the "upstream" tree is always consistent with respect to the new process definition.
	/// </ol>
	/// @author Thorben Lindhauer
	/// </summary>
	public class MigrateProcessInstanceCmd : AbstractMigrationCmd<Void>
	{

	  protected internal static readonly MigrationLogger LOGGER = ProcessEngineLogger.MIGRATION_LOGGER;

	  protected internal bool writeOperationLog;

	  public MigrateProcessInstanceCmd(MigrationPlanExecutionBuilderImpl migrationPlanExecutionBuilder, bool writeOperationLog) : base(migrationPlanExecutionBuilder)
	  {
		this.writeOperationLog = writeOperationLog;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
	  public override Void execute(CommandContext commandContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.migration.MigrationPlan migrationPlan = executionBuilder.getMigrationPlan();
		MigrationPlan migrationPlan = executionBuilder.MigrationPlan;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Collection<String> processInstanceIds = collectProcessInstanceIds(commandContext);
		ICollection<string> processInstanceIds = collectProcessInstanceIds(commandContext);

		ensureNotNull(typeof(BadUserRequestException), "Migration plan cannot be null", "migration plan", migrationPlan);
		ensureNotEmpty(typeof(BadUserRequestException), "Process instance ids cannot empty", "process instance ids", processInstanceIds);
		ensureNotContainsNull(typeof(BadUserRequestException), "Process instance ids cannot be null", "process instance ids", processInstanceIds);

		ProcessDefinitionEntity sourceDefinition = resolveSourceProcessDefinition(commandContext);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity targetDefinition = resolveTargetProcessDefinition(commandContext);
		ProcessDefinitionEntity targetDefinition = resolveTargetProcessDefinition(commandContext);

		checkAuthorizations(commandContext, sourceDefinition, targetDefinition, processInstanceIds);
		if (writeOperationLog)
		{
		  writeUserOperationLog(commandContext, sourceDefinition, targetDefinition, processInstanceIds.Count, false);
		}

		commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this, commandContext, migrationPlan, processInstanceIds, targetDefinition));

		return null;
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly MigrateProcessInstanceCmd outerInstance;

		  private CommandContext commandContext;
		  private MigrationPlan migrationPlan;
		  private ICollection<string> processInstanceIds;
		  private ProcessDefinitionEntity targetDefinition;

		  public CallableAnonymousInnerClass(MigrateProcessInstanceCmd outerInstance, CommandContext commandContext, MigrationPlan migrationPlan, ICollection<string> processInstanceIds, ProcessDefinitionEntity targetDefinition)
		  {
			  this.outerInstance = outerInstance;
			  this.commandContext = commandContext;
			  this.migrationPlan = migrationPlan;
			  this.processInstanceIds = processInstanceIds;
			  this.targetDefinition = targetDefinition;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			foreach (string processInstanceId in processInstanceIds)
			{
			  outerInstance.migrateProcessInstance(commandContext, processInstanceId, migrationPlan, targetDefinition);
			}
			return null;
		  }

	  }

	  public virtual Void migrateProcessInstance(CommandContext commandContext, string processInstanceId, MigrationPlan migrationPlan, ProcessDefinitionEntity targetProcessDefinition)
	  {
		ensureNotNull(typeof(BadUserRequestException), "Process instance id cannot be null", "process instance id", processInstanceId);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity processInstance = commandContext.getExecutionManager().findExecutionById(processInstanceId);
		ExecutionEntity processInstance = commandContext.ExecutionManager.findExecutionById(processInstanceId);

		ensureProcessInstanceExist(processInstanceId, processInstance);
		ensureOperationAllowed(commandContext, processInstance, targetProcessDefinition);
		ensureSameProcessDefinition(processInstance, migrationPlan.SourceProcessDefinitionId);

		MigratingProcessInstanceValidationReportImpl processInstanceReport = new MigratingProcessInstanceValidationReportImpl();

		// Initialize migration: match migration instructions to activity instances and collect required entities
		MigratingInstanceParser migratingInstanceParser = new MigratingInstanceParser(Context.ProcessEngineConfiguration.ProcessEngine);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.migration.instance.MigratingProcessInstance migratingProcessInstance = migratingInstanceParser.parse(processInstance.getId(), migrationPlan, processInstanceReport);
		MigratingProcessInstance migratingProcessInstance = migratingInstanceParser.parse(processInstance.Id, migrationPlan, processInstanceReport);

		validateInstructions(commandContext, migratingProcessInstance, processInstanceReport);

		if (processInstanceReport.hasFailures())
		{
		  throw LOGGER.failingMigratingProcessInstanceValidation(processInstanceReport);
		}

		executeInContext(() =>
		{
	  deleteUnmappedActivityInstances(migratingProcessInstance);
		}, migratingProcessInstance.SourceDefinition);

		executeInContext(() =>
		{
	  migrateProcessInstance(migratingProcessInstance);
		}, migratingProcessInstance.TargetDefinition);

		return null;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected <T> void executeInContext(final Runnable runnable, org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity contextDefinition)
	  protected internal virtual void executeInContext<T>(ThreadStart runnable, ProcessDefinitionEntity contextDefinition)
	  {
		ProcessApplicationContextUtil.doContextSwitch(runnable, contextDefinition);
	  }

	  /// <summary>
	  /// delete unmapped instances in a bottom-up fashion (similar to deleteCascade and regular BPMN execution)
	  /// </summary>
	  protected internal virtual void deleteUnmappedActivityInstances(MigratingProcessInstance migratingProcessInstance)
	  {
		ISet<MigratingScopeInstance> leafInstances = collectLeafInstances(migratingProcessInstance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.migration.instance.DeleteUnmappedInstanceVisitor visitor = new org.camunda.bpm.engine.impl.migration.instance.DeleteUnmappedInstanceVisitor(executionBuilder.isSkipCustomListeners(), executionBuilder.isSkipIoMappings());
		DeleteUnmappedInstanceVisitor visitor = new DeleteUnmappedInstanceVisitor(executionBuilder.SkipCustomListeners, executionBuilder.SkipIoMappings);

		foreach (MigratingScopeInstance leafInstance in leafInstances)
		{
		  MigratingScopeInstanceBottomUpWalker walker = new MigratingScopeInstanceBottomUpWalker(leafInstance);

		  walker.addPreVisitor(visitor);

		  walker.walkUntil(new WalkConditionAnonymousInnerClass(this, visitor));
		}
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<MigratingScopeInstance>
	  {
		  private readonly MigrateProcessInstanceCmd outerInstance;

		  private DeleteUnmappedInstanceVisitor visitor;

		  public WalkConditionAnonymousInnerClass(MigrateProcessInstanceCmd outerInstance, DeleteUnmappedInstanceVisitor visitor)
		  {
			  this.outerInstance = outerInstance;
			  this.visitor = visitor;
		  }


		  public bool isFulfilled(MigratingScopeInstance element)
		  {
			// walk until top of instance tree is reached or until
			// a node is reached for which we have not yet visited every child
			return element == null || !visitor.hasVisitedAll(element.ChildScopeInstances);
		  }
	  }

	  protected internal virtual ISet<MigratingScopeInstance> collectLeafInstances(MigratingProcessInstance migratingProcessInstance)
	  {
		ISet<MigratingScopeInstance> leafInstances = new HashSet<MigratingScopeInstance>();

		foreach (MigratingScopeInstance migratingScopeInstance in migratingProcessInstance.MigratingScopeInstances)
		{
		  if (migratingScopeInstance.ChildScopeInstances.Count == 0)
		  {
			leafInstances.Add(migratingScopeInstance);
		  }
		}

		return leafInstances;
	  }

	  protected internal virtual void validateInstructions(CommandContext commandContext, MigratingProcessInstance migratingProcessInstance, MigratingProcessInstanceValidationReportImpl processInstanceReport)
	  {
		IList<MigratingActivityInstanceValidator> migratingActivityInstanceValidators = commandContext.ProcessEngineConfiguration.MigratingActivityInstanceValidators;
		IList<MigratingTransitionInstanceValidator> migratingTransitionInstanceValidators = commandContext.ProcessEngineConfiguration.MigratingTransitionInstanceValidators;
		IList<MigratingCompensationInstanceValidator> migratingCompensationInstanceValidators = commandContext.ProcessEngineConfiguration.MigratingCompensationInstanceValidators;

		IDictionary<MigratingActivityInstance, MigratingActivityInstanceValidationReportImpl> instanceReports = new Dictionary<MigratingActivityInstance, MigratingActivityInstanceValidationReportImpl>();

		foreach (MigratingActivityInstance migratingActivityInstance in migratingProcessInstance.MigratingActivityInstances)
		{
		  MigratingActivityInstanceValidationReportImpl instanceReport = validateActivityInstance(migratingActivityInstance, migratingProcessInstance, migratingActivityInstanceValidators);
		  instanceReports[migratingActivityInstance] = instanceReport;
		}

		foreach (MigratingEventScopeInstance migratingEventScopeInstance in migratingProcessInstance.MigratingEventScopeInstances)
		{
		  MigratingActivityInstance ancestorInstance = migratingEventScopeInstance.ClosestAncestorActivityInstance;

		  validateEventScopeInstance(migratingEventScopeInstance, migratingProcessInstance, migratingCompensationInstanceValidators, instanceReports[ancestorInstance]);
		}

		foreach (MigratingCompensationEventSubscriptionInstance migratingEventSubscriptionInstance in migratingProcessInstance.MigratingCompensationSubscriptionInstances)
		{
		  MigratingActivityInstance ancestorInstance = migratingEventSubscriptionInstance.ClosestAncestorActivityInstance;

		  validateCompensateSubscriptionInstance(migratingEventSubscriptionInstance, migratingProcessInstance, migratingCompensationInstanceValidators, instanceReports[ancestorInstance]);
		}

		foreach (MigratingActivityInstanceValidationReportImpl instanceReport in instanceReports.Values)
		{
		  if (instanceReport.hasFailures())
		  {
			processInstanceReport.addActivityInstanceReport(instanceReport);
		  }
		}

		foreach (MigratingTransitionInstance migratingTransitionInstance in migratingProcessInstance.MigratingTransitionInstances)
		{
		  MigratingTransitionInstanceValidationReportImpl instanceReport = validateTransitionInstance(migratingTransitionInstance, migratingProcessInstance, migratingTransitionInstanceValidators);
		  if (instanceReport.hasFailures())
		  {
			processInstanceReport.addTransitionInstanceReport(instanceReport);
		  }
		}


	  }

	  protected internal virtual MigratingActivityInstanceValidationReportImpl validateActivityInstance(MigratingActivityInstance migratingActivityInstance, MigratingProcessInstance migratingProcessInstance, IList<MigratingActivityInstanceValidator> migratingActivityInstanceValidators)
	  {
		MigratingActivityInstanceValidationReportImpl instanceReport = new MigratingActivityInstanceValidationReportImpl(migratingActivityInstance);
		foreach (MigratingActivityInstanceValidator migratingActivityInstanceValidator in migratingActivityInstanceValidators)
		{
		  migratingActivityInstanceValidator.validate(migratingActivityInstance, migratingProcessInstance, instanceReport);
		}
		return instanceReport;
	  }

	  protected internal virtual MigratingTransitionInstanceValidationReportImpl validateTransitionInstance(MigratingTransitionInstance migratingTransitionInstance, MigratingProcessInstance migratingProcessInstance, IList<MigratingTransitionInstanceValidator> migratingTransitionInstanceValidators)
	  {
		MigratingTransitionInstanceValidationReportImpl instanceReport = new MigratingTransitionInstanceValidationReportImpl(migratingTransitionInstance);
		foreach (MigratingTransitionInstanceValidator migratingTransitionInstanceValidator in migratingTransitionInstanceValidators)
		{
		  migratingTransitionInstanceValidator.validate(migratingTransitionInstance, migratingProcessInstance, instanceReport);
		}
		return instanceReport;
	  }

	  protected internal virtual void validateEventScopeInstance(MigratingEventScopeInstance eventScopeInstance, MigratingProcessInstance migratingProcessInstance, IList<MigratingCompensationInstanceValidator> migratingTransitionInstanceValidators, MigratingActivityInstanceValidationReportImpl instanceReport)
	  {
		foreach (MigratingCompensationInstanceValidator validator in migratingTransitionInstanceValidators)
		{
		  validator.validate(eventScopeInstance, migratingProcessInstance, instanceReport);
		}
	  }

	  protected internal virtual void validateCompensateSubscriptionInstance(MigratingCompensationEventSubscriptionInstance eventSubscriptionInstance, MigratingProcessInstance migratingProcessInstance, IList<MigratingCompensationInstanceValidator> migratingTransitionInstanceValidators, MigratingActivityInstanceValidationReportImpl instanceReport)
	  {
		foreach (MigratingCompensationInstanceValidator validator in migratingTransitionInstanceValidators)
		{
		  validator.validate(eventSubscriptionInstance, migratingProcessInstance, instanceReport);
		}
	  }

	  /// <summary>
	  /// Migrate activity instances to their new activities and process definition. Creates new
	  /// scope instances as necessary.
	  /// </summary>
	  protected internal virtual void migrateProcessInstance(MigratingProcessInstance migratingProcessInstance)
	  {
		MigratingActivityInstance rootActivityInstance = migratingProcessInstance.RootInstance;

		MigratingProcessElementInstanceTopDownWalker walker = new MigratingProcessElementInstanceTopDownWalker(rootActivityInstance);

		walker.addPreVisitor(new MigratingActivityInstanceVisitor(executionBuilder.SkipCustomListeners, executionBuilder.SkipIoMappings));
		walker.addPreVisitor(new MigrationCompensationInstanceVisitor());

		walker.walkUntil();
	  }

	  protected internal virtual void ensureProcessInstanceExist(string processInstanceId, ExecutionEntity processInstance)
	  {
		if (processInstance == null)
		{
		  throw LOGGER.processInstanceDoesNotExist(processInstanceId);
		}
	  }

	  protected internal virtual void ensureSameProcessDefinition(ExecutionEntity processInstance, string processDefinitionId)
	  {
		if (!processDefinitionId.Equals(processInstance.ProcessDefinitionId))
		{
		  throw LOGGER.processDefinitionOfInstanceDoesNotMatchMigrationPlan(processInstance, processDefinitionId);
		}
	  }

	  protected internal virtual void ensureOperationAllowed(CommandContext commandContext, ExecutionEntity processInstance, ProcessDefinitionEntity targetProcessDefinition)
	  {
		foreach (CommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
		{
		  checker.checkMigrateProcessInstance(processInstance, targetProcessDefinition);
		}
	  }


	}

}