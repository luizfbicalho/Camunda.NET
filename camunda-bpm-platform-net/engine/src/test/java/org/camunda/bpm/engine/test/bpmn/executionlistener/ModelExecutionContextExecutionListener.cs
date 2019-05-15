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
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using FlowElement = org.camunda.bpm.model.bpmn.instance.FlowElement;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ModelExecutionContextExecutionListener : ExecutionListener
	{

	  public static BpmnModelInstance modelInstance;
	  public static FlowElement flowElement;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {
		modelInstance = execution.BpmnModelInstance;
		flowElement = execution.BpmnModelElementInstance;
	  }

	  public static void clear()
	  {
		modelInstance = null;
		flowElement = null;
	  }
	}

}