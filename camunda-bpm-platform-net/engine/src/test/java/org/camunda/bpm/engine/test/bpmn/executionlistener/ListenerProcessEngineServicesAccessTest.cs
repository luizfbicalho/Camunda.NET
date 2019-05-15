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
namespace org.camunda.bpm.engine.test.bpmn.executionlistener
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using AbstractProcessEngineServicesAccessTest = org.camunda.bpm.engine.test.bpmn.common.AbstractProcessEngineServicesAccessTest;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using ManualTask = org.camunda.bpm.model.bpmn.instance.ManualTask;
	using Task = org.camunda.bpm.model.bpmn.instance.Task;
	using CamundaExecutionListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaExecutionListener;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ListenerProcessEngineServicesAccessTest : AbstractProcessEngineServicesAccessTest
	{

	  protected internal override Type TestServiceAccessibleClass
	  {
		  get
		  {
			return typeof(AccessServicesListener);
		  }
	  }

	  protected internal override Type QueryClass
	  {
		  get
		  {
			return typeof(PerformQueryListener);
		  }
	  }

	  protected internal override Type StartProcessInstanceClass
	  {
		  get
		  {
			return typeof(StartProcessListener);
		  }
	  }

	  protected internal override Type ProcessEngineStartProcessClass
	  {
		  get
		  {
			return typeof(ProcessEngineStartProcessListener);
		  }
	  }

	  protected internal override Task createModelAccessTask(BpmnModelInstance modelInstance, Type delegateClass)
	  {
		ManualTask task = modelInstance.newInstance(typeof(ManualTask));
		task.Id = "manualTask";
		CamundaExecutionListener executionListener = modelInstance.newInstance(typeof(CamundaExecutionListener));
		executionListener.CamundaEvent = org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		executionListener.CamundaClass = delegateClass.FullName;
		task.builder().addExtensionElement(executionListener);
		return task;
	  }

	  public class AccessServicesListener : ExecutionListener
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  assertCanAccessServices(execution.ProcessEngineServices);
		}
	  }

	  public class PerformQueryListener : ExecutionListener
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  assertCanPerformQuery(execution.ProcessEngineServices);
		}
	  }

	  public class StartProcessListener : ExecutionListener
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  assertCanStartProcessInstance(execution.ProcessEngineServices);
		}
	  }

	  public class ProcessEngineStartProcessListener : ExecutionListener
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void notify(DelegateExecution execution)
		{
		  assertCanStartProcessInstance(execution.ProcessEngine);
		}
	  }

	}

}