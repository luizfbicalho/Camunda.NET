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
namespace org.camunda.bpm.qa.performance.engine.steps
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.performance.engine.steps.PerfTestConstants.TASK_ID;

	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using PerfTestRunContext = org.camunda.bpm.qa.performance.engine.framework.PerfTestRunContext;

	/// <summary>
	/// <para><seealso cref="TaskListener"/> recording the current task id in the <seealso cref="PerfTestRunContext"/>
	/// using the key <seealso cref="PerfTestConstants#TASK_ID"/>.</para>
	/// 
	/// <para>This is mainly used for removing the necessity for querying for the task Id.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TaskIdRecorder : TaskListener
	{

	  public virtual void notify(DelegateTask delegateTask)
	  {
		PerfTestRunContext perfTestRunContext = org.camunda.bpm.qa.performance.engine.framework.PerfTestRunContext_Fields.currentContext.get();
		if (perfTestRunContext != null)
		{
		  perfTestRunContext.setVariable(TASK_ID, delegateTask.Id);
		}
	  }

	}

}