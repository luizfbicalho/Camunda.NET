﻿using System;

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
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using ShortValue = org.camunda.bpm.engine.variable.value.ShortValue;

	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class ShortValueSerializer : PrimitiveValueSerializer<ShortValue>
	{

	  public ShortValueSerializer() : base(ValueType.SHORT)
	  {
	  }

	  public virtual ShortValue convertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.shortValue((short?) untypedValue.Value, untypedValue.Transient);
	  }

	  public override ShortValue readValue(ValueFields valueFields)
	  {
		long? longValue = valueFields.LongValue;
		short? shortValue = null;

		if (longValue != null)
		{
		  shortValue = Convert.ToInt16(longValue.Value);
		}

		return Variables.shortValue(shortValue);
	  }

	  public virtual void writeValue(ShortValue value, ValueFields valueFields)
	  {

		short? shortValue = value.Value;

		if (shortValue != null)
		{
		  valueFields.LongValue = shortValue.Value;
		  valueFields.TextValue = value.ToString();
		}
		else
		{
		  valueFields.LongValue = null;
		  valueFields.TextValue = null;
		}
	  }

	}

}