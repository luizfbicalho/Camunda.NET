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
namespace org.camunda.bpm.model.bpmn.builder
{
	using Script = org.camunda.bpm.model.bpmn.instance.Script;
	using ScriptTask = org.camunda.bpm.model.bpmn.instance.ScriptTask;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public abstract class AbstractScriptTaskBuilder<B> : AbstractTaskBuilder<B, ScriptTask> where B : AbstractScriptTaskBuilder<B>
	{

	  protected internal AbstractScriptTaskBuilder(BpmnModelInstance modelInstance, ScriptTask element, Type selfType) : base(modelInstance, element, selfType)
	  {
	  }

	  /// <summary>
	  /// Sets the script format of the build script task.
	  /// </summary>
	  /// <param name="scriptFormat">  the script format to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B scriptFormat(string scriptFormat)
	  {
		element.ScriptFormat = scriptFormat;
		return myself;
	  }

	  /// <summary>
	  /// Sets the script of the build script task.
	  /// </summary>
	  /// <param name="script">  the script to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B script(Script script)
	  {
		element.Script = script;
		return myself;
	  }

	  public virtual B scriptText(string scriptText)
	  {
		Script script = createChild(typeof(Script));
		script.TextContent = scriptText;
		return myself;
	  }

	  /// <summary>
	  /// camunda extensions </summary>

	  /// <summary>
	  /// Sets the camunda result variable of the build script task.
	  /// </summary>
	  /// <param name="camundaResultVariable">  the result variable to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaResultVariable(string camundaResultVariable)
	  {
		element.CamundaResultVariable = camundaResultVariable;
		return myself;
	  }

	  /// <summary>
	  /// Sets the camunda resource of the build script task.
	  /// </summary>
	  /// <param name="camundaResource">  the resource to set </param>
	  /// <returns> the builder object </returns>
	  public virtual B camundaResource(string camundaResource)
	  {
		element.CamundaResource = camundaResource;
		return myself;
	  }

	}

}