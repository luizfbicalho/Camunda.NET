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
namespace org.camunda.spin.plugin.variables
{
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using VariableSerializerFactory = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializerFactory;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Deployment = org.camunda.bpm.engine.test.Deployment;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class FallbackSerializationTest : PluggableProcessEngineTestCase
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/spin/plugin/oneTaskProcess.bpmn20.xml";

	  [Deployment(resources : ONE_TASK_PROCESS)]
	  public virtual void testSerializationOfUnknownFormat()
	  {
		// given
		ProcessInstance instance = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		ObjectValue objectValue = Variables.serializedObjectValue("foo").serializationDataFormat("application/foo").objectTypeName("org.camunda.Foo").create();

		runtimeService.setVariable(instance.Id, "var", objectValue);

		// then
		try
		{
		  runtimeService.getVariable(instance.Id, "var");
		  fail();
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("Fallback serializer cannot handle deserialized objects", e.Message);
		}

		ObjectValue returnedValue = runtimeService.getVariableTyped(instance.Id, "var", false);
		assertFalse(returnedValue.Deserialized);
		assertEquals("application/foo", returnedValue.SerializationDataFormat);
		assertEquals("foo", returnedValue.ValueSerialized);
		assertEquals("org.camunda.Foo", returnedValue.ObjectTypeName);

	  }
	}

}