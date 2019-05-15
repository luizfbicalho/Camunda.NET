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
namespace org.camunda.bpm.integrationtest.functional.spin
{

	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using JsonSerializable = org.camunda.bpm.integrationtest.functional.spin.dataformat.JsonSerializable;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class RuntimeServiceDelegate : JavaDelegate
	{

	  public const string VARIABLE_NAME = "var";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		RuntimeService runtimeService = execution.ProcessEngineServices.RuntimeService;

		ObjectValue jsonSerializeable = Variables.objectValue(createJsonSerializable()).serializationDataFormat(Variables.SerializationDataFormats.JSON).create();

		// this should be executed in the context of the current process application
		runtimeService.setVariable(execution.ProcessInstanceId, VARIABLE_NAME, jsonSerializeable);

	  }

	  public static JsonSerializable createJsonSerializable()
	  {
		return new JsonSerializable(new DateTime(JsonSerializable.ONE_DAY_IN_MILLIS * 10));
	  }
	}

}