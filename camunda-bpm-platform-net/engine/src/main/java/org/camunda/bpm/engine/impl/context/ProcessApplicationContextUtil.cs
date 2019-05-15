using System.Threading;

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
namespace org.camunda.bpm.engine.impl.context
{
	using ProcessApplicationReference = org.camunda.bpm.application.ProcessApplicationReference;
	using ProcessApplicationLogger = org.camunda.bpm.application.impl.ProcessApplicationLogger;
	using ProcessApplicationManager = org.camunda.bpm.engine.impl.application.ProcessApplicationManager;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using ResourceDefinitionEntity = org.camunda.bpm.engine.impl.repository.ResourceDefinitionEntity;
	using ClassLoaderUtil = org.camunda.bpm.engine.impl.util.ClassLoaderUtil;

	public class ProcessApplicationContextUtil
	{

	  private static readonly ProcessApplicationLogger LOG = ProcessApplicationLogger.PROCESS_APPLICATION_LOGGER;

	  public static ProcessApplicationReference getTargetProcessApplication(CoreExecution execution)
	  {
		if (execution is ExecutionEntity)
		{
		  return getTargetProcessApplication((ExecutionEntity) execution);
		}
		else
		{
		  return getTargetProcessApplication((CaseExecutionEntity) execution);
		}
	  }

	  public static ProcessApplicationReference getTargetProcessApplication(ExecutionEntity execution)
	  {
		if (execution == null)
		{
		  return null;
		}

		ProcessApplicationReference processApplicationForDeployment = getTargetProcessApplication((ProcessDefinitionEntity) execution.getProcessDefinition());

		// logg application context switch details
		if (LOG.ContextSwitchLoggable && processApplicationForDeployment == null)
		{
		  loggContextSwitchDetails(execution);
		}

		return processApplicationForDeployment;
	  }

	  public static ProcessApplicationReference getTargetProcessApplication(CaseExecutionEntity execution)
	  {
		if (execution == null)
		{
		  return null;
		}

		ProcessApplicationReference processApplicationForDeployment = getTargetProcessApplication((CaseDefinitionEntity) execution.CaseDefinition);

		// logg application context switch details
		if (LOG.ContextSwitchLoggable && processApplicationForDeployment == null)
		{
		  loggContextSwitchDetails(execution);
		}

		return processApplicationForDeployment;
	  }

	  public static ProcessApplicationReference getTargetProcessApplication(TaskEntity task)
	  {
		if (task.ProcessDefinition != null)
		{
		  return getTargetProcessApplication(task.ProcessDefinition);
		}
		else if (task.CaseDefinition != null)
		{
		  return getTargetProcessApplication(task.CaseDefinition);
		}
		else
		{
		  return null;
		}
	  }

	  public static ProcessApplicationReference getTargetProcessApplication(ResourceDefinitionEntity definition)
	  {
		ProcessApplicationReference reference = getTargetProcessApplication(definition.DeploymentId);

		if (reference == null && areProcessApplicationsRegistered())
		{
		  ResourceDefinitionEntity previous = definition.PreviousDefinition;

		  // do it in a iterative way instead of recursive to avoid
		  // a possible StackOverflowException in cases with a lot
		  // of versions of a definition
		  while (previous != null)
		  {
			reference = getTargetProcessApplication(previous.DeploymentId);

			if (reference == null)
			{
			  previous = previous.PreviousDefinition;
			}
			else
			{
			  return reference;
			}

		  }
		}

		return reference;
	  }

	  public static ProcessApplicationReference getTargetProcessApplication(string deploymentId)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		ProcessApplicationManager processApplicationManager = processEngineConfiguration.ProcessApplicationManager;

		ProcessApplicationReference processApplicationForDeployment = processApplicationManager.getProcessApplicationForDeployment(deploymentId);

		return processApplicationForDeployment;
	  }

	  public static bool areProcessApplicationsRegistered()
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		ProcessApplicationManager processApplicationManager = processEngineConfiguration.ProcessApplicationManager;

		return processApplicationManager.hasRegistrations();
	  }

	  private static void loggContextSwitchDetails(ExecutionEntity execution)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> executionContext = Context.getCoreExecutionContext();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
		CoreExecutionContext<CoreExecution> executionContext = Context.CoreExecutionContext;
		// only log for first atomic op:
		if (executionContext == null || (executionContext.Execution != execution))
		{
		  ProcessApplicationManager processApplicationManager = Context.ProcessEngineConfiguration.ProcessApplicationManager;
		  LOG.debugNoTargetProcessApplicationFound(execution, processApplicationManager);
		}

	  }

	  private static void loggContextSwitchDetails(CaseExecutionEntity execution)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> executionContext = Context.getCoreExecutionContext();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
		CoreExecutionContext<CoreExecution> executionContext = Context.CoreExecutionContext;
		// only log for first atomic op:
		if (executionContext == null || (executionContext.Execution != execution))
		{
		  ProcessApplicationManager processApplicationManager = Context.ProcessEngineConfiguration.ProcessApplicationManager;
		  LOG.debugNoTargetProcessApplicationFoundForCaseExecution(execution, processApplicationManager);
		}

	  }

	  public static bool requiresContextSwitch(ProcessApplicationReference processApplicationReference)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.application.ProcessApplicationReference currentProcessApplication = Context.getCurrentProcessApplication();
		ProcessApplicationReference currentProcessApplication = Context.CurrentProcessApplication;

		if (processApplicationReference == null)
		{
		  return false;
		}

		if (currentProcessApplication == null)
		{
		  return true;
		}
		else
		{
		  if (!processApplicationReference.Name.Equals(currentProcessApplication.Name))
		  {
			return true;
		  }
		  else
		  {
			// check whether the thread context has been manipulated since last context switch. This can happen as a result of
			// an operation causing the container to switch to a different application.
			// Example: JavaDelegate implementation (inside PA) invokes an EJB from different application which in turn interacts with the Process engine.
			ClassLoader processApplicationClassLoader = ProcessApplicationClassloaderInterceptor.ProcessApplicationClassLoader;
			ClassLoader currentClassloader = ClassLoaderUtil.ContextClassloader;
			return currentClassloader != processApplicationClassLoader;
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static void doContextSwitch(final Runnable runnable, org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity contextDefinition)
	  public static void doContextSwitch(ThreadStart runnable, ProcessDefinitionEntity contextDefinition)
	  {
		ProcessApplicationReference processApplication = getTargetProcessApplication(contextDefinition);
		if (requiresContextSwitch(processApplication))
		{
		  Context.executeWithinProcessApplication(new CallableAnonymousInnerClass(runnable)
		 , processApplication);
		}
		else
		{
		  runnable.run();
		}
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private ThreadStart runnable;

		  public CallableAnonymousInnerClass(ThreadStart runnable)
		  {
			  this.runnable = runnable;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			runnable.run();
			return null;
		  }
	  }
	}

}