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
namespace org.camunda.bpm.engine.impl.scripting
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceUtil = org.camunda.bpm.engine.impl.util.ResourceUtil;

	/// <summary>
	/// A script which is provided by an external resource.
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class ResourceExecutableScript : SourceExecutableScript
	{

	  protected internal string scriptResource;

	  public ResourceExecutableScript(string language, string scriptResource) : base(language, null)
	  {
		this.scriptResource = scriptResource;
	  }

	  public override object evaluate(ScriptEngine engine, VariableScope variableScope, Bindings bindings)
	  {
		if (string.ReferenceEquals(scriptSource, null))
		{
		  loadScriptSource();
		}
		return base.evaluate(engine, variableScope, bindings);
	  }

	  protected internal virtual void loadScriptSource()
	  {
		  lock (this)
		  {
			if (string.ReferenceEquals(ScriptSource, null))
			{
			  DeploymentEntity deployment = Context.CoreExecutionContext.Deployment;
			  string source = ResourceUtil.loadResourceContent(scriptResource, deployment);
			  ScriptSource = source;
			}
		  }
	  }

	}

}