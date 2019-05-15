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
namespace org.camunda.bpm.integrationtest.functional.spin.dataformat
{
	using JsonGenerator = com.fasterxml.jackson.core.JsonGenerator;
	using JsonProcessingException = com.fasterxml.jackson.core.JsonProcessingException;
	using SerializerProvider = com.fasterxml.jackson.databind.SerializerProvider;
	using StdSerializer = com.fasterxml.jackson.databind.ser.std.StdSerializer;

	/// <summary>
	/// @author Svetlana Dorokhova.
	/// </summary>
	public class XmlSerializableJsonSerializer : StdSerializer<XmlSerializable>
	{

	  public XmlSerializableJsonSerializer() : base(typeof(XmlSerializable))
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void serialize(org.camunda.bpm.integrationtest.functional.spin.XmlSerializable value, com.fasterxml.jackson.core.JsonGenerator gen, com.fasterxml.jackson.databind.SerializerProvider serializers) throws java.io.IOException, com.fasterxml.jackson.core.JsonProcessingException
	  public override void serialize(XmlSerializable value, JsonGenerator gen, SerializerProvider serializers)
	  {
		gen.writeString(value.Property);
	  }
	}

}