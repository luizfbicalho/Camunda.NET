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
namespace org.camunda.bpm.engine.impl.form.engine
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormData = org.camunda.bpm.engine.form.FormData;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ScriptInvocation = org.camunda.bpm.engine.impl.@delegate.ScriptInvocation;
	using ResourceEntity = org.camunda.bpm.engine.impl.persistence.entity.ResourceEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptFactory = org.camunda.bpm.engine.impl.scripting.ScriptFactory;
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JuelFormEngine : FormEngine
	{

	  public virtual string Name
	  {
		  get
		  {
			return "juel";
		  }
	  }

	  public virtual object renderStartForm(StartFormData startForm)
	  {
		if (string.ReferenceEquals(startForm.FormKey, null))
		{
		  return null;
		}
		string formTemplateString = getFormTemplateString(startForm, startForm.FormKey);
		return executeScript(formTemplateString, null);
	  }


	  public virtual object renderTaskForm(TaskFormData taskForm)
	  {
		if (string.ReferenceEquals(taskForm.FormKey, null))
		{
		  return null;
		}
		string formTemplateString = getFormTemplateString(taskForm, taskForm.FormKey);
		TaskEntity task = (TaskEntity) taskForm.Task;
		return executeScript(formTemplateString, task.getExecution());
	  }

	  protected internal virtual object executeScript(string scriptSrc, VariableScope scope)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		ScriptFactory scriptFactory = processEngineConfiguration.ScriptFactory;
		ExecutableScript script = scriptFactory.createScriptFromSource(ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE, scriptSrc);

		ScriptInvocation invocation = new ScriptInvocation(script, scope);
		try
		{
		  processEngineConfiguration.DelegateInterceptor.handleInvocation(invocation);
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException(e);
		}

		return invocation.InvocationResult;
	  }

	  protected internal virtual string getFormTemplateString(FormData formInstance, string formKey)
	  {
		string deploymentId = formInstance.DeploymentId;

		ResourceEntity resourceStream = Context.CommandContext.ResourceManager.findResourceByDeploymentIdAndResourceName(deploymentId, formKey);

		ensureNotNull("Form with formKey '" + formKey + "' does not exist", "resourceStream", resourceStream);

		sbyte[] resourceBytes = resourceStream.Bytes;
		string encoding = "UTF-8";
		string formTemplateString = "";
		try
		{
		  formTemplateString = StringHelper.NewString(resourceBytes, encoding);
		}
		catch (UnsupportedEncodingException e)
		{
		  throw new ProcessEngineException("Unsupported encoding of :" + encoding, e);
		}
		return formTemplateString;
	  }
	}

}