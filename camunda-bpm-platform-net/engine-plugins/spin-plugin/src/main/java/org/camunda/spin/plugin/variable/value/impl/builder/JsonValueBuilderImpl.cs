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
namespace org.camunda.spin.plugin.variable.value.impl.builder
{
	using SerializationDataFormat = org.camunda.bpm.engine.variable.value.SerializationDataFormat;
	using SpinJsonNode = org.camunda.spin.json.SpinJsonNode;
	using JsonValueBuilder = org.camunda.spin.plugin.variable.value.builder.JsonValueBuilder;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class JsonValueBuilderImpl : SpinValueBuilderImpl<JsonValue>, JsonValueBuilder
	{

	  public JsonValueBuilderImpl(JsonValue value) : base(value)
	  {
	  }

	  public JsonValueBuilderImpl(string value) : this(new JsonValueImpl(value))
	  {
	  }

	  public JsonValueBuilderImpl(SpinJsonNode value) : this(new JsonValueImpl(value))
	  {
	  }

	  public override JsonValueBuilder serializationDataFormat(SerializationDataFormat dataFormat)
	  {
		return (JsonValueBuilderImpl) base.serializationDataFormat(dataFormat);
	  }

	  public override JsonValueBuilder serializationDataFormat(string dataFormatName)
	  {
		return (JsonValueBuilderImpl) base.serializationDataFormat(dataFormatName);
	  }

	}

}