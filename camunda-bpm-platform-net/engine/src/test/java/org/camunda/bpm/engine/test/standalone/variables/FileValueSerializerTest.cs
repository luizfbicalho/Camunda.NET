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
namespace org.camunda.bpm.engine.test.standalone.variables
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.instanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using FileValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.FileValueSerializer;
	using ValueFields = org.camunda.bpm.engine.impl.variable.serializer.ValueFields;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using FileValueTypeImpl = org.camunda.bpm.engine.variable.impl.type.FileValueTypeImpl;
	using UntypedValueImpl = org.camunda.bpm.engine.variable.impl.value.UntypedValueImpl;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class FileValueSerializerTest
	{

	  private const string SEPARATOR = "#";
	  private FileValueSerializer serializer;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		serializer = new FileValueSerializer();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTypeIsFileValueType()
	  public virtual void testTypeIsFileValueType()
	  {
		assertThat(serializer.Type, @is(instanceOf(typeof(FileValueTypeImpl))));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteFilenameOnlyValue()
	  public virtual void testWriteFilenameOnlyValue()
	  {
		string filename = "test.txt";
		FileValue fileValue = Variables.fileValue(filename).create();
		ValueFields valueFields = new MockValueFields();

		serializer.writeValue(fileValue, valueFields);

		assertThat(valueFields.ByteArrayValue, @is(nullValue()));
		assertThat(valueFields.TextValue, @is(filename));
		assertThat(valueFields.TextValue2, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteEmptyFilenameOnlyValue()
	  public virtual void testWriteEmptyFilenameOnlyValue()
	  {
		string filename = "";
		FileValue fileValue = Variables.fileValue(filename).create();
		ValueFields valueFields = new MockValueFields();

		serializer.writeValue(fileValue, valueFields);

		assertThat(valueFields.ByteArrayValue, @is(nullValue()));
		assertThat(valueFields.TextValue, @is(filename));
		assertThat(valueFields.TextValue2, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteMimetypeAndFilenameValue()
	  public virtual void testWriteMimetypeAndFilenameValue()
	  {
		string filename = "test.txt";
		string mimeType = "text/json";
		FileValue fileValue = Variables.fileValue(filename).mimeType(mimeType).create();
		ValueFields valueFields = new MockValueFields();

		serializer.writeValue(fileValue, valueFields);

		assertThat(valueFields.ByteArrayValue, @is(nullValue()));
		assertThat(valueFields.TextValue, @is(filename));
		assertThat(valueFields.TextValue2, @is(mimeType + SEPARATOR));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteMimetypeFilenameAndBytesValue() throws java.io.UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testWriteMimetypeFilenameAndBytesValue()
	  {
		string filename = "test.txt";
		string mimeType = "text/json";
		Stream @is = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt");
		FileValue fileValue = Variables.fileValue(filename).mimeType(mimeType).file(@is).create();
		ValueFields valueFields = new MockValueFields();

		serializer.writeValue(fileValue, valueFields);

		assertThat(new string(valueFields.ByteArrayValue, "UTF-8"), @is("text"));
		assertThat(valueFields.TextValue, @is(filename));
		assertThat(valueFields.TextValue2, @is(mimeType + SEPARATOR));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteMimetypeFilenameBytesValueAndEncoding() throws java.io.UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testWriteMimetypeFilenameBytesValueAndEncoding()
	  {
		string filename = "test.txt";
		string mimeType = "text/json";
		Charset encoding = Charset.forName("UTF-8");
		Stream @is = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt");
		FileValue fileValue = Variables.fileValue(filename).mimeType(mimeType).encoding(encoding).file(@is).create();
		ValueFields valueFields = new MockValueFields();

		serializer.writeValue(fileValue, valueFields);

		assertThat(new string(valueFields.ByteArrayValue, "UTF-8"), @is("text"));
		assertThat(valueFields.TextValue, @is(filename));
		assertThat(valueFields.TextValue2, @is(mimeType + SEPARATOR + encoding.name()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteMimetypeFilenameAndBytesValueWithShortcutMethod() throws java.net.URISyntaxException, java.io.UnsupportedEncodingException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testWriteMimetypeFilenameAndBytesValueWithShortcutMethod()
	  {
		File file = new File(this.GetType().ClassLoader.getResource("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt").toURI());
		FileValue fileValue = Variables.fileValue(file);
		ValueFields valueFields = new MockValueFields();

		serializer.writeValue(fileValue, valueFields);

		assertThat(new string(valueFields.ByteArrayValue, "UTF-8"), @is("text"));
		assertThat(valueFields.TextValue, @is("simpleFile.txt"));
		assertThat(valueFields.TextValue2, @is("text/plain" + SEPARATOR));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(expected = UnsupportedOperationException.class) public void testThrowsExceptionWhenConvertingUnknownUntypedValueToTypedValue()
	  public virtual void testThrowsExceptionWhenConvertingUnknownUntypedValueToTypedValue()
	  {
		serializer.convertToTypedValue((UntypedValueImpl) Variables.untypedValue(new object()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadFileNameMimeTypeAndByteArray() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testReadFileNameMimeTypeAndByteArray()
	  {
		Stream @is = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt");
		sbyte[] data = new sbyte[@is.available()];
		DataInputStream dataInputStream = new DataInputStream(@is);
		dataInputStream.readFully(data);
		dataInputStream.close();
		MockValueFields valueFields = new MockValueFields();
		string filename = "file.txt";
		valueFields.TextValue = filename;
		valueFields.ByteArrayValue = data;
		string mimeType = "text/plain";
		valueFields.TextValue2 = mimeType + SEPARATOR;

		FileValue fileValue = serializer.readValue(valueFields, true);

		assertThat(fileValue.Filename, @is(filename));
		assertThat(fileValue.MimeType, @is(mimeType));
		checkStreamFromValue(fileValue, "text");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadFileNameEncodingAndByteArray() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testReadFileNameEncodingAndByteArray()
	  {
		Stream @is = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt");
		sbyte[] data = new sbyte[@is.available()];
		DataInputStream dataInputStream = new DataInputStream(@is);
		dataInputStream.readFully(data);
		dataInputStream.close();
		MockValueFields valueFields = new MockValueFields();
		string filename = "file.txt";
		valueFields.TextValue = filename;
		valueFields.ByteArrayValue = data;
		string encoding = SEPARATOR + "UTF-8";
		valueFields.TextValue2 = encoding;

		FileValue fileValue = serializer.readValue(valueFields, true);

		assertThat(fileValue.Filename, @is(filename));
		assertThat(fileValue.Encoding, @is("UTF-8"));
		assertThat(fileValue.EncodingAsCharset, @is(Charset.forName("UTF-8")));
		checkStreamFromValue(fileValue, "text");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadFullValue() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testReadFullValue()
	  {
		Stream @is = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt");
		sbyte[] data = new sbyte[@is.available()];
		DataInputStream dataInputStream = new DataInputStream(@is);
		dataInputStream.readFully(data);
		dataInputStream.close();
		MockValueFields valueFields = new MockValueFields();
		string filename = "file.txt";
		valueFields.TextValue = filename;
		valueFields.ByteArrayValue = data;
		string mimeType = "text/plain";
		string encoding = "UTF-16";
		valueFields.TextValue2 = mimeType + SEPARATOR + encoding;

		FileValue fileValue = serializer.readValue(valueFields, true);

		assertThat(fileValue.Filename, @is(filename));
		assertThat(fileValue.MimeType, @is(mimeType));
		assertThat(fileValue.Encoding, @is("UTF-16"));
		assertThat(fileValue.EncodingAsCharset, @is(Charset.forName("UTF-16")));
		checkStreamFromValue(fileValue, "text");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadFilenameAndByteArrayValue() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testReadFilenameAndByteArrayValue()
	  {
		Stream @is = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/standalone/variables/simpleFile.txt");
		sbyte[] data = new sbyte[@is.available()];
		DataInputStream dataInputStream = new DataInputStream(@is);
		dataInputStream.readFully(data);
		dataInputStream.close();
		MockValueFields valueFields = new MockValueFields();
		string filename = "file.txt";
		valueFields.TextValue = filename;
		valueFields.ByteArrayValue = data;

		FileValue fileValue = serializer.readValue(valueFields, true);

		assertThat(fileValue.Filename, @is(filename));
		assertThat(fileValue.MimeType, @is(nullValue()));
		checkStreamFromValue(fileValue, "text");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadFilenameValue() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testReadFilenameValue()
	  {
		MockValueFields valueFields = new MockValueFields();
		string filename = "file.txt";
		valueFields.TextValue = filename;

		FileValue fileValue = serializer.readValue(valueFields, true);

		assertThat(fileValue.Filename, @is(filename));
		assertThat(fileValue.MimeType, @is(nullValue()));
		assertThat(fileValue.Value, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadEmptyFilenameValue()
	  public virtual void testReadEmptyFilenameValue()
	  {
		MockValueFields valueFields = new MockValueFields();
		string filename = "";
		valueFields.TextValue = filename;

		FileValue fileValue = serializer.readValue(valueFields, true);

		assertThat(fileValue.Filename, @is(""));
		assertThat(fileValue.MimeType, @is(nullValue()));
		assertThat(fileValue.Value, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadNullFilenameValue()
	  public virtual void testReadNullFilenameValue()
	  {
		MockValueFields valueFields = new MockValueFields();
		string filename = null;
		valueFields.TextValue = filename;

		FileValue fileValue = serializer.readValue(valueFields, true);

		assertThat(fileValue.Filename, @is(""));
		assertThat(fileValue.MimeType, @is(nullValue()));
		assertThat(fileValue.Value, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNameIsFile()
	  public virtual void testNameIsFile()
	  {
		assertThat(serializer.Name, @is("file"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteFilenameAndEncodingValue()
	  public virtual void testWriteFilenameAndEncodingValue()
	  {
		string filename = "test.txt";
		string encoding = "UTF-8";
		FileValue fileValue = Variables.fileValue(filename).encoding(encoding).create();
		ValueFields valueFields = new MockValueFields();

		serializer.writeValue(fileValue, valueFields);

		assertThat(valueFields.ByteArrayValue, @is(nullValue()));
		assertThat(valueFields.TextValue, @is(filename));
		assertThat(valueFields.TextValue2, @is(SEPARATOR + encoding));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(expected = IllegalArgumentException.class) public void testSerializeFileValueWithoutName()
	  public virtual void testSerializeFileValueWithoutName()
	  {
		Variables.fileValue((string) null).file("abc".GetBytes()).create();
	  }

	  private void checkStreamFromValue(TypedValue value, string expected)
	  {
		Stream stream = (Stream) value.Value;
		using (Scanner scanner = new Scanner(stream))
		{
		  assertThat(scanner.nextLine(), @is(expected));
		}
	  }

	  private class MockValueFields : ValueFields
	  {

		internal string name;
		internal string textValue;
		internal string textValue2;
		internal long? longValue;
		internal double? doubleValue;
		internal sbyte[] bytes;

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual string TextValue
		{
			get
			{
			  return textValue;
			}
			set
			{
			  this.textValue = value;
			}
		}


		public virtual string TextValue2
		{
			get
			{
			  return textValue2;
			}
			set
			{
			  this.textValue2 = value;
			}
		}


		public virtual long? LongValue
		{
			get
			{
			  return longValue;
			}
			set
			{
			  this.longValue = value;
			}
		}


		public virtual double? DoubleValue
		{
			get
			{
			  return doubleValue;
			}
			set
			{
			  this.doubleValue = value;
			}
		}


		public virtual sbyte[] ByteArrayValue
		{
			get
			{
			  return bytes;
			}
			set
			{
			  this.bytes = value;
			}
		}


	  }
	}

}