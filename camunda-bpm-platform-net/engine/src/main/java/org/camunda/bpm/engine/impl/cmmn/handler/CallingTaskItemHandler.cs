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
namespace org.camunda.bpm.engine.impl.cmmn.handler
{
	using CallingTaskActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CallingTaskActivityBehavior;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using BaseCallableElement = org.camunda.bpm.engine.impl.core.model.BaseCallableElement;
	using CallableElementBinding = org.camunda.bpm.engine.impl.core.model.BaseCallableElement.CallableElementBinding;
	using DefaultCallableElementTenantIdProvider = org.camunda.bpm.engine.impl.core.model.DefaultCallableElementTenantIdProvider;
	using ConstantValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ConstantValueProvider;
	using NullValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.NullValueProvider;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ElValueProvider = org.camunda.bpm.engine.impl.el.ElValueProvider;
	using Expression = org.camunda.bpm.engine.impl.el.Expression;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.StringUtil.isCompositeExpression;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class CallingTaskItemHandler : TaskItemHandler
	{

	  protected internal override void initializeActivity(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		base.initializeActivity(element, activity, context);

		initializeCallableElement(element, activity, context);
	  }

	  protected internal virtual void initializeCallableElement(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context)
	  {
		Deployment deployment = context.Deployment;
		string deploymentId = null;
		if (deployment != null)
		{
		  deploymentId = deployment.Id;
		}

		BaseCallableElement callableElement = createCallableElement();
		callableElement.DeploymentId = deploymentId;

		// set callableElement on behavior
		CallingTaskActivityBehavior behavior = (CallingTaskActivityBehavior) activity.ActivityBehavior;
		behavior.CallableElement = callableElement;

		// definition key
		initializeDefinitionKey(element, activity, context, callableElement);

		// binding
		initializeBinding(element, activity, context, callableElement);

		// version
		initializeVersion(element, activity, context, callableElement);

		// tenant-id
		initializeTenantId(element, activity, context, callableElement);
	  }

	  protected internal virtual void initializeDefinitionKey(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, BaseCallableElement callableElement)
	  {
		ExpressionManager expressionManager = context.ExpressionManager;
		string definitionKey = getDefinitionKey(element, activity, context);
		ParameterValueProvider definitionKeyProvider = createParameterValueProvider(definitionKey, expressionManager);
		callableElement.DefinitionKeyValueProvider = definitionKeyProvider;
	  }

	  protected internal virtual void initializeBinding(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, BaseCallableElement callableElement)
	  {
		string binding = getBinding(element, activity, context);

		if (BaseCallableElement.CallableElementBinding.DEPLOYMENT.Value.Equals(binding))
		{
		  callableElement.Binding = BaseCallableElement.CallableElementBinding.DEPLOYMENT;
		}
		else if (BaseCallableElement.CallableElementBinding.LATEST.Value.Equals(binding))
		{
		  callableElement.Binding = BaseCallableElement.CallableElementBinding.LATEST;
		}
		else if (BaseCallableElement.CallableElementBinding.VERSION.Value.Equals(binding))
		{
		  callableElement.Binding = BaseCallableElement.CallableElementBinding.VERSION;
		}
	  }

	  protected internal virtual void initializeVersion(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, BaseCallableElement callableElement)
	  {
		ExpressionManager expressionManager = context.ExpressionManager;
		string version = getVersion(element, activity, context);
		ParameterValueProvider versionProvider = createParameterValueProvider(version, expressionManager);
		callableElement.VersionValueProvider = versionProvider;
	  }

	  protected internal virtual void initializeTenantId(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context, BaseCallableElement callableElement)
	  {
		ParameterValueProvider tenantIdProvider;

		ExpressionManager expressionManager = context.ExpressionManager;
		string tenantId = getTenantId(element, activity, context);
		if (!string.ReferenceEquals(tenantId, null) && tenantId.Length > 0)
		{
		  tenantIdProvider = createParameterValueProvider(tenantId, expressionManager);
		}
		else
		{
		  tenantIdProvider = new DefaultCallableElementTenantIdProvider();
		}

		callableElement.TenantIdProvider = tenantIdProvider;
	  }

	  protected internal virtual ParameterValueProvider createParameterValueProvider(string value, ExpressionManager expressionManager)
	  {
		if (string.ReferenceEquals(value, null))
		{
		  return new NullValueProvider();

		}
		else if (isCompositeExpression(value, expressionManager))
		{
		  Expression expression = expressionManager.createExpression(value);
		  return new ElValueProvider(expression);

		}
		else
		{
		  return new ConstantValueProvider(value);
		}
	  }

	  protected internal abstract BaseCallableElement createCallableElement();

	  protected internal abstract string getDefinitionKey(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context);

	  protected internal abstract string getBinding(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context);

	  protected internal abstract string getVersion(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context);

	  protected internal abstract string getTenantId(CmmnElement element, CmmnActivity activity, CmmnHandlerContext context);

	}

}