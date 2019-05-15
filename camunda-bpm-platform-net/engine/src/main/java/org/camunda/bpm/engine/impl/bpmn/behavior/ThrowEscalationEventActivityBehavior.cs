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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{

	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using Escalation = org.camunda.bpm.engine.impl.bpmn.parser.Escalation;
	using EscalationEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.EscalationEventDefinition;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmScope = org.camunda.bpm.engine.impl.pvm.PvmScope;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using ActivityExecutionHierarchyWalker = org.camunda.bpm.engine.impl.tree.ActivityExecutionHierarchyWalker;
	using ActivityExecutionMappingCollector = org.camunda.bpm.engine.impl.tree.ActivityExecutionMappingCollector;
	using ActivityExecutionTuple = org.camunda.bpm.engine.impl.tree.ActivityExecutionTuple;
	using OutputVariablesPropagator = org.camunda.bpm.engine.impl.tree.OutputVariablesPropagator;
	using ReferenceWalker = org.camunda.bpm.engine.impl.tree.ReferenceWalker;
	using TreeVisitor = org.camunda.bpm.engine.impl.tree.TreeVisitor;

	/// <summary>
	/// The activity behavior for an intermediate throwing escalation event and an escalation end event.
	/// 
	/// @author Philipp Ossler
	/// 
	/// </summary>
	public class ThrowEscalationEventActivityBehavior : AbstractBpmnActivityBehavior
	{

	  protected internal readonly Escalation escalation;

	  public ThrowEscalationEventActivityBehavior(Escalation escalation)
	  {
		this.escalation = escalation;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.pvm.PvmActivity currentActivity = execution.getActivity();
		PvmActivity currentActivity = execution.Activity;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final EscalationEventDefinitionFinder escalationEventDefinitionFinder = new EscalationEventDefinitionFinder(escalation.getEscalationCode(), currentActivity);
		EscalationEventDefinitionFinder escalationEventDefinitionFinder = new EscalationEventDefinitionFinder(this, escalation.EscalationCode, currentActivity);
		ActivityExecutionMappingCollector activityExecutionMappingCollector = new ActivityExecutionMappingCollector(execution);

		ActivityExecutionHierarchyWalker walker = new ActivityExecutionHierarchyWalker(execution);
		walker.addScopePreVisitor(escalationEventDefinitionFinder);
		walker.addExecutionPreVisitor(activityExecutionMappingCollector);
		walker.addExecutionPreVisitor(new OutputVariablesPropagator());

		walker.walkUntil(new WalkConditionAnonymousInnerClass(this, escalationEventDefinitionFinder));

		EscalationEventDefinition escalationEventDefinition = escalationEventDefinitionFinder.EscalationEventDefinition;
		if (escalationEventDefinition != null)
		{
		  executeEscalationHandler(escalationEventDefinition, activityExecutionMappingCollector);
		}

		if (escalationEventDefinition == null || !escalationEventDefinition.CancelActivity)
		{
		  leaveExecution(execution, currentActivity, escalationEventDefinition);
		}
	  }

	  private class WalkConditionAnonymousInnerClass : ReferenceWalker.WalkCondition<ActivityExecutionTuple>
	  {
		  private readonly ThrowEscalationEventActivityBehavior outerInstance;

		  private org.camunda.bpm.engine.impl.bpmn.behavior.ThrowEscalationEventActivityBehavior.EscalationEventDefinitionFinder escalationEventDefinitionFinder;

		  public WalkConditionAnonymousInnerClass(ThrowEscalationEventActivityBehavior outerInstance, org.camunda.bpm.engine.impl.bpmn.behavior.ThrowEscalationEventActivityBehavior.EscalationEventDefinitionFinder escalationEventDefinitionFinder)
		  {
			  this.outerInstance = outerInstance;
			  this.escalationEventDefinitionFinder = escalationEventDefinitionFinder;
		  }


		  public bool isFulfilled(ActivityExecutionTuple element)
		  {
			return escalationEventDefinitionFinder.EscalationEventDefinition != null || element == null;
		  }
	  }

	  protected internal virtual void executeEscalationHandler(EscalationEventDefinition escalationEventDefinition, ActivityExecutionMappingCollector activityExecutionMappingCollector)
	  {

		PvmActivity escalationHandler = escalationEventDefinition.EscalationHandler;
		PvmScope escalationScope = getScopeForEscalation(escalationEventDefinition);
		ActivityExecution escalationExecution = activityExecutionMappingCollector.getExecutionForScope(escalationScope);

		if (!string.ReferenceEquals(escalationEventDefinition.EscalationCodeVariable, null))
		{
		  escalationExecution.setVariable(escalationEventDefinition.EscalationCodeVariable, escalation.EscalationCode);
		}

		escalationExecution.executeActivity(escalationHandler);
	  }

	  protected internal virtual PvmScope getScopeForEscalation(EscalationEventDefinition escalationEventDefinition)
	  {
		PvmActivity escalationHandler = escalationEventDefinition.EscalationHandler;
		if (escalationEventDefinition.CancelActivity)
		{
		  return escalationHandler.EventScope;
		}
		else
		{
		  return escalationHandler.FlowScope;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void leaveExecution(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, final org.camunda.bpm.engine.impl.pvm.PvmActivity currentActivity, org.camunda.bpm.engine.impl.bpmn.parser.EscalationEventDefinition escalationEventDefinition)
	  protected internal virtual void leaveExecution(ActivityExecution execution, PvmActivity currentActivity, EscalationEventDefinition escalationEventDefinition)
	  {

		// execution tree could have been expanded by triggering a non-interrupting event
		ExecutionEntity replacingExecution = ((ExecutionEntity) execution).ReplacedBy;

		ExecutionEntity leavingExecution = (ExecutionEntity)(replacingExecution != null ? replacingExecution : execution);
		leave(leavingExecution);
	  }

	  protected internal class EscalationEventDefinitionFinder : TreeVisitor<PvmScope>
	  {
		  private readonly ThrowEscalationEventActivityBehavior outerInstance;


		protected internal EscalationEventDefinition escalationEventDefinition;

		protected internal readonly string escalationCode;
		protected internal readonly PvmActivity throwEscalationActivity;

		public EscalationEventDefinitionFinder(ThrowEscalationEventActivityBehavior outerInstance, string escalationCode, PvmActivity throwEscalationActivity)
		{
			this.outerInstance = outerInstance;
		  this.escalationCode = escalationCode;
		  this.throwEscalationActivity = throwEscalationActivity;
		}

		public virtual void visit(PvmScope scope)
		{
		  IList<EscalationEventDefinition> escalationEventDefinitions = scope.Properties.get(BpmnProperties.ESCALATION_EVENT_DEFINITIONS);
		  this.escalationEventDefinition = findMatchingEscalationEventDefinition(escalationEventDefinitions);
		}

		protected internal virtual EscalationEventDefinition findMatchingEscalationEventDefinition(IList<EscalationEventDefinition> escalationEventDefinitions)
		{
		  foreach (EscalationEventDefinition escalationEventDefinition in escalationEventDefinitions)
		  {
			if (isMatchingEscalationCode(escalationEventDefinition) && !isReThrowingEscalationEventSubprocess(escalationEventDefinition))
			{
			  return escalationEventDefinition;
			}
		  }
		  return null;
		}

		protected internal virtual bool isMatchingEscalationCode(EscalationEventDefinition escalationEventDefinition)
		{
		  string escalationCode = escalationEventDefinition.EscalationCode;
		  return string.ReferenceEquals(escalationCode, null) || escalationCode.Equals(this.escalationCode);
		}

		protected internal virtual bool isReThrowingEscalationEventSubprocess(EscalationEventDefinition escalationEventDefinition)
		{
		  PvmActivity escalationHandler = escalationEventDefinition.EscalationHandler;
		  return escalationHandler.SubProcessScope && escalationHandler.Equals(throwEscalationActivity.FlowScope);
		}

		public virtual EscalationEventDefinition EscalationEventDefinition
		{
			get
			{
			  return escalationEventDefinition;
			}
		}

	  }

	}

}