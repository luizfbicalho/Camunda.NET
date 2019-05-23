using System;

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
namespace org.camunda.bpm.application.impl.@event
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// <para><seealso cref="ExecutionListener"/> and <seealso cref="TaskListener"/> implementation delegating to
	/// the <seealso cref="ExecutionListener"/> and <seealso cref="TaskListener"/> provided by a
	/// <seealso cref="ProcessApplicationInterface ProcessApplication"/>.</para>
	/// 
	/// <para>If the process application does not provide an execution listener (ie.
	/// <seealso cref="ProcessApplicationInterface.getExecutionListener()"/> returns null), the
	/// request is silently ignored.</para>
	/// 
	/// <para>If the process application does not provide a task listener (ie.
	/// <seealso cref="ProcessApplicationInterface.getTaskListener()"/> returns null), the
	/// request is silently ignored.</para>
	/// 
	/// 
	/// @author Daniel Meyer </summary>
	/// <seealso cref= ProcessApplicationInterface#getExecutionListener() </seealso>
	/// <seealso cref= ProcessApplicationInterface#getTaskListener()
	///  </seealso>
	public class ProcessApplicationEventListenerDelegate : ExecutionListener, TaskListener
	{

	  private static ProcessApplicationLogger LOG = ProcessApplicationLogger.PROCESS_APPLICATION_LOGGER;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(final org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void notify(DelegateExecution execution)
	  {
		Callable<Void> notification = new CallableAnonymousInnerClass(this, execution);
		performNotification(execution, notification);
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly ProcessApplicationEventListenerDelegate outerInstance;

		  private DelegateExecution execution;

		  public CallableAnonymousInnerClass(ProcessApplicationEventListenerDelegate outerInstance, DelegateExecution execution)
		  {
			  this.outerInstance = outerInstance;
			  this.execution = execution;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.notifyExecutionListener(execution);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void notify(final org.camunda.bpm.engine.delegate.DelegateTask delegateTask)
	  public virtual void notify(DelegateTask delegateTask)
	  {
		if (delegateTask.Execution == null)
		{
		  LOG.taskNotRelatedToExecution(delegateTask);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.delegate.DelegateExecution execution = delegateTask.getExecution();
		  DelegateExecution execution = delegateTask.Execution;
		  Callable<Void> notification = new CallableAnonymousInnerClass2(this, delegateTask);
		  try
		  {
			performNotification(execution, notification);
		  }
		  catch (Exception e)
		  {
			throw LOG.exceptionWhileNotifyingPaTaskListener(e);
		  }
		}
	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly ProcessApplicationEventListenerDelegate outerInstance;

		  private DelegateTask delegateTask;

		  public CallableAnonymousInnerClass2(ProcessApplicationEventListenerDelegate outerInstance, DelegateTask delegateTask)
		  {
			  this.outerInstance = outerInstance;
			  this.delegateTask = delegateTask;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.notifyTaskListener(delegateTask);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void performNotification(final org.camunda.bpm.engine.delegate.DelegateExecution execution, java.util.concurrent.Callable<Void> notification) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void performNotification(DelegateExecution execution, Callable<Void> notification)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.ProcessApplicationReference processApp = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil.getTargetProcessApplication((org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity) execution);
		ProcessApplicationReference processApp = ProcessApplicationContextUtil.getTargetProcessApplication((ExecutionEntity) execution);
		if (processApp == null)
		{
		  // ignore silently
		  LOG.noTargetProcessApplicationForExecution(execution);

		}
		else
		{
		  if (ProcessApplicationContextUtil.requiresContextSwitch(processApp))
		  {
			// this should not be necessary since context switch is already performed by OperationContext and / or DelegateInterceptor
			Context.executeWithinProcessApplication(notification, processApp, new InvocationContext(execution));

		  }
		  else
		  {
			// context switch already performed
			notification.call();

		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void notifyExecutionListener(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  protected internal virtual void notifyExecutionListener(DelegateExecution execution)
	  {
		ProcessApplicationReference processApp = Context.CurrentProcessApplication;
		try
		{
		  ProcessApplicationInterface processApplication = processApp.ProcessApplication;
		  ExecutionListener executionListener = processApplication.ExecutionListener;
		  if (executionListener != null)
		  {
			executionListener.notify(execution);

		  }
		  else
		  {
			LOG.paDoesNotProvideExecutionListener(processApp.Name);

		  }
		}
		catch (ProcessApplicationUnavailableException e)
		{
		  // Process Application unavailable => ignore silently
		  LOG.cannotInvokeListenerPaUnavailable(processApp.Name, e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void notifyTaskListener(org.camunda.bpm.engine.delegate.DelegateTask task) throws Exception
	  protected internal virtual void notifyTaskListener(DelegateTask task)
	  {
		ProcessApplicationReference processApp = Context.CurrentProcessApplication;
		try
		{
		  ProcessApplicationInterface processApplication = processApp.ProcessApplication;
		  TaskListener taskListener = processApplication.TaskListener;
		  if (taskListener != null)
		  {
			taskListener.notify(task);

		  }
		  else
		  {
			LOG.paDoesNotProvideTaskListener(processApp.Name);

		  }
		}
		catch (ProcessApplicationUnavailableException e)
		{
		  // Process Application unavailable => ignore silently
		  LOG.cannotInvokeListenerPaUnavailable(processApp.Name, e);
		}
	  }

	}

}