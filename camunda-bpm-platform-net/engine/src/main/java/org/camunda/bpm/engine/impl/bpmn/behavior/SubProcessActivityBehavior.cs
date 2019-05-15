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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using CompensationUtil = org.camunda.bpm.engine.impl.bpmn.helper.CompensationUtil;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using CompositeActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.CompositeActivityBehavior;


	/// <summary>
	/// Implementation of the BPMN 2.0 subprocess (formally known as 'embedded' subprocess):
	/// a subprocess defined within another process definition.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class SubProcessActivityBehavior : AbstractBpmnActivityBehavior, CompositeActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		PvmActivity activity = execution.Activity;
		PvmActivity initialActivity = activity.Properties.get(BpmnProperties.INITIAL_ACTIVITY);

		ensureNotNull("No initial activity found for subprocess " + execution.Activity.Id, "initialActivity", initialActivity);

		execution.executeActivity(initialActivity);
	  }

	  public virtual void concurrentChildExecutionEnded(ActivityExecution scopeExecution, ActivityExecution endedExecution)
	  {
		// join
		endedExecution.remove();
		scopeExecution.tryPruneLastConcurrentChild();
		scopeExecution.forceUpdate();
	  }

	  public virtual void complete(ActivityExecution scopeExecution)
	  {
		leave(scopeExecution);
	  }

	  public override void doLeave(ActivityExecution execution)
	  {
		CompensationUtil.createEventScopeExecution((ExecutionEntity) execution);

		base.doLeave(execution);
	  }

	}

}