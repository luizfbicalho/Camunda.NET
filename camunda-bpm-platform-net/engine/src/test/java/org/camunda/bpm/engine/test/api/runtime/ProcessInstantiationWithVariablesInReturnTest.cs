using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertFalse;
	using ProcessInstanceWithVariables = org.camunda.bpm.engine.runtime.ProcessInstanceWithVariables;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using Rule = org.junit.Rule;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertTrue;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using JavaSerializable = org.camunda.bpm.engine.test.api.variables.JavaSerializable;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.TypedValueAssert.assertObjectValueSerializedJava;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// Represents the test class for the process instantiation on which
	/// the process instance is returned with variables.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfigurationImpl.HISTORY_AUDIT)]
	public class ProcessInstantiationWithVariablesInReturnTest
	{
		private bool InstanceFieldsInitialized = false;

		public ProcessInstantiationWithVariablesInReturnTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			chain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testHelper);
		}


	  protected internal const string SUBPROCESS_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstanceModificationTest.subprocess.bpmn20.xml";
	  protected internal const string SET_VARIABLE_IN_DELEGATE_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstantiationWithVariablesInReturn.setVariableInDelegate.bpmn20.xml";
	  protected internal const string SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstantiationWithVariablesInReturn.setVariableInDelegateWithWaitState.bpmn20.xml";
	  protected internal const string SIMPLE_PROCESS = "org/camunda/bpm/engine/test/api/runtime/ProcessInstantiationWithVariablesInReturn.simpleProcess.bpmn20.xml";

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }
	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  public ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain chain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule).around(testHelper);
	  public RuleChain chain;

	  private void checkVariables(VariableMap map, int expectedSize)
	  {
		IList<HistoricVariableInstance> variables = engineRule.HistoryService.createHistoricVariableInstanceQuery().orderByVariableName().asc().list();

		assertEquals(expectedSize, variables.Count);

		assertEquals(variables.Count, map.size());
		foreach (HistoricVariableInstance instance in variables)
		{
		  assertTrue(map.containsKey(instance.Name));
		  object instanceValue = instance.TypedValue.Value;
		  object mapValue = map.getValueTyped(instance.Name).Value;
		  if (instanceValue == null)
		  {
			assertNull(mapValue);
		  }
		  else if (instanceValue is sbyte[])
		  {
			assertTrue(Arrays.Equals((sbyte[]) instanceValue, (sbyte[]) mapValue));
		  }
		  else
		  {
			assertEquals(instanceValue, mapValue);
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void testVariablesWithoutDeserialization(String processDefinitionKey) throws Exception
	  private void testVariablesWithoutDeserialization(string processDefinitionKey)
	  {
		//given serializable variable
		JavaSerializable javaSerializable = new JavaSerializable("foo");

		MemoryStream baos = new MemoryStream();
		(new ObjectOutputStream(baos)).writeObject(javaSerializable);
		string serializedObject = StringUtil.fromBytes(Base64.encodeBase64(baos.toByteArray()), engineRule.ProcessEngine);

		//when execute process with serialized variable and wait state
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey(processDefinitionKey).setVariable("serializedVar", serializedObjectValue(serializedObject).serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName(typeof(JavaSerializable).FullName).create()).executeWithVariablesInReturn(false, false);

		//then returned instance contains serialized variable
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		ObjectValue serializedVar = (ObjectValue) map.getValueTyped("serializedVar");
		assertFalse(serializedVar.Deserialized);
		assertObjectValueSerializedJava(serializedVar, javaSerializable);

		//access on value should fail because variable is not deserialized
		try
		{
		  serializedVar.Value;
		  Assert.fail("Deserialization should fail!");
		}
		catch (System.InvalidOperationException ise)
		{
		  assertTrue(ise.Message.Equals("Object is not deserialized."));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SIMPLE_PROCESS) public void testReturnVariablesFromStartWithoutDeserialization() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : SIMPLE_PROCESS)]
	  public virtual void testReturnVariablesFromStartWithoutDeserialization()
	  {
		testVariablesWithoutDeserialization("simpleProcess");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SUBPROCESS_PROCESS) public void testReturnVariablesFromStartWithoutDeserializationWithWaitstate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  [Deployment(resources : SUBPROCESS_PROCESS)]
	  public virtual void testReturnVariablesFromStartWithoutDeserializationWithWaitstate()
	  {
		testVariablesWithoutDeserialization("subprocess");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SIMPLE_PROCESS) public void testReturnVariablesFromStart()
	  [Deployment(resources : SIMPLE_PROCESS)]
	  public virtual void testReturnVariablesFromStart()
	  {
		//given execute process with variables
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey("simpleProcess").setVariable("aVariable1", "aValue1").setVariableLocal("aVariable2", "aValue2").setVariables(Variables.createVariables().putValue("aVariable3", "aValue3")).setVariablesLocal(Variables.createVariables().putValue("aVariable4", new sbyte[]{127, 34, 64})).executeWithVariablesInReturn(false, false);

		//when returned instance contains variables
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		// then variables equal to variables which are accessible via query
		checkVariables(map, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SUBPROCESS_PROCESS) public void testReturnVariablesFromStartWithWaitstate()
	  [Deployment(resources : SUBPROCESS_PROCESS)]
	  public virtual void testReturnVariablesFromStartWithWaitstate()
	  {
		//given execute process with variables and wait state
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey("subprocess").setVariable("aVariable1", "aValue1").setVariableLocal("aVariable2", "aValue2").setVariables(Variables.createVariables().putValue("aVariable3", "aValue3")).setVariablesLocal(Variables.createVariables().putValue("aVariable4", new sbyte[]{127, 34, 64})).executeWithVariablesInReturn(false, false);

		//when returned instance contains variables
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		// then variables equal to variables which are accessible via query
		checkVariables(map, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SUBPROCESS_PROCESS) public void testReturnVariablesFromStartWithWaitstateStartInSubProcess()
	  [Deployment(resources : SUBPROCESS_PROCESS)]
	  public virtual void testReturnVariablesFromStartWithWaitstateStartInSubProcess()
	  {
		//given execute process with variables and wait state in sub process
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey("subprocess").setVariable("aVariable1", "aValue1").setVariableLocal("aVariable2", "aValue2").setVariables(Variables.createVariables().putValue("aVariable3", "aValue3")).setVariablesLocal(Variables.createVariables().putValue("aVariable4", new sbyte[]{127, 34, 64})).startBeforeActivity("innerTask").executeWithVariablesInReturn(true, true);

		//when returned instance contains variables
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		// then variables equal to variables which are accessible via query
		checkVariables(map, 4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SET_VARIABLE_IN_DELEGATE_PROCESS) public void testReturnVariablesFromExecution()
	  [Deployment(resources : SET_VARIABLE_IN_DELEGATE_PROCESS)]
	  public virtual void testReturnVariablesFromExecution()
	  {

		//given executed process which sets variables in java delegate
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey("variableProcess").executeWithVariablesInReturn();
		//when returned instance contains variables
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		// then variables equal to variables which are accessible via query
		checkVariables(map, 8);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS) public void testReturnVariablesFromExecutionWithWaitstate()
	  [Deployment(resources : SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS)]
	  public virtual void testReturnVariablesFromExecutionWithWaitstate()
	  {

		//given executed process which sets variables in java delegate
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey("variableProcess").executeWithVariablesInReturn();
		//when returned instance contains variables
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		// then variables equal to variables which are accessible via query
		checkVariables(map, 8);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SET_VARIABLE_IN_DELEGATE_PROCESS) public void testReturnVariablesFromStartAndExecution()
	  [Deployment(resources : SET_VARIABLE_IN_DELEGATE_PROCESS)]
	  public virtual void testReturnVariablesFromStartAndExecution()
	  {

		//given executed process which sets variables in java delegate
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey("variableProcess").setVariable("aVariable1", "aValue1").setVariableLocal("aVariable2", "aValue2").setVariables(Variables.createVariables().putValue("aVariable3", "aValue3")).setVariablesLocal(Variables.createVariables().putValue("aVariable4", new sbyte[]{127, 34, 64})).executeWithVariablesInReturn();
		//when returned instance contains variables
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		// then variables equal to variables which are accessible via query
		checkVariables(map, 12);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS) public void testReturnVariablesFromStartAndExecutionWithWaitstate()
	  [Deployment(resources : SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS)]
	  public virtual void testReturnVariablesFromStartAndExecutionWithWaitstate()
	  {

		//given executed process which overwrites these four variables in java delegate
		// and adds four additional variables
		ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.createProcessInstanceByKey("variableProcess").setVariable("stringVar", "aValue1").setVariableLocal("integerVar", 56789).setVariables(Variables.createVariables().putValue("longVar", 123L)).setVariablesLocal(Variables.createVariables().putValue("byteVar", new sbyte[]{127, 34, 64})).executeWithVariablesInReturn(false, false);
		//when returned instance contains variables
		VariableMap map = procInstance.Variables;
		assertNotNull(map);

		// then variables equal to variables which are accessible via query
		checkVariables(map, 8);
	  }

	}

}