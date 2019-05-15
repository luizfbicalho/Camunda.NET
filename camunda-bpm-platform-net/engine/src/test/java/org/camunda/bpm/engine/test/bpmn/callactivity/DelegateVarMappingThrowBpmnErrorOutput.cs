﻿/*
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
namespace org.camunda.bpm.engine.test.bpmn.callactivity
{
	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateVariableMapping = org.camunda.bpm.engine.@delegate.DelegateVariableMapping;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class DelegateVarMappingThrowBpmnErrorOutput : DelegateVariableMapping
	{

	  public virtual void mapInputVariables(DelegateExecution superExecution, VariableMap subVariables)
	  {
	  }

	  public virtual void mapOutputVariables(DelegateExecution superExecution, VariableScope subInstance)
	  {
		throw new BpmnError("1234");
	  }
	}

}