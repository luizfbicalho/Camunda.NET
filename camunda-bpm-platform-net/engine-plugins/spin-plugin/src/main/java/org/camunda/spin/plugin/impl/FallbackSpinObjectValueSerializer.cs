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
	using AbstractObjectValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.AbstractObjectValueSerializer;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class FallbackSpinObjectValueSerializer : AbstractObjectValueSerializer
	{

	  private static readonly SpinPluginLogger LOG = SpinPluginLogger.LOGGER;

	  public const string DESERIALIZED_OBJECTS_EXCEPTION_MESSAGE = "Fallback serializer cannot handle deserialized objects";

	  protected internal string serializationFormat;

	  public FallbackSpinObjectValueSerializer(string serializationFormat) : base(serializationFormat)
	  {
		this.serializationFormat = serializationFormat;
	  }

	  public override string Name
	  {
		  get
		  {
			return "spin://" + serializationFormat;
		  }
	  }

	  protected internal override string getTypeNameForDeserialized(object deserializedObject)
	  {
		throw LOG.fallbackSerializerCannotDeserializeObjects();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected byte[] serializeToByteArray(Object deserializedObject) throws Exception
	  protected internal override sbyte[] serializeToByteArray(object deserializedObject)
	  {
		throw LOG.fallbackSerializerCannotDeserializeObjects();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected Object deserializeFromByteArray(byte[] object, String objectTypeName) throws Exception
	  protected internal override object deserializeFromByteArray(sbyte[] @object, string objectTypeName)
	  {
		throw LOG.fallbackSerializerCannotDeserializeObjects();
	  }

	  protected internal override bool SerializationTextBased
	  {
		  get
		  {
			return true;
		  }
	  }

	  protected internal override bool canSerializeValue(object value)
	  {
		throw LOG.fallbackSerializerCannotDeserializeObjects();
	  }

	}

}