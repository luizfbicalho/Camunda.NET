using System;
using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.application.impl
{


	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using ProcessApplicationManager = org.camunda.bpm.engine.impl.application.ProcessApplicationManager;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessApplicationLogger : ProcessEngineLogger
	{

	  public virtual void taskNotRelatedToExecution(DelegateTask delegateTask)
	  {
		logDebug("001", "Task {} not related to an execution, target process application cannot be determined.", delegateTask);
	  }

	  public virtual ProcessEngineException exceptionWhileNotifyingPaTaskListener(Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Exception while notifying process application task listener."), e);
	  }

	  public virtual void noTargetProcessApplicationForExecution(DelegateExecution execution)
	  {
		logDebug("003", "No target process application found for execution {}", execution);
	  }

	  public virtual void paDoesNotProvideExecutionListener(string paName)
	  {
		logDebug("004", "Target process application '{}' does not provide an ExecutionListener.", paName);
	  }

	  public virtual void cannotInvokeListenerPaUnavailable(string paName, ProcessApplicationUnavailableException e)
	  {
		logDebug("005", "Exception while invoking listener: target process application '{}' unavailable", paName, e);
	  }

	  public virtual void paDoesNotProvideTaskListener(string paName)
	  {
		logDebug("006", "Target process application '{}' does not provide a TaskListener.", paName);
	  }

	  public virtual void paElResolversDiscovered(string summary)
	  {
		logDebug("007", summary);
	  }

	  public virtual void noElResolverProvided(string paName, string @string)
	  {
		logWarn("008", "Process Application '{}': No ELResolver provided by ProcessApplicationElResolver {}", paName, @string);

	  }

	  public virtual ProcessApplicationExecutionException processApplicationExecutionException(Exception e)
	  {
		return new ProcessApplicationExecutionException(e);
	  }

	  public virtual ProcessEngineException ejbPaCannotLookupSelfReference(NamingException e)
	  {
		return new ProcessEngineException(exceptionMessage("009", "Cannot lookup self reference to EjbProcessApplication"), e);
	  }

	  public virtual ProcessEngineException ejbPaCannotAutodetectName(NamingException e)
	  {
		return new ProcessEngineException(exceptionMessage("010", "Could not autodetect EjbProcessApplicationName"), e);
	  }

	  public virtual ProcessApplicationUnavailableException processApplicationUnavailableException(string name, Exception cause)
	  {
		return new ProcessApplicationUnavailableException(exceptionMessage("011", "Process Application '{}' unavailable", name), cause);
	  }

	  public virtual ProcessApplicationUnavailableException processApplicationUnavailableException(string name)
	  {
		return new ProcessApplicationUnavailableException(exceptionMessage("011", "Process Application '{}' unavailable", name));
	  }

	  public virtual void servletDeployerNoPaFound(string ctxName)
	  {
		logDebug("012", "Listener invoked for context '{}' but no process application annotation detected.", ctxName);
	  }

	  public virtual ServletException multiplePasException(ISet<Type> c, string appId)
	  {

		StringBuilder builder = new StringBuilder();
		builder.Append("An application must not contain more than one class annotated with @ProcessApplication.\n Application '");
		builder.Append(appId);
		builder.Append("' contains the following @ProcessApplication classes:\n");
		foreach (Type clazz in c)
		{
		  builder.Append("  ");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  builder.Append(clazz.FullName);
		  builder.Append("\n");
		}
		string msg = builder.ToString();

		return new ServletException(exceptionMessage("013", msg));
	  }

	  public virtual ServletException paWrongTypeException(Type paClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return new ServletException(exceptionMessage("014", "Class '{}' is annotated with @{} but is not a subclass of {}", paClass, typeof(ProcessApplication).FullName, typeof(AbstractProcessApplication).FullName));
	  }

	  public virtual void detectedPa(Type paClass)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		logInfo("015", "Detected @ProcessApplication class '{}'", paClass.FullName);
	  }

	  public virtual void alreadyDeployed()
	  {
		logWarn("016", "Ignoring call of deploy() on process application that is already deployed.");
	  }

	  public virtual void notDeployed()
	  {
		logWarn("017", "Calling undeploy() on process application that is not deployed.");
	  }

	  public virtual void couldNotRemoveDefinitionsFromCache(Exception t)
	  {
		logError("018", "Unregistering process application for deployment but could not remove process definitions from deployment cache.", t);
	  }

	  public virtual ProcessEngineException exceptionWhileRegisteringDeploymentsWithJobExecutor(Exception e)
	  {
		return new ProcessEngineException(exceptionMessage("019", "Exception while registering deployment with job executor"), e);
	  }

	  public virtual void exceptionWhileUnregisteringDeploymentsWithJobExecutor(Exception e)
	  {
		logError("020", "Exceptions while unregistering deployments with job executor", e);

	  }

	  public virtual void registrationSummary(string @string)
	  {
		logInfo("021", @string);
	  }

	  public virtual void exceptionWhileLoggingRegistrationSummary(Exception e)
	  {
		logError("022", "Exception while logging registration summary", e);
	  }

	  public virtual bool ContextSwitchLoggable
	  {
		  get
		  {
			return DebugEnabled;
		  }
	  }

	  public virtual void debugNoTargetProcessApplicationFound(ExecutionEntity execution, ProcessApplicationManager processApplicationManager)
	  {
		logDebug("023", "no target process application found for Execution[{}], ProcessDefinition[{}], Deployment[{}] Registrations[{}]", execution.Id, execution.ProcessDefinitionId, execution.getProcessDefinition().DeploymentId, processApplicationManager.RegistrationSummary);
	  }

	  public virtual void debugNoTargetProcessApplicationFoundForCaseExecution(CaseExecutionEntity execution, ProcessApplicationManager processApplicationManager)
	  {
		logDebug("024", "no target process application found for CaseExecution[{}], CaseDefinition[{}], Deployment[{}] Registrations[{}]", execution.Id, execution.CaseDefinitionId, ((CaseDefinitionEntity)execution.CaseDefinition).DeploymentId, processApplicationManager.RegistrationSummary);
	  }
	}

}