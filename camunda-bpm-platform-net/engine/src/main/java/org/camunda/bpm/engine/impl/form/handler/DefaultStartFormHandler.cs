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
namespace org.camunda.bpm.engine.impl.form.handler
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DefaultStartFormHandler : DefaultFormHandler, StartFormHandler
	{

	  protected internal Expression formKey;

	  public override void parseConfiguration(Element activityElement, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
	  {
		base.parseConfiguration(activityElement, deployment, processDefinition, bpmnParse);

		ExpressionManager expressionManager = Context.ProcessEngineConfiguration.ExpressionManager;

		string formKeyAttribute = activityElement.attributeNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "formKey");

		if (!string.ReferenceEquals(formKeyAttribute, null))
		{
		  this.formKey = expressionManager.createExpression(formKeyAttribute);
		}

		if (formKey != null)
		{
		  processDefinition.StartFormKey = true;
		}
	  }

	  public virtual StartFormData createStartFormData(ProcessDefinitionEntity processDefinition)
	  {
		StartFormDataImpl startFormData = new StartFormDataImpl();

		if (formKey != null)
		{
		  startFormData.FormKey = formKey.ExpressionText;
		}
		startFormData.DeploymentId = deploymentId;
		startFormData.ProcessDefinition = processDefinition;
		initializeFormProperties(startFormData, null);
		initializeFormFields(startFormData, null);
		return startFormData;
	  }

	  public virtual ExecutionEntity submitStartFormData(ExecutionEntity processInstance, VariableMap properties)
	  {
		submitFormVariables(properties, processInstance);
		return processInstance;
	  }

	  // getters //////////////////////////////////////////////

	  public virtual Expression FormKey
	  {
		  get
		  {
			return formKey;
		  }
	  }
	}

}