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
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using StartFormDataImpl = org.camunda.bpm.engine.impl.form.StartFormDataImpl;
	using StartFormHandler = org.camunda.bpm.engine.impl.form.handler.StartFormHandler;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class MyStartFormHandler : MyDelegationService, StartFormHandler
	{

	  public virtual void parseConfiguration(Element activityElement, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
	  {
		// do nothing
	  }

	  public virtual void submitFormVariables(VariableMap properties, VariableScope variableScope)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		IdentityService identityService = processEngineConfiguration.IdentityService;
		RuntimeService runtimeService = processEngineConfiguration.RuntimeService;

		logAuthentication(identityService);
		logInstancesCount(runtimeService);
	  }

	  public virtual StartFormData createStartFormData(ProcessDefinitionEntity processDefinition)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		IdentityService identityService = processEngineConfiguration.IdentityService;
		RuntimeService runtimeService = processEngineConfiguration.RuntimeService;

		logAuthentication(identityService);
		logInstancesCount(runtimeService);

		StartFormDataImpl result = new StartFormDataImpl();
		result.ProcessDefinition = processDefinition;
		return result;
	  }

	}

}