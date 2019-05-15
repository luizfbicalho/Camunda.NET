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
//	import static org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ActivityBehaviorUtil.getActivityBehavior;

	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class AtomicOperationCaseInstanceCreate : AbstractCmmnEventAtomicOperation
	{

	  public virtual string CanonicalName
	  {
		  get
		  {
			return "case-instance-create";
		  }
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return CREATE;
		  }
	  }

	  protected internal virtual CmmnExecution eventNotificationsStarted(CmmnExecution execution)
	  {
		// the case instance perform a transition directly
		// to state ACTIVE
		execution.CurrentState = ACTIVE;

		return execution;
	  }

	  protected internal override void postTransitionNotification(CmmnExecution execution)
	  {
		// the case instance is associated with the
		// casePlanModel as activity
		CmmnActivityBehavior behavior = getActivityBehavior(execution);

		// perform start() on associated behavior
		// because the case instance is ACTIVE
		behavior.started(execution);
	  }

	}

}