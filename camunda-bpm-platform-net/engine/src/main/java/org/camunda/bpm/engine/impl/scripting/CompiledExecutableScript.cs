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

	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;

	public class CompiledExecutableScript : ExecutableScript
	{

	  private static readonly ScriptLogger LOG = ProcessEngineLogger.SCRIPT_LOGGER;

	  protected internal CompiledScript compiledScript;

	  protected internal CompiledExecutableScript(string language) : this(language, null)
	  {
	  }

	  protected internal CompiledExecutableScript(string language, CompiledScript compiledScript) : base(language)
	  {
		this.compiledScript = compiledScript;
	  }

	  public virtual CompiledScript CompiledScript
	  {
		  get
		  {
			return compiledScript;
		  }
		  set
		  {
			this.compiledScript = value;
		  }
	  }


	  public override object evaluate(ScriptEngine scriptEngine, VariableScope variableScope, Bindings bindings)
	  {
		try
		{
		  LOG.debugEvaluatingCompiledScript(language);
		  return CompiledScript.eval(bindings);
		}
		catch (ScriptException e)
		{
		  if (e.InnerException is BpmnError)
		  {
			throw (BpmnError) e.InnerException;
		  }
		  string activityIdMessage = getActivityIdExceptionMessage(variableScope);
		  throw new ScriptEvaluationException("Unable to evaluate script" + activityIdMessage + ": " + e.Message, e);
		}
	  }

	}

}