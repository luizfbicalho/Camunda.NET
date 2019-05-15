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
namespace org.camunda.spin.plugin.variable.value.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.Spin.S;

	using AbstractTypedValue = org.camunda.bpm.engine.variable.impl.value.AbstractTypedValue;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using SpinValueType = org.camunda.spin.plugin.variable.type.SpinValueType;
	using DataFormat = org.camunda.spin.spi.DataFormat;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class SpinValueImpl : AbstractTypedValue<Spin<JavaToDotNetGenericWildcard>>, SpinValue
	{

	  private const long serialVersionUID = 1L;
	  protected internal string serializedValue;
	  protected internal bool isDeserialized;
	  protected internal string dataFormatName;

	  public SpinValueImpl<T1>(Spin<T1> value, string serializedValue, string dataFormatName, bool isDeserialized, ValueType type, bool isTransient) : base(value, type)
	  {


		this.serializedValue = serializedValue;
		this.dataFormatName = dataFormatName;
		this.isDeserialized = isDeserialized;
		this.isTransient = isTransient;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.spin.Spin<?> getValue()
	  public virtual Spin<object> Value
	  {
		  get
		  {
			if (isDeserialized)
			{
			  return base.Value;
			}
			else
			{
			  // deserialize the serialized value by using
			  // the given data format
			  value = S(ValueSerialized, SerializationDataFormat);
			  isDeserialized = true;
    
			  ValueSerialized = null;
    
			  return value;
			}
		  }
	  }

	  public virtual SpinValueType Type
	  {
		  get
		  {
			return (SpinValueType) base.Type;
		  }
	  }

	  public virtual bool Deserialized
	  {
		  get
		  {
			return isDeserialized;
		  }
	  }

	  public virtual string ValueSerialized
	  {
		  get
		  {
			return serializedValue;
		  }
		  set
		  {
			this.serializedValue = value;
		  }
	  }


	  public virtual string SerializationDataFormat
	  {
		  get
		  {
			return dataFormatName;
		  }
		  set
		  {
			this.dataFormatName = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.spin.spi.DataFormat<? extends org.camunda.spin.Spin<?>> getDataFormat()
	  public virtual DataFormat<Spin<object>> DataFormat
	  {
		  get
		  {
			if (isDeserialized)
			{
			  return DataFormats.getDataFormat(dataFormatName);
			}
			else
			{
			  throw new System.InvalidOperationException("Spin value is not deserialized.");
			}
		  }
	  }

	}

}