using System.Collections.Generic;
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
namespace org.camunda.bpm.engine.test.variables
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.instanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.collection.IsMapContaining.hasEntry;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsEqual.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using Variables = org.camunda.bpm.engine.variable.Variables;
	using FileValueTypeImpl = org.camunda.bpm.engine.variable.impl.type.FileValueTypeImpl;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using IoUtil = org.camunda.commons.utils.IoUtil;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class FileValueTypeImplTest
	{

	  private FileValueTypeImpl type;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		type = new FileValueTypeImpl();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void nameShouldBeFile()
	  public virtual void nameShouldBeFile()
	  {
		assertThat(type.Name, @is("file"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotHaveParent()
	  public virtual void shouldNotHaveParent()
	  {
		assertThat(type.Parent, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void isPrimitiveValue()
	  public virtual void isPrimitiveValue()
	  {
		assertThat(type.PrimitiveValueType, @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void isNotAnAbstractType()
	  public virtual void isNotAnAbstractType()
	  {
		assertThat(type.Abstract, @is(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void canNotConvertFromAnyValue()
	  public virtual void canNotConvertFromAnyValue()
	  {
		// we just use null to make sure false is always returned
		assertThat(type.canConvertFromTypedValue(null), @is(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(expected = IllegalArgumentException.class) public void convertingThrowsException()
	  public virtual void convertingThrowsException()
	  {
		type.convertFromTypedValue(Variables.untypedNullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createValueFromFile() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void createValueFromFile()
	  {
		File file = new File(this.GetType().ClassLoader.getResource("org/camunda/bpm/engine/test/variables/simpleFile.txt").toURI());
		TypedValue value = type.createValue(file, Collections.singletonMap<string, object> (FileValueTypeImpl.VALUE_INFO_FILE_NAME, "simpleFile.txt"));
		assertThat(value, @is(instanceOf(typeof(FileValue))));
		assertThat(value.Type, @is(instanceOf(typeof(FileValueTypeImpl))));
		checkStreamFromValue(value, "text");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createValueFromStream()
	  public virtual void createValueFromStream()
	  {
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		TypedValue value = type.createValue(file, Collections.singletonMap<string, object> (FileValueTypeImpl.VALUE_INFO_FILE_NAME, "simpleFile.txt"));
		assertThat(value, @is(instanceOf(typeof(FileValue))));
		assertThat(value.Type, @is(instanceOf(typeof(FileValueTypeImpl))));
		checkStreamFromValue(value, "text");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createValueFromBytes() throws java.io.IOException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void createValueFromBytes()
	  {
		File file = new File(this.GetType().ClassLoader.getResource("org/camunda/bpm/engine/test/variables/simpleFile.txt").toURI());
		TypedValue value = type.createValue(file, Collections.singletonMap<string, object> (FileValueTypeImpl.VALUE_INFO_FILE_NAME, "simpleFile.txt"));
		assertThat(value, @is(instanceOf(typeof(FileValue))));
		assertThat(value.Type, @is(instanceOf(typeof(FileValueTypeImpl))));
		checkStreamFromValue(value, "text");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(expected = IllegalArgumentException.class) public void createValueFromObject() throws java.io.IOException, java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void createValueFromObject()
	  {
		type.createValue(new object(), Collections.singletonMap<string, object> (FileValueTypeImpl.VALUE_INFO_FILE_NAME, "simpleFile.txt"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createValueWithProperties()
	  public virtual void createValueWithProperties()
	  {
		// given
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["filename"] = "someFileName";
		properties["mimeType"] = "someMimeType";
		properties["encoding"] = "someEncoding";

		TypedValue value = type.createValue(file, properties);

		assertThat(value, @is(instanceOf(typeof(FileValue))));
		FileValue fileValue = (FileValue) value;
		assertThat(fileValue.Filename, @is("someFileName"));
		assertThat(fileValue.MimeType, @is("someMimeType"));
		assertThat(fileValue.Encoding, @is("someEncoding"));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void createValueWithNullProperties()
	  public virtual void createValueWithNullProperties()
	  {
		// given
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["filename"] = "someFileName";
		properties["mimeType"] = null;
		properties["encoding"] = "someEncoding";

		// when
		try
		{
		  type.createValue(file, properties);
		  fail("expected exception");
		}
		catch (System.ArgumentException e)
		{
		  // then
		  assertThat(e.Message, containsString("The provided mime type is null. Set a non-null value info property with key 'filename'"));
		}

		// given
		file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");

		properties["mimeType"] = "someMimetype";
		properties["encoding"] = null;

		// when
		try
		{
		  type.createValue(file, properties);
		  fail("expected exception");
		}
		catch (System.ArgumentException e)
		{
		  // then
		  assertThat(e.Message, containsString("The provided encoding is null. Set a non-null value info property with key 'encoding'"));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(expected = IllegalArgumentException.class) public void cannotCreateFileWithoutName()
	  public virtual void cannotCreateFileWithoutName()
	  {
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		type.createValue(file, System.Linq.Enumerable.Empty<string, object> ());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(expected = IllegalArgumentException.class) public void cannotCreateFileWithoutValueInfo()
	  public virtual void cannotCreateFileWithoutValueInfo()
	  {
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		type.createValue(file, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotCreateFileWithInvalidTransientFlag()
	  public virtual void cannotCreateFileWithInvalidTransientFlag()
	  {
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		IDictionary<string, object> info = new Dictionary<string, object>();
		info["filename"] = "bar";
		info["transient"] = "foo";
		try
		{
		  type.createValue(file, info);
		}
		catch (System.ArgumentException e)
		{
		  assertThat(e.Message, containsString("The property 'transient' should have a value of type 'boolean'."));
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void valueInfoContainsFileTypeNameTransientFlagAndCharsetEncoding()
	  public virtual void valueInfoContainsFileTypeNameTransientFlagAndCharsetEncoding()
	  {
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		string fileName = "simpleFile.txt";
		string fileType = "text/plain";
		Charset encoding = Charset.forName("UTF-8");
		FileValue fileValue = Variables.fileValue(fileName).file(file).mimeType(fileType).encoding(encoding).setTransient(true).create();
		IDictionary<string, object> info = type.getValueInfo(fileValue);

		assertThat(info, hasEntry(FileValueTypeImpl.VALUE_INFO_FILE_NAME, (object) fileName));
		assertThat(info, hasEntry(FileValueTypeImpl.VALUE_INFO_FILE_MIME_TYPE, (object) fileType));
		assertThat(info, hasEntry(FileValueTypeImpl.VALUE_INFO_FILE_ENCODING, (object) encoding.name()));
		assertThat(info, hasEntry(FileValueTypeImpl.VALUE_INFO_TRANSIENT, (object) true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void valueInfoContainsFileTypeNameAndStringEncoding()
	  public virtual void valueInfoContainsFileTypeNameAndStringEncoding()
	  {
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		string fileName = "simpleFile.txt";
		string fileType = "text/plain";
		string encoding = "UTF-8";
		FileValue fileValue = Variables.fileValue(fileName).file(file).mimeType(fileType).encoding(encoding).create();
		IDictionary<string, object> info = type.getValueInfo(fileValue);

		assertThat(info, hasEntry(FileValueTypeImpl.VALUE_INFO_FILE_NAME, (object) fileName));
		assertThat(info, hasEntry(FileValueTypeImpl.VALUE_INFO_FILE_MIME_TYPE, (object) fileType));
		assertThat(info, hasEntry(FileValueTypeImpl.VALUE_INFO_FILE_ENCODING, (object) encoding));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fileByteArrayIsEqualToFileValueContent() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void fileByteArrayIsEqualToFileValueContent()
	  {
		Stream file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		string fileName = "simpleFile.txt";

		FileValue fileValue = Variables.fileValue(fileName).file(file).create();
		file = this.GetType().ClassLoader.getResourceAsStream("org/camunda/bpm/engine/test/variables/simpleFile.txt");
		assertThat(IoUtil.inputStreamAsByteArray(fileValue.Value), equalTo(IoUtil.inputStreamAsByteArray(file)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void fileByteArrayIsEqualToFileValueContentCase2() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void fileByteArrayIsEqualToFileValueContentCase2()
	  {

		sbyte[] bytes = new sbyte[]{(sbyte)-16, (sbyte)-128, (sbyte)-128, (sbyte)-128};
		Stream byteStream = new MemoryStream(bytes);

		string fileName = "simpleFile.txt";

		FileValue fileValue = Variables.fileValue(fileName).file(byteStream).create();
		assertThat(IoUtil.inputStreamAsByteArray(fileValue.Value), equalTo(bytes));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void doesNotHaveParent()
	  public virtual void doesNotHaveParent()
	  {
		assertThat(type.Parent, @is(nullValue()));
	  }


	  private void checkStreamFromValue(TypedValue value, string expected)
	  {
		Stream stream = (Stream) value.Value;
		Scanner scanner = new Scanner(stream);
		assertThat(scanner.nextLine(), @is(expected));
	  }
	}

}