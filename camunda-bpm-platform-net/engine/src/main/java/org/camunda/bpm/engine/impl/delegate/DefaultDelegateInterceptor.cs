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
namespace org.camunda.bpm.engine.impl.@delegate
{

	using InvocationContext = org.camunda.bpm.application.InvocationContext;
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CoreExecutionContext = org.camunda.bpm.engine.impl.context.CoreExecutionContext;
	using ProcessApplicationContextUtil = org.camunda.bpm.engine.impl.context.ProcessApplicationContextUtil;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using DelegateInterceptor = org.camunda.bpm.engine.impl.interceptor.DelegateInterceptor;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;

	/// <summary>
	/// The default implementation of the DelegateInterceptor.
	/// <p/>
	/// This implementation has the following features:
	/// <ul>
	/// <li>it performs context switch into the target process application (if applicable)</li>
	/// <li>it checks autorizations if <seealso cref="ProcessEngineConfigurationImpl.isAuthorizationEnabledForCustomCode()"/> is true</li>
	/// </ul>
	/// 
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// </summary>
	public class DefaultDelegateInterceptor : DelegateInterceptor
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void handleInvocation(final DelegateInvocation invocation) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void handleInvocation(DelegateInvocation invocation)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.ProcessApplicationReference processApplication = getProcessApplicationForInvocation(invocation);
		ProcessApplicationReference processApplication = getProcessApplicationForInvocation(invocation);

		if (processApplication != null && ProcessApplicationContextUtil.requiresContextSwitch(processApplication))
		{
		  Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(this, invocation)
		 , processApplication, new InvocationContext(invocation.ContextExecution));
		}
		else
		{
		  handleInvocationInContext(invocation);
		}

	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly DefaultDelegateInterceptor outerInstance;

		  private org.camunda.bpm.engine.impl.@delegate.DelegateInvocation invocation;

		  public CallableAnonymousInnerClass(DefaultDelegateInterceptor outerInstance, org.camunda.bpm.engine.impl.@delegate.DelegateInvocation invocation)
		  {
			  this.outerInstance = outerInstance;
			  this.invocation = invocation;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			outerInstance.handleInvocation(invocation);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void handleInvocationInContext(final DelegateInvocation invocation) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal virtual void handleInvocationInContext(DelegateInvocation invocation)
	  {
		CommandContext commandContext = Context.CommandContext;
		bool wasAuthorizationCheckEnabled = commandContext.AuthorizationCheckEnabled;
		bool wasUserOperationLogEnabled = commandContext.UserOperationLogEnabled;
		BaseDelegateExecution contextExecution = invocation.ContextExecution;

		ProcessEngineConfigurationImpl configuration = Context.ProcessEngineConfiguration;

		bool popExecutionContext = false;

		try
		{
		  if (!configuration.AuthorizationEnabledForCustomCode)
		  {
			// the custom code should be executed without authorization
			commandContext.disableAuthorizationCheck();
		  }

		  try
		  {
			commandContext.disableUserOperationLog();

			try
			{
			  if (contextExecution != null && !isCurrentContextExecution(contextExecution))
			  {
				popExecutionContext = setExecutionContext(contextExecution);
			  }

			  invocation.proceed();
			}
			finally
			{
			  if (popExecutionContext)
			  {
				Context.removeExecutionContext();
			  }
			}
		  }
		  finally
		  {
			if (wasUserOperationLogEnabled)
			{
			  commandContext.enableUserOperationLog();
			}
		  }
		}
		finally
		{
		  if (wasAuthorizationCheckEnabled)
		  {
			commandContext.enableAuthorizationCheck();
		  }
		}

	  }

	  /// <returns> true if the execution context is modified by this invocation </returns>
	  protected internal virtual bool setExecutionContext(BaseDelegateExecution execution)
	  {
		if (execution is ExecutionEntity)
		{
		  Context.ExecutionContext = (ExecutionEntity) execution;
		  return true;
		}
		else if (execution is CaseExecutionEntity)
		{
		  Context.ExecutionContext = (CaseExecutionEntity) execution;
		  return true;
		}
		return false;
	  }

	  protected internal virtual bool isCurrentContextExecution(BaseDelegateExecution execution)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.context.CoreExecutionContext<?> coreExecutionContext = org.camunda.bpm.engine.impl.context.Context.getCoreExecutionContext();
		CoreExecutionContext<object> coreExecutionContext = Context.CoreExecutionContext;
		return coreExecutionContext != null && coreExecutionContext.Execution == execution;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.application.ProcessApplicationReference getProcessApplicationForInvocation(final DelegateInvocation invocation)
	  protected internal virtual ProcessApplicationReference getProcessApplicationForInvocation(DelegateInvocation invocation)
	  {

		BaseDelegateExecution contextExecution = invocation.ContextExecution;
		ResourceDefinitionEntity contextResource = invocation.ContextResource;

		if (contextExecution != null)
		{
		  return ProcessApplicationContextUtil.getTargetProcessApplication((CoreExecution) contextExecution);
		}
		else if (contextResource != null)
		{
		  return ProcessApplicationContextUtil.getTargetProcessApplication(contextResource);
		}
		else
		{
		  return null;
		}
	  }

	}

}