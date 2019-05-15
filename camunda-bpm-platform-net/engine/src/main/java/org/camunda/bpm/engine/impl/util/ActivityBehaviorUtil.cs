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
namespace org.camunda.bpm.engine.impl.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using PvmException = org.camunda.bpm.engine.impl.pvm.PvmException;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ActivityBehaviorUtil
	{

	  public static CmmnActivityBehavior getActivityBehavior(CmmnExecution execution)
	  {
		string id = execution.Id;

		CmmnActivity activity = execution.Activity;
		ensureNotNull(typeof(PvmException), "Case execution '" + id + "' has no current activity.", "activity", activity);

		CmmnActivityBehavior behavior = activity.ActivityBehavior;
		ensureNotNull(typeof(PvmException), "There is no behavior specified in " + activity + " for case execution '" + id + "'.", "behavior", behavior);

		return behavior;
	  }

	  public static ActivityBehavior getActivityBehavior(PvmExecutionImpl execution)
	  {
		string id = execution.Id;

		PvmActivity activity = execution.getActivity();
		ensureNotNull(typeof(PvmException), "Execution '" + id + "' has no current activity.", "activity", activity);

		ActivityBehavior behavior = activity.ActivityBehavior;
		ensureNotNull(typeof(PvmException), "There is no behavior specified in " + activity + " for execution '" + id + "'.", "behavior", behavior);

		return behavior;
	  }

	}

}