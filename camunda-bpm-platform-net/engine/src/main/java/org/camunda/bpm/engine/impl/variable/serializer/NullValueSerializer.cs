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
	using NullValueImpl = org.camunda.bpm.engine.variable.impl.value.NullValueImpl;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Used to serialize untyped null values.
	/// 
	/// @author Daniel Meyer
	/// @author Tom Baeyens
	/// </summary>
	public class NullValueSerializer : AbstractTypedValueSerializer<NullValueImpl>
	{

	  public NullValueSerializer() : base(ValueType.NULL)
	  {
	  }

	  public override string Name
	  {
		  get
		  {
			return ValueType.NULL.Name.ToLower();
		  }
	  }

	  public override NullValueImpl convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return !untypedValue.Transient ? NullValueImpl.INSTANCE : NullValueImpl.INSTANCE_TRANSIENT;
	  }

	  public virtual void writeValue(NullValueImpl value, ValueFields valueFields)
	  {
		// nothing to do
	  }

	  public override NullValueImpl readValue(ValueFields valueFields, bool deserialize)
	  {
		return NullValueImpl.INSTANCE;
	  }

	  protected internal override bool canWriteValue(TypedValue value)
	  {
		return value.Value == null;
	  }

	}

}