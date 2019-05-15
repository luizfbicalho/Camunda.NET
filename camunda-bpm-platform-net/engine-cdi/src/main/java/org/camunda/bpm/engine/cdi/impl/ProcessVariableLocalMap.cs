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
namespace org.camunda.bpm.engine.cdi.impl
{
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Allows to expose the local process variables of the current business process as a
	/// java.util.Map<String,Object>
	/// <p/>
	/// The map delegates changes to
	/// <seealso cref="BusinessProcess#setVariableLocal(String, Object)"/> and
	/// <seealso cref="BusinessProcess#getVariableLocal(String)"/>, so that they are not flushed
	/// prematurely.
	/// 
	/// @author Michael Scholz
	/// </summary>
	public class ProcessVariableLocalMap : AbstractVariableMap
	{

	  protected internal override object getVariable(string variableName)
	  {
		return businessProcess.getVariableLocal(variableName);
	  }

	  protected internal override T getVariableTyped<T>(string variableName) where T : org.camunda.bpm.engine.variable.value.TypedValue
	  {
		return businessProcess.getVariableLocalTyped(variableName);
	  }

	  protected internal override void setVariable(string variableName, object value)
	  {
		businessProcess.setVariableLocal(variableName, value);
	  }
	}

}