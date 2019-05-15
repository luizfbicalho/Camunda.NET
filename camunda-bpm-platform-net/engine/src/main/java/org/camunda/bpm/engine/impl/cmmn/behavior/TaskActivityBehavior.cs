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
namespace org.camunda.bpm.engine.impl.cmmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.ACTIVE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.FAILED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler.PROPERTY_IS_BLOCKING;

	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class TaskActivityBehavior : StageOrTaskActivityBehavior
	{

	  public override void onReactivation(CmmnActivityExecution execution)
	  {
		ensureTransitionAllowed(execution, FAILED, ACTIVE, "re-activate");
	  }

	  protected internal override void performStart(CmmnActivityExecution execution)
	  {
		execution.complete();
	  }

	  public override void fireExitCriteria(CmmnActivityExecution execution)
	  {
		execution.exit();
	  }

	  protected internal virtual bool isBlocking(CmmnActivityExecution execution)
	  {
		CmmnActivity activity = execution.Activity;
		object isBlockingProperty = activity.getProperty(PROPERTY_IS_BLOCKING);
		if (isBlockingProperty != null && isBlockingProperty is bool?)
		{
		  return (bool?) isBlockingProperty.Value;
		}
		return false;
	  }

	  protected internal override string TypeName
	  {
		  get
		  {
			return "task";
		  }
	  }


	}

}