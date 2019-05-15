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
namespace org.camunda.bpm.engine.impl.form.handler
{

	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DelegateTaskFormHandler : DelegateFormHandler, TaskFormHandler
	{

	  public DelegateTaskFormHandler(TaskFormHandler formHandler, DeploymentEntity deployment) : base(formHandler, deployment.Id)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.form.TaskFormData createTaskForm(final org.camunda.bpm.engine.impl.persistence.entity.TaskEntity task)
	  public virtual TaskFormData createTaskForm(TaskEntity task)
	  {
		return performContextSwitch(new CallableAnonymousInnerClass(this, task));
	  }

	  private class CallableAnonymousInnerClass : Callable<TaskFormData>
	  {
		  private readonly DelegateTaskFormHandler outerInstance;

		  private TaskEntity task;

		  public CallableAnonymousInnerClass(DelegateTaskFormHandler outerInstance, TaskEntity task)
		  {
			  this.outerInstance = outerInstance;
			  this.task = task;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.form.TaskFormData call() throws Exception
		  public TaskFormData call()
		  {
			CreateTaskFormInvocation invocation = new CreateTaskFormInvocation((TaskFormHandler) outerInstance.formHandler, task);
			Context.ProcessEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
			return (TaskFormData) invocation.InvocationResult;
		  }
	  }

	  public override FormHandler FormHandler
	  {
		  get
		  {
			return (TaskFormHandler) formHandler;
		  }
	  }

	}

}