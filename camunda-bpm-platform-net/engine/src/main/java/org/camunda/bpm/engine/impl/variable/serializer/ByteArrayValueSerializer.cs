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
namespace org.camunda.bpm.engine.impl.variable.serializer
{

	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class ByteArrayValueSerializer : PrimitiveValueSerializer<BytesValue>
	{

	  public ByteArrayValueSerializer() : base(ValueType.BYTES)
	  {
	  }

	  public virtual BytesValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		object value = untypedValue.Value;
		if (value is sbyte[])
		{
		  return Variables.byteArrayValue((sbyte[]) value, untypedValue.Transient);
		}
		else
		{
		  sbyte[] data = IoUtil.readInputStream((Stream) value, null);
		  return Variables.byteArrayValue(data, untypedValue.Transient);
		}
	  }

	  public override BytesValue readValue(ValueFields valueFields)
	  {
		return Variables.byteArrayValue(valueFields.ByteArrayValue);
	  }

	  public virtual void writeValue(BytesValue variableValue, ValueFields valueFields)
	  {
		valueFields.ByteArrayValue = variableValue.Value;
	  }

	  protected internal override bool canWriteValue(TypedValue typedValue)
	  {
		return base.canWriteValue(typedValue) || typedValue.Value is Stream;
	  }

	}

}