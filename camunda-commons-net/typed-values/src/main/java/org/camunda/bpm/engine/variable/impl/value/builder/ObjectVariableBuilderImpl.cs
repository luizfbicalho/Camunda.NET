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
namespace org.camunda.bpm.engine.variable.impl.value.builder
{
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using SerializationDataFormat = org.camunda.bpm.engine.variable.value.SerializationDataFormat;
	using ObjectValueBuilder = org.camunda.bpm.engine.variable.value.builder.ObjectValueBuilder;
	using TypedValueBuilder = org.camunda.bpm.engine.variable.value.builder.TypedValueBuilder;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ObjectVariableBuilderImpl : ObjectValueBuilder
	{

	  protected internal ObjectValueImpl variableValue;

	  public ObjectVariableBuilderImpl(object value)
	  {
		variableValue = new ObjectValueImpl(value);
	  }

	  public ObjectVariableBuilderImpl(ObjectValue value)
	  {
		variableValue = (ObjectValueImpl) value;
	  }

	  public virtual ObjectValue create()
	  {
		return variableValue;
	  }

	  public virtual ObjectValueBuilder serializationDataFormat(string dataFormatName)
	  {
		variableValue.SerializationDataFormat = dataFormatName;
		return this;
	  }

	  public virtual ObjectValueBuilder serializationDataFormat(SerializationDataFormat dataFormat)
	  {
		return serializationDataFormat(dataFormat.Name);
	  }

	  public override TypedValueBuilder<ObjectValue> setTransient(bool isTransient)
	  {
		variableValue.Transient = isTransient;
		return this;
	  }

	}

}