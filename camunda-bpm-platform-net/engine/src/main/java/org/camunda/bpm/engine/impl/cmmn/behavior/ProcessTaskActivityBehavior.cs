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
namespace org.camunda.bpm.engine.impl.cmmn.behavior
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.CallableElementUtil.getProcessDefinitionToCall;

	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ProcessTaskActivityBehavior : ProcessOrCaseTaskActivityBehavior
	{

	  protected internal new static readonly CmmnBehaviorLogger LOG = ProcessEngineLogger.CMNN_BEHAVIOR_LOGGER;

	  protected internal override void triggerCallableElement(CmmnActivityExecution execution, IDictionary<string, object> variables, string businessKey)
	  {
		ProcessDefinitionImpl definition = getProcessDefinitionToCall(execution, CallableElement);
		PvmProcessInstance processInstance = execution.createSubProcessInstance(definition, businessKey);
		processInstance.start(variables);
	  }

	  protected internal override string TypeName
	  {
		  get
		  {
			return "process task";
		  }
	  }

	}

}