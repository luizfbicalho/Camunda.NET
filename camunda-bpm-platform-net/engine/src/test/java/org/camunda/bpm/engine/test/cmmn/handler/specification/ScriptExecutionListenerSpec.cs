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
namespace org.camunda.bpm.engine.test.cmmn.handler.specification
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateListener = org.camunda.bpm.engine.@delegate.DelegateListener;
	using ScriptCaseExecutionListener = org.camunda.bpm.engine.impl.cmmn.listener.ScriptCaseExecutionListener;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using CamundaCaseExecutionListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaCaseExecutionListener;
	using CamundaScript = org.camunda.bpm.model.cmmn.instance.camunda.CamundaScript;

	public class ScriptExecutionListenerSpec : AbstractExecutionListenerSpec
	{

	  //could be configurable
	  protected internal const string SCRIPT_FORMAT = "org.camunda.bpm.test.caseexecutionlistener.ABC";

	  public ScriptExecutionListenerSpec(string eventName) : base(eventName)
	  {
	  }

	  protected internal override void configureCaseExecutionListener(CmmnModelInstance modelInstance, CamundaCaseExecutionListener listener)
	  {
		CamundaScript script = SpecUtil.createElement(modelInstance, listener, null, typeof(CamundaScript));
		string scriptValue = "${myScript}";
		script.CamundaScriptFormat = SCRIPT_FORMAT;
		script.TextContent = scriptValue;
	  }

	  public override void verifyListener<T1>(DelegateListener<T1> listener) where T1 : org.camunda.bpm.engine.@delegate.BaseDelegateExecution
	  {
		assertTrue(listener is ScriptCaseExecutionListener);

		ScriptCaseExecutionListener scriptListener = (ScriptCaseExecutionListener) listener;
		ExecutableScript executableScript = scriptListener.Script;
		assertNotNull(executableScript);
		assertEquals(SCRIPT_FORMAT, executableScript.Language);
	  }

	}

}