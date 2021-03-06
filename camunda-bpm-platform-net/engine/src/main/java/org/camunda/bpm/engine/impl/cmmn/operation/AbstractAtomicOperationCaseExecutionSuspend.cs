﻿/*
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
namespace org.camunda.bpm.engine.impl.cmmn.operation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.SUSPENDED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ActivityBehaviorUtil.getActivityBehavior;

	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using CmmnCompositeActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnCompositeActivityBehavior;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class AbstractAtomicOperationCaseExecutionSuspend : AbstractCmmnEventAtomicOperation
	{

	  protected internal virtual CmmnExecution eventNotificationsStarted(CmmnExecution execution)
	  {
		execution.CurrentState = SUSPENDED;

		return execution;

	  }

	  protected internal override void postTransitionNotification(CmmnExecution execution)
	  {
		CmmnExecution parent = execution.Parent;
		if (parent != null)
		{
		  notifyParent(parent, execution);
		}
	  }

	  protected internal virtual void notifyParent(CmmnExecution parent, CmmnExecution execution)
	  {
		CmmnActivityBehavior behavior = getActivityBehavior(parent);
		if (behavior is CmmnCompositeActivityBehavior)
		{
		  CmmnCompositeActivityBehavior compositeBehavior = (CmmnCompositeActivityBehavior) behavior;
		  compositeBehavior.handleChildSuspension(parent, execution);
		}
	  }

	}

}