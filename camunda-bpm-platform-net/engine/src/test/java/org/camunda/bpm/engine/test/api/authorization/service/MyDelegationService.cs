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
namespace org.camunda.bpm.engine.test.api.authorization.service
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class MyDelegationService
	{

	  public static Authentication CURRENT_AUTHENTICATION;
	  public static long? INSTANCES_COUNT;

	  // fetch current authentication //////////////////////////////////////////

	  public virtual void logAuthentication(DelegateExecution execution)
	  {
		logAuthentication(execution.ProcessEngineServices);
	  }

	  public virtual void logAuthentication(DelegateTask task)
	  {
		logAuthentication(task.ProcessEngineServices);
	  }

	  protected internal virtual void logAuthentication(ProcessEngineServices services)
	  {
		IdentityService identityService = services.IdentityService;
		logAuthentication(identityService);
	  }

	  protected internal virtual void logAuthentication(IdentityService identityService)
	  {
		CURRENT_AUTHENTICATION = identityService.CurrentAuthentication;
	  }

	  // execute a query /////////////////////////////////////////////////////////

	  public virtual void logInstancesCount(DelegateExecution execution)
	  {
		logInstancesCount(execution.ProcessEngineServices);
	  }

	  public virtual void logInstancesCount(DelegateTask task)
	  {
		logInstancesCount(task.ProcessEngineServices);
	  }

	  protected internal virtual void logInstancesCount(ProcessEngineServices services)
	  {
		RuntimeService runtimeService = services.RuntimeService;
		logInstancesCount(runtimeService);
	  }

	  protected internal virtual void logInstancesCount(RuntimeService runtimeService)
	  {
		INSTANCES_COUNT = runtimeService.createProcessInstanceQuery().count();
	  }

	  // execute a command ///////////////////////////////////////////////////////

	  public virtual void executeCommand(DelegateExecution execution)
	  {
		executeCommand(execution.ProcessEngineServices);
	  }

	  public virtual void executeCommand(DelegateTask task)
	  {
		executeCommand(task.ProcessEngineServices);
	  }

	  protected internal virtual void executeCommand(ProcessEngineServices services)
	  {
		RuntimeService runtimeService = services.RuntimeService;
		executeCommand(runtimeService);
	  }

	  protected internal virtual void executeCommand(RuntimeService runtimeService)
	  {
		runtimeService.startProcessInstanceByKey("process");
	  }

	  // helper /////////////////////////////////////////////////////////////////

	  public static void clearProperties()
	  {
		CURRENT_AUTHENTICATION = null;
		INSTANCES_COUNT = null;
	  }

	}

}