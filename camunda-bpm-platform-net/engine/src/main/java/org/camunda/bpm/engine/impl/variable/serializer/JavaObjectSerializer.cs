using System;
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
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;

	/// <summary>
	/// Uses default java serialization to serialize java objects as byte streams.
	/// 
	/// @author Daniel Meyer
	/// @author Tom Baeyens
	/// </summary>
	public class JavaObjectSerializer : AbstractObjectValueSerializer
	{

	  public const string NAME = "serializable";

	  public JavaObjectSerializer() : base(SerializationDataFormats.JAVA.Name)
	  {
	  }

	  public virtual string Name
	  {
		  get
		  {
			return NAME;
		  }
	  }

	  protected internal override bool SerializationTextBased
	  {
		  get
		  {
			return false;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object deserializeFromByteArray(byte[] bytes, String objectTypeName) throws Exception
	  protected internal override object deserializeFromByteArray(sbyte[] bytes, string objectTypeName)
	  {
		MemoryStream bais = new MemoryStream(bytes);
		ObjectInputStream ois = null;
		try
		{
		  ois = new ClassloaderAwareObjectInputStream(bais);
		  return ois.readObject();
		}
		finally
		{
		  IoUtil.closeSilently(ois);
		  IoUtil.closeSilently(bais);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected byte[] serializeToByteArray(Object deserializedObject) throws Exception
	  protected internal override sbyte[] serializeToByteArray(object deserializedObject)
	  {
		MemoryStream baos = new MemoryStream();
		ObjectOutputStream ois = null;
		try
		{
		  ois = new ObjectOutputStream(baos);
		  ois.writeObject(deserializedObject);
		  return baos.toByteArray();
		}
		finally
		{
		  IoUtil.closeSilently(ois);
		  IoUtil.closeSilently(baos);
		}
	  }

	  protected internal override string getTypeNameForDeserialized(object deserializedObject)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		return deserializedObject.GetType().FullName;
	  }

	  protected internal override bool canSerializeValue(object value)
	  {
		return value is Serializable;
	  }

	  protected internal class ClassloaderAwareObjectInputStream : ObjectInputStream
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ClassloaderAwareObjectInputStream(java.io.InputStream in) throws java.io.IOException
		public ClassloaderAwareObjectInputStream(Stream @in) : base(@in)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Class resolveClass(java.io.ObjectStreamClass desc) throws java.io.IOException, ClassNotFoundException
		protected internal virtual Type resolveClass(ObjectStreamClass desc)
		{
		  return ReflectUtil.loadClass(desc.Name);
		}

	  }
	}

}