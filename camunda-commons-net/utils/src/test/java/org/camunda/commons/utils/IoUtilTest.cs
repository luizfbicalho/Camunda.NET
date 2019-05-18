using System;
using System.Text;
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
namespace org.camunda.commons.utils
{
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.fail;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class IoUtilTest
	{

	  public const string TEST_FILE_NAME = "org/camunda/commons/utils/testFile.txt";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldTransformBetweenInputStreamAndString()
	  public virtual void shouldTransformBetweenInputStreamAndString()
	  {
		Stream inputStream = IoUtil.stringAsInputStream("test");
		string @string = IoUtil.inputStreamAsString(inputStream);
		assertThat(@string).isEqualTo("test");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldTransformFromInputStreamToByteArray()
	  public virtual void shouldTransformFromInputStreamToByteArray()
	  {
		string testString = "Test String";
		Stream inputStream = IoUtil.stringAsInputStream(testString);
		assertThat(IoUtil.inputStreamAsByteArray(inputStream)).isEqualTo(testString.GetBytes(IoUtil.ENCODING_CHARSET));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldTransformFromStringToInputStreamToByteArray()
	  public virtual void shouldTransformFromStringToInputStreamToByteArray()
	  {
		string testString = "Test String";
		Stream inputStream = IoUtil.stringAsInputStream(testString);

		string newString = IoUtil.inputStreamAsString(inputStream);
		assertThat(testString).isEqualTo(newString);

		inputStream = IoUtil.stringAsInputStream(testString);
		sbyte[] newBytes = newString.GetBytes(IoUtil.ENCODING_CHARSET);
		assertThat(IoUtil.inputStreamAsByteArray(inputStream)).isEqualTo(newBytes);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getFileContentAsString()
	  public virtual void getFileContentAsString()
	  {
		assertThat(IoUtil.fileAsString(TEST_FILE_NAME)).isEqualTo("This is a Test!");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailGetFileContentAsStringWithGarbageAsFilename()
	  public virtual void shouldFailGetFileContentAsStringWithGarbageAsFilename()
	  {
		try
		{
		  IoUtil.fileAsString("asd123");
		  fail("Expected: IoUtilException");
		}
		catch (IoUtilException)
		{
		  // happy way
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getFileContentAsStream()
	  public virtual void getFileContentAsStream()
	  {
		Stream stream = IoUtil.fileAsStream(TEST_FILE_NAME);
		StreamReader reader = new StreamReader(stream);
		StringBuilder output = new StringBuilder();
		string line;
		try
		{
		  while (!string.ReferenceEquals((line = reader.ReadLine()), null))
		  {
			output.Append(line);
		  }
		  assertThat(output.ToString()).isEqualTo("This is a Test!");
		}
		catch (Exception)
		{
		  fail("Something went wrong while reading the input stream");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailGetFileContentAsStreamWithGarbageAsFilename()
	  public virtual void shouldFailGetFileContentAsStreamWithGarbageAsFilename()
	  {
		try
		{
		  IoUtil.fileAsStream("asd123");
		  fail("Expected: IoUtilException");
		}
		catch (IoUtilException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void getFileFromClassPath()
	  public virtual void getFileFromClassPath()
	  {
		File file = IoUtil.getClasspathFile(TEST_FILE_NAME);

		assertThat(file).NotNull;
		assertThat(file.Name).isEqualTo("testFile.txt");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailGetFileFromClassPathWithGarbage()
	  public virtual void shouldFailGetFileFromClassPathWithGarbage()
	  {
		try
		{
		  IoUtil.getClasspathFile("asd123");
		  fail("Expected: IoUtilException");
		}
		catch (IoUtilException)
		{
		  // happy way
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailGetFileFromClassPathWithNull()
	  public virtual void shouldFailGetFileFromClassPathWithNull()
	  {
		try
		{
		  IoUtil.getClasspathFile(null);
		  fail("Expected: IoUtilException");
		}
		catch (IoUtilException)
		{
		  // happy way
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUseFallBackWhenCustomClassLoaderIsWrong()
	  public virtual void shouldUseFallBackWhenCustomClassLoaderIsWrong()
	  {
		File file = IoUtil.getClasspathFile(TEST_FILE_NAME, new ClassLoaderAnonymousInnerClass(this));
		assertThat(file).NotNull;
		assertThat(file.Name).isEqualTo("testFile.txt");
	  }

	  private class ClassLoaderAnonymousInnerClass : ClassLoader
	  {
		  private readonly IoUtilTest outerInstance;

		  public ClassLoaderAnonymousInnerClass(IoUtilTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override URL getResource(string name)
		  {
			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldUseFallBackWhenCustomClassLoaderIsNull()
	  public virtual void shouldUseFallBackWhenCustomClassLoaderIsNull()
	  {
		File file = IoUtil.getClasspathFile(TEST_FILE_NAME, null);
		assertThat(file).NotNull;
		assertThat(file.Name).isEqualTo("testFile.txt");
	  }
	}

}