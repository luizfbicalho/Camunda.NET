using System.Collections.Generic;

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
namespace org.camunda.bpm.integrationtest.jobexecutor
{


	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using BusinessProcess = org.camunda.bpm.engine.cdi.BusinessProcess;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using EventSubscriptionEntity = org.camunda.bpm.engine.impl.persistence.entity.EventSubscriptionEntity;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public class SendSignalDelegate implements org.camunda.bpm.engine.delegate.JavaDelegate
	public class SendSignalDelegate : JavaDelegate
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.RuntimeService runtimeService;
		private RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.cdi.BusinessProcess businessProcess;
	  private BusinessProcess businessProcess;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		businessProcess.setVariable("processName", "throwSignal-visited (was " + businessProcess.getVariable("processName") + ")");

		string signalProcessInstanceId = (string) execution.getVariable("signalProcessInstanceId");
		string executionId = runtimeService.createExecutionQuery().processInstanceId(signalProcessInstanceId).signalEventSubscriptionName("alert").singleResult().Id;

		CommandContext commandContext = Context.CommandContext;
		IList<EventSubscriptionEntity> findSignalEventSubscriptionsByEventName = commandContext.EventSubscriptionManager.findSignalEventSubscriptionsByNameAndExecution("alert", executionId);

		foreach (EventSubscriptionEntity signalEventSubscriptionEntity in findSignalEventSubscriptionsByEventName)
		{
			signalEventSubscriptionEntity.eventReceived(null, true);
		}
	  }
	}

}