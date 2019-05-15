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
namespace org.camunda.spin.plugin.impl
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using AbstractObjectValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractObjectValueSerializer;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using DataFormat = org.camunda.spin.spi.DataFormat;
	using DataFormatMapper = org.camunda.spin.spi.DataFormatMapper;
	using DataFormatReader = org.camunda.spin.spi.DataFormatReader;
	using DataFormatWriter = org.camunda.spin.spi.DataFormatWriter;

	/// <summary>
	/// Implementation of a <seealso cref="TypedValueSerializer"/> for <seealso cref="ObjectValue ObjectValues"/> using a
	/// Spin-provided <seealso cref="DataFormat"/> to serialize and deserialize java objects.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class SpinObjectValueSerializer : AbstractObjectValueSerializer
	{

	  protected internal string name;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected org.camunda.spin.spi.DataFormat<?> dataFormat;
	  protected internal DataFormat<object> dataFormat;

	  public SpinObjectValueSerializer<T1>(string name, DataFormat<T1> dataFormat) : base(dataFormat.Name)
	  {
		this.name = name;
		this.dataFormat = dataFormat;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  protected internal override bool SerializationTextBased
	  {
		  get
		  {
			// for the moment we assume that all spin data formats are text based.
			return true;
		  }
	  }

	  protected internal override string getTypeNameForDeserialized(object deserializedObject)
	  {
		return dataFormat.Mapper.getCanonicalTypeName(deserializedObject);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected byte[] serializeToByteArray(Object deserializedObject) throws Exception
	  protected internal override sbyte[] serializeToByteArray(object deserializedObject)
	  {
		DataFormatMapper mapper = dataFormat.Mapper;
		DataFormatWriter writer = dataFormat.Writer;

		MemoryStream @out = new MemoryStream();
		StreamWriter outWriter = new StreamWriter(@out, Context.ProcessEngineConfiguration.DefaultCharset);
		StreamWriter bufferedWriter = new StreamWriter(outWriter);

		try
		{
		  object mappedObject = mapper.mapJavaToInternal(deserializedObject);
		  writer.writeToWriter(bufferedWriter, mappedObject);
		  return @out.toByteArray();
		}
		finally
		{
		  IoUtil.closeSilently(@out);
		  IoUtil.closeSilently(outWriter);
		  IoUtil.closeSilently(bufferedWriter);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object deserializeFromByteArray(byte[] bytes, String objectTypeName) throws Exception
	  protected internal override object deserializeFromByteArray(sbyte[] bytes, string objectTypeName)
	  {
		DataFormatMapper mapper = dataFormat.Mapper;
		DataFormatReader reader = dataFormat.Reader;

		MemoryStream bais = new MemoryStream(bytes);
		StreamReader inReader = new StreamReader(bais, Context.ProcessEngineConfiguration.DefaultCharset);
		StreamReader bufferedReader = new StreamReader(inReader);

		try
		{
		  object mappedObject = reader.readInput(bufferedReader);
		  return mapper.mapInternalToJava(mappedObject, objectTypeName);
		}
		finally
		{
		  IoUtil.closeSilently(bais);
		  IoUtil.closeSilently(inReader);
		  IoUtil.closeSilently(bufferedReader);
		}
	  }

	  protected internal override bool canSerializeValue(object value)
	  {
		return dataFormat.Mapper.canMap(value);
	  }

	}

}