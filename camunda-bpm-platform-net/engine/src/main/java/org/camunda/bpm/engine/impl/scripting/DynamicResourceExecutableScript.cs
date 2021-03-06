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
namespace org.camunda.bpm.engine.impl.scripting
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceUtil = org.camunda.bpm.engine.impl.util.ResourceUtil;

	/// <summary>
	/// A script which resource path is dynamically determined during the execution.
	/// Therefore it has to be executed in the context of an atomic operation.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class DynamicResourceExecutableScript : DynamicExecutableScript
	{

	  public DynamicResourceExecutableScript(string language, Expression scriptResourceExpression) : base(scriptResourceExpression, language)
	  {
	  }

	  public override string getScriptSource(VariableScope variableScope)
	  {
		string scriptPath = evaluateExpression(variableScope);
		return ResourceUtil.loadResourceContent(scriptPath, Deployment);
	  }

	  protected internal virtual DeploymentEntity Deployment
	  {
		  get
		  {
			return Context.BpmnExecutionContext.Deployment;
		  }
	  }

	}

}